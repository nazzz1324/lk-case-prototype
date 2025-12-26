using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class GroupDisciplineConfiguration : IEntityTypeConfiguration<GroupDiscipline>
    {
        public void Configure(EntityTypeBuilder<GroupDiscipline> builder)
        {
            builder.HasData(new List<GroupDiscipline>()
            {
                new GroupDiscipline() { GroupId = 10, DisciplineId = 100 },
                new GroupDiscipline() { GroupId = 10, DisciplineId = 101 },
                new GroupDiscipline() { GroupId = 11, DisciplineId = 100 },
                new GroupDiscipline() { GroupId = 11, DisciplineId = 101},
                new GroupDiscipline() { GroupId = 12, DisciplineId = 100 },
                new GroupDiscipline() { GroupId = 12, DisciplineId = 102 },
            });
        }
    }
}