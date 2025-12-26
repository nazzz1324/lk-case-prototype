using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class CompetenceProleConfiguration : IEntityTypeConfiguration<CompetenceProle>
    {
        public void Configure(EntityTypeBuilder<CompetenceProle> builder)
        {
            builder.HasData(new List<CompetenceProle>()
            {
                new CompetenceProle() { ProleId = 200, CompetenceId = 556 },
                new CompetenceProle() { ProleId = 200, CompetenceId = 557 },
                new CompetenceProle() { ProleId = 201, CompetenceId = 556 },
                new CompetenceProle() { ProleId = 201, CompetenceId = 557 },
            });
        }
    }
}