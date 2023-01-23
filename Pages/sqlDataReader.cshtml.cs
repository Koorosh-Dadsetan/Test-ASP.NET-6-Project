using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClosedXML.Excel;

namespace Test_Project.Pages
{
    public class sqlDataReader : PageModel
    {
        private DataTable dataTable = new DataTable("Employees Sheet");

        private string connectionString =
                "Data Source=DESKTOP-90OC7A4\\SQLEXPRESS;Initial Catalog=Test_db;Integrated Security=true";

        private string queryString =
            "SELECT [id] ,[FullName] ,[Mobile] ,[Age] ,[Address] FROM [Test_db].[dbo].[Employees]";

        public IEnumerable<DataRow> Cultures { get; set; }

        public int TotalRecords { get; set; }

        public int PageNo { get; set; }

        public int PageSize { get; set; }


        public void OnGet(int p = 1 ,int s = 5)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    dataTable.Load(reader);

                    reader.Close();
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    dataTable.Load(reader);

                    reader.Close();
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
