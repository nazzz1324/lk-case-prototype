using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Account.Domain.Entity.LinkedEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class DisciplineConfiguration : IEntityTypeConfiguration<Discipline>
    {
        public void Configure(EntityTypeBuilder<Discipline> builder)
        {
            builder.Property(d => d.Id).ValueGeneratedOnAdd().IsRequired();//
            builder.Property(d => d.Index).HasMaxLength(50);;//
            builder.Property(d => d.Name).IsRequired().HasMaxLength(200);//
            builder.Property(d => d.Description).HasMaxLength(500);//
            builder.HasData(new List<Discipline>()
            {
                new Discipline()
                {
                    Id = 100,
                    Index = "Б.О.1.1",
                    Name = "Основы программирования на C#",
                    Description = "Введение в язык C#, базовые конструкции, ООП, работа с коллекциями"
                },
                new Discipline()
                {
                    Id = 101,
                    Index = "Б.О.1.2",
                    Name = "Разработка веб-приложений на ASP.NET Core",
                    Description = "Создание RESTful API, работа с Entity Framework, аутентификация и авторизация"
                },
                new Discipline()
                {
                    Id = 102,
                    Index = "Б.О.2.1",
                    Name = "Системный анализ и проектирование",
                    Description = "Методологии разработки ПО, UML-диаграммы, сбор требований, проектирование архитектуры"
                }
            });


            builder.HasMany(d => d.Indicators)
                .WithMany(i => i.Disciplines)
                .UsingEntity<IndicatorDiscipline>(
                l => l.HasOne<Indicator>().WithMany().HasForeignKey(x => x.IndicatorId),
                l => l.HasOne<Discipline>().WithMany().HasForeignKey(x => x.DisciplineId)//
                );
            builder.HasMany(d => d.Teachers)
                .WithMany(t => t.Disciplines)
                .UsingEntity<TeacherDiscipline>(
                l => l.HasOne<Teacher>().WithMany().HasForeignKey(x => x.TeacherId),
                l => l.HasOne<Discipline>().WithMany().HasForeignKey(x => x.DisciplineId)//
                );
            builder.HasMany(d => d.Groups)
                .WithMany(g => g.Disciplines)
                .UsingEntity<GroupDiscipline>(
                l => l.HasOne<Group>().WithMany().HasForeignKey(x => x.GroupId),
                l => l.HasOne<Discipline>().WithMany().HasForeignKey(x => x.DisciplineId)//
                );

            builder.HasMany(d => d.DisciplineScores) 
                .WithOne(ds => ds.Discipline);//
        }
    }
}