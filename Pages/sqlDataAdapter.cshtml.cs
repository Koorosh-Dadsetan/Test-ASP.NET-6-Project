using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClosedXML.Excel;
using System.Linq;

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
        public IEnumerable<DataRow> Cultures { get; set; }

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
            TempData["p"] = p;

            PageSize = s;
            TempData["s"] = s;
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


    }
}
