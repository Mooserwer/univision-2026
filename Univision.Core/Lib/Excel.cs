using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;

using System.IO;
using System.Data;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using System.Web;

namespace Univision.Core.Lib
{
  /// <summary>
  /// 엑셀 조회 및 저장 클래스
  /// </summary>
  public partial class Excel
  {
    /// <summary>
    /// 엑셀 데이터를 Dataset으로 변환 시트는 DataTable로 구분
    /// </summary>
    /// <param name="st"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public DataSet ReadToDataSet(HttpPostedFileBase file)
    {
      if (file != null)
      {
        var fileName = Path.GetFileName(file.FileName);
        var fileExtension = Path.GetExtension(file.FileName);
        using (Stream st = file.InputStream)
        {
          DataSet ds = new DataSet();
          IExcelDataReader reader = null;

          if (fileExtension == ".xls")
          {
            reader = ExcelReaderFactory.CreateBinaryReader(st);
          }
          else if (fileExtension == ".xlsx")
          {
            reader = ExcelReaderFactory.CreateOpenXmlReader(st);
          }

          ds = reader.AsDataSet(new ExcelDataSetConfiguration()
          {
            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
            {
              UseHeaderRow = true
            }
          });
          st.Close();
          st.Dispose();
          return ds;
        }
      }

      return null;
    }

