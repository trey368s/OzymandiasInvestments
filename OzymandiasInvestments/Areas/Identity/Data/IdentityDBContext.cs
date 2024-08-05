using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzymandiasInvestments.Areas.Identity.Data;
using OzymandiasInvestments.Models.SolutionModels;

namespace OzymandiasInvestments.Areas.Identity.Data;

public class IdentityDBContext : IdentityDbContext<OzymandiasInvestmentsUser>
{
    public IdentityDBContext(DbContextOptions<IdentityDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.ApplyConfiguration(new OzymandiasInvestmentsUserEntityConfiguration());
    }
}

internal class OzymandiasInvestmentsUserEntityConfiguration : IEntityTypeConfiguration<OzymandiasInvestmentsUser>
{
    public void Configure(EntityTypeBuilder<OzymandiasInvestmentsUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(255);
        builder.Property(u => u.LastName).HasMaxLength(255);
        builder.Property(u => u.AlpacaApiKey).HasMaxLength(255);
        builder.Property(u => u.AlpacaApiSecret).HasMaxLength(255);
        builder.Property(u => u.PhoneNumber).HasMaxLength(10);
    }
}

public class InvestmentDbContext : DbContext
{
    public InvestmentDbContext(DbContextOptions<InvestmentDbContext> options) : base(options)
    {
    }
    public DbSet<InvestmentsModel> Investment { get; set; }
}