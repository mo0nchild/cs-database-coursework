using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class HumanQualityConfiguration : IEntityTypeConfiguration<Models.Humanquality>
    {
        public HumanQualityConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Humanquality> entity_builder)
        {
            entity_builder.HasKey(e => e.Humanqualityid).HasName("humanquality_pkey");

            entity_builder.ToTable("humanquality");

            entity_builder.HasIndex(e => e.Qualityname, "humanquality_qualityname_key").IsUnique();

            entity_builder.Property(e => e.Humanqualityid).HasColumnName("humanqualityid");
            entity_builder.Property(e => e.Qualityname)
                .HasMaxLength(30)
                .HasColumnName("qualityname");
            entity_builder.Property(e => e.Qualitytype)
                .HasMaxLength(30)
                .HasColumnName("qualitytype");
        }
    }
}
