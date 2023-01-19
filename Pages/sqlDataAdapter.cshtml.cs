using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
    }
}
