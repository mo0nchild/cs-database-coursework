using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabaseAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialСreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "city",
                columns: table => new
                {
                    cityid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cityname = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    country = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("city_pkey", x => x.cityid);
                });

            migrationBuilder.CreateTable(
                name: "datingtype",
                columns: table => new
                {
                    datingtypeid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    typeofdating = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("datingtype_pkey", x => x.datingtypeid);
                });

            migrationBuilder.CreateTable(
                name: "gendertype",
                columns: table => new
                {
                    gendertypeid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gendertypename = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("gendertype_pkey", x => x.gendertypeid);
                });

            migrationBuilder.CreateTable(
                name: "hobby",
                columns: table => new
                {
                    hobbyid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hobbyname = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    hobbytype = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true, defaultValueSql: "NULL::character varying")
                },
                constraints: table =>
                {
                    table.PrimaryKey("hobby_pkey", x => x.hobbyid);
                });

            migrationBuilder.CreateTable(
                name: "humanquality",
                columns: table => new
                {
                    humanqualityid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    qualityname = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    qualitytype = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("humanquality_pkey", x => x.humanqualityid);
                });

            migrationBuilder.CreateTable(
                name: "post",
                columns: table => new
                {
                    postid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    postname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("post_pkey", x => x.postid);
                });

            migrationBuilder.CreateTable(
                name: "userpicture",
                columns: table => new
                {
                    userpictureid = table.Column<int>(type: "integer", nullable: false),
                    filepath = table.Column<string>(type: "text", nullable: false),
                    picturename = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("userpicture_pkey", x => x.userpictureid);
                });

            migrationBuilder.CreateTable(
                name: "location",
                columns: table => new
                {
                    locationid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    street = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Index = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true, defaultValueSql: "NULL::character varying"),
                    cityid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("location_pkey", x => x.locationid);
                    table.ForeignKey(
                        name: "location_cityid_fkey",
                        column: x => x.cityid,
                        principalTable: "city",
                        principalColumn: "cityid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact",
                columns: table => new
                {
                    contactid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    surname = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    patronymic = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true, defaultValueSql: "NULL::character varying"),
                    familystatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL::character varying"),
                    birthday = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    emailaddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phonenumber = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true, defaultValueSql: "NULL::character varying"),
                    locationid = table.Column<int>(type: "integer", nullable: true),
                    gendertypeid = table.Column<int>(type: "integer", nullable: false),
                    userpictureid = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("contact_pkey", x => x.contactid);
                    table.ForeignKey(
                        name: "contact_gendertypeid_fkey",
                        column: x => x.gendertypeid,
                        principalTable: "gendertype",
                        principalColumn: "gendertypeid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "contact_locationid_fkey",
                        column: x => x.locationid,
                        principalTable: "location",
                        principalColumn: "locationid");
                    table.ForeignKey(
                        name: "contact_userpictureid_fkey",
                        column: x => x.userpictureid,
                        principalTable: "userpicture",
                        principalColumn: "userpictureid");
                });

            migrationBuilder.CreateTable(
                name: "authorization",
                columns: table => new
                {
                    authorizationid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    password = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    isadmin = table.Column<bool>(type: "boolean", nullable: false),
                    contactid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("authorization_pkey", x => x.authorizationid);
                    table.ForeignKey(
                        name: "authorization_contactid_fkey",
                        column: x => x.contactid,
                        principalTable: "contact",
                        principalColumn: "contactid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact_hobby",
                columns: table => new
                {
                    contactid = table.Column<int>(type: "integer", nullable: false),
                    hobbyid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("contact_hobby_pkey", x => new { x.contactid, x.hobbyid });
                    table.ForeignKey(
                        name: "contact_hobby_contactid_fkey",
                        column: x => x.contactid,
                        principalTable: "contact",
                        principalColumn: "contactid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "contact_hobby_hobbyid_fkey",
                        column: x => x.hobbyid,
                        principalTable: "hobby",
                        principalColumn: "hobbyid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact_humanquality",
                columns: table => new
                {
                    contactid = table.Column<int>(type: "integer", nullable: false),
                    humanqualityid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("contact_humanquality_pkey", x => new { x.contactid, x.humanqualityid });
                    table.ForeignKey(
                        name: "contact_humanquality_contactid_fkey",
                        column: x => x.contactid,
                        principalTable: "contact",
                        principalColumn: "contactid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "contact_humanquality_humanqualityid_fkey",
                        column: x => x.humanqualityid,
                        principalTable: "humanquality",
                        principalColumn: "humanqualityid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    employeeid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    companyname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true, defaultValueSql: "NULL::character varying"),
                    postid = table.Column<int>(type: "integer", nullable: true),
                    contactid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_pkey", x => x.employeeid);
                    table.ForeignKey(
                        name: "employee_contactid_fkey",
                        column: x => x.contactid,
                        principalTable: "contact",
                        principalColumn: "contactid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "employee_postid_fkey",
                        column: x => x.postid,
                        principalTable: "post",
                        principalColumn: "postid");
                });

            migrationBuilder.CreateTable(
                name: "friends",
                columns: table => new
                {
                    friendid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    contactid1 = table.Column<int>(type: "integer", nullable: false),
                    contactid2 = table.Column<int>(type: "integer", nullable: false),
                    starttime = table.Column<DateOnly>(type: "date", nullable: false),
                    datingtypeid = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("friends_pkey", x => new { x.friendid, x.contactid1, x.contactid2 });
                    table.UniqueConstraint("AK_friends_friendid", x => x.friendid);
                    table.ForeignKey(
                        name: "friends_contactid1_fkey",
                        column: x => x.contactid1,
                        principalTable: "contact",
                        principalColumn: "contactid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "friends_contactid2_fkey",
                        column: x => x.contactid2,
                        principalTable: "contact",
                        principalColumn: "contactid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "friends_datingtypeid_fkey",
                        column: x => x.datingtypeid,
                        principalTable: "datingtype",
                        principalColumn: "datingtypeid");
                });

            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    messageid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    messagebody = table.Column<string>(type: "text", nullable: false),
                    sendtime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    friendid = table.Column<int>(type: "integer", nullable: false),
                    contactid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("message_pkey", x => x.messageid);
                    table.ForeignKey(
                        name: "message_contactid_fkey",
                        column: x => x.contactid,
                        principalTable: "contact",
                        principalColumn: "contactid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "message_friendid_fkey",
                        column: x => x.friendid,
                        principalTable: "friends",
                        principalColumn: "friendid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "authorization_contactid_key",
                table: "authorization",
                column: "contactid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "authorization_login_key",
                table: "authorization",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "city_cityname_key",
                table: "city",
                column: "cityname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contact_gendertypeid",
                table: "contact",
                column: "gendertypeid");

            migrationBuilder.CreateIndex(
                name: "IX_contact_locationid",
                table: "contact",
                column: "locationid");

            migrationBuilder.CreateIndex(
                name: "IX_contact_userpictureid",
                table: "contact",
                column: "userpictureid");

            migrationBuilder.CreateIndex(
                name: "IX_contact_hobby_hobbyid",
                table: "contact_hobby",
                column: "hobbyid");

            migrationBuilder.CreateIndex(
                name: "IX_contact_humanquality_humanqualityid",
                table: "contact_humanquality",
                column: "humanqualityid");

            migrationBuilder.CreateIndex(
                name: "datingtype_typeofdating_key",
                table: "datingtype",
                column: "typeofdating",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_contactid",
                table: "employee",
                column: "contactid");

            migrationBuilder.CreateIndex(
                name: "IX_employee_postid",
                table: "employee",
                column: "postid");

            migrationBuilder.CreateIndex(
                name: "friends_friendid_key",
                table: "friends",
                column: "friendid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_friends_contactid1",
                table: "friends",
                column: "contactid1");

            migrationBuilder.CreateIndex(
                name: "IX_friends_contactid2",
                table: "friends",
                column: "contactid2");

            migrationBuilder.CreateIndex(
                name: "IX_friends_datingtypeid",
                table: "friends",
                column: "datingtypeid");

            migrationBuilder.CreateIndex(
                name: "gendertype_gendertypename_key",
                table: "gendertype",
                column: "gendertypename",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "hobby_hobbyname_key",
                table: "hobby",
                column: "hobbyname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "humanquality_qualityname_key",
                table: "humanquality",
                column: "qualityname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_location_cityid",
                table: "location",
                column: "cityid");

            migrationBuilder.CreateIndex(
                name: "location_street_key",
                table: "location",
                column: "street",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_message_contactid",
                table: "message",
                column: "contactid");

            migrationBuilder.CreateIndex(
                name: "IX_message_friendid",
                table: "message",
                column: "friendid");

            migrationBuilder.CreateIndex(
                name: "post_postname_key",
                table: "post",
                column: "postname",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "authorization");

            migrationBuilder.DropTable(
                name: "contact_hobby");

            migrationBuilder.DropTable(
                name: "contact_humanquality");

            migrationBuilder.DropTable(
                name: "employee");

            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "hobby");

            migrationBuilder.DropTable(
                name: "humanquality");

            migrationBuilder.DropTable(
                name: "post");

            migrationBuilder.DropTable(
                name: "friends");

            migrationBuilder.DropTable(
                name: "contact");

            migrationBuilder.DropTable(
                name: "datingtype");

            migrationBuilder.DropTable(
                name: "gendertype");

            migrationBuilder.DropTable(
                name: "location");

            migrationBuilder.DropTable(
                name: "userpicture");

            migrationBuilder.DropTable(
                name: "city");
        }
    }
}
