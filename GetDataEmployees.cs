using System.Globalization;

namespace Test_Project
{
    public class GetDataEmployees
    {
        DataTable dataTable = new DataTable();

        public string connString =
            "Data Source=DESKTOP-90OC7A4\\SQLEXPRESS;Initial Catalog=Test_db;Integrated Security=true";
        public string query =
            "SELECT id ,FullName,Mobile,Age,Address FROM [Test_db].[dbo].[Employees]";

        public IEnumerable<CultureInfo> myMethodhjg()
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

            return (IEnumerable<CultureInfo>)results;
        }

    }
}
