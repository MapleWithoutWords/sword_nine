using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SPCS.Common.Helper
{
    /// <summary>
    /// npoi组件的excel操作
    /// </summary>
    public class NpoiExcelHelper
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        public static NpoiExcelHelper Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 单例的实例
        /// </summary>
        private static readonly NpoiExcelHelper _instance = new NpoiExcelHelper();

        /// <summary>
        /// 私有化构造函数
        /// </summary>
        private NpoiExcelHelper()
        {

        }

        /// <summary>
        /// 读取excel中的sheet转换成list集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inStream"></param>
        /// <param name="fileNameExit"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public List<T> GetList<T>(Stream inStream, out string errorMsg, string fileNameExit = ".xlsx", string sheetName = null) where T : new()
        {
            using (inStream)
            {
                errorMsg = null;
                IWorkbook workbook = null;
                if (fileNameExit.IndexOf(".xlsx") >= 0)
                {
                    workbook = new XSSFWorkbook(inStream);
                }
                else if (fileNameExit.IndexOf(".xls") >= 0)
                {
                    workbook = new HSSFWorkbook(inStream);
                }
                else
                {
                    return null;
                }
                return GetList<T>(workbook, out errorMsg, sheetName);

            }
        }
        public IWorkbook GetWorkBook(Stream inStream)
        {
            return WorkbookFactory.Create(inStream);
        }

        public List<T> GetList<T>(IWorkbook workbook, out string errorMsg, string sheetName = null) where T : new()
        {
            errorMsg = null;
            ISheet sheet = null;
            if (sheetName != null)
            {
                sheet = workbook.GetSheet(sheetName);
            }
            else
            {
                sheet = workbook.GetSheetAt(0);
            }
            var colRow = sheet.GetRow(0);

            IDictionary<string, int> dicHeader = new Dictionary<string, int>();
            for (int i = 0; i < colRow.Cells.Count; i++)
            {
                var colCell = colRow.GetCell(i);
                if (colCell == null)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(colCell.StringCellValue))
                {
                    continue;
                }
                dicHeader[colCell.StringCellValue] = i;
            }

            var ty = typeof(T);
            var tyPros = ty.GetProperties();

            List<T> result = new List<T>(sheet.LastRowNum);
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                T objT = new T();
                foreach (var item in tyPros)
                {
                    string tempName = item.Name;
                    string tempType = item.PropertyType.Name;
                    var attrs = (ExcelImportAttribute[])item.GetCustomAttributes(typeof(ExcelImportAttribute), false);
                    if (attrs.Length > 0)
                    {
                        tempName = attrs[0].ExcelColumnName;
                        tempType = attrs[0].ExcelColumnType;
                    }
                    if (dicHeader.ContainsKey(tempName) == false)
                    {
                        continue;
                    }
                    try
                    {
                        var cell = row.GetCell(dicHeader[tempName]);
                        if (cell == null)
                        {
                            continue;
                        }
                        var retObj = GetVal(tempType, cell, item.PropertyType, out errorMsg);
                        if (errorMsg.IsNoNullAndNoEmpty())
                        {
                            errorMsg += $",第{i + 1}行";
                            return null;
                        }
                        item.SetValue(objT, retObj);
                    }
                    catch (Exception ex)
                    {
                        errorMsg = ex.Message + $"。第{i + 1}行，【{tempName}】列";
                        return null;
                    }
                }
                result.Add(objT);
            }
            return result;
        }

        private object GetVal(string tempType, ICell cell, Type toType, out string errorMsg)
        {
            errorMsg = null;
            if (tempType.Equals("string", StringComparison.InvariantCultureIgnoreCase))
            {
                if (cell.StringCellValue.IsNullOrEmpty())
                {
                    return "";
                }
                return Convert.ChangeType(cell.StringCellValue, toType);
            }
            else if (tempType.Equals("datetime", StringComparison.InvariantCultureIgnoreCase))
            {
                if (cell.StringCellValue.IsNullOrEmpty())
                {
                    return default(DateTime);
                }
                if (DateTime.TryParse(cell.StringCellValue, out DateTime outTime))
                {
                    return outTime;
                }

                errorMsg = "时间类型格式错误";
                return null;
            }
            else if (tempType.Equals("number", StringComparison.InvariantCultureIgnoreCase))
            {
                return Convert.ChangeType(cell.NumericCellValue, toType);
            }
            else if (tempType.Equals("bool", StringComparison.InvariantCultureIgnoreCase))
            {
                return Convert.ChangeType(cell.BooleanCellValue, toType);
            }
            else
            {
                return Convert.ChangeType(cell.StringCellValue, toType);
            }
        }

        /// <summary>
        /// 读取excel中的sheet转换成list集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inStream"></param>
        /// <param name="fileNameExit"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string filePath, out string errorMsg, string sheetName = null) where T : new()
        {
            errorMsg = null;
            if (File.Exists(filePath) == false)
            {
                errorMsg = "文件路径不存在";
                return null;
            }
            return GetList<T>(File.OpenRead(filePath), out errorMsg, Path.GetExtension(filePath), sheetName);
        }

        /// <summary>
        /// 读取excel中的sheet为datatable
        /// </summary>
        /// <param name="inStream"></param>
        /// <param name="fileNameExit"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public DataTable GetDataTable(Stream inStream, string fileNameExit, string sheetName = null)
        {
            DataTable dt = new DataTable();
            IWorkbook workbook = null;
            if (fileNameExit.IndexOf(".xlsx") >= 0)
            {
                workbook = new XSSFWorkbook(inStream);
            }
            else if (fileNameExit.IndexOf(".xls") >= 0)
            {
                workbook = new HSSFWorkbook(inStream);
            }
            else
            {
                return null;
            }
            ISheet sheet = null;
            if (sheetName != null)
            {
                sheet = workbook.GetSheet(sheetName);
            }
            else
            {
                sheet = workbook.GetSheetAt(0);
            }
            using (inStream)
            {
                var firstRow = sheet.GetRow(0);
                if (firstRow == null)
                {
                    return dt;
                }
                int cellCount = firstRow.Cells.Count;
                dt.TableName = sheetName;
                foreach (var cell in firstRow.Cells)
                {
                    if (string.IsNullOrEmpty(cell.StringCellValue) == false)
                    {

                        DataColumn column = new DataColumn(cell.StringCellValue);
                        dt.Columns.Add(column);
                    }

                }
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    DataRow dataRow = dt.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                    {
                        if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                            dataRow[j] = row.GetCell(j).ToString();
                    }
                    dt.Rows.Add(dataRow);
                }
                return dt;
            }
        }

        /// <summary>
        /// 读取excel中的sheet为datatable
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string filePath, string sheetName = null)
        {
            if (File.Exists(filePath) == false)
            {
                return null;
            }
            return GetDataTable(File.OpenRead(filePath), Path.GetExtension(filePath), sheetName);
        }

        /// <summary>
        /// 获取excel所有的sheet页
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        public List<DataTable> GetAllSheet(Stream stream, string fileExt)
        {
            IWorkbook workbook = null;
            List<DataTable> list = new List<DataTable>();
            try
            {
                if (fileExt.IndexOf(".xlsx") >= 0) // 2007版本
                    workbook = new XSSFWorkbook(stream);
                else if (fileExt.IndexOf(".xls") >= 0) // 2003版本
                    workbook = new HSSFWorkbook(stream);
                else
                {
                    return null;
                }
                for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                {
                    var sheetName = workbook.GetSheetName(sheetIndex);
                    if (sheetName.Contains("Ignor"))
                    {
                        continue;
                    }
                    var sheet = workbook.GetSheet(sheetName);
                    var firstRow = sheet.GetRow(0);
                    if (firstRow == null)
                    {
                        continue;
                    }
                    int cellCount = firstRow.Cells.Count;
                    DataTable dt = new DataTable();
                    dt.TableName = sheetName;
                    foreach (var cell in firstRow.Cells)
                    {
                        if (string.IsNullOrEmpty(cell.StringCellValue) == false)
                        {

                            DataColumn column = new DataColumn(cell.StringCellValue);
                            dt.Columns.Add(column);
                        }

                    }
                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        var row = sheet.GetRow(i);
                        if (row == null)
                        {
                            continue;
                        }
                        DataRow dataRow = dt.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        dt.Rows.Add(dataRow);
                    }
                    list.Add(dt);
                }


                return list;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取excel所有的sheet页
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        public List<DataTable> GetAllSheet(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                return null;
            }
            return GetAllSheet(File.OpenRead(filePath), Path.GetExtension(filePath));
        }

        /// <summary>
        /// 将list转换成excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="fileNameExit"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public IWorkbook Export<T>(List<T> list, string fileNameExit = ".xlsx", string sheetName = "Sheet1")
        {
            IWorkbook workbook = null;
            if (fileNameExit.IndexOf(".xlsx") >= 0)
            {
                workbook = new XSSFWorkbook();
            }
            else if (fileNameExit.IndexOf(".xls") >= 0)
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                return null;
            }
            var sheet = workbook.CreateSheet(sheetName);
            var headerRow = sheet.CreateRow(0);
            Type ty = typeof(T);
            var tyPros = ty.GetProperties();
            for (int i = 0; i < tyPros.Length; i++)
            {

                var attrs = (ExcelImportAttribute[])tyPros[i].GetCustomAttributes(typeof(ExcelImportAttribute), false);
                if (attrs.Length <= 0)
                {
                    continue;
                }
                headerRow.CreateCell(i).SetCellValue(attrs[0].ExcelColumnName);
            }
            for (int i = 0; i < list.Count; i++)
            {
                var objT = list.ElementAt(i);
                var row = sheet.CreateRow(i + 1);
                for (int j = 0; j < tyPros.Length; j++)
                {
                    var pro = tyPros[j];
                    var cell = row.CreateCell(j);

                    #region 设置excel列的类型
                    if (pro.PropertyType == typeof(DateTime))
                    {
                        cell.SetCellType(CellType.String);
                    }
                    else if (pro.PropertyType == typeof(double) || pro.PropertyType == typeof(int) || pro.PropertyType == typeof(long) || pro.PropertyType == typeof(float))
                    {
                        cell.SetCellType(CellType.Numeric);
                    }
                    else
                    {
                        cell.SetCellType(CellType.String);
                    }
                    #endregion

                    cell.SetCellValue(pro.GetValue(objT)?.ToString());
                }
            }
            return workbook;
        }

        /// <summary>
        /// 将list写入到excel文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="fileNameExit"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public string Export<T>(string filePath, List<T> list, string sheetName = "Sheet1")
        {
            var stream = Export(list, Path.GetExtension(filePath), sheetName);
            if (stream == null)
            {
                return null;
            }
            using (var fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                stream.Write(fs);
                return filePath;
            }
        }

        /// <summary>
        /// 将datatable写入到excel流中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileNameExit"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public IWorkbook Export(DataTable dt, string fileNameExit = ".xlsx", string sheetName = "Sheet1")
        {
            IWorkbook workbook = null;
            if (fileNameExit.IndexOf(".xlsx") >= 0)
            {
                workbook = new XSSFWorkbook();
            }
            else if (fileNameExit.IndexOf(".xls") >= 0)
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                return null;
            }
            var sheet = workbook.CreateSheet(sheetName);
            var headerRow = sheet.CreateRow(0);

            for (int i = 0; i < dt.Columns.Count; i++)
            {

                headerRow.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    row.CreateCell(j).SetCellValue(dt.Rows[i][j]?.ToString());
                }
            }

            return workbook;
        }


        /// <summary>
        /// 将datatable写入到excel文件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public string Export(string filePath, DataTable dt, string sheetName = "Sheet1")
        {
            var workbook = Export(dt, Path.GetExtension(filePath), sheetName);
            if (workbook == null)
            {
                return null;
            }
            using (var fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
                return filePath;
            }
        }



        /// <summary>
        /// 将datatable集合写入到流
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="fileNameExit"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public IWorkbook Export(List<DataTable> tables, string fileNameExit = ".xlsx")
        {
            IWorkbook workbook = null;
            if (fileNameExit.IndexOf(".xlsx") >= 0)
            {
                workbook = new XSSFWorkbook();
            }
            else if (fileNameExit.IndexOf(".xls") >= 0)
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                return null;
            }
            foreach (var dt in tables)
            {
                if (string.IsNullOrEmpty(dt.TableName))
                {
                    continue;
                }
                var sheet = workbook.CreateSheet(dt.TableName);
                var headerRow = sheet.CreateRow(0);
                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    headerRow.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(dt.Rows[i][j]?.ToString());
                    }
                }
            }
            return workbook;
        }

        /// <summary>
        /// 将datatable集合写入到excel文件
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="fileNameExit"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public string Export(string filePath, List<DataTable> tables)
        {
            var workbook = Export(tables, Path.GetExtension(filePath));
            if (workbook == null)
            {
                return null;
            }
            using (var fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
                return filePath;
            }
        }

        /// <summary>
        /// 获取excel中所有名称管理器的名称
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public List<ExcelName> GetWorkbookRefName(IWorkbook workbook)
        {
            List<ExcelName> retlist = new List<ExcelName>();
            for (int i = 0; i < workbook.NumberOfNames; i++)
            {
                var iname = workbook.GetNameAt(i);
                var flag = Regex.IsMatch(iname.RefersToFormula, @"^.+!\$[A-Z]+\$[1-9]+");
                if (flag)
                {
                    //retlist.Add(new ExcelName
                    //{
                    //    SheetName = iname.SheetName,
                    //    DicCellIndexName
                    //});
                }
            }
            return retlist;
        }
        /// <summary>
        /// 获取excel中列的下标
        /// </summary>
        /// <param name="rowchar"></param>
        /// <returns></returns>
        public int GetExcelDataCellIndex(string rowchar)
        {
            int index = -1;
            for (int i = 0; i < rowchar.Length; i++)
            {
                var item = rowchar[i];
                index += ((int)item - (int)'A' + 1) * (int)Math.Pow(26, rowchar.Length - i - 1);
            }
            return index;
        }
    }
    public class ExcelName
    {
        /// <summary>
        /// sheet名称
        /// </summary>
        public string SheetName { get; set; }
        /// <summary>
        /// 从那行开始
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 列序号对应的名称
        /// </summary>
        public Dictionary<int, string> DicCellIndexName { get; set; } = new Dictionary<int, string>();
    }
}
