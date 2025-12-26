using Account.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.Property(t => t.Id).ValueGeneratedOnAdd().IsRequired();//
            builder.Property(t => t.Firstname).IsRequired().HasMaxLength(40);//
            builder.Property(t => t.Lastname).IsRequired().HasMaxLength(40);//
            builder.Property(t => t.Middlename).HasMaxLength(40);//
            builder.HasData(new List<Teacher>()
            {
                new Teacher()
                {
                    Id = 310,
                    Firstname = "Рустам",
                    Lastname = "Бадмаев",
                    Middlename = "Наилевич",
                },
                new Teacher()
                {
                    Id = 333,
                    Firstname = "Вероника",
                    Lastname = "Суворова",
                    Middlename = "Александровна",
                }
            });

            builder.HasOne(t => t.User)
                .WithOne(u => u.Teacher);//

            builder.HasMany(t => t.IndicatorScores)
                .WithOne(si => si.Teacher);//

            builder.HasMany(t => t.Disciplines)
                .WithMany(d => d.Teachers);//
        }
    }
}