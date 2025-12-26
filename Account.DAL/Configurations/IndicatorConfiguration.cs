using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Account.Domain.Entity.LinkedEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class IndicatorConfiguration : IEntityTypeConfiguration<Indicator>
    {
        public void Configure(EntityTypeBuilder<Indicator> builder)
        {
            builder.Property(i => i.Id).ValueGeneratedOnAdd().IsRequired();//
            builder.Property(i => i.Index).IsRequired().HasMaxLength(10);//
            builder.Property(i => i.Name).IsRequired().HasMaxLength(200);//
            builder.Property(i => i.Description).HasMaxLength(500);//
            builder.Property(i => i.CompetenceId);//
            builder.HasData(new List<Indicator>()
            {
                new Indicator()
                {
                    Id = 556,
                    Index = "ПК-2.1",
                    Name = "Знание работы с ИС",
                    Description = "Знать: принципы ор-ганизации, разработ-ки, " +
                    "развития и экс-плуатации корпора-тивных информаци-онных систем",
                    CompetenceId = 556
                },
                new Indicator()
                {
                    Id = 557,
                    Index = "ПК-2.2",
                    Name = "Умение работы с ИС",
                    Description = "Уметь: применять принципы организа-ции, разработки, " +
                    "развития и эксплуа-тации корпоратив-ных информацион-ных систем",
                    CompetenceId = 556
                },
                new Indicator()
                {
                    Id = 558,
                    Index = "ПК-2.3",
                    Name = "Владение навыками работы с ИС",
                    Description = "Владеть: навыками организации, разра-ботки," +
                    " развития и эксплуатации корпо-ративных информа-ционных систем",
                    CompetenceId = 556
                },
                new Indicator()
                {
                    Id = 559,
                    Index = "ПК-5.1",
                    Name = "Знание технологий моделирования",
                    Description = "Знать: современные технологии инфор-мационного моде-лирования и методы информационных процессов для мо-делирования объек-тов",
                    CompetenceId = 557
                },
                new Indicator()
                {
                    Id = 560,
                    Index = "ПК-5.2",
                    Name = "Умение применять сервисы для моделирования",
                    Description = "Уметь: применять известные сервисы информационных технологий для мо-делирования объек-тов и процессов",
                    CompetenceId = 557
                },
                new Indicator()
                {
                    Id = 561,
                    Index = "ПК-5.3",
                    Name = "Владение навыками работы для моделирования",
                    Description = " Владеть: навыками разработки моделей, методов, алгоритмов и сервисов для моделирования объектов и процессов",
                    CompetenceId = 557

                },
            });

            builder.HasMany(i => i.Disciplines)
                .WithMany(d => d.Indicators);//

            builder.HasOne(i => i.Competence)
                .WithMany(c => c.Indicators)
                .HasForeignKey(i => i.CompetenceId)
                .OnDelete(DeleteBehavior.ClientSetNull);//

            builder.HasMany(i => i.IndicatorScores)
                .WithOne(s => s.Indicator);//
        }
    }
}