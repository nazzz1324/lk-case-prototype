using Account.Domain.Entity.AuthRole;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.DAL.Configurations.AuthRole
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        void IEntityTypeConfiguration<Role>.Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(30);
            builder.HasData(new List<Role>()
            {
                new Role()
                {
                    Id = 1,
                    Name = "Student",
                },
                new Role()
                {
                    Id = 2,
                    Name = "Teacher",
                },
                new Role()
                {
                    Id = 3,
                    Name = "Admin",
                }
            });
        }
    }
}
