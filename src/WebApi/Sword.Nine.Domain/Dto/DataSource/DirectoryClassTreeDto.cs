using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Sword.Nine.Domain
{
    /// <summary>
    /// 目录类别树
    /// </summary>
    public class DirectoryClassTreeDto
    {
        /// <summary>
        /// 子节点
        /// </summary>
        public List<DirectoryClassTreeDto> Childrens { get; set; } = new List<DirectoryClassTreeDto>();
        /// <summary>
        /// 节点id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 上级节点id
        /// </summary>
        public string ParentId { get; set; } = Guid.Empty.ToString();
        /// <summary>
        /// 上级节点id
        /// </summary>
        public string ParentName { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 类别表名称：如果type为类别，则该字段有值
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 类别所属目录id，如果type为类别，则该字段有值
        /// </summary>
        public string ClassDirectoryId { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int SeqNo { get; set; }
        /// <summary>
        /// 节点类型：【0：目录】、【1：类别】
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="allDatas"></param>
        public void AddChildrens(List<DirectoryClassTreeDto> allDatas)
        {
            Childrens = allDatas.Where(e => e.ParentId == this.Id).ToList();
            if (Childrens.Count<1)
            {
                return;
            }
            foreach (var item in Childrens)
            {
                item.ParentName = allDatas.FirstOrDefault(e => e.Id == item.ParentId)?.Name;
                item.AddChildrens(allDatas);
            }
        }
    }
}
