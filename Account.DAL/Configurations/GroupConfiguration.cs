using Account.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.Property(g => g.Id).ValueGeneratedOnAdd().IsRequired();//
            builder.Property(g => g.Name).IsRequired().HasMaxLength(10);//
            builder.Property(g => g.ProleId);//
            builder.HasData(new List<Group>()
            {
                new Group()
                {
                    Id = 10,
                    Name = "ПИ-421Б",
                    ProleId = 201
                },
                new Group()
                {
                    Id = 11,
                    Name = "ПИ-422Б",
                    ProleId = 200
                },
                new Group()
                {
                    Id = 12,
                    Name = "СИ-421Б",
                    ProleId = 200
                }
            });

            //builder.HasOne(g => g.EducationForm)
            //    .WithMany(f => f.Groups)
            //    .HasForeignKey(g => g.EducationFormId)
            //    .OnDelete(DeleteBehavior.ClientSetNull); 

            //builder.HasOne(g => g.Faculty)
            //    .WithMany(f => f.Groups)
            //    .HasForeignKey(g => g.FacultyId)
            //    .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasMany(g => g.Students)
                .WithOne(s => s.Group);//

            builder.HasMany(g => g.Disciplines)
                .WithMany(d => d.Groups);//
            
            builder.HasOne(g => g.ProfessionalRole)
                .WithMany(pr => pr.Groups)
                .HasForeignKey(g => g.ProleId)
                .OnDelete(DeleteBehavior.ClientSetNull);//
        }
    }
}