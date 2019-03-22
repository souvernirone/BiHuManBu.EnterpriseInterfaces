using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using BiHuManBu.ExternalInterfaces.Models.Model;
using System.Net;
using System.Configuration;
using log4net;

namespace BiHuManBu.ExternalInterfaces.API.Filters
{
    public class ExcelHelper<T>
    {
        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        readonly int _startIndex = ApplicationSettingsFactory.GetApplicationSettings().ExcelStartRowIndex;
        readonly Hashtable _fromEnglishToChineseForBatchRenewalItem = new Hashtable() { { "LicenseNo", "车牌号" }, { "VinNo", "车架号" }, { "EngineNo", "发动机号" }, { "MoldName", "品牌型号" }, { "RegisterDate", "初登日期" }, { "LastYearSource", "去年投保公司" }, { "ForceEndDate", "交强险到期时间" }, { "BizEndDate", "商业险到期时间" }, { "CustomerName", "客户姓名" }, { "Mobile", "客户电话1" }, { "Client_mobile_other", "客户电话2" }, { "CategoryInfo", "客户类别" }, { "Remark", "客户备注" }, { "SalesManName", "业务员姓名" }, { "SalesManAccount", "业务员账号" } };
        readonly Hashtable _getExcel = new Hashtable() { { "LicenseNo", "车牌号" }, { "Name", "车主" }, { "MoldName", "品牌型号" }, { "EngineNo", "发动机号" }, { "VinNo", "车架号" }, { "RegisterDate", "注册日期" }, { "PreSource", "上年承保公司" }, { "BusinessRisksEndTime", "商业险到期日期" }, { "ForceRisksEndTime", "交强险到期日期" }, { "Mobile", "手机号" }, { "InsuredPeopleName", "被保险人姓名" }, { "IdCard", "被保险人证件号" }, { "CustomerName", "客户姓名" }, { "Mobile1", "客户电话1" }, { "Mobile2", "客户电话2" }, { "Remark", "客户备注" }, { "CategoryInfo", "客户类别" }, { "SalesManName", "业务员姓名" }, { "SalesManAccount", "业务员账号" } };
        readonly Hashtable _errorExcel = new Hashtable() { { "LicenseNo", "车牌号" }, { "VinNo", "车架号" }, { "EngineNo", "发动机号" }, { "MoldName", "品牌型号" }, { "RegisterDate", "初登日期" }, { "LastYearSource", "去年投保公司" }, { "ForceEndDate", "交强险到期时间" }, { "BizEndDate", "商业险到期时间" }, { "CustomerName", "客户姓名" }, { "Mobile", "客户电话1" }, { "MobileOther", "客户电话2" }, { "Remark", "备注" }, { "CategoryInfo", "客户类别" }, { "SalesManName", "业务员姓名" }, { "SalesManAccount", "业务员账号" }, { "ErrorMsg", "错误信息" } };
    
