using System;
using System.Collections.Generic;
using System.Text;

namespace Sword.Nine.Domain
{
    /// <summary>
    /// 绘图模型
    /// </summary>
    public class MxCellDto
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 绘图位置信息
        /// </summary>
        public Geometry Geometry { get; set; }
        /// <summary>
        /// 样式
        /// </summary>
        public string Style { get; set; }
        /// <summary>
        /// 指定该cell是否可被线连接
        /// </summary>
        public object Vertex { get; set; }
        /// <summary>
        /// 是否是线
        /// </summary>
        public bool Edge { get; set; }
        /// <summary>
        /// 指定该cell是否可被线连接
        /// </summary>
        public bool Connectable { get; set; }
        /// <summary>
        /// 显示隐藏
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Collapsed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string[] MxTransient { get; set; }
        /// <summary>
        /// 子集
        /// </summary>
        public List<MxCellDto> Children { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 连接源头
        /// </summary>
        public object Source { get; set; }
        /// <summary>
        /// 连接目标
        /// </summary>
        public object Target { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MxObjectId { get; set; }
        /// <summary>
        /// 自定义属性：id
        /// </summary>
        public string DataId { get; set; }
    }

    public class Geometry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        //public bool TRANSLATE_CONTROL_POINTS { get; set; }
        //public object alternateBounds { get; set; }
        //public object sourcePoint { get; set; }
        //public object targetPoint { get; set; }
        //public object points { get; set; }
        //public object offset { get; set; }
        //public bool relative { get; set; }
    }
}
