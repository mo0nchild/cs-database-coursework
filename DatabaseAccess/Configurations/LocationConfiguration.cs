using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Models.Location>
    {
        public LocationConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Location> entity_builder)
        {
            entity_builder.HasKey(e => e.Locationid).HasName("location_pkey");
            entity_builder.ToTable("location");
            entity_builder.HasIndex(e => e.Street, "location_street_key").IsUnique();

            entity_builder.Property(e => e.Locationid).HasColumnName("locationid");
            entity_builder.Property(e => e.Cityid).HasColumnName("cityid");
            entity_builder.Property(e => e.Street)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("street");

            entity_builder.HasOne(d => d.City).WithMany(p => p.Locations)
                .HasForeignKey(d => d.Cityid)
                .HasConstraintName("location_cityid_fkey");
        }
    }
}
