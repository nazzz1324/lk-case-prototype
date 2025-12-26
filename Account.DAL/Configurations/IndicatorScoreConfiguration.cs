using Account.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class IndicatorScoreConfiguration : IEntityTypeConfiguration<IndicatorScore>
    {
        public void Configure(EntityTypeBuilder<IndicatorScore> builder)
        {
            builder.Property(s => s.Id).ValueGeneratedOnAdd().IsRequired();//
            builder.Property(s => s.ScoreValue).IsRequired();//
            //builder.Property(s => s.Semester).IsRequired();//
            builder.Property(s => s.MaxScore).IsRequired();//
            builder.Property(s => s.StudentId).IsRequired();//
            builder.Property(s => s.IndicatorId).IsRequired();//
            builder.Property(s => s.TeacherId).IsRequired();//
            
            builder.HasOne(s => s.Student)
                .WithMany(st => st.IndicatorScores)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull);//

            builder.HasOne(s => s.Indicator)
                .WithMany(i =>i.IndicatorScores)
                .HasForeignKey(s => s.IndicatorId)
                .OnDelete(DeleteBehavior.ClientSetNull);//

            builder.HasOne(s => s.Teacher)
                .WithMany(t => t.IndicatorScores)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull);//

            builder.HasIndex(i => new { i.IndicatorId, i.StudentId, i.TeacherId })
                .IsUnique();
        }
    }
}