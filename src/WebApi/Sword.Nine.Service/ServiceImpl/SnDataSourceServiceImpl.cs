using System;
using Microsoft.Extensions.Logging;
using Sword.Nine.Domain;
using Sword.Nine.Dao;
using NoRain.Reposition.BaseReposition;
using NoRain.Service;
using System.Threading.Tasks;
using NoRain.Common;
using System.Collections.Generic;
using System.Linq;
using Sword.Nine.Rule;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace Sword.Nine.Service
{
    /// <summary>
    /// 数据源表服务实现类
    /// </summary>
    public partial class SnDataSourceServiceImpl : BaseServiceImpl<SnDataSource>, ISnDataSourceService
    {
        /// <summary>
        /// 数据访问层类
        /// </summary>
        public ISnDataSourceDal DalImpl { get; set; }
        /// <summary>
        /// 类别目录数据访问层
        /// </summary>
        public ISnClassDirectoryDal DirectoryDal { get; set; }
        /// <sumary>
        /// 类别表数据层
        /// </sumary>
        public ISnClassDal ClassDal { get; set; }
        /// <sumary>
        /// 类别属性数据层
        /// </sumary>
        public ISnClassAttributeDal ClassAttributeDal { get; set; }
        /// <sumary>
        /// 规则表数据层
        /// </sumary>
        public ISnRuleDal RuleDal { get; set; }
        /// <sumary>
        /// 模板表数据层
        /// </sumary>
        public ISnTemplateDal TemplateDal { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="baseReposition"></param>
        /// <param name="dalImpl"></param>
        public SnDataSourceServiceImpl(ILoggerFactory loggerFactory,
                                    IBaseReposition<SnDataSource> baseReposition,
                                    ISnDataSourceDal dalImpl, ISnClassDal ClassDal,
                                    ISnClassAttributeDal ClassAttributeDal,
                                    ISnTemplateDal TemplateDal,
                                    ISnClassDirectoryDal DirectoryDal,
                                    ISnRuleDal RuleDal, IHttpContextAccessor conotext) : base(loggerFactory, baseReposition, conotext)
        {
            this.DalImpl = dalImpl;
            this.ClassDal = ClassDal;
            this.ClassAttributeDal = ClassAttributeDal;
            this.RuleDal = RuleDal;
            this.TemplateDal = TemplateDal;
            this.DirectoryDal = DirectoryDal;
        }
        /// <summary>
        /// 校验数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<APIResult> ValidDataAsync(SnDataSource entity)
        {
            Expression<Func<SnDataSource, bool>> expression = null;
            if (entity.Id.IsGuidAndNoGuidEmpty())
            {
                expression = e => e.IsDeleted == false && (e.Name == entity.Name || entity.Code == e.Code) && e.Id == entity.Id;
            }
            else
            {
                expression = e => e.IsDeleted == false && (e.Name == entity.Name || entity.Code == e.Code);
            }
            var repeatData = await DalImpl.ValidDataAsync(expression);
            if (repeatData)
            {
                return APIResult.ErrorResult("名称或编码重复，请修改");
            }
            return APIResult.SuccessResult();
        }

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="dataSourceId"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public async Task<APIResult> GeneratorCodeAsync(string dataSourceId, string templateId)
        {
            var dataSourceEntity = await DalImpl.GetByIdAsync(dataSourceId);
            var templateEntity = await TemplateDal.GetByIdAsync(templateId);
            var classEntityList = await ClassDal.GetByDataSourceIdAsync(dataSourceId);
            classEntityList = classEntityList.Where(e => e.Type != 1).ToList();
            var attributeEntityList = await ClassAttributeDal.GetByDataSourceIdAsync(dataSourceId);
            var ruleEntityList = await RuleDal.GetByDataSourceIdAsync(dataSourceId);

            if (dataSourceEntity == null)
            {
                return APIResult.ErrorResult("数据源不存在");
            }
            if (templateEntity == null)
            {
                return APIResult.ErrorResult("模板不存在");
            }

            var dsDto = GetCodeGenerateData(dataSourceEntity, templateEntity, classEntityList, attributeEntityList, ruleEntityList);

            var assemFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateEntity.AssemblyDirectory.TrimStart('/'), templateEntity.StartFileName);
            if (File.Exists(assemFilePath) == false)
            {
                return APIResult.ErrorResult("模板文件不存在");
            }
            var codeType = Assembly.LoadFile(assemFilePath).GetTypes().Where(e => e.IsAbstract == false && typeof(ICodeBI).IsAssignableFrom(e)).FirstOrDefault();
            if (codeType == null)
            {
                return APIResult.ErrorResult("模板文件非法");
            }
            var codeBi = Activator.CreateInstance(codeType) as ICodeBI;
            if (codeBi == null)
            {
                return APIResult.ErrorResult("模板文件非法");
            }
            //var codeBi = new CodeBI();
            string saveDir = AppDomain.CurrentDomain.BaseDirectory;
            //TODO：保存目录做成可配置
            saveDir = Path.Combine(saveDir, "wwwroot", $"codeList/{(templateEntity.Type == 0 ? "backstage" : "frontweb")}");
            if (Directory.Exists(saveDir) == false)
            {
                Directory.CreateDirectory(saveDir);
            }
            var runResult = codeBi.Run(dsDto, saveDir);
            //压缩目录
            var zipFilePath = Path.Combine(saveDir, $"{ dataSourceEntity.Code}.zip");
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }
            ZipFile.CreateFromDirectory(Path.Combine(saveDir, dataSourceEntity.Code), zipFilePath);

            return APIResult.SuccessResult($"codeList/{(templateEntity.Type == 0 ? "backstage" : "frontweb")}/{ dataSourceEntity.Code}.zip");
        }

        /// <summary>
        /// 获取代码生成所需要的数据
        /// </summary>
        /// <param name="dataSourceEntity"></param>
        /// <param name="templateEntity"></param>
        /// <param name="classEntityList"></param>
        /// <param name="attributeEntityList"></param>
        /// <param name="ruleEntityList"></param>
        /// <returns></returns>
        private DataSourceDto GetCodeGenerateData(SnDataSource dataSourceEntity, SnTemplate templateEntity, List<SnClassDto> classEntityList, List<SnClassAttributeListDto> attributeEntityList, List<SnRuleDto> ruleEntityList)
        {
            DataSourceDto dsDto = new DataSourceDto()
            {
                NameSpace = dataSourceEntity.NameSpace,
                ProjectName = dataSourceEntity.Code,
                DirPathName = templateEntity.AssemblyDirectory,
            };
            foreach (var classItem in classEntityList)
            {
                var tableDto = new TableDto
                {
                    Code = classItem.Code,
                    DirectoryCode = classItem.DirectoryCode,
                    Name = classItem.Name,
                    Remark = classItem.Remark,
                    TableName = classItem.TableName,
                };

                foreach (var attrItem in attributeEntityList.Where(e => e.ClassId == classItem.Id))
                {
                    var attrDto = new AttributeDto
                    {
                        ClassName = classItem.Name,
                        ClassTableName = classItem.TableName,
                        Name = attrItem.Name,
                        Code = attrItem.Code,
                        ColumnName = attrItem.ColumnName,
                        Length = attrItem.Length,
                        ReferenceClassName = attrItem.ReferenceClassName,
                        ReferenceClassCode = attrItem.ReferenceClassCode,
                        ReferenceClassTableName = attrItem.ReferenceClassTableName,
                        Remark = attrItem.Remark,
                        ValueType = (ValueTypeEnum)attrItem.ValueType,
                    };

                    foreach (var ruleItem in ruleEntityList.Where(e => e.ClassAttributeId == attrItem.Id))
                    {
                        if (ruleItem.ValueType == 0 && (bool.TryParse(ruleItem.Value, out bool boolValue) == false || boolValue == false))
                        {
                            continue;
                        }
                        if (ruleItem.ValueType == 1 && string.IsNullOrEmpty(ruleItem.Value))
                        {
                            continue;
                        }
                        RuleDto ruleDto = new RuleDto()
                        {
                            RuleType = ruleItem.EnumValue,
                            Data = ruleItem.Value
                        };

                        attrDto.RuleDatas.Add(ruleDto);
                    }
                    tableDto.AttrDatas.Add(attrDto);
                }

                dsDto.TableList.Add(tableDto);
            }
            return dsDto;
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<APIResult> ImportAsync(DataSoureceImportDto dto)
        {

            var allDbDataSourceList = await DalImpl.GetAllListAsync();

            List<SnDataSource> updateDataSourceList = new List<SnDataSource>();
            List<SnDataSource> updateOrInsertDataSourceList = new List<SnDataSource>();
            List<SnClassDirectory> updateDirectoryList = new List<SnClassDirectory>();
            List<SnClassDirectory> updateOrInsertDirectoryList = new List<SnClassDirectory>();
            List<SnClassDto> updateClassList = new List<SnClassDto>();
            List<SnClassDto> updateOrInsertClassList = new List<SnClassDto>();
            List<SnClassAttributeListDto> updateAttributeList = new List<SnClassAttributeListDto>();
            List<SnClassAttributeListDto> updateOrInsertAttributeList = new List<SnClassAttributeListDto>();
            int rowIndex = 1;

            foreach (var item in dto.UpdateOrInsertDataSourceList)
            {
                var en = item.ToEntity(this.UserId);
                var dbDataSource = allDbDataSourceList.FirstOrDefault(e => e.Code == en.Code);
                if (dbDataSource != null)
                {
                    dbDataSource.SetValue(en);
                    en = dbDataSource;
                    updateDataSourceList.Add(en);
                }
                updateOrInsertDataSourceList.Add(en);
            }
            rowIndex = 1;
            var dataSourceIdList = updateOrInsertDataSourceList.Select(e => e.Id);
            var allDbDirectoryList = await DirectoryDal.GetAllListAsync();
            var allDbClassList = await ClassDal.GetListByLambdaAsync(e => e.IsDeleted == false && dataSourceIdList.Contains(e.DataSourceId));
            var allDbAttributeList = await ClassAttributeDal.GetListByLambdaAsync(e => e.IsDeleted == false && dataSourceIdList.Contains(e.DataSourceId));
            foreach (var item in dto.UpdateOrInsertDirectoryList)
            {
                var en = item.ToEntity(this.UserId);

                var dsen = updateOrInsertDataSourceList.FirstOrDefault(e => e.SeqNo.ToString() == en.DataSourceId);
                if (dsen == null)
                {
                    return APIResult.ErrorResult($"文件中，目录页，第{rowIndex + 1}行，数据源序号非法");
                }
                en.DataSourceId = dsen.Id;

                var dbDirectory = allDbDirectoryList.FirstOrDefault(e => e.Code == en.Code && e.DataSourceId == en.DataSourceId);
                if (dbDirectory != null)
                {
                    dbDirectory.SetValue(en);
                    en = dbDirectory;
                    updateDirectoryList.Add(en);
                }

                updateOrInsertDirectoryList.Add(en);
            }
            foreach (var item in updateOrInsertDirectoryList)
            {
                var parentEn = updateOrInsertDirectoryList.FirstOrDefault(e => e.SeqNo.ToString() == item.ParentId);
                item.ParentId = parentEn == null ? Guid.Empty.ToString() : parentEn.Id;
                item.Path = parentEn == null ? $"|{item.Code}" : $"{parentEn}|{item.Code}";
            }
            rowIndex = 1;
            foreach (var item in dto.UpdateOrInsertClassList)
            {
                var en = item.ToEntity(this.UserId);

                var dsen = updateOrInsertDataSourceList.FirstOrDefault(e => e.SeqNo.ToString() == en.DataSourceId);
                if (dsen == null)
                {
                    return APIResult.ErrorResult($"文件中，目录页，第{rowIndex + 1}行，数据源序号非法");
                }
                en.DataSourceId = dsen.Id;

                var directoryEn = updateOrInsertDirectoryList.FirstOrDefault(e => e.SeqNo.ToString() == en.ClassDirectoryId);
                en.ClassDirectoryId = directoryEn == null ? Guid.Empty.ToString() : directoryEn.Id;

                var dbClass = allDbClassList.FirstOrDefault(e => e.Code == en.Code && e.DataSourceId == en.DataSourceId);
                if (dbClass != null)
                {
                    dbClass.SetValue(en);
                    en = dbClass;
                    updateClassList.Add(en);
                }

                updateOrInsertClassList.Add(en);
            }
            rowIndex = 1;
            foreach (var item in dto.UpdateOrInsertAttributeList)
            {
                var en = item.ToEntity(this.UserId);

                var classEn = updateOrInsertClassList.FirstOrDefault(e => e.SeqNo.ToString() == en.ClassId);
                if (classEn == null)
                {
                    return APIResult.ErrorResult($"文件中，目录页，第{rowIndex + 1}行，类别序号非法");
                }
                en.DataSourceId = classEn.DataSourceId;
                en.ClassId = classEn.Id;

                var refClassEn = updateOrInsertClassList.FirstOrDefault(e => e.SeqNo.ToString() == en.ReferenceId);
                en.ReferenceId = refClassEn == null ? "" : refClassEn.Id;

                var dbattr = allDbAttributeList.FirstOrDefault(e => (e.ColumnName == en.ColumnName || e.Code == en.Code) && e.ClassId == en.ClassId && e.DataSourceId == en.DataSourceId);
                if (dbattr != null)
                {
                    dbattr.SetValue(en);
                    en = dbattr;
                    updateAttributeList.Add(en);
                }

                updateOrInsertAttributeList.Add(en);
            }

            await this.InsertAsync(updateOrInsertDataSourceList.Where(e => updateDataSourceList.Count(x => x.Code == e.Code) < 1).ToList());
            await this.UpdateAsync(updateDataSourceList);
            var insertDirectoryList = updateOrInsertDirectoryList.Where(e => updateDirectoryList.Count(x => x.Code == e.Code && x.DataSourceId == e.DataSourceId) < 1).ToList();
            await DirectoryDal.InsertAsync(insertDirectoryList);
            await DirectoryDal.UpdateAsync(updateDirectoryList);
            var insertClassList = updateOrInsertClassList.Where(e => updateClassList.Count(x => x.Code == e.Code && x.DataSourceId == e.DataSourceId) < 1).ToList();
            await ClassDal.InsertAsync(insertClassList);
            await ClassDal.UpdateAsync(updateClassList);
            var insertAttributeList = updateOrInsertAttributeList.Where(e => updateAttributeList.Count(x => (e.ColumnName == x.ColumnName || e.Code == x.Code) && e.ClassId == x.ClassId && x.DataSourceId == e.DataSourceId) < 1).ToList();
            await ClassAttributeDal.InsertAsync(insertAttributeList);
            await ClassAttributeDal.UpdateAsync(updateAttributeList);

            return APIResult.SuccessResult();
        }

        /// <summary>
        /// 发布数据源
        /// </summary>
        /// <param name="dataSourceId"></param>
        /// <returns></returns>
        public async Task<APIResult> PublishAsync(string dataSourceId)
        {
            var allClassIdList = await ClassDal.GetSelectByLambdaAsync(e => e.DataSourceId == dataSourceId && e.Type == 0 && e.IsDeleted == false, e => e.Id);
            var ret = await ClassDal.PublishAsync(allClassIdList);
            return APIResult.SuccessResult(ret);
        }
    }
}
