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
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        void IEntityTypeConfiguration<UserRole>.Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasData(new List<UserRole>()
            {
                new UserRole() { UserId = 200, RoleId = 1 },
                new UserRole() { UserId = 201, RoleId = 1 },
                new UserRole() { UserId = 202, RoleId = 1 },
                new UserRole() { UserId = 203, RoleId = 1 },
                new UserRole() { UserId = 204, RoleId = 1 },
                new UserRole() { UserId = 205, RoleId = 1 },
                new UserRole() { UserId = 206, RoleId = 1 },
                new UserRole() { UserId = 207, RoleId = 1 },
                new UserRole() { UserId = 208, RoleId = 1 },
                new UserRole() { UserId = 209, RoleId = 1 },
                new UserRole() { UserId = 210, RoleId = 1 },
                new UserRole() { UserId = 211, RoleId = 1 },
                new UserRole() { UserId = 310, RoleId = 2 },
                new UserRole() { UserId = 333, RoleId = 2 },
            });

        }
    }
}
