using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Models.Post>
    {
        public PostConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Post> entity_builder)
        {
            entity_builder.HasKey(e => e.Postid).HasName("post_pkey");

            entity_builder.ToTable("post");

            entity_builder.HasIndex(e => e.Postname, "post_postname_key").IsUnique();

            entity_builder.Property(e => e.Postid).HasColumnName("postid");
            entity_builder.Property(e => e.Postname)
                .HasMaxLength(50)
                .HasColumnName("postname");
        }
    }
}
