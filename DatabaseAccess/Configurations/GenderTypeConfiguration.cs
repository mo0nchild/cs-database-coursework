using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class GenderTypeConfiguration : IEntityTypeConfiguration<Models.Gendertype>
    {
        public GenderTypeConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Gendertype> entity_builder)
        {
            entity_builder.HasKey(e => e.Gendertypeid).HasName("gendertype_pkey");

            entity_builder.ToTable("gendertype");

            entity_builder.HasIndex(e => e.Gendertypename, "gendertype_gendertypename_key").IsUnique();

            entity_builder.Property(e => e.Gendertypeid).HasColumnName("gendertypeid");
            entity_builder.Property(e => e.Gendertypename)
                .HasMaxLength(20)
                .HasColumnName("gendertypename");
        }
    }
}
