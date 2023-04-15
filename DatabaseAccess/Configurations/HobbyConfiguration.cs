using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class HobbyConfiguration : IEntityTypeConfiguration<Models.Hobby>
    {
        public HobbyConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Hobby> entity_builder)
        {
            entity_builder.HasKey(e => e.Hobbyid).HasName("hobby_pkey");

            entity_builder.ToTable("hobby");

            entity_builder.HasIndex(e => e.Hobbyname, "hobby_hobbyname_key").IsUnique();

            entity_builder.Property(e => e.Hobbyid).HasColumnName("hobbyid");
            entity_builder.Property(e => e.Hobbyname)
                .HasMaxLength(30)
                .HasColumnName("hobbyname");
            entity_builder.Property(e => e.Hobbytype)
                .HasMaxLength(30)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("hobbytype");
        }
    }
}
