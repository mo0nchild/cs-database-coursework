using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Models.Contact>
    {
        public ContactConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Contact> entity_builder)
        {
            entity_builder.HasKey(e => e.Contactid).HasName("contact_pkey");

            entity_builder.ToTable("contact");

            entity_builder.Property(e => e.Contactid).HasColumnName("contactid");
            entity_builder.Property(e => e.Birthday)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("birthday");
            entity_builder.Property(e => e.Emailaddress)
                .HasMaxLength(100)
                .HasColumnName("emailaddress");
            entity_builder.Property(e => e.Familystatus)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("familystatus");
            entity_builder.Property(e => e.Gendertypeid).HasColumnName("gendertypeid");
            entity_builder.Property(e => e.Locationid).HasColumnName("locationid");
            entity_builder.Property(e => e.Name).HasMaxLength(30);
            entity_builder.Property(e => e.Patronymic)
                .HasMaxLength(30)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("patronymic");
            entity_builder.Property(e => e.Phonenumber)
                .HasMaxLength(12)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("phonenumber");
            entity_builder.Property(e => e.Surname)
                .HasMaxLength(30)
                .HasColumnName("surname");
            entity_builder.Property(e => e.Userpictureid).HasColumnName("userpictureid");

            entity_builder.HasOne(d => d.Gendertype).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.Gendertypeid)
                .HasConstraintName("contact_gendertypeid_fkey");

            entity_builder.HasOne(d => d.Location).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.Locationid)
                .HasConstraintName("contact_locationid_fkey");

            entity_builder.HasOne(d => d.Userpicture).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.Userpictureid)
                .HasConstraintName("contact_userpictureid_fkey");

            entity_builder.HasMany(d => d.Hobbies).WithMany(p => p.Contacts)
                .UsingEntity<Dictionary<string, object>>(
                    "ContactHobby",
                    r => r.HasOne<Models.Hobby>().WithMany()
                        .HasForeignKey("Hobbyid")
                        .HasConstraintName("contact_hobby_hobbyid_fkey"),
                    l => l.HasOne<Models.Contact>().WithMany()
                        .HasForeignKey("Contactid")
                        .HasConstraintName("contact_hobby_contactid_fkey"),
                    j =>
                    {
                        j.HasKey("Contactid", "Hobbyid").HasName("contact_hobby_pkey");
                        j.ToTable("contact_hobby");
                        j.IndexerProperty<int>("Contactid").HasColumnName("contactid");
                        j.IndexerProperty<int>("Hobbyid").HasColumnName("hobbyid");
                    });

            entity_builder.HasMany(d => d.Humanqualities).WithMany(p => p.Contacts)
                .UsingEntity<Dictionary<string, object>>(
                    "ContactHumanquality",
                    r => r.HasOne<Models.Humanquality>().WithMany()
                        .HasForeignKey("Humanqualityid")
                        .HasConstraintName("contact_humanquality_humanqualityid_fkey"),
                    l => l.HasOne<Models.Contact>().WithMany()
                        .HasForeignKey("Contactid")
                        .HasConstraintName("contact_humanquality_contactid_fkey"),
                    j =>
                    {
                        j.HasKey("Contactid", "Humanqualityid").HasName("contact_humanquality_pkey");
                        j.ToTable("contact_humanquality");
                        j.IndexerProperty<int>("Contactid").HasColumnName("contactid");
                        j.IndexerProperty<int>("Humanqualityid").HasColumnName("humanqualityid");
                    });
        }
    }
}
