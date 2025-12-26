using Account.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class DisciplineScoreConfiguration : IEntityTypeConfiguration<DisciplineScore>
    {
        public void Configure(EntityTypeBuilder<DisciplineScore> builder)
        {
            builder.Property(ds => ds.Id).ValueGeneratedOnAdd().IsRequired(); //
            builder.Property(ds => ds.Score).IsRequired(); //
            //builder.Property(ds => ds.Semester).IsRequired(); //
            builder.Property(ds => ds.StudentId).IsRequired();//
            builder.Property(ds => ds.DisciplineId).IsRequired();//

            builder.HasOne(ds => ds.Student)
                .WithMany(s => s.DisciplineScores)
                .HasForeignKey(ds => ds.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull);//

            builder.HasOne(ds => ds.Discipline)
                .WithMany(d => d.DisciplineScores)
                .HasForeignKey(ds => ds.DisciplineId)
                .OnDelete(DeleteBehavior.ClientSetNull);//

            builder.HasIndex(ds => new { ds.DisciplineId, ds.StudentId})
                .IsUnique();
        }
    }
}