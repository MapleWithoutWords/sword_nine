using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace NoRain.Reposition
{
    public class BaseEntity
    {

        ///<summary>
        ///<para>Id</para>
        ///</summary>
        [Key]
        [Required(ErrorMessage = "Id不能为null,可以为空字符串''", AllowEmptyStrings = true)]
        [StringLength(36, ErrorMessage = "Id最长不能超256个字符!")]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();
        ///<summary>
        ///<para>并发版本控制</para>
        ///</summary>
        //[SugarColumn(ColumnName = "Version")]
        //public virtual DateTime Version { get; set; } = DateTime.Now;

        ///<summary>
        ///<para>最后更新时间</para>
        ///</summary>
        [SugarColumn(ColumnName = "LastUpdateTime")]
        public virtual DateTime LastUpdateTime { get; set; } = DateTime.Now;

        ///<summary>
        ///<para>最后更新人</para>
        ///</summary>
        [StringLength(36, ErrorMessage = "最后更新人 最长不能超 36 个字符!")]
        [SugarColumn(ColumnName = "LastUpdateUser")]
        public virtual string LastUpdateUser { get; set; } = Guid.Empty.ToString();

        ///<summary>
        ///<para>数据状态</para>
        ///</summary>
        [SugarColumn(ColumnName = "IsDeleted")]
        public virtual bool IsDeleted { get; set; } = false;

        ///<summary>
        ///<para>创建时间</para>
        ///</summary>
        [SugarColumn(ColumnName = "CreateTime")]
        public virtual DateTime CreateTime { get; set; } = DateTime.Now;

        ///<summary>
        ///<para>创建人</para>
        ///</summary>
        [StringLength(36, ErrorMessage = "创建人 最长不能超 36 个字符!")]
        [SugarColumn(ColumnName = "CreateUser")]
        public virtual string CreateUser { get; set; } = Guid.Empty.ToString();

        ///<summary>
        ///<para>状态，0表示正常；1表示禁用</para>
        ///</summary>
        [Required]
        [SugarColumn(ColumnName = "Status")]
        public virtual int Status { get; set; } = 0;

        /// <summary>
        /// 对象赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="to">返回结果的对象，新对象</param>
        /// <param name="so">源对象</param>
        /// <returns></returns>
        public void SetValue<T>(T so, params string[] ignorArrs) where T : BaseEntity
        {
            var propertys = so.GetType().GetProperties();
            foreach (var propertyInfo in propertys)
            {
                var pname = propertyInfo.Name;
                if (ignorArrs.Any(e => e == pname))
                {
                    continue;
                }
                var value = propertyInfo.GetValue(so);
                if (value == null)
                {
                    continue;
                }
                if (value != null && this.GetType().GetProperties().Select(c => c.Name).Contains(pname)
                    && pname != "Version"
                    //&& pname != "Flag"
                    //&& pname != "Status"
                    && pname != "CreateUser"
                    && pname != "LastUpdateTime"
                    && pname != "CreateTime"
                    && pname != "IsDeleted"
                    && pname != "TenantId"
                    && pname != "Id"
                    && pname != "LastUpdateUser")
                {
                    this.GetType().GetProperty(pname).SetValue(this, value);
                }
            }
        }
        /// <summary>
        /// 设置用户Id
        /// </summary>
        /// <param name="userId"></param>
        public void SetUserId(string userId, bool falg = false)
        {
            if (this.Id.IsGuidAndNoGuidEmpty() == false || falg)
            {
                this.Id = Guid.NewGuid().ToString();
            }
            if (this.CreateUser.IsGuidAndNoGuidEmpty() == false || falg)
            {
                this.CreateUser = userId;
            }
            if (this.CreateTime == default)
            {
                this.CreateTime = DateTime.Now;
            }
            this.LastUpdateUser = userId;
            this.LastUpdateTime = DateTime.Now;
        }
    }
}
