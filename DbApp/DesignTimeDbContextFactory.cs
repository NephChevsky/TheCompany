using DbApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class DesignTimeDbContextFactory :
        IDesignTimeDbContextFactory<TheCompanyDbContext>
{
    public TheCompanyDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<TheCompanyDbContext>();
        builder.UseSqlServer("Server=localhost;Database=TheCompany;Trusted_Connection=True;");
        return new TheCompanyDbContext(builder.Options);
    }
}