        private readonly string _fileServerURL = ConfigurationManager.AppSettings["FileServerURL"].ToString();
        /// <summary>  
        /// 导出Excel  
        /// </summary>  
        /// <param name="lists"></param>  
        /// <param name="head">中文列名对照</param>  
        /// <param name="workbookFile">保存路径</param>  
        public void GetExcel(List<DownLoadExcel> lists, List<ExcelErrorData> errorlist, string SuccessFulSheetName, string FailedSheetName, string requestEnginoName, string AlreadyDel, string UploadFail, string workbookFile)
        {
            IWorkbook _iWorkBook = null;
            try
            {
                _iWorkBook = new XSSFWorkbook();
            }
            catch
            {
                _iWorkBook = new HSSFWorkbook();
            }
            MemoryStream ms = new MemoryStream();
            int alDel = 0;
            foreach (DownLoadExcel item in lists)
            {
                int IsTest = item.Istest;
                if (IsTest == 3)
                {
                    alDel++;
                }
            }
            if (lists.Count == alDel)
            {
                DownLoadExcel downexcel = lists.Find(delegate(DownLoadExcel elle) { return elle.Istest == 3; });
                //如果存在垃圾数据
                if (downexcel != null)
                {
                    IRow dataRow = null;
                    ISheet delSheet = _iWorkBook.CreateSheet(AlreadyDel);
                    IRow delRow = delSheet.CreateRow(0);
                    Type typedel = typeof(DownLoadExcel);
                    PropertyInfo[] delproperties = typedel.GetProperties();
                    int del = 1;
                    for (int i = 0; i < delproperties.Length - 2; i++)
                    {
                        delRow.CreateCell(i).SetCellValue(_getExcel[delproperties[i].Name] == null ? delproperties[i].Name : _getExcel[delproperties[i].Name].ToString());
                    }
                    foreach (DownLoadExcel item in lists)
                    {
                        int IsTest = Convert.ToInt32(item.GetType().GetProperty("Istest").GetValue(item, null));
                        if (IsTest == 3)
                        {
                            dataRow = delSheet.CreateRow(del);
                            del++;
                            for (int i = 0; i < delproperties.Length - 2; i++)
                            {
                                dataRow.CreateCell(i).SetCellValue(delproperties[i].GetValue(item, null) == null ? "" : delproperties[i].GetValue(item, null).ToString());
                            }
                        }
                    }
                }
                ISheet successfulSheet = _iWorkBook.CreateSheet(SuccessFulSheetName);
                ISheet failedSheet = _iWorkBook.CreateSheet(FailedSheetName);
                ISheet requestEnginoSheet = _iWorkBook.CreateSheet(requestEnginoName);
                // Type errortype = typeof(bx_batchrenewal_erroritem);
                //如果错误数据存在
                if (errorlist.Any())
                {
                    ISheet errorSheet = _iWorkBook.CreateSheet(UploadFail);
                    IRow errorRow = errorSheet.CreateRow(0);
                    Type typeerror = typeof(ExcelErrorData);
                    int errorj = 1;
                    PropertyInfo[] propertieserror = typeerror.GetProperties();
                    for (int i = 0; i < propertieserror.Length - 3; i++)
                    {
                        errorRow.CreateCell(i).SetCellValue(_errorExcel[propertieserror[i].Name] == null ? propertieserror[i].Name : _errorExcel[propertieserror[i].Name].ToString());
                    }
                    foreach (ExcelErrorData item in errorlist)
                    {
                        IRow dataRow = null;

                        dataRow = errorSheet.CreateRow(errorj);
                        errorj++;
                        for (int i = 0; i < propertieserror.Length - 3; i++)
                        {
                            dataRow.CreateCell(i).SetCellValue(propertieserror[i].GetValue(item, null) == null ? "" : propertieserror[i].GetValue(item, null).ToString());
                        }
                    }
                }
                IRow headerSuccessfulRow = successfulSheet.CreateRow(0);
                IRow headerFailedRow = failedSheet.CreateRow(0);
                IRow headerRequestEnginoRow = requestEnginoSheet.CreateRow(0);
                int j = 1;
                int y = 1;
                int z = 1;
                Type type = typeof(DownLoadExcel);
                PropertyInfo[] properties = type.GetProperties();
                for (int i = 0; i < properties.Length - 2; i++)
                {
                    headerSuccessfulRow.CreateCell(i).SetCellValue(_getExcel[properties[i].Name] == null ? properties[i].Name : _getExcel[properties[i].Name].ToString());

                    headerFailedRow.CreateCell(i).SetCellValue(_getExcel[properties[i].Name] == null ? properties[i].Name : _getExcel[properties[i].Name].ToString());
                    headerRequestEnginoRow.CreateCell(i).SetCellValue(_getExcel[properties[i].Name] == null ? properties[i].Name : _getExcel[properties[i].Name].ToString());
                }
                foreach (DownLoadExcel item in lists)
                {
                    IRow dataRow = null;
                    int itemStatus = Convert.ToInt32(item.GetType().GetProperty("ItemStatus").GetValue(item, null));

                    int IsTest = Convert.ToInt32(item.GetType().GetProperty("Istest").GetValue(item, null));

                    if (IsTest == 0)
                    {
                        if (itemStatus == 1)
                        {
                            dataRow = successfulSheet.CreateRow(j);
                            j++;
                            for (int i = 0; i < properties.Length - 2; i++)
                            {
                                dataRow.CreateCell(i).SetCellValue(properties[i].GetValue(item, null) == null ? "" : properties[i].GetValue(item, null).ToString());
                            }
                        }
                        else if (itemStatus == 2)
                        {
                            dataRow = failedSheet.CreateRow(y);
                            for (int i = 0; i < properties.Length - 2; i++)
                            {
                                dataRow.CreateCell(i).SetCellValue(properties[i].GetValue(item, null) == null ? "" : properties[i].GetValue(item, null).ToString());
                            }
                            y++;
                        }
                        else
                        {
                            dataRow = requestEnginoSheet.CreateRow(z);
                            z++;
                            for (int i = 0; i < properties.Length - 2; i++)
                            {
                                //未取到险种不显示数据
                                if (i == 6 || i == 7 || i == 8)
                                {
                                    dataRow.CreateCell(i).SetCellValue("");
                                }
                                else
                                {
                                    dataRow.CreateCell(i).SetCellValue(properties[i].GetValue(item, null) == null ? "" : properties[i].GetValue(item, null).ToString());
                                }
                            }
                        }
                    }
                }
                _iWorkBook.Write(ms);
                successfulSheet = null;
                failedSheet = null;
                headerSuccessfulRow = null;
                headerFailedRow = null;
                requestEnginoSheet = null;
                headerRequestEnginoRow = null;
            }
            else
            {
                //如果存在垃圾数据
                DownLoadExcel downexcel = lists.Find(delegate(DownLoadExcel elle) { return elle.Istest == 3; });
                ISheet successfulSheet = _iWorkBook.CreateSheet(SuccessFulSheetName);
                ISheet failedSheet = _iWorkBook.CreateSheet(FailedSheetName);
                ISheet requestEnginoSheet = _iWorkBook.CreateSheet(requestEnginoName);
                // Type errortype = typeof(bx_batchrenewal_erroritem);
                if (downexcel != null)
                {
                    IRow dataRow = null;
                    ISheet delSheet = _iWorkBook.CreateSheet(AlreadyDel);
                    IRow delRow = delSheet.CreateRow(0);
                    Type typedel = typeof(DownLoadExcel);
                    PropertyInfo[] delproperties = typedel.GetProperties();
                    int del = 1;
                    for (int i = 0; i < delproperties.Length - 2; i++)
                    {
                        delRow.CreateCell(i).SetCellValue(_getExcel[delproperties[i].Name] == null ? delproperties[i].Name : _getExcel[delproperties[i].Name].ToString());
                    }
                    foreach (DownLoadExcel item in lists)
                    {
                        int IsTest = Convert.ToInt32(item.GetType().GetProperty("Istest").GetValue(item, null));

                        if (IsTest == 3)
                        {
                            dataRow = delSheet.CreateRow(del);
                            del++;
                            for (int i = 0; i < delproperties.Length - 2; i++)
                            {

                                dataRow.CreateCell(i).SetCellValue(delproperties[i].GetValue(item, null) == null ? "" : delproperties[i].GetValue(item, null).ToString());
                            }
                        }
                    }
                }
                //如果错误数据存在
                if (errorlist.Any())
                {
                    ISheet errorSheet = _iWorkBook.CreateSheet(UploadFail);
                    IRow errorRow = errorSheet.CreateRow(0);
                    Type typeerror = typeof(ExcelErrorData);
                    int errorj = 1;
                    PropertyInfo[] propertieserror = typeerror.GetProperties();
                    for (int i = 0; i < propertieserror.Length - 3; i++)
                    {
                        errorRow.CreateCell(i).SetCellValue(_errorExcel[propertieserror[i].Name] == null ? propertieserror[i].Name : _errorExcel[propertieserror[i].Name].ToString());
                    }
                    foreach (ExcelErrorData item in errorlist)
                    {
                        IRow dataRow = null;

                        dataRow = errorSheet.CreateRow(errorj);
                        errorj++;
                        for (int i = 0; i < propertieserror.Length - 3; i++)
                        {
                            dataRow.CreateCell(i).SetCellValue(propertieserror[i].GetValue(item, null) == null ? "" : propertieserror[i].GetValue(item, null).ToString());
                        }
                    }
                }
                IRow headerSuccessfulRow = successfulSheet.CreateRow(0);
                IRow headerFailedRow = failedSheet.CreateRow(0);
                IRow headerRequestEnginoRow = requestEnginoSheet.CreateRow(0);
                int j = 1;
                int y = 1;
                int z = 1;
                Type type = typeof(DownLoadExcel);
                PropertyInfo[] properties = type.GetProperties();
                for (int i = 0; i < properties.Length - 2; i++)
                {
                    headerSuccessfulRow.CreateCell(i).SetCellValue(_getExcel[properties[i].Name] == null ? properties[i].Name : _getExcel[properties[i].Name].ToString());
                    headerFailedRow.CreateCell(i).SetCellValue(_getExcel[properties[i].Name] == null ? properties[i].Name : _getExcel[properties[i].Name].ToString());
                    headerRequestEnginoRow.CreateCell(i).SetCellValue(_getExcel[properties[i].Name] == null ? properties[i].Name : _getExcel[properties[i].Name].ToString());
                }
                foreach (DownLoadExcel item in lists)
                {
                    IRow dataRow = null;
                    int itemStatus = Convert.ToInt32(item.GetType().GetProperty("ItemStatus").GetValue(item, null));
                    int IsTest = Convert.ToInt32(item.GetType().GetProperty("Istest").GetValue(item, null));
                    if (IsTest == 0)
                    {
                        if (itemStatus == 1)
                        {
                            dataRow = successfulSheet.CreateRow(j);
                            j++;
                            for (int i = 0; i < properties.Length - 2; i++)
                            {
                                //未取到险种不显示数据
                                dataRow.CreateCell(i).SetCellValue(properties[i].GetValue(item, null) == null ? "" : properties[i].GetValue(item, null).ToString());
                            }
                        }
                        else if (itemStatus == 2)
                        {
                            dataRow = failedSheet.CreateRow(y);
                            for (int i = 0; i < properties.Length - 2; i++)
                            {
                                dataRow.CreateCell(i).SetCellValue(properties[i].GetValue(item, null) == null ? "" : properties[i].GetValue(item, null).ToString());
                            }
                            y++;
                        }
                        else
                        {
                            dataRow = requestEnginoSheet.CreateRow(z);
                            z++;
                            for (int i = 0; i < properties.Length - 2; i++)
                            {
                                //未取到险种不显示数据
                                if (i == 6 || i == 7 || i == 8)
                                {
                                    dataRow.CreateCell(i).SetCellValue("");
                                }
                                else
                                {
                                    dataRow.CreateCell(i).SetCellValue(properties[i].GetValue(item, null) == null ? "" : properties[i].GetValue(item, null).ToString());
                                }
                            }
                        }
                    }
                }
                _iWorkBook.Write(ms);
                successfulSheet = null;
                failedSheet = null;
                headerSuccessfulRow = null;
                headerFailedRow = null;
                requestEnginoSheet = null;
                headerRequestEnginoRow = null;
            }
            _iWorkBook = null;
            FileStream fs = new FileStream(workbookFile, FileMode.Create, FileAccess.Write);
            byte[] data = ms.ToArray();
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
            data = null;
            ms = null;
            fs = null;
        }
        public void GetExcel(List<T> lists, string sheetName, string workbookFile)
        {
            IWorkbook _iWorkBook = null;
            try
            {
                _iWorkBook = new XSSFWorkbook();
            }
            catch
            {
                _iWorkBook = new HSSFWorkbook();
            }
            MemoryStream ms = new MemoryStream();
            ISheet sheet = _iWorkBook.CreateSheet(sheetName);
            IRow headerRow = sheet.CreateRow(0);
            int j = 1;
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length - 3; i++)
            {
                headerRow.CreateCell(i).SetCellValue(_errorExcel[properties[i].Name] == null ? properties[i].Name : _errorExcel[properties[i].Name].ToString());
            }
            foreach (T item in lists)
            {
                IRow dataRow = sheet.CreateRow(j);
                j++;
                for (int i = 0; i < properties.Length - 3; i++)
                {
                    dataRow.CreateCell(i).SetCellValue(properties[i].GetValue(item, null) == null ? "" : properties[i].GetValue(item, null).ToString());
                }
            }
            _iWorkBook.Write(ms);
            sheet = null;
            headerRow = null;
            _iWorkBook = null;
            FileStream fs = new FileStream(workbookFile, FileMode.Create, FileAccess.Write);
            byte[] data = ms.ToArray();
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
            data = null;
            ms = null;
            fs = null;
        }

