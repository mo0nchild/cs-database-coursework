using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class CityConfiguration : IEntityTypeConfiguration<Models.City>
    {
        public CityConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.City> entity_builder)
        {
            entity_builder.HasKey(e => e.Cityid).HasName("city_pkey");

            entity_builder.ToTable("city");

            entity_builder.HasIndex(e => e.Cityname, "city_cityname_key").IsUnique();

            entity_builder.Property(e => e.Cityid).HasColumnName("cityid");
            entity_builder.Property(e => e.Cityname)
                .HasMaxLength(30)
                .HasColumnName("cityname");
            entity_builder.Property(e => e.Country)
                .HasMaxLength(30)
                .HasColumnName("country");
        }
    }
}
