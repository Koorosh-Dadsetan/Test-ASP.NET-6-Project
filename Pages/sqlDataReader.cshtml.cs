using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Test_Project.Pages
{
    public class sqlDataReader : PageModel
    {
        DataTable dataTable = new DataTable();

        string connectionString =
                "Data Source=DESKTOP-90OC7A4\\SQLEXPRESS;Initial Catalog=Test_db;Integrated Security=true";

        string queryString =
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
    }
}