        /// <summary>  
        /// 导入Excel  
        /// </summary>  
        /// <param name="workbookFile">Excel所在路径</param>
        /// <param name="storePath">文件存放地址，包含文件名(最好加上时间戳避免文件名重复)</param>
        /// <param name="categories">客户类集合</param>
        /// <param name="agentInfos">传入代理人的集合</param>
        /// <param name="excelErrorDataList">输出的错误数据集合</param>
        /// <param name="message">返回的错误信息</param>
        /// <returns></returns>  
        public List<T> FromExcel(string workbookFile, string storePath, List<bx_customercategories> categories, List<bx_agent> agentInfos, out List<ExcelErrorData> excelErrorDataList, out string message)
        {
            IWorkbook iWorkBook = null;
            message = "";
            if (HttpDownload(workbookFile, storePath))
            {
                using (FileStream file = new FileStream(storePath, FileMode.Open, FileAccess.Read))
                {
                    if (workbookFile.IndexOf(".xlsx", StringComparison.Ordinal) > 0) // 2007版本
                        iWorkBook = new XSSFWorkbook(file);
                    else if (workbookFile.IndexOf(".xls", StringComparison.Ordinal) > 0) // 2003版本
                        iWorkBook = new HSSFWorkbook(file);
                }
                List<T> lists = new List<T>();
                ISheet sheet = iWorkBook.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(_startIndex);
                excelErrorDataList = new List<ExcelErrorData>();
                if (headerRow == null || headerRow.GetCell(headerRow.FirstCellNum) == null || headerRow.GetCell(headerRow.FirstCellNum).ToString() != "车牌号")
                {
                    message = "<font style=\"color:red\">上传失败：</font>您上传的模板不正确";
                    return null;
                }
                //如果模板已更新
                if (headerRow.GetCell((headerRow.FirstCellNum) + 5).ToString() == "客户手机号" || headerRow.GetCell((headerRow.FirstCellNum) + 5).ToString() == "客户电话1" || headerRow.GetCell((headerRow.FirstCellNum) + 12).ToString() != "客户类别" || headerRow.GetCell((headerRow.FirstCellNum) + 13).ToString() != "业务员姓名" || headerRow.GetCell((headerRow.FirstCellNum) + 4).ToString() != "初登日期")
                {
                    message = "<font style=\"color:red\">上传失败：</font>模板已更新，请重新下载新模板";
                    return null;
                }
                int cellCount = headerRow.LastCellNum;
                List<T> bag = new List<T>();

                var totalCount = sheet.LastRowNum - sheet.FirstRowNum - _startIndex;
                if (totalCount <= 0)
                {
                    message = "<font style=\"color:red\">上传失败：</font>无任何数据";
                    return null;
                }
                GetListData(sheet, headerRow, sheet.FirstRowNum + _startIndex + 1, sheet.LastRowNum, bag, excelErrorDataList, categories, agentInfos);
                if (bag.Count <= 0)
                {
                    message = "<font style=\"color:red\">上传失败：</font>请在模板中查看上传规则，并检查数据正确性";
                }
                return bag.ToList();
            }
            else
            {
                message = "<font style=\"color:red\">读取出错</font>无任何数据";
                excelErrorDataList = new List<ExcelErrorData>();
                return null;
            }
        }
        object valueType(Type t, ICell value)
        {
            object o = null;
            string strt = t.ToString();
            if (t.Name == "Nullable`1")
            {
                strt = t.GetGenericArguments()[0].Name;
            }
            switch (strt)
            {
                case "System.Decimal":
                    o = decimal.Parse(value.NumericCellValue.ToString().Trim());
                    break;
                case "System.Int32":
                    o = int.Parse(value.NumericCellValue.ToString().Trim());
                    break;
                case "System.Float":
                    o = float.Parse(value.NumericCellValue.ToString().Trim());
                    break;
                case "System.DateTime":

                    if (HSSFDateUtil.IsCellDateFormatted(value))
                    {
                        o = value.DateCellValue;
                    }
                    else
                    {
                        o = null;
                    }
                    break;
                case "System.Int64":
                    o = Int64.Parse(value.NumericCellValue.ToString().Trim());
                    break;
                case "Int64":
                    o = value == null ? null : (Int64.Parse(value.NumericCellValue.ToString().Trim())) as long?;
                    break;
                case "DateTime":
                    if (HSSFDateUtil.IsCellDateFormatted(value))
                    {
                        o = value.DateCellValue;
                    }
                    else
                    {
                        o = null;
                    }

                    break;
                default:
                    o = value.ToString().Trim();
                    break;
            }
            return o;
        }
        #region 优化方法GetListData
        int type = 0;//这个全局变量在下面用到
        public List<T> GetListData(ISheet sheet, IRow headerRow, int startIndex, int endIndex, List<T> lists, List<ExcelErrorData> errorDataList, List<bx_customercategories> categories, List<bx_agent> agent)
        {
            int out_i = startIndex;
            int out_endIndex = endIndex;
            for (int i = startIndex; i <= endIndex; i++)
            {
                //1 去掉空行  如果返回false从新循环
                if (RemoveEmptyRow(sheet, endIndex, i, out out_i, out out_endIndex))
                {
                    Dictionary<int, string> excelDictionary = null;
                    if (CellValuation(sheet, i, out excelDictionary))//2 给每个单元格判断空 不为空 赋值.ToString().Trim()  如果返回false从新循环
                    {
                        if (SpecificCheckCell(excelDictionary, i, sheet, errorDataList, categories, agent))//3 执行具体的校验  这一步下面还要细分8
                        {
                            ConversionCell(excelDictionary, i, sheet, headerRow, categories, agent, lists);//4 这个是一些转化      车架号转大写
                        }
                    }
                }
                i = out_i;
                endIndex = out_endIndex;
            }
            return lists;
        }
        #region 这个是我写的优化方法    校验规则 的内部的4个小方法
        //去掉空行
        public bool RemoveEmptyRow(ISheet sheet, int endIndex, int i, out int out_i, out int out_endIndex)
        {
            if (sheet.GetRow(endIndex) == null)
            {
                out_i = --i;
                out_endIndex = --endIndex;
                return false;
            }
            for (int j = 0; j <= 14; j++)
            {
                bool c0_c14 = sheet.GetRow(endIndex).GetCell(j) == null || string.IsNullOrWhiteSpace(sheet.GetRow(endIndex).GetCell(j).ToString());
                if (!c0_c14)
                {
                    out_i = i;
                    out_endIndex = endIndex;
                    return true;
                }
            }
            out_i = --i;
            out_endIndex = --endIndex;
            return false;
        }

        //取单元格赋值
        public bool CellValuation(ISheet sheet, int i, out Dictionary<int, string> excelDictionary)
        {
            IRow row = sheet.GetRow(i) ?? sheet.CreateRow(i);
            try
            {
                excelDictionary = new Dictionary<int, string>();
                for (int j = 0; j <= 14; j++)
                {
                    var cell = row.GetCell(j);
                    var cellValue = cell == null ? string.Empty : cell.ToString().Trim();
                    excelDictionary.Add(j, cellValue);
                }
                return true;
            }
            catch (Exception ex)
            {
                excelDictionary = null;
                return false;
            }
        }
        public bool SpecificCheckCell(Dictionary<int, string> excelDictionary, int i, ISheet sheet, List<ExcelErrorData> errorDataList, List<bx_customercategories> categories, List<bx_agent> agent)
        {
            //type = 0;//有车架号和发动机号
            Hashtable htLicenseNos = new Hashtable();
            StringBuilder sb = new StringBuilder();
            //这里具体校验分了八个步骤
            //1去空行
            if (!SpecificCheckCell_ToEmpty(excelDictionary))
            {
                return false;
            }
            //2验证  车牌 车架 发动机
            SpecificCheckCell_LicenseNo_VinNo_EngineNo(excelDictionary, sb);
            //3读取项目里 验证保险公司 
            SpecificCheckCell_LastYearSource(excelDictionary, sb);
            //4验证客户类别
            SpecificCheckCell_CategoryInfo(excelDictionary, sb, categories);
            //5业务员账号
            SpecificCheckCell_Agent(excelDictionary, sb, agent);
            //6交强险 和 商业险 初登日期
            SpecificCheckCell_ForceEndDate_BizEndDate(excelDictionary, sb);
            //7单元格最大长度
            SpecificCheckCell_CellLength(sheet, sb, i);
            //8判断 substring>0执行拼接和进入错误模型
            if (!SpecificCheckCell_ToErrorDataList(excelDictionary, sheet, sb, i, errorDataList))
            {
                return false;
            }
            return true;
        }
        #region   校验  下面的八个方法
        public bool SpecificCheckCell_ToEmpty(Dictionary<int, string> excelDictionary)
        {
            if (string.IsNullOrEmpty(excelDictionary[0]) && string.IsNullOrEmpty(excelDictionary[1]) &&
                string.IsNullOrEmpty(excelDictionary[2]) && string.IsNullOrEmpty(excelDictionary[3]) &&
                string.IsNullOrEmpty(excelDictionary[4]) && string.IsNullOrEmpty(excelDictionary[5]) &&
                string.IsNullOrEmpty(excelDictionary[6]) && string.IsNullOrEmpty(excelDictionary[7]) &&
                string.IsNullOrEmpty(excelDictionary[8]) && string.IsNullOrEmpty(excelDictionary[9]) &&
                string.IsNullOrEmpty(excelDictionary[10]) && string.IsNullOrEmpty(excelDictionary[11]) &&
                string.IsNullOrEmpty(excelDictionary[12]) && string.IsNullOrEmpty(excelDictionary[13]) && string.IsNullOrEmpty(excelDictionary[14])
                )
            {
                return false;
            }
            return true;
        }
        public void SpecificCheckCell_LicenseNo_VinNo_EngineNo(Dictionary<int, string> excelDictionary, StringBuilder sb)
        {
            if (excelDictionary[0] == null || string.IsNullOrEmpty(excelDictionary[0]))
            {
                if ((string.IsNullOrEmpty(excelDictionary[1])) && (string.IsNullOrEmpty(excelDictionary[2])))
                {
                    sb.Append("车牌号为空时，车架号、发动机号不能为空；");
                }
                type = 1;
            }
            //if (excelDictionary[0] == null || string.IsNullOrEmpty(excelDictionary[0]))
            //{
            //    if ((secondCell == null || string.IsNullOrEmpty(secondCellValue)) && (thirdCell == null || string.IsNullOrEmpty(thirdCellValue)))
            //    {
            //        sb.Append("车牌号为空时，车架号、发动机号不能为空；");
            //    }
            //    type = 1;
            //}
            //if (excelDictionary[0] == null || string.IsNullOrEmpty(excelDictionary[0]))
            //{
            //    if (excelDictionary[1] == null || string.IsNullOrEmpty(excelDictionary[1]))
            //    {
            //        sb.Append("车牌号为空时，车架号不能为空；");
            //    }
            //    type = 1;
            //}
            if (!string.IsNullOrEmpty(excelDictionary[0]) && !(new Regex(@"^[\u4e00-\u9fa5]{1}[A-z]{1}[\da-zA-Z]{4,5}$").IsMatch((excelDictionary[0]))))
            {
                sb.Append("车牌号不符合规则；");
            }
            //正则校验简化
            if (!string.IsNullOrEmpty(excelDictionary[1]) && !(new Regex(@"^(?=.*\d)(?=.*[a-zA-Z])[\da-zA-Z]+$").IsMatch((excelDictionary[1]))))
            {
                sb.Append("车架号不符合规则(只能由数字和字母组合而成)；");
            }
            ////当车牌号为空的时候
            //if (string.IsNullOrEmpty(excelDictionary[1]))
            //{
            //    if (!string.IsNullOrEmpty(excelDictionary[2]))
            //    {
            //        sb.Append("车架号或发动机号未上传完整；");
            //    }
            //}

            //if (string.IsNullOrEmpty(excelDictionary[2]))
            //{
            //    if (!string.IsNullOrEmpty(excelDictionary[1]))
            //    {
            //        sb.Append("车架号或发动机号未上传完整；");
            //        //车架号不等于17位
            //    }
            //}
            if (!string.IsNullOrEmpty(excelDictionary[1]))
            {
                if (excelDictionary[1].Length != 17)
                {
                    sb.Append("车架号不正确；");
                }
            }
        }
        public void SpecificCheckCell_LastYearSource(Dictionary<int, string> excelDictionary, StringBuilder sb)
        {

            if (excelDictionary[5] != null && !string.IsNullOrEmpty(excelDictionary[5]))
            {
                //读取XML文件获取数据
                Dictionary<int, string> yearsources = ReadXmlNodes("YearSource.xml");
                string newyearsource = excelDictionary[5].ToString().Trim();
                if (!yearsources.Values.Contains(newyearsource))
                {
                    sb.Append("保险公司名称无法识别，请参照模板中保险公司名称示例填写名称 ；");
                }
            }
        }
        public void SpecificCheckCell_CategoryInfo(Dictionary<int, string> excelDictionary, StringBuilder sb, List<bx_customercategories> categories)
        {
            //未设置客户类别
            if (categories.Count == 0)
            {
                if (excelDictionary[12] != null && !string.IsNullOrEmpty(excelDictionary[12]))
                {
                    //客户类别
                    sb.Append("未设置客户类别，请先设置；");
                }
            }
            else
            {
                if (excelDictionary[12] != null && !string.IsNullOrEmpty(excelDictionary[12]))
                {
                    //如果未查到客户类别
                    if (categories.Where(x => x.CategoryInfo == excelDictionary[12]).ToList().Count == 0)
                    {
                        //客户类别
                        sb.Append("上传的客户类别不包含于设置的类别中，您可以在系统设置中添加此类别；");
                    }
                }
            }
        }
        public void SpecificCheckCell_Agent(Dictionary<int, string> excelDictionary, StringBuilder sb, List<bx_agent> agent)
        {
            //如果业务员账号与姓名都未录入
            if (!string.IsNullOrWhiteSpace(excelDictionary[13]) || !string.IsNullOrWhiteSpace(excelDictionary[14]))
            {
                if (excelDictionary[13] != null & !string.IsNullOrEmpty(excelDictionary[13]) & excelDictionary[14] != null & !string.IsNullOrEmpty(excelDictionary[14]))
                {
                    //如果未该客户账号
                    if (agent.Where(x => x.AgentAccount == excelDictionary[14]).ToList().Count == 0)
                    {
                        //客户账号
                        sb.Append("系统没有查到该业务员账号或姓名,请核对姓名或者账号是否准确无误；");
                    }

                }
                if (excelDictionary[12] != null & !string.IsNullOrEmpty(excelDictionary[13]) & string.IsNullOrEmpty(excelDictionary[14]))
                {
                    //如果未该客户姓名查到多条
                    if (agent.Where(x => x.AgentName == excelDictionary[13]).ToList().Count > 1)
                    {
                        //客户账号
                        sb.Append("有多个相同姓名的业务员,请直接填写业务员账号再上传；");
                    }
                    //如果未该客户姓名查到多条
                    if (agent.Where(x => x.AgentName == excelDictionary[13]).ToList().Count == 0)
                    {
                        //客户账号
                        sb.Append("系统没有查到该业务员账号或姓名,请核对姓名或者账号是否准确无误；");
                    }

                }
                else
                {
                    //如果未该客户账号
                    if (agent.Where(x => x.AgentAccount == excelDictionary[14]).ToList().Count == 0)
                    {
                        //客户账号
                        sb.Append("系统没有查到该业务员账号或姓名,请核对姓名或者账号是否准确无误；");
                    }
                }
            }
        }
        public void SpecificCheckCell_ForceEndDate_BizEndDate(Dictionary<int, string> excelDictionary, StringBuilder sb)
        {
            if (!string.IsNullOrEmpty(excelDictionary[6]))
            {
                try
                {
                    DateTime.Parse(excelDictionary[6]);
                }
                catch
                {
                    sb.Append("交强险到期时间录入不正确；");
                }

            }
            if (!string.IsNullOrEmpty(excelDictionary[7]))
            {
                try
                {
                    DateTime.Parse(excelDictionary[7]);
                }
                catch
                {
                    sb.Append("商业险到期时间录入不正确；");
                }
            }
            if (!string.IsNullOrEmpty(excelDictionary[4]))
            {
                try
                {
                    DateTime.Parse(excelDictionary[4]);
                }
                catch
                {
                    sb.Append("初登日期录入不正确；");
                }
            }
        }
        public void SpecificCheckCell_CellLength(ISheet sheet, StringBuilder sb, int i)
        {
            if (!CheckExcelRowData(sheet, i, 2, 100))
            {
                sb.Append("发动机号最大长度为100；");
            }
            if (!CheckExcelRowData(sheet, i, 3, 50))
            {
                sb.Append("品牌型号最大长度为100；");
            }
            if (!CheckExcelRowData(sheet, i, 4, 50))
            {
                sb.Append("初登日期最大长度为200；");
            }
            if (!CheckExcelRowData(sheet, i, 5, 200))
            {
                sb.Append("去年投保公司最大长度为200；");
            }
            if (!CheckExcelRowData(sheet, i, 6, 50))
            {
                sb.Append("交强险到期时间最大长度为50；");
            }
            if (!CheckExcelRowData(sheet, i, 7, 50))
            {
                sb.Append("商业险到期时间最大长度为50；");
            }

            if (!CheckExcelRowData(sheet, i, 8, 200))
            {
                sb.Append("客户姓名最大长度为100；");
            }
            if (!CheckExcelRowData(sheet, i, 9, 20))
            {
                sb.Append("客户电话1最大长度为20；");
            }
            if (!CheckExcelRowData(sheet, i, 10, 20))
            {
                sb.Append("客户电话2最大长度为20；");
            }
            if (!CheckExcelRowData(sheet, i, 11, 200))
            {
                sb.Append("客户备注最大长度为200；");
            }
        }
        public bool SpecificCheckCell_ToErrorDataList(Dictionary<int, string> excelDictionary, ISheet sheet, StringBuilder sb, int i, List<ExcelErrorData> errorDataList)
        {
            ExcelErrorData excelErrorData = new ExcelErrorData();
            if (sb.ToString().Length > 0)
            {
                excelErrorData.ErrorMsg = sb.ToString();
                excelErrorData.LicenseNo = excelDictionary[0] == null ? null : excelDictionary[0].ToString().Length > 20 ? excelDictionary[0].ToString().Substring(0, 20) : excelDictionary[0].ToString();
                excelErrorData.VinNo = excelDictionary[1] == null ? null : excelDictionary[1].ToString().Length > 100 ? excelDictionary[1].ToString().Substring(0, 100) : excelDictionary[1].ToString();
                excelErrorData.EngineNo = excelDictionary[2] == null ? null : excelDictionary[2].ToString().Length > 50 ? excelDictionary[2].ToString().Substring(0, 50) : excelDictionary[2].ToString();
                excelErrorData.MoldName = excelDictionary[3] == null ? null : excelDictionary[3].ToString().Length > 50 ? excelDictionary[3].ToString().Substring(0, 50) : excelDictionary[3].ToString();
                excelErrorData.MoldName = excelDictionary[4] == null ? null : excelDictionary[4].ToString().Length > 50 ? excelDictionary[4].ToString().Substring(0, 50) : excelDictionary[4].ToString();
                excelErrorData.LastYearSource = excelDictionary[5] == null ? null : excelDictionary[5].ToString().Length > 50 ? excelDictionary[4].ToString().Substring(0, 50) : excelDictionary[5].ToString();
                excelErrorData.ForceEndDate = excelDictionary[6] == null ? null : excelDictionary[6].ToString().Length > 50 ? excelDictionary[6].ToString().Substring(0, 50) : excelDictionary[6].ToString();
                excelErrorData.BizEndDate = excelDictionary[7] == null ? null : excelDictionary[7].ToString().Length > 50 ? excelDictionary[7].ToString().Substring(0, 50) : excelDictionary[7].ToString();
                excelErrorData.CustomerName = excelDictionary[8] == null ? null : excelDictionary[8].ToString().Length > 100 ? excelDictionary[7].ToString().Substring(0, 100) : excelDictionary[8].ToString();
                excelErrorData.Mobile = excelDictionary[9] == null ? null : excelDictionary[9].ToString().Length > 20 ? excelDictionary[9].ToString().Substring(0, 20) : excelDictionary[9].ToString();
                excelErrorData.MobileOther = excelDictionary[10] == null ? null : excelDictionary[10].ToString().Length > 20 ? excelDictionary[10].ToString().Substring(0, 20) : excelDictionary[10].ToString();
                excelErrorData.Remark = excelDictionary[11] == null ? null : excelDictionary[11].ToString().Length > 200 ? excelDictionary[11].ToString().Substring(0, 200) : excelDictionary[11].ToString();
                excelErrorData.CategoryInfo = excelDictionary[12] == null ? null : excelDictionary[12].ToString().Length > 200 ? excelDictionary[12].ToString().Substring(0, 200) : excelDictionary[12].ToString();
                excelErrorData.SalesManName = excelDictionary[13] == null ? null : excelDictionary[13].ToString().Length > 50 ? excelDictionary[12].ToString().Substring(0, 50) : excelDictionary[13].ToString();
                excelErrorData.SalesManAccount = excelDictionary[14] == null ? null : excelDictionary[14].ToString().Length > 100 ? excelDictionary[13].ToString().Substring(0, 100) : excelDictionary[14].ToString();
                //SetExcelErrorDataInfo(excelErrorData, sheet, i);
                errorDataList.Add(excelErrorData);
                return false;
            }
            return true;
        }
        #endregion
        //转化
        public bool ConversionCell(Dictionary<int, string> excelDictionary, int i, ISheet sheet, IRow headerRow, List<bx_customercategories> categories, List<bx_agent> agent, List<T> lists)
        {
            //int type = 0;//有车架号和发动机号
            IRow row = sheet.GetRow(i);
            Hashtable htLicenseNos = new Hashtable();
            Dictionary<int, string> yearsources = ReadXmlNodes("YearSource.xml");
            var t = Activator.CreateInstance<T>();
            var properties = t.GetType().GetProperties();
            if (string.IsNullOrWhiteSpace(excelDictionary[0]) || (!string.IsNullOrWhiteSpace(excelDictionary[0].ToUpper()) && !htLicenseNos.Contains(excelDictionary[0].ToUpper())))
            {
                //其次  hash  value 没有重复数据 
                if (string.IsNullOrWhiteSpace(excelDictionary[1]) || (!string.IsNullOrWhiteSpace(excelDictionary[1].ToUpper()) && !htLicenseNos.Contains(excelDictionary[1].ToUpper())))
                {
                    if (!string.IsNullOrWhiteSpace(excelDictionary[0].ToUpper()) && !string.IsNullOrWhiteSpace(excelDictionary[1].ToUpper()))
                    {
                        htLicenseNos.Add(excelDictionary[0].ToUpper(), null);
                        htLicenseNos.Add(excelDictionary[1].ToUpper(), null);
                    }
                    else if (string.IsNullOrWhiteSpace(excelDictionary[0].ToUpper()) && !string.IsNullOrWhiteSpace(excelDictionary[1].ToUpper()))
                    {
                        htLicenseNos.Add(excelDictionary[1].ToUpper(), null);
                    }
                    else if (!string.IsNullOrWhiteSpace(excelDictionary[0].ToUpper()) && string.IsNullOrWhiteSpace(excelDictionary[1].ToUpper()))
                    {
                        htLicenseNos.Add(excelDictionary[0].ToUpper(), null);
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            foreach (PropertyInfo column in properties)
            {
                int j = headerRow.Cells.FindIndex(delegate(ICell c)
                {
                    return c.StringCellValue == (_fromEnglishToChineseForBatchRenewalItem[column.Name] == null ? column.Name : _fromEnglishToChineseForBatchRenewalItem[column.Name].ToString());
                });
                if (j >= 0 && row.GetCell(j) != null)
                {
                    object value = valueType(column.PropertyType, row.GetCell(j));
                    if (column.Name == "LicenseNo")
                    {
                        value = value.ToString().ToUpper();
                    }
                    
                    //车架号转大写
                    if (column.Name == "VinNo")
                    {
                        value = value.ToString().ToUpper();
                    }
                    //去年投保
                    if (column.Name == "LastYearSource")
                    {
                        string va = value.ToString();
                        if (yearsources.Values.Contains(va))
                        {
                            var firstKey = yearsources.FirstOrDefault(q => q.Value == va).Key;
                            value = firstKey.ToString();
                        }
                        else
                        {
                            value = "-1";
                        }
                    }
                    //客户类别
                    if (column.Name == "CategoryInfo")
                    {
                        string va = value.ToString();
                        if (!string.IsNullOrWhiteSpace(va))
                        {
                            var firstKey = categories.FirstOrDefault(q => q.CategoryInfo == va).Id;
                            value = firstKey.ToString();
                        }
                        else
                        {
                            value = "0";
                        }
                    }
                    if (column.Name == "SalesManName")//业务员姓名
                    {
                        string va = value.ToString();
                        if (!string.IsNullOrWhiteSpace(va))
                        {
                            if (agent.Where(x => x.AgentName == va).ToList().Count > 0)
                            {
                                var firstKey = agent.FirstOrDefault(q => q.AgentName == va).Id;
                                value = va + "," + firstKey.ToString();
                            }
                            else
                            {
                                value = va + "," + "0";
                            }
                        }
                        else
                        {
                            value = "";
                        }
                    }
                    if (column.Name == "SalesManAccount")//业务员账号
                    {
                        string va = value.ToString();
                        if (!string.IsNullOrWhiteSpace(va))
                        {
                            var firstKey = agent.FirstOrDefault(q => q.AgentAccount == va).Id;
                            value = va + "," + firstKey.ToString();
                        }
                        else
                        {
                            value = "";
                        }
                    }
                    column.SetValue(t, value, null);
                }
            }
            if (type == 1)
            {
                t.GetType().GetProperty("IsLastYearNewCar").SetValue(t, 2, null);
            }
            else
            {
                t.GetType().GetProperty("IsLastYearNewCar").SetValue(t, 1, null);
            }
            lists.Add(t);
            return true;
        }
        #endregion
        #endregion
        public Dictionary<int, string> ReadXmlNodes(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                string str = System.AppDomain.CurrentDomain.BaseDirectory;

                xmlDoc.Load(str + @"/YearSource.xml");
                XmlNodeList xnList = xmlDoc.SelectNodes("//YearSource");
                var ltSoure = new Dictionary<int, string>();
                foreach (XmlNode xn in xnList)
                {
                    int YearSourceId = Convert.ToInt32((xn.SelectSingleNode("YearSourceId")).InnerText);
                    string YearSourceName = (xn.SelectSingleNode("YearSourceName")).InnerText;
                    ltSoure.Add(YearSourceId, YearSourceName);
                }
                return ltSoure;
            }
            catch (Exception e)
            {
                //显示错误信息  
                Console.WriteLine(e.Message);
                return null;
            }
        }
        private bool CheckExcelRowData(ISheet sheet, int rowIndex, int cellIndex, int dataLength)
        {
            bool isPassChecked = true;
            if (sheet.GetRow(rowIndex).GetCell(cellIndex) != null && !string.IsNullOrEmpty(sheet.GetRow(rowIndex).GetCell(cellIndex).ToString()) && sheet.GetRow(rowIndex).GetCell(cellIndex).ToString().Trim().Length > dataLength)
            {
                isPassChecked = false;
            }
            return isPassChecked;
        }
        /// <summary>
        /// http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="storePath">文件存放地址，包含文件名(最好加上时间戳避免文件名重复)</param>
        /// <returns></returns>
        public bool HttpDownload(string url, string storePath)
        {
            try
            {
                url = _fileServerURL + url;
                _logInfo.Info("读取文件路径" + url);
                string tempPath = System.IO.Path.GetDirectoryName(storePath);
                System.IO.Directory.CreateDirectory(tempPath);  //创建临时文件目录
                string tempFile = tempPath + @"\" + System.IO.Path.GetFileName(storePath) + ".temp"; //临时文件
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);    //存在则删除
                }
                if (File.Exists(storePath))
                {
                    File.Delete(storePath);    //存在则删除
                }
                FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                //Stream stream = new FileStream(tempFile, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    //stream.Write(bArr, 0, size);
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                //stream.Close();
                fs.Close();
                responseStream.Close();
                System.IO.File.Move(tempFile, storePath);
                return true;
            }
            catch (Exception ex)
            {
                _logInfo.Info("读取批量续保文件路径" + url + " 错误信息：" + ex.Message);
                return false;
            }
        }
    }
}
