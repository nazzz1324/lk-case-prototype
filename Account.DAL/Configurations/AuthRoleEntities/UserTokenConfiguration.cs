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
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();
            builder.Property(x => x.RefreshToken)
                .IsRequired();
            builder.Property(x => x.RefreshTokenExpiryTime)
                .IsRequired();
            builder.HasOne(x => x.User)
                .WithOne(x => x.UserToken);

        }
    }
}
