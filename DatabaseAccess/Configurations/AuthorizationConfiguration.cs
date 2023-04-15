using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class AuthorizationConfiguration : IEntityTypeConfiguration<Models.Authorization>
    {
        public AuthorizationConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Authorization> entity_builder)
        {
            entity_builder.HasKey(e => e.Authorizationid).HasName("authorization_pkey");

            entity_builder.ToTable("authorization");

            entity_builder.HasIndex(e => e.Contactid, "authorization_contactid_key").IsUnique();
            entity_builder.HasIndex(e => e.Login, "authorization_login_key").IsUnique();

            entity_builder.Property(e => e.Authorizationid).HasColumnName("authorizationid");
            entity_builder.Property(e => e.Contactid).HasColumnName("contactid");
            entity_builder.Property(e => e.Isadmin).HasColumnName("isadmin");
            entity_builder.Property(e => e.Login)
                .HasMaxLength(30)
                .HasColumnName("login");
            entity_builder.Property(e => e.Password)
                .HasMaxLength(30)
                .HasColumnName("password");

            entity_builder.HasOne(d => d.Contact).WithOne(p => p.Authorization)
                .HasForeignKey<Models.Authorization>(d => d.Contactid)
                .HasConstraintName("authorization_contactid_fkey");
        }
    }
}
