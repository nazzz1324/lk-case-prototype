using Account.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class ProfessionalRoleConfiguration : IEntityTypeConfiguration<ProfessionalRole>
    {
        public void Configure(EntityTypeBuilder<ProfessionalRole> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd().IsRequired();//
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);//
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.HasData(new List<ProfessionalRole>()
            {
                new ProfessionalRole()
                {
                    Id = 200,
                    Name = "Системный Аналитик",
                    Description = "Системный анализ",
                    
                },
                new ProfessionalRole()
                {
                    Id = 201,
                    Name = ".Net WEB Developer",
                    Description = "Rider мак капучинка коворкинг клавиатура тап-тап",

                },
            });//

            builder.HasMany(pr => pr.Competences)  
                .WithMany(c => c.ProfessionalRoles);//
            
            builder.HasMany(pr => pr.Groups)
                .WithOne(g => g.ProfessionalRole);//
        }
    }
}