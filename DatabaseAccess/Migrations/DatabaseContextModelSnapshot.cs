﻿// <auto-generated />
using System;
using DatabaseAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabaseAccess.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ContactHobby", b =>
                {
                    b.Property<int>("Contactid")
                        .HasColumnType("integer")
                        .HasColumnName("contactid");

                    b.Property<int>("Hobbyid")
                        .HasColumnType("integer")
                        .HasColumnName("hobbyid");

                    b.HasKey("Contactid", "Hobbyid")
                        .HasName("contact_hobby_pkey");

                    b.HasIndex("Hobbyid");

                    b.ToTable("contact_hobby", (string)null);
                });

            modelBuilder.Entity("ContactHumanquality", b =>
                {
                    b.Property<int>("Contactid")
                        .HasColumnType("integer")
                        .HasColumnName("contactid");

                    b.Property<int>("Humanqualityid")
                        .HasColumnType("integer")
                        .HasColumnName("humanqualityid");

                    b.HasKey("Contactid", "Humanqualityid")
                        .HasName("contact_humanquality_pkey");

                    b.HasIndex("Humanqualityid");

                    b.ToTable("contact_humanquality", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Authorization", b =>
                {
                    b.Property<int>("Authorizationid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("authorizationid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Authorizationid"));

                    b.Property<int>("Contactid")
                        .HasColumnType("integer")
                        .HasColumnName("contactid");

                    b.Property<bool>("Isadmin")
                        .HasColumnType("boolean")
                        .HasColumnName("isadmin");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("login");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("password");

                    b.HasKey("Authorizationid")
                        .HasName("authorization_pkey");

                    b.HasIndex(new[] { "Contactid" }, "authorization_contactid_key")
                        .IsUnique();

                    b.HasIndex(new[] { "Login" }, "authorization_login_key")
                        .IsUnique();

                    b.ToTable("authorization", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.City", b =>
                {
                    b.Property<int>("Cityid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("cityid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Cityid"));

                    b.Property<string>("Cityname")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("cityname");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("country");

                    b.HasKey("Cityid")
                        .HasName("city_pkey");

                    b.HasIndex(new[] { "Cityname" }, "city_cityname_key")
                        .IsUnique();

                    b.ToTable("city", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Contact", b =>
                {
                    b.Property<int>("Contactid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("contactid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Contactid"));

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("birthday");

                    b.Property<string>("Emailaddress")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("emailaddress");

                    b.Property<string>("Familystatus")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("familystatus")
                        .HasDefaultValueSql("NULL::character varying");

                    b.Property<int>("Gendertypeid")
                        .HasColumnType("integer")
                        .HasColumnName("gendertypeid");

                    b.Property<int?>("Locationid")
                        .HasColumnType("integer")
                        .HasColumnName("locationid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("Patronymic")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("patronymic")
                        .HasDefaultValueSql("NULL::character varying");

                    b.Property<string>("Phonenumber")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(12)
                        .HasColumnType("character varying(12)")
                        .HasColumnName("phonenumber")
                        .HasDefaultValueSql("NULL::character varying");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("surname");

                    b.Property<int?>("Userpictureid")
                        .HasColumnType("integer")
                        .HasColumnName("userpictureid");

                    b.HasKey("Contactid")
                        .HasName("contact_pkey");

                    b.HasIndex("Gendertypeid");

                    b.HasIndex("Locationid");

                    b.HasIndex("Userpictureid");

                    b.ToTable("contact", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Datingtype", b =>
                {
                    b.Property<int>("Datingtypeid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("datingtypeid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Datingtypeid"));

                    b.Property<string>("Typeofdating")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("typeofdating");

                    b.HasKey("Datingtypeid")
                        .HasName("datingtype_pkey");

                    b.HasIndex(new[] { "Typeofdating" }, "datingtype_typeofdating_key")
                        .IsUnique();

                    b.ToTable("datingtype", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Employee", b =>
                {
                    b.Property<int>("Employeeid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("employeeid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Employeeid"));

                    b.Property<string>("Companyname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("companyname");

                    b.Property<int>("Contactid")
                        .HasColumnType("integer")
                        .HasColumnName("contactid");

                    b.Property<int?>("Postid")
                        .HasColumnType("integer")
                        .HasColumnName("postid");

                    b.Property<string>("Status")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("status")
                        .HasDefaultValueSql("NULL::character varying");

                    b.HasKey("Employeeid")
                        .HasName("employee_pkey");

                    b.HasIndex("Contactid");

                    b.HasIndex("Postid");

                    b.ToTable("employee", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Friend", b =>
                {
                    b.Property<int>("Friendid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("friendid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Friendid"));

                    b.Property<int>("Contactid1")
                        .HasColumnType("integer")
                        .HasColumnName("contactid1");

                    b.Property<int>("Contactid2")
                        .HasColumnType("integer")
                        .HasColumnName("contactid2");

                    b.Property<int?>("Datingtypeid")
                        .HasColumnType("integer")
                        .HasColumnName("datingtypeid");

                    b.Property<DateOnly>("Starttime")
                        .HasColumnType("date")
                        .HasColumnName("starttime");

                    b.HasKey("Friendid", "Contactid1", "Contactid2")
                        .HasName("friends_pkey");

                    b.HasIndex("Contactid1");

                    b.HasIndex("Contactid2");

                    b.HasIndex("Datingtypeid");

                    b.HasIndex(new[] { "Friendid" }, "friends_friendid_key")
                        .IsUnique();

                    b.ToTable("friends", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Gendertype", b =>
                {
                    b.Property<int>("Gendertypeid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("gendertypeid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Gendertypeid"));

                    b.Property<string>("Gendertypename")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("gendertypename");

                    b.HasKey("Gendertypeid")
                        .HasName("gendertype_pkey");

                    b.HasIndex(new[] { "Gendertypename" }, "gendertype_gendertypename_key")
                        .IsUnique();

                    b.ToTable("gendertype", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Hobby", b =>
                {
                    b.Property<int>("Hobbyid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("hobbyid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Hobbyid"));

                    b.Property<string>("Hobbyname")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("hobbyname");

                    b.Property<string>("Hobbytype")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("hobbytype")
                        .HasDefaultValueSql("NULL::character varying");

                    b.HasKey("Hobbyid")
                        .HasName("hobby_pkey");

                    b.HasIndex(new[] { "Hobbyname" }, "hobby_hobbyname_key")
                        .IsUnique();

                    b.ToTable("hobby", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Humanquality", b =>
                {
                    b.Property<int>("Humanqualityid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("humanqualityid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Humanqualityid"));

                    b.Property<string>("Qualityname")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("qualityname");

                    b.Property<string>("Qualitytype")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("qualitytype");

                    b.HasKey("Humanqualityid")
                        .HasName("humanquality_pkey");

                    b.HasIndex(new[] { "Qualityname" }, "humanquality_qualityname_key")
                        .IsUnique();

                    b.ToTable("humanquality", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Location", b =>
                {
                    b.Property<int>("Locationid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("locationid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Locationid"));

                    b.Property<int>("Cityid")
                        .HasColumnType("integer")
                        .HasColumnName("cityid");

                    b.Property<string>("Index")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasDefaultValueSql("NULL::character varying");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("street");

                    b.HasKey("Locationid")
                        .HasName("location_pkey");

                    b.HasIndex("Cityid");

                    b.HasIndex(new[] { "Street" }, "location_street_key")
                        .IsUnique();

                    b.ToTable("location", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Message", b =>
                {
                    b.Property<int>("Messageid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("messageid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Messageid"));

                    b.Property<int>("Contactid")
                        .HasColumnType("integer")
                        .HasColumnName("contactid");

                    b.Property<int>("Friendid")
                        .HasColumnType("integer")
                        .HasColumnName("friendid");

                    b.Property<string>("Messagebody")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("messagebody");

                    b.Property<DateTime>("Sendtime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("sendtime");

                    b.HasKey("Messageid")
                        .HasName("message_pkey");

                    b.HasIndex("Contactid");

                    b.HasIndex("Friendid");

                    b.ToTable("message", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Post", b =>
                {
                    b.Property<int>("Postid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("postid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Postid"));

                    b.Property<string>("Postname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("postname");

                    b.HasKey("Postid")
                        .HasName("post_pkey");

                    b.HasIndex(new[] { "Postname" }, "post_postname_key")
                        .IsUnique();

                    b.ToTable("post", (string)null);
                });

            modelBuilder.Entity("DatabaseAccess.Models.Userpicture", b =>
                {
                    b.Property<int>("Userpictureid")
                        .HasColumnType("integer")
                        .HasColumnName("userpictureid");

                    b.Property<string>("Filepath")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("filepath");

                    b.Property<string>("Picturename")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("picturename");

                    b.HasKey("Userpictureid")
                        .HasName("userpicture_pkey");

                    b.ToTable("userpicture", (string)null);
                });

            modelBuilder.Entity("ContactHobby", b =>
                {
                    b.HasOne("DatabaseAccess.Models.Contact", null)
                        .WithMany()
                        .HasForeignKey("Contactid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("contact_hobby_contactid_fkey");

                    b.HasOne("DatabaseAccess.Models.Hobby", null)
                        .WithMany()
                        .HasForeignKey("Hobbyid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("contact_hobby_hobbyid_fkey");
                });

            modelBuilder.Entity("ContactHumanquality", b =>
                {
                    b.HasOne("DatabaseAccess.Models.Contact", null)
                        .WithMany()
                        .HasForeignKey("Contactid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("contact_humanquality_contactid_fkey");

                    b.HasOne("DatabaseAccess.Models.Humanquality", null)
                        .WithMany()
                        .HasForeignKey("Humanqualityid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("contact_humanquality_humanqualityid_fkey");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Authorization", b =>
                {
                    b.HasOne("DatabaseAccess.Models.Contact", "Contact")
                        .WithOne("Authorization")
                        .HasForeignKey("DatabaseAccess.Models.Authorization", "Contactid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("authorization_contactid_fkey");

                    b.Navigation("Contact");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Contact", b =>
                {
                    b.HasOne("DatabaseAccess.Models.Gendertype", "Gendertype")
                        .WithMany("Contacts")
                        .HasForeignKey("Gendertypeid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("contact_gendertypeid_fkey");

                    b.HasOne("DatabaseAccess.Models.Location", "Location")
                        .WithMany("Contacts")
                        .HasForeignKey("Locationid")
                        .HasConstraintName("contact_locationid_fkey");

                    b.HasOne("DatabaseAccess.Models.Userpicture", "Userpicture")
                        .WithMany("Contacts")
                        .HasForeignKey("Userpictureid")
                        .HasConstraintName("contact_userpictureid_fkey");

                    b.Navigation("Gendertype");

                    b.Navigation("Location");

                    b.Navigation("Userpicture");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Employee", b =>
                {
                    b.HasOne("DatabaseAccess.Models.Contact", "Contact")
                        .WithMany("Employees")
                        .HasForeignKey("Contactid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("employee_contactid_fkey");

                    b.HasOne("DatabaseAccess.Models.Post", "Post")
                        .WithMany("Employees")
                        .HasForeignKey("Postid")
                        .HasConstraintName("employee_postid_fkey");

                    b.Navigation("Contact");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Friend", b =>
                {
                    b.HasOne("DatabaseAccess.Models.Contact", "Contactid1Navigation")
                        .WithMany("FriendContactid1Navigations")
                        .HasForeignKey("Contactid1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("friends_contactid1_fkey");

                    b.HasOne("DatabaseAccess.Models.Contact", "Contactid2Navigation")
                        .WithMany("FriendContactid2Navigations")
                        .HasForeignKey("Contactid2")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("friends_contactid2_fkey");

                    b.HasOne("DatabaseAccess.Models.Datingtype", "Datingtype")
                        .WithMany("Friends")
                        .HasForeignKey("Datingtypeid")
                        .HasConstraintName("friends_datingtypeid_fkey");

                    b.Navigation("Contactid1Navigation");

                    b.Navigation("Contactid2Navigation");

                    b.Navigation("Datingtype");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Location", b =>
                {
                    b.HasOne("DatabaseAccess.Models.City", "City")
                        .WithMany("Locations")
                        .HasForeignKey("Cityid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("location_cityid_fkey");

                    b.Navigation("City");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Message", b =>
                {
                    b.HasOne("DatabaseAccess.Models.Contact", "Contact")
                        .WithMany("Messages")
                        .HasForeignKey("Contactid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("message_contactid_fkey");

                    b.HasOne("DatabaseAccess.Models.Friend", "Friend")
                        .WithMany("Messages")
                        .HasForeignKey("Friendid")
                        .HasPrincipalKey("Friendid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("message_friendid_fkey");

                    b.Navigation("Contact");

                    b.Navigation("Friend");
                });

            modelBuilder.Entity("DatabaseAccess.Models.City", b =>
                {
                    b.Navigation("Locations");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Contact", b =>
                {
                    b.Navigation("Authorization");

                    b.Navigation("Employees");

                    b.Navigation("FriendContactid1Navigations");

                    b.Navigation("FriendContactid2Navigations");

                    b.Navigation("Messages");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Datingtype", b =>
                {
                    b.Navigation("Friends");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Friend", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Gendertype", b =>
                {
                    b.Navigation("Contacts");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Location", b =>
                {
                    b.Navigation("Contacts");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Post", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("DatabaseAccess.Models.Userpicture", b =>
                {
                    b.Navigation("Contacts");
                });
#pragma warning restore 612, 618
        }
    }
}