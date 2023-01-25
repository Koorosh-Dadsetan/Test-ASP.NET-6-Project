using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClosedXML.Excel;
using System.Text;
using SelectPdf;

namespace Test_Project.Pages
{
    public class sqlDataAdapter : PageModel
    {
        private DataTable dataTable = new DataTable("Employees Sheet");

        private string connString =
            "Data Source=DESKTOP-90OC7A4\\SQLEXPRESS;Initial Catalog=Test_db;Integrated Security=true";
        private string query =
            "SELECT id as N'ردیف' ,FullName as N'نام و نام خانوادگی' ,Mobile as N'موبایل' ,Age as N'سن' ,Address as N'آدرس' FROM [Test_db].[dbo].[Employees]";

        [BindProperty]
        public IEnumerable<DataRow>? Cultures { get; set; }

        [BindProperty]
        public int TotalRecords { get; set; }

        [BindProperty]
        public int PageNo { get; set; }

        [BindProperty]
        public int PageSize { get; set; }


        public void OnGet(int p = 1, int s = 5)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);

                    conn.Close();
                    da.Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            var results = from myRow in dataTable.AsEnumerable()
                          select myRow;

            Cultures = results
                .Skip((p - 1) * s).Take(s);

            TotalRecords = results.Count();

            PageNo = p;

            PageSize = s;
        }


        public FileResult OnPostExportExcel()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);

                    conn.Close();
                    da.Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            using (XLWorkbook wb = new XLWorkbook { RightToLeft = true })
            {
                var ws = wb.Worksheets.Add(dataTable);

                var myCustomStyle = XLWorkbook.DefaultStyle;
                myCustomStyle.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                myCustomStyle.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.RangeUsed(XLCellsUsedOptions.AllContents).Style = myCustomStyle;

                ws.Column(1).AdjustToContents();
                ws.Column(2).AdjustToContents();
                ws.Column(3).AdjustToContents();
                ws.Column(4).AdjustToContents();
                ws.Column(5).AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
                }
            }
        }

        public FileResult OnPostExportPDF()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);

                    conn.Close();
                    da.Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("<html> <head> <style> table { font - family: arial, sans - serif; width: 100 %; box - shadow: 0 0 40px 0 rgba(0, 0, 0, .15); } th { background - color: #6c7ae0; color: #fff; } td, th { border: 1px solid #dddddd; text - align: center; padding: 8px; } tr: nth - child(even) { background - color: #f8f6ff; } h1 { text-align:center; } </ style > </head> <body> < h1> صفحه تست SqlDataAdapter</ h1 > < br /> ");
            sb.Append("<table> <tr> <th> ردیف </th> <th> نام و نام خانوادگی </th> <th> موبایل </th> <th> سن </th> <th> آدرس </th> </tr> ");

            foreach (DataRow row in dataTable.Rows)
            {
                sb.Append("<tr> <td>" + row["ردیف"].ToString() + "</td> <td>" + row["نام و نام خانوادگی"].ToString() + "</td> <td>" + row["موبایل"].ToString() + "</td> <td>" + row["سن"].ToString() + "</td> <td>" + row["آدرس"].ToString() + "</td> </tr>");
            }

            sb.Append("</table> </body> </html>");

            var htmlString = "<html><head></head><body><h1>Koorosh Dadsetan</h1></body></html>";

            using (MemoryStream stream = new MemoryStream())
            {
                HtmlToPdf converter = new HtmlToPdf();

                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;

                PdfDocument doc = converter.ConvertHtmlString(htmlString);

                //PdfDocument doc = converter.ConvertUrl("https://localhost:7109/sqldataadapter");

                doc.Save(stream);
                doc.Close();

                return File(stream.ToArray(), "application/pdf", "Employees.pdf");
            }
        }


    }
}
