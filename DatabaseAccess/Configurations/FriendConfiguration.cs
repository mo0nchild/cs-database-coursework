using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class FriendConfiguration : IEntityTypeConfiguration<Models.Friend>
    {
        public FriendConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Friend> entity_builder)
        {
            entity_builder.HasKey(e => new { e.Friendid, e.Contactid1, e.Contactid2 }).HasName("friends_pkey");

            entity_builder.ToTable("friends");

            entity_builder.HasIndex(e => e.Friendid, "friends_friendid_key").IsUnique();

            entity_builder.Property(e => e.Friendid)
                .ValueGeneratedOnAdd()
                .HasColumnName("friendid");
            entity_builder.Property(e => e.Contactid1).HasColumnName("contactid1");
            entity_builder.Property(e => e.Contactid2).HasColumnName("contactid2");
            entity_builder.Property(e => e.Datingtypeid).HasColumnName("datingtypeid");
            entity_builder.Property(e => e.Starttime).HasColumnName("starttime");

            entity_builder.HasOne(d => d.Contactid1Navigation).WithMany(p => p.FriendContactid1Navigations)
                .HasForeignKey(d => d.Contactid1)
                .HasConstraintName("friends_contactid1_fkey");

            entity_builder.HasOne(d => d.Contactid2Navigation).WithMany(p => p.FriendContactid2Navigations)
                .HasForeignKey(d => d.Contactid2)
                .HasConstraintName("friends_contactid2_fkey");

            entity_builder.HasOne(d => d.Datingtype).WithMany(p => p.Friends)
                .HasForeignKey(d => d.Datingtypeid)
                .HasConstraintName("friends_datingtypeid_fkey");
        }
    }
}
