using Microsoft.EntityFrameworkCore;
using Test_Project.Models;

namespace Test_Project
{
    public class DBCtx : DbContext
    {
        public DBCtx(DbContextOptions<DBCtx> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
