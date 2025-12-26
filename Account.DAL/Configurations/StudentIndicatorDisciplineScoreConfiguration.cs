using Account.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.DAL.Configurations
{
    public class StudentIndicatorDisciplineScoreConfiguration
    : IEntityTypeConfiguration<StudentIndicatorDisciplineScore>
    {
        public void Configure(EntityTypeBuilder<StudentIndicatorDisciplineScore> builder)
        {
            builder.Property(x => x.TeacherId)
                   .IsRequired();
            builder.HasIndex(e => new { e.StudentId, e.DisciplineId, e.IndicatorId })
                   .IsUnique();
            builder.HasIndex(e => new { e.StudentId, e.DisciplineId });
            builder.HasIndex(e => e.DisciplineId);
            builder.HasIndex(e => e.TeacherId);

        }
    }
}
