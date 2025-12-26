using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class IndicatorDisciplineConfiguration : IEntityTypeConfiguration<IndicatorDiscipline>
    {
        public void Configure(EntityTypeBuilder<IndicatorDiscipline> builder)
        {
            builder.HasData(new List<IndicatorDiscipline>()
            {
                new IndicatorDiscipline() { IndicatorId = 556, DisciplineId = 100 },
                new IndicatorDiscipline() { IndicatorId = 557, DisciplineId = 100 },
                new IndicatorDiscipline() { IndicatorId = 558, DisciplineId = 100 },
                new IndicatorDiscipline() { IndicatorId = 558, DisciplineId = 101 },
                new IndicatorDiscipline() { IndicatorId = 559, DisciplineId = 101},
                new IndicatorDiscipline() { IndicatorId = 557, DisciplineId = 101 },
                new IndicatorDiscipline() { IndicatorId = 560, DisciplineId = 102 },
                new IndicatorDiscipline() { IndicatorId = 561, DisciplineId = 102 },
                new IndicatorDiscipline() { IndicatorId = 556, DisciplineId = 102 },
            });
        }
    }
}