    /// <summary>
    /// 엑셀 내려주기 
    /// </summary>
    /// <param name="ds">데이터셋</param>
    /// <param name="fileNameWithoutExtension">확장자 없는 파일명만</param>
    /// <returns></returns>
    public FileContentResult WriteExcel(DataSet ds, string fileNameWithoutExtension, string sheetName = null)
    {
      ExcelPackage pck = new ExcelPackage();
      if (ds != null && ds.Tables.Count > 0)
      {

        string tableName = string.Empty;
        int dtRow = 0;
        ExcelWorksheet sheet = null;
        foreach (DataTable dt in ds.Tables)
        {
          if (!string.IsNullOrEmpty(sheetName))
            tableName = sheetName + (dtRow + 1).ToString();

          if (string.IsNullOrEmpty(tableName))
            tableName = "Sheet" + (dtRow + 1).ToString();

          sheet = pck.Workbook.Worksheets.Add(tableName);
          Color color = Color.FromArgb(180, 198, 231);

          int rowCnt = 1;

          for (int i = 0; i < dt.Rows.Count; i++)
          {
            for (int j = 0; j < dt.Rows[i].ItemArray.Length; j++)
            {
              if (i == 0)
              {
                sheet.Cells[1, j + 1].Value = dt.Columns[j].ColumnName;
                sheet.Cells[1, j + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[1, j + 1].Style.Fill.BackgroundColor.SetColor(color);
                sheet.Cells[1, j + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[1, j + 1].Style.Font.Bold = true;
                sheet.Cells[1, j + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[1, j + 1].AutoFitColumns();

              }

              sheet.Cells[rowCnt + 1, j + 1].Value = dt.Rows[i][j];
              sheet.Cells[rowCnt + 1, j + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
              sheet.Cells[rowCnt + 1, j + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
              sheet.Cells[rowCnt + 1, j + 1].AutoFitColumns();
            }
            rowCnt++;
          }

          dtRow++;
        }
      }
      string fileName = fileNameWithoutExtension + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx";


      var result = new FileContentResult(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
      result.FileDownloadName = fileName;
      //return new FileContentResult(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", File_Name);
      return result;
    }

    public FileContentResult WriteExcel(DataSet ds, int[] widths, string fileNameWithoutExtension, string sheetName = null)
    {
      ExcelPackage pck = new ExcelPackage();
      if (ds != null && ds.Tables.Count > 0)
      {

        string tableName = string.Empty;
        int dtRow = 0;
        ExcelWorksheet sheet = null;
        foreach (DataTable dt in ds.Tables)
        {
          if (!string.IsNullOrEmpty(sheetName))
            tableName = sheetName + (dtRow + 1).ToString();

          if (string.IsNullOrEmpty(tableName))
            tableName = "Sheet" + (dtRow + 1).ToString();

          sheet = pck.Workbook.Worksheets.Add(tableName);
          Color color = Color.FromArgb(180, 198, 231);

          int rowCnt = 1;

          for (int i = 0; i < dt.Rows.Count; i++)
          {
            for (int j = 0; j < dt.Rows[i].ItemArray.Length; j++)
            {
              if (i == 0)
              {
                sheet.Cells[1, j + 1].Value = dt.Columns[j].ColumnName;
                sheet.Cells[1, j + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[1, j + 1].Style.Fill.BackgroundColor.SetColor(color);
                sheet.Cells[1, j + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[1, j + 1].Style.Font.Bold = true;
                sheet.Cells[1, j + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[1, j + 1].AutoFitColumns();

              }
              
              sheet.Cells[rowCnt + 1, j + 1].Value = dt.Rows[i][j];
              sheet.Cells[rowCnt + 1, j + 1].Style.WrapText = true;
              sheet.Cells[rowCnt + 1, j + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
              sheet.Cells[rowCnt + 1, j + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
              //sheet.Cells[rowCnt + 1, j + 1].AutoFitColumns();
            }
            rowCnt++;
          }
          
          dtRow++;
        }
        for (int i = 1; i <= sheet.Dimension.End.Column && i <= widths.Length; i++)
        {
          sheet.Column(i).Width = widths[i-1];
        }
      }

      string fileName = fileNameWithoutExtension + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx";


      var result = new FileContentResult(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
      result.FileDownloadName = fileName;
      //return new FileContentResult(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", File_Name);
      return result;
    }

    /// <summary>
    /// 엑셀 내려주기
    /// </summary>
    /// <param name="dt">데이터 테이블</param>
    /// <param name="fileNameWithoutExtension">확장자 없는 파일명만</param>
    /// <param name="sheetName">시트 명</param>
    /// <returns></returns>
    public FileContentResult WriteExcel(DataTable dt, string fileNameWithoutExtension, string sheetName = null)
    {
      DataSet ds = new DataSet();
      ds.Tables.Add(dt);
      return WriteExcel(ds, fileNameWithoutExtension, sheetName);
    }

    public FileContentResult WriteExcel(DataTable dt, int[] widths, string fileNameWithoutExtension, string sheetName = null)
    {
      DataSet ds = new DataSet();
      ds.Tables.Add(dt);
      return WriteExcel(ds, widths, fileNameWithoutExtension, sheetName);
    }

  }

  public partial class Excel<T> where T : class
  {
    /// <summary>
    /// 리스트를 엑셀려 내려주는 함수 
    /// </summary>
    /// <param name="list">클래스 리스트</param>
    /// <param name="fileNameWithoutExtension">확장자 없는 파일명만</param>
    /// <param name="sheetName">시트 명</param>
    /// <returns></returns>
    public FileContentResult WriteExcel(List<T> list, string fileNameWithoutExtension, string sheetName = null)
    {
      Type type = typeof(T);
      var properties = type.GetProperties();

      DataTable dt = new DataTable();

      string displayName = string.Empty;

      foreach (PropertyInfo info in properties)
      {
        displayName = GetAttributeDisplayName(info);
        if (string.IsNullOrEmpty(displayName))
          displayName = info.Name;

        dt.Columns.Add(new DataColumn(displayName, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
      }

      foreach (T entity in list)
      {
        object[] values = new object[properties.Length];
        for (int i = 0; i < properties.Length; i++)
        {
          values[i] = properties[i].GetValue(entity);
        }

        dt.Rows.Add(values);
      }

      Excel excel = new Excel();
      return excel.WriteExcel(dt, fileNameWithoutExtension, sheetName);


    }

    public FileContentResult WriteExcel(List<T> list, List<string> include_field, string fileNameWithoutExtension, string sheetName = null)
    {
      Type type = typeof(T);
      var properties = type.GetProperties();

      DataTable dt = new DataTable();

      string displayName = string.Empty;

      foreach (PropertyInfo info in properties)
      {
        displayName = GetAttributeDisplayName(info);
        if (string.IsNullOrEmpty(displayName))
          displayName = info.Name;

        if (include_field.Contains(displayName))
        {        
          dt.Columns.Add(new DataColumn(displayName.Replace('_', ' '), Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
        }
      }

      foreach (T entity in list)
      {
        object[] values = new object[include_field.Count];
        int idx = 0;
        for (int i = 0; i < properties.Length; i++)
        {
          if (include_field.Contains(properties[i].Name))
          {
            values[idx++] = properties[i].GetValue(entity);
          }
        }

        dt.Rows.Add(values);
      }

      Excel excel = new Excel();
      return excel.WriteExcel(dt, fileNameWithoutExtension, sheetName);


    }

    public FileContentResult WriteExcel(List<T> list, int[] widths, string fileNameWithoutExtension, string sheetName = null)
    {
      Type type = typeof(T);
      var properties = type.GetProperties();

      DataTable dt = new DataTable();

      string displayName = string.Empty;

      foreach (PropertyInfo info in properties)
      {
        displayName = GetAttributeDisplayName(info);
        if (string.IsNullOrEmpty(displayName))
          displayName = info.Name;

        dt.Columns.Add(new DataColumn(displayName, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
      }

      foreach (T entity in list)
      {
        object[] values = new object[properties.Length];
        for (int i = 0; i < properties.Length; i++)
        {
          values[i] = properties[i].GetValue(entity);
        }

        dt.Rows.Add(values);
      }

      Excel excel = new Excel();
      return excel.WriteExcel(dt, widths, fileNameWithoutExtension, sheetName);


    }
    private string GetAttributeDisplayName(PropertyInfo property)
    {
      var atts = property.GetCustomAttributes(
          typeof(DisplayNameAttribute), true);
      if (atts.Length == 0)
        return null;
      return (atts[0] as DisplayNameAttribute).DisplayName;
    }
  }
}
