using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ATTENDANCE_AUTOMAILER.Models;
using ClosedXML.Excel;

namespace ATTENDANCE_AUTOMAILER.Services
{
    /// <summary>
    /// Builds the Excel attachment: one summary sheet plus one sheet per violation type.
    /// </summary>
    public static class ExcelReportBuilder
    {
        private static readonly string[] Headers =
        {
            "Location", "Department", "Safety Pass No", "Worker Name", "Vendor Code", "Vendor Name",
            "WO No", "CA/CO Name", "Date", "Shift", "Violation Type", "Severity",
            "In Time", "Out Time", "Total Hours", "No. of Days", "Start Date", "End Date",
            "Remarks", "Required Action"
        };

        public static string Build(ViolationResult result, string outputFolder)
        {
            string fileName = string.Format(
                CultureInfo.InvariantCulture,
                "Attendance_Violation_{0}_Shift{1}_{2:yyyyMMdd_HHmmss}.xlsx",
                result.LocationCode,
                result.Shift == null ? "ALL" : result.Shift.Code,
                result.AttendanceDate);

            string path = Path.Combine(outputFolder, fileName);

            using (XLWorkbook workbook = new XLWorkbook())
            {
                WriteSummary(workbook, result);
                WriteSheet(workbook, "Overstay High", result.OfType(ViolationRow.OverstayHigh).ToList());
                WriteSheet(workbook, "Overstay Medium", result.OfType(ViolationRow.OverstayMedium).ToList());
                WriteSheet(workbook, "Continuous Shift", result.OfType(ViolationRow.ContinuousShift).ToList());
                WriteSheet(workbook, "Consecutive Days", result.OfType(ViolationRow.ConsecutiveDays).ToList());
                WriteSheet(workbook, "Missing In Punch", result.OfType(ViolationRow.MissingInPunch).ToList());
                WriteSheet(workbook, "Missing Out Punch", result.OfType(ViolationRow.MissingOutPunch).ToList());
                workbook.SaveAs(path);
            }

            return path;
        }

        private static void WriteSummary(XLWorkbook workbook, ViolationResult result)
        {
            IXLWorksheet sheet = workbook.Worksheets.Add("Summary");

            List<KeyValuePair<string, string>> info = new List<KeyValuePair<string, string>>();
            info.Add(new KeyValuePair<string, string>("Location", result.LocationCode));
            info.Add(new KeyValuePair<string, string>("Attendance Date", result.AttendanceDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("Shift", result.Shift == null ? "ALL" : result.Shift.Code + " (" + result.Shift.WindowText + ")"));
            info.Add(new KeyValuePair<string, string>("Generated On", result.GeneratedOn.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("Workers in shift", result.WorkersInShift.ToString(CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("Total workers in attendance", result.TotalWorkers.ToString(CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("Attendance segments read", result.SegmentsRead.ToString(CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("Total violations", result.Violations.Count.ToString(CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("High concern violations", result.HighRiskCount.ToString(CultureInfo.InvariantCulture)));

            int row = 1;
            foreach (KeyValuePair<string, string> pair in info)
            {
                sheet.Cell(row, 1).SetValue(pair.Key);
                sheet.Cell(row, 1).Style.Font.Bold = true;
                sheet.Cell(row, 2).SetValue(pair.Value);
                row++;
            }

            row++;
            sheet.Cell(row, 1).SetValue("Violation Type");
            sheet.Cell(row, 2).SetValue("Code");
            sheet.Cell(row, 3).SetValue("Count");
            IXLRange headerRange = sheet.Range(row, 1, row, 3);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(31, 56, 100);
            row++;

            string[] types = { ViolationRow.OverstayHigh, ViolationRow.OverstayMedium, ViolationRow.ContinuousShift, ViolationRow.ConsecutiveDays, ViolationRow.MissingInPunch, ViolationRow.MissingOutPunch };
            foreach (string type in types)
            {
                sheet.Cell(row, 1).SetValue(ViolationName(type));
                sheet.Cell(row, 2).SetValue(type);
                sheet.Cell(row, 3).SetValue(result.CountOf(type));
                row++;
            }

            sheet.Columns().AdjustToContents();
        }

        private static void WriteSheet(XLWorkbook workbook, string sheetName, List<ViolationRow> rows)
        {
            IXLWorksheet sheet = workbook.Worksheets.Add(sheetName);

            for (int column = 0; column < Headers.Length; column++)
            {
                sheet.Cell(1, column + 1).SetValue(Headers[column]);
            }

            IXLRange headerRange = sheet.Range(1, 1, 1, Headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(31, 56, 100);
            headerRange.Style.Alignment.WrapText = true;

            int row = 2;
            foreach (ViolationRow item in rows)
            {
                sheet.Cell(row, 1).SetValue(item.LocationCode);
                sheet.Cell(row, 2).SetValue(item.DepartmentName);
                sheet.Cell(row, 3).SetValue(item.SafetyPassNo);
                sheet.Cell(row, 4).SetValue(item.WorkerName);
                sheet.Cell(row, 5).SetValue(item.VendorCode);
                sheet.Cell(row, 6).SetValue(item.VendorName);
                sheet.Cell(row, 7).SetValue(item.WorkOrderNo);
                sheet.Cell(row, 8).SetValue(item.CaCoName);
                sheet.Cell(row, 9).SetValue(item.AttendanceDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture));
                sheet.Cell(row, 10).SetValue(item.ShiftCode);
                sheet.Cell(row, 11).SetValue(ViolationName(item.ViolationType));
                sheet.Cell(row, 12).SetValue(item.Severity);
                sheet.Cell(row, 13).SetValue(item.InTimeText);
                sheet.Cell(row, 14).SetValue(item.OutTimeText);
                sheet.Cell(row, 15).SetValue(item.TotalHours);
                sheet.Cell(row, 16).SetValue(item.TotalDays);
                sheet.Cell(row, 17).SetValue(item.StartDate.HasValue ? item.StartDate.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) : string.Empty);
                sheet.Cell(row, 18).SetValue(item.EndDate.HasValue ? item.EndDate.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) : string.Empty);
                sheet.Cell(row, 19).SetValue(item.Remarks);
                sheet.Cell(row, 20).SetValue(item.RequiredAction);
                row++;
            }

            if (rows.Count > 0)
            {
                sheet.Range(1, 1, row - 1, Headers.Length).SetAutoFilter();
            }

            sheet.SheetView.FreezeRows(1);
            sheet.Columns().AdjustToContents();
        }

        public static string ViolationName(string violationType)
        {
            switch (violationType)
            {
                case ViolationRow.OverstayHigh:
                    return "Overstay - high concern";
                case ViolationRow.OverstayMedium:
                    return "Overstay - medium";
                case ViolationRow.ContinuousShift:
                    return "Continuous shift duty";
                case ViolationRow.ConsecutiveDays:
                    return "Continuous working days";
                case ViolationRow.MissingInPunch:
                    return "Missing In punch";
                case ViolationRow.MissingOutPunch:
                    return "Missing Out punch";
                default:
                    return violationType;
            }
        }
    }
}
