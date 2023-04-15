using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class DatingTypeConfiguration : IEntityTypeConfiguration<Models.Datingtype>
    {
        public DatingTypeConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Datingtype> entity_builder)
        {
            entity_builder.HasKey(e => e.Datingtypeid).HasName("datingtype_pkey");

            entity_builder.ToTable("datingtype");

            entity_builder.HasIndex(e => e.Typeofdating, "datingtype_typeofdating_key").IsUnique();

            entity_builder.Property(e => e.Datingtypeid).HasColumnName("datingtypeid");
            entity_builder.Property(e => e.Typeofdating)
                .HasMaxLength(30)
                .HasColumnName("typeofdating");
        }
    }
}
