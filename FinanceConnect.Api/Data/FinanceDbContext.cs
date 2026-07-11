using Microsoft.EntityFrameworkCore;
using FinanceConnect.Api.Models;

namespace FinanceConnect.Api.Data;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }
}
