using Account.Domain.Entity;
using Account.Domain.Entity.LinkedEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(s => s.Id).ValueGeneratedOnAdd().IsRequired();//
            builder.Property(s => s.Firstname).IsRequired().HasMaxLength(25);//
            builder.Property(s => s.Lastname).IsRequired().HasMaxLength(25);//
            builder.Property(s => s.Middlename).HasMaxLength(25);//
            builder.Property(s => s.GroupId);
            builder.HasData(new List<Student>()
            {
                new Student()
                {
                    Id = 200,
                    Firstname = "Илсур",
                    Lastname = "Галиев",
                    Middlename = "Ринатович",
                    GroupId = 10
                },
                new Student()
                {
                    Id = 201,
                    Firstname = "Айгуль",
                    Lastname = "Хакимова",
                    Middlename = "Фаридовна",
                    GroupId = 10
                },
                new Student()
                {
                    Id = 202,
                    Firstname = "Рамиль",
                    Lastname = "Юсупов",
                    Middlename = "Ильдарович",
                    GroupId = 11
                },
                new Student()
                {
                    Id = 203,
                    Firstname = "Гузель",
                    Lastname = "Сафиуллина",
                    Middlename = "Рашидовна",
                    GroupId = 11
                },
                new Student()
                {
                    Id = 204,
                    Firstname = "Ильнар",
                    Lastname = "Валиев",
                    Middlename = "Айдарович",
                    GroupId = 12
                },
                new Student()
                {
                    Id = 205,
                    Firstname = "Лейсан",
                    Lastname = "Гарифуллина",
                    Middlename = "Рустамовна",
                    GroupId = 12
                },
                new Student()
                {
                    Id = 206,
                    Firstname = "Азат",
                    Lastname = "Хасанов",
                    Middlename = "Фанисович",
                    GroupId = 10
                },
                new Student()
                {
                    Id = 207,
                    Firstname = "Эльвира",
                    Lastname = "Шакирова",
                    Middlename = "Ильгизовна",
                    GroupId = 10
                },
                new Student()
                {
                    Id = 208,
                    Firstname = "Радик",
                    Lastname = "Зарипов",
                    Middlename = "Маратович",
                    GroupId = 11
                },
                new Student()
                {
                    Id = 209,
                    Firstname = "Зиля",
                    Lastname = "Ахметова",
                    Middlename = "Камилевна",
                    GroupId = 11
                },
                new Student()
                {
                    Id = 210,
                    Firstname = "Рустам",
                    Lastname = "Бакиров",
                    Middlename = "Наилевич",
                    GroupId = 12
                },
                new Student()
                {
                    Id = 211,
                    Firstname = "Алия",
                    Lastname = "Газизова",
                    Middlename = "Рафисовна",
                    GroupId = 12
                }
            });

            //builder.Property(s => s.DateOfBirth).IsRequired();
            //builder.Property(s => s.EnrollmentYear).IsRequired();

            builder.HasOne(s => s.User)
                .WithOne(u => u.Student);//

            builder.HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull);//

            builder.HasMany(s => s.IndicatorScores)
                .WithOne(si => si.Student);//

            builder.HasMany(s => s.DisciplineScores)
                .WithOne(ds => ds.Student);//

            builder.HasMany(s => s.CompetenceScores)
                .WithOne(cs => cs.Student);//
        }
    }
}