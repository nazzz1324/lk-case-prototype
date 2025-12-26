using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class TeacherDisciplineConfiguration : IEntityTypeConfiguration<TeacherDiscipline>
    {
        public void Configure(EntityTypeBuilder<TeacherDiscipline> builder)
        {
            builder.HasData(new List<TeacherDiscipline>()
            {
                new TeacherDiscipline() { TeacherId = 333, DisciplineId = 100 },
                new TeacherDiscipline() { TeacherId = 333, DisciplineId = 101 },
                new TeacherDiscipline() { TeacherId = 310, DisciplineId = 101 },
                new TeacherDiscipline() { TeacherId = 310, DisciplineId = 102 },
            });
        }
    }
}