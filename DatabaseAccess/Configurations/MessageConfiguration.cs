using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Models.Message>
    {
        public MessageConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Message> entity_builder)
        {
            entity_builder.HasKey(e => e.Messageid).HasName("message_pkey");

            entity_builder.ToTable("message");

            entity_builder.Property(e => e.Messageid).HasColumnName("messageid");
            entity_builder.Property(e => e.Contactid).HasColumnName("contactid");
            entity_builder.Property(e => e.Friendid).HasColumnName("friendid");
            entity_builder.Property(e => e.Messagebody).HasColumnName("messagebody");
            entity_builder.Property(e => e.Sendtime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("sendtime");

            entity_builder.HasOne(d => d.Contact).WithMany(p => p.Messages)
                .HasForeignKey(d => d.Contactid)
                .HasConstraintName("message_contactid_fkey");

            entity_builder.HasOne(d => d.Friend).WithMany(p => p.Messages)
                .HasPrincipalKey(p => p.Friendid)
                .HasForeignKey(d => d.Friendid)
                .HasConstraintName("message_friendid_fkey");
        }
    }
}
