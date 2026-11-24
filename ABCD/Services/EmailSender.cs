using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using ATTENDANCE_AUTOMAILER.Helpers;
using ATTENDANCE_AUTOMAILER.Models;

namespace ATTENDANCE_AUTOMAILER.Services
{
    /// <summary>
    /// Builds the violation mail (BRD requirement 5 layout) and sends it, or drops an .eml file in
    /// Output\MailDrop when no SMTP host is configured.
    /// </summary>
    public static class EmailSender
    {
        public static string BuildSubject(ViolationResult result)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "[{0}] {1} - Shift {2} - {3:dd-MM-yyyy} - {4} case(s)",
                AppSettings.MailSubjectPrefix,
                result.LocationCode,
                result.Shift == null ? "ALL" : result.Shift.Code,
                result.AttendanceDate,
                result.Violations.Count);
        }

        public static string BuildBody(ViolationResult result)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<html><body style=\"font-family:Segoe UI,Arial,sans-serif;font-size:13px;color:#222;\">");
            html.Append("<p>Dear Sir/Madam,</p>");
            html.Append("<p>Attendance validation for the shift mentioned below has been completed. ");
            html.Append("The violations identified under the attendance violation rules are listed for corrective action.</p>");

            html.Append("<table cellspacing=\"0\" cellpadding=\"6\" style=\"border-collapse:collapse;font-size:13px;margin-bottom:14px;\">");
            html.Append(Row("Location", result.LocationCode));
            html.Append(Row("Attendance Date", result.AttendanceDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)));
            html.Append(Row("Shift", result.Shift == null ? "ALL" : result.Shift.Code + " (" + result.Shift.WindowText + ")"));
            html.Append(Row("Workers in this shift", result.WorkersInShift.ToString(CultureInfo.InvariantCulture)));
            html.Append(Row("Total violations", result.Violations.Count.ToString(CultureInfo.InvariantCulture)));
            html.Append(Row("High concern", result.HighRiskCount.ToString(CultureInfo.InvariantCulture)));
            html.Append(Row("Generated on", result.GeneratedOn.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)));
            html.Append("</table>");

            html.Append("<h4 style=\"margin:18px 0 6px 0;color:#1F3864;\">Violation summary</h4>");
            html.Append("<table cellspacing=\"0\" cellpadding=\"6\" style=\"border-collapse:collapse;font-size:13px;border:1px solid #d0d0d0;\">");
            html.Append("<tr style=\"background:#1F3864;color:#fff;\"><th align=\"left\">Violation Type</th><th align=\"left\">Code</th><th align=\"left\">Count</th></tr>");
            string[] types = { ViolationRow.OverstayHigh, ViolationRow.OverstayMedium, ViolationRow.ContinuousShift, ViolationRow.ConsecutiveDays, ViolationRow.MissingInPunch, ViolationRow.MissingOutPunch };
            foreach (string type in types)
            {
                html.Append("<tr><td style=\"border:1px solid #e0e0e0;\">").Append(ExcelReportBuilder.ViolationName(type))
                    .Append("</td><td style=\"border:1px solid #e0e0e0;\">").Append(type)
                    .Append("</td><td style=\"border:1px solid #e0e0e0;\">").Append(result.CountOf(type).ToString(CultureInfo.InvariantCulture))
                    .Append("</td></tr>");
            }
            html.Append("</table>");

            if (result.PunchCountByShift.Count > 0)
            {
                html.Append("<h4 style=\"margin:18px 0 6px 0;color:#1F3864;\">Punches received in the shift(s)</h4>");
                html.Append("<table cellspacing=\"0\" cellpadding=\"6\" style=\"border-collapse:collapse;font-size:13px;border:1px solid #d0d0d0;\">");
                html.Append("<tr style=\"background:#1F3864;color:#fff;\"><th align=\"left\">Shift</th><th align=\"left\">Workers</th></tr>");
                foreach (KeyValuePair<string, int> pair in result.PunchCountByShift.OrderBy(p => p.Key))
                {
                    html.Append("<tr><td style=\"border:1px solid #e0e0e0;\">").Append(pair.Key)
                        .Append("</td><td style=\"border:1px solid #e0e0e0;\">").Append(pair.Value.ToString(CultureInfo.InvariantCulture))
                        .Append("</td></tr>");
                }
                html.Append("</table>");
            }

            int limit = AppSettings.MaxViolationRowsInMail;
            List<ViolationRow> shown = result.Violations.Take(limit).ToList();

            if (shown.Count > 0)
            {
                html.Append("<h4 style=\"margin:18px 0 6px 0;color:#1F3864;\">Violation details</h4>");
                html.Append("<table cellspacing=\"0\" cellpadding=\"5\" style=\"border-collapse:collapse;font-size:12px;border:1px solid #d0d0d0;\">");
                html.Append("<tr style=\"background:#1F3864;color:#fff;\">");
                string[] headers = { "Safety Pass No", "Employee Name", "Department", "Date", "Shift", "Violation Type", "In", "Out", "Hours", "Remarks", "Required Action" };
                foreach (string header in headers)
                {
                    html.Append("<th align=\"left\">").Append(header).Append("</th>");
                }
                html.Append("</tr>");

                bool alternate = false;
                foreach (ViolationRow row in shown)
                {
                    string background = alternate ? " background:#f5f7fb;" : string.Empty;
                    alternate = !alternate;
                    html.Append("<tr").Append(background).Append(">");
                    AppendCell(html, row.SafetyPassNo);
                    AppendCell(html, row.WorkerName);
                    AppendCell(html, row.DepartmentName);
                    AppendCell(html, row.AttendanceDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture));
                    AppendCell(html, row.ShiftCode);
                    AppendCell(html, ExcelReportBuilder.ViolationName(row.ViolationType) + " (" + row.Severity + ")");
                    AppendCell(html, row.InTimeText);
                    AppendCell(html, row.OutTimeText);
                    AppendCell(html, row.ViolationType == ViolationRow.ConsecutiveDays ? row.TotalDays + " days" : row.TotalHours.ToString("0.00", CultureInfo.InvariantCulture));
                    AppendCell(html, row.Remarks);
                    AppendCell(html, row.RequiredAction);
                    html.Append("</tr>");
                }
                html.Append("</table>");

                if (result.Violations.Count > shown.Count)
                {
                    html.Append("<p style=\"font-size:12px;color:#555;\">")
                        .Append(string.Format(CultureInfo.InvariantCulture, "{0} more violation(s) are available in the attached Excel report.", result.Violations.Count - shown.Count))
                        .Append("</p>");
                }
            }
            else
            {
                html.Append("<p style=\"padding:10px;background:#e8f5e9;border-left:4px solid #2e7d32;\">No attendance violation found for this shift.</p>");
            }

            html.Append("<p style=\"font-size:12px;color:#555;margin-top:16px;\">The complete violation list is attached as an Excel report. ");
            html.Append("Please take corrective action and update the Time Office.</p>");
            html.Append("<p style=\"font-size:12px;color:#777;\">This is a system generated mail from the Attendance Automailer batch (ACE). Please do not reply.</p>");
            html.Append("</body></html>");
            return html.ToString();
        }

        /// <summary>
        /// Sends the mail. Returns a short status text for the log.
        /// </summary>
        public static string Send(ViolationResult result, string attachmentPath, List<string> to, List<string> cc)
        {
            if (to.Count == 0)
            {
                throw new InvalidOperationException("No mail recipients configured (MailTo / --to).");
            }

            if (string.IsNullOrEmpty(AppSettings.MailFrom))
            {
                throw new InvalidOperationException("MailFrom is not configured.");
            }

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(AppSettings.MailFrom);
                foreach (string address in to)
                {
                    message.To.Add(address);
                }

                foreach (string address in cc)
                {
                    if (!to.Contains(address, StringComparer.OrdinalIgnoreCase))
                    {
                        message.CC.Add(address);
                    }
                }

                message.Subject = BuildSubject(result);
                message.SubjectEncoding = Encoding.UTF8;
                message.Body = BuildBody(result);
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;

                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    message.Attachments.Add(new Attachment(attachmentPath));
                }

                using (SmtpClient client = new SmtpClient())
                {
                    if (string.IsNullOrEmpty(AppSettings.SmtpHost))
                    {
                        string dropFolder = Path.Combine(AppSettings.GetFolder("OutputDirectory", "Output"), "MailDrop");
                        if (!Directory.Exists(dropFolder))
                        {
                            Directory.CreateDirectory(dropFolder);
                        }

                        client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        client.PickupDirectoryLocation = dropFolder;
                        client.Send(message);
                        return "SMTP not configured - mail written to " + Path.Combine(dropFolder, "*.eml");
                    }

                    client.Host = AppSettings.SmtpHost;
                    client.Port = AppSettings.SmtpPort;
                    client.EnableSsl = AppSettings.SmtpEnableSsl;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    if (!string.IsNullOrEmpty(AppSettings.SmtpUser))
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new System.Net.NetworkCredential(AppSettings.SmtpUser, AppSettings.SmtpPassword);
                    }

                    client.Send(message);
                    return "Mail sent to " + string.Join("; ", to) + (cc.Count > 0 ? " (cc " + string.Join("; ", cc) + ")" : string.Empty);
                }
            }
        }

        /// <summary>Department wise CC recipients from DepartmentMailMap.</summary>
        public static List<string> ResolveDepartmentRecipients(ViolationResult result)
        {
            List<string> addresses = new List<string>();
            string map = AppSettings.DepartmentMailMap;
            if (string.IsNullOrEmpty(map))
            {
                return addresses;
            }

            Dictionary<string, List<string>> parsed = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (string entry in map.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int separator = entry.IndexOf('=');
                if (separator <= 0)
                {
                    continue;
                }

                string departmentCode = entry.Substring(0, separator).Trim();
                string mailList = entry.Substring(separator + 1);
                parsed[departmentCode] = mailList.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim())
                    .Where(a => a.Length > 0)
                    .ToList();
            }

            HashSet<string> departments = new HashSet<string>(
                result.Violations.Select(v => (v.DepartmentCode ?? string.Empty).Trim()),
                StringComparer.OrdinalIgnoreCase);

            foreach (string department in departments)
            {
                List<string> mapped;
                if (parsed.TryGetValue(department, out mapped))
                {
                    foreach (string address in mapped)
                    {
                        if (!addresses.Contains(address, StringComparer.OrdinalIgnoreCase))
                        {
                            addresses.Add(address);
                        }
                    }
                }
            }

            return addresses;
        }

        private static string Row(string label, string value)
        {
            return "<tr><td style=\"padding:4px 10px 4px 0;color:#555;white-space:nowrap;\">" + label +
                   "</td><td style=\"font-weight:600;\">" + value + "</td></tr>";
        }

        private static void AppendCell(StringBuilder html, string value)
        {
            html.Append("<td style=\"border:1px solid #e0e0e0;vertical-align:top;\">")
                .Append(string.IsNullOrEmpty(value) ? "&nbsp;" : value)
                .Append("</td>");
        }
    }
}
