using Account.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class CompetenceScoreConfiguration : IEntityTypeConfiguration<CompetenceScore>
    {
        public void Configure(EntityTypeBuilder<CompetenceScore> builder)
        {
            builder.Property(cs => cs.Id).ValueGeneratedOnAdd().IsRequired();//
            builder.Property(cs => cs.Score).IsRequired();//
            //builder.Property(cs => cs.Semester).IsRequired();//
            builder.Property(cs => cs.CompetenceId).IsRequired();//
            builder.Property(cs => cs.StudentId).IsRequired();//

            builder.HasOne(cs => cs.Student)
                .WithMany(s => s.CompetenceScores)
                .HasForeignKey(cs => cs.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull);//

            builder.HasOne(cs => cs.Competence)
                .WithMany(c => c.CompetenceScores)
                .HasForeignKey(cs => cs.CompetenceId)
                .OnDelete(DeleteBehavior.ClientSetNull);//

            builder.HasIndex(cs => new { cs.CompetenceId, cs.StudentId })
                .IsUnique();
        }
    }
}
