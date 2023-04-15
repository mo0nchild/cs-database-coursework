using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class UserPictureConfiguration : IEntityTypeConfiguration<Models.Userpicture>
    {
        public UserPictureConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Userpicture> entity_builder)
        {
            entity_builder.HasKey(e => e.Userpictureid).HasName("userpicture_pkey");

            entity_builder.ToTable("userpicture");

            entity_builder.Property(e => e.Userpictureid)
                .ValueGeneratedNever()
                .HasColumnName("userpictureid");
            entity_builder.Property(e => e.Filepath).HasColumnName("filepath");
            entity_builder.Property(e => e.Picturename)
                .HasMaxLength(50)
                .HasColumnName("picturename");
        }
    }
}
