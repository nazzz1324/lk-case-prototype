using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.DAL.Configurations.AuthRole
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Login).IsRequired().HasMaxLength(30);
            builder.Property(x => x.Password).IsRequired();
            builder.HasData(new List<User>()
            {
                new User()
                {
                    Id = 200,
                    Login = "ilsur_200",
                    Password = "StPass123@",
                },
                new User()
                {
                    Id = 201,
                    Login = "aigul_201",
                    Password = "StPass456#",
                },
                new User()
                {
                    Id = 202,
                    Login = "ramil_202",
                    Password = "StPass789$",
                },
                new User()
                {
                    Id = 203,
                    Login = "guzel_203",
                    Password = "StPass101%",
                },
                new User()
                {
                    Id = 204,
                    Login = "ilnar_204",
                    Password = "StPass202&",
                },
                new User()
                {
                    Id = 205,
                    Login = "leysan_205",
                    Password = "StPass303*",
                },
                new User()
                {
                    Id = 206,
                    Login = "azat_206",
                    Password = "StPass404(",
                },
                new User()
                {
                    Id = 207,
                    Login = "elvira_207",
                    Password = "StPass505)",
                },
                new User()
                {
                    Id = 208,
                    Login = "radik_208",
                    Password = "StPass606=",
                },
                new User()
                {
                    Id = 209,
                    Login = "zilya_209",
                    Password = "StPass707+",
                },
                new User()
                {
                    Id = 210,
                    Login = "rustam_210",
                    Password = "StPass808-",
                },
                new User()
                {
                    Id = 211,
                    Login = "aliya_211",
                    Password = "StPass909_",
                },
                new User()
                {
                    Id = 310,
                    Login = "badmaev",
                    Password = "StPass808-",
                },
                new User()
                {
                    Id = 333,
                    Login = "VA",
                    Password = "StPass909_",
                }
            });

            builder.HasMany(x => x.Roles)
                .WithMany(x => x.Users)
                .UsingEntity<UserRole>(
                l => l.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId),
                l => l.HasOne<User>().WithMany().HasForeignKey(x => x.UserId)
                );

            builder.HasOne(x => x.UserToken)
                .WithOne(x => x.User);

            builder.HasOne(u => u.Student)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(t => t.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(u => u.Teacher)
                .WithOne(t => t.User)
                .HasForeignKey<Teacher>(t => t.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}


