using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Syncfusion.XlsIO;


namespace Test_Project.Pages
{
    public class sqlDataAdapter : PageModel
    {
        DataTable dataTable = new DataTable();

        string connString =
            "Data Source=DESKTOP-90OC7A4\\SQLEXPRESS;Initial Catalog=Test_db;Integrated Security=true";
        string query =
            "SELECT id ,FullName,Mobile,Age,Address FROM [Test_db].[dbo].[Employees]";


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

            PageSize = s;
        }


        public IActionResult OnGetMyOnClick(int p = 1, int s = 5)
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




            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                //Create a new workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];



                //Import data from the data table with column header, at first row and first column, 
                //and by its column type.
                sheet.ImportDataTable(dataTable, true, 1, 1, true);

                //Creating Excel table or list object and apply style to the table
                IListObject table = sheet.ListObjects.Create("Employee_PersonalDetails", sheet.UsedRange);

                table.BuiltInTableStyle = TableBuiltInStyles.TableStyleMedium14;

                //Autofit the columns
                sheet.UsedRange.AutofitColumns();

                //Save the file in the given path
                var fileStream = new FileStream(@"E:\Output.xlsx", FileMode.Create);
                workbook.SaveAs(fileStream);
                fileStream.Dispose();
            }

            TempData["msg"] = "Excel file downloaded sucesfully.";

            return Redirect("https://localhost:7109/sqldataadapter");

        }

    }
}
