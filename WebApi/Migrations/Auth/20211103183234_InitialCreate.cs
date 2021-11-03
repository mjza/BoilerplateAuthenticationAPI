﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations.Auth
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "WebApidbo");

            migrationBuilder.CreateTable(
                name: "countries",
                schema: "WebApidbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tell_code = table.Column<long>(type: "bigint", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "genders",
                schema: "WebApidbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "locales",
                schema: "WebApidbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locales", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "nationalities",
                schema: "WebApidbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nationalities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "professions",
                schema: "WebApidbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_professions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "titles",
                schema: "WebApidbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_titles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cities",
                schema: "WebApidbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    country_id = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.id);
                    table.ForeignKey(
                        name: "cities$cities_country_id_foreign",
                        column: x => x.country_id,
                        principalSchema: "WebApidbo",
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accounts",
                schema: "WebApidbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    title_id = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    first_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    birthday = table.Column<DateTime>(type: "date", nullable: true),
                    gender_id = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false, defaultValueSql: "(N'f')"),
                    settings = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "(N'{}')"),
                    nationality_id = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    profession_id = table.Column<Guid>(type: "UniqueIdentifier", nullable: true),
                    accept_terms = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    locale_id = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValueSql: "(N'en')"),
                    role = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    street = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    postcode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    verification_token = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    verified_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    reset_token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    reset_token_expires_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    password_reseted_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    password_hash = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                    table.ForeignKey(
                        name: "accounts$accounts_gender_id_foreign",
                        column: x => x.gender_id,
                        principalSchema: "WebApidbo",
                        principalTable: "genders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "accounts$accounts_locale_id_foreign",
                        column: x => x.locale_id,
                        principalSchema: "WebApidbo",
                        principalTable: "locales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "accounts$accounts_nationality_id_foreign",
                        column: x => x.nationality_id,
                        principalSchema: "WebApidbo",
                        principalTable: "nationalities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "accounts$accounts_profession_id_foreign",
                        column: x => x.profession_id,
                        principalSchema: "WebApidbo",
                        principalTable: "professions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "accounts$accounts_title_id_foreign",
                        column: x => x.title_id,
                        principalSchema: "WebApidbo",
                        principalTable: "titles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "WebApidbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    token = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    expires_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    created_by_ip = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    revoked_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    revoked_by_ip = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Replaced_by_token = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "refresh_tokens$FK_RefreshToken_Accounts_AccountId",
                        column: x => x.account_id,
                        principalSchema: "WebApidbo",
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "WebApidbo",
                table: "countries",
                columns: new[] { "id", "name", "tell_code" },
                values: new object[,]
                {
                    { "de", "Germany", 0L },
                    { "us", "USA", 0L }
                });

            migrationBuilder.InsertData(
                schema: "WebApidbo",
                table: "genders",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { "m", "Male" },
                    { "f", "Female" },
                    { "d", "Diversed" }
                });

            migrationBuilder.InsertData(
                schema: "WebApidbo",
                table: "locales",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { "de", "Deutsch" },
                    { "en", "English" }
                });

            migrationBuilder.InsertData(
                schema: "WebApidbo",
                table: "nationalities",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { "us", "American" },
                    { "de", "German" }
                });

            migrationBuilder.InsertData(
                schema: "WebApidbo",
                table: "professions",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("54a130d2-502f-4cf1-a376-63edeb000001"), "Others" },
                    { new Guid("54a130d2-502f-4cf1-a376-63edeb000002"), "CEO" },
                    { new Guid("54a130d2-502f-4cf1-a376-63edeb000003"), "Employee" }
                });

            migrationBuilder.InsertData(
                schema: "WebApidbo",
                table: "titles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { "dr", "Dr." },
                    { "mr", "Mr." },
                    { "mrs", "Mrs." },
                    { "ms", "Ms." },
                    { "pr", "Prof." }
                });

            migrationBuilder.InsertData(
                schema: "WebApidbo",
                table: "accounts",
                columns: new[] { "id", "accept_terms", "birthday", "created_at", "email", "first_name", "gender_id", "last_name", "locale_id", "nationality_id", "number", "password_hash", "password_reseted_at", "postcode", "profession_id", "reset_token", "reset_token_expires_at", "role", "settings", "street", "title_id", "updated_at", "verification_token", "verified_at" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), true, new DateTime(1985, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2019, 12, 30, 9, 10, 30, 0, DateTimeKind.Unspecified), "super1@gmail.com", "Super", "m", "1", "en", "de", "137a", "$2a$11$zW9.9gyeqD31PnTqqlJTF.0pOKK8/YxM52J7JhQIiF.phYXY8yJm2", null, "50825", new Guid("54a130d2-502f-4cf1-a376-63edeb000003"), null, null, 0, "{}", "Bonnstr.", "mr", null, null, new DateTime(2020, 1, 1, 9, 10, 30, 0, DateTimeKind.Unspecified) },
                    { new Guid("00000000-0000-0000-0000-000000000002"), true, new DateTime(1985, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 1, 5, 9, 10, 30, 0, DateTimeKind.Unspecified), "admin1@gmail.com", "Admin", "m", "1", "de", "de", "138a", "$2a$11$rQv2R8IMC6ehATVOP7/Iuunfa5kd.fNOzyHZKgj.EBbomhmOiIuqe", null, "53698", new Guid("54a130d2-502f-4cf1-a376-63edeb000001"), null, null, 1, "{}", "Bonnstr.", "mr", null, null, new DateTime(2020, 4, 1, 9, 10, 30, 0, DateTimeKind.Unspecified) },
                    { new Guid("00000000-0000-0000-0000-000000000006"), false, new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 1, 7, 9, 10, 30, 0, DateTimeKind.Unspecified), "patient1@gmail.com", "Patient", "m", "1", "en", "de", "137c", "$2a$11$r7Di9joy6vScFNI7zUCES.S6rRvfyLRnk9KlCYA0l1PN5CAeYj7hO", null, null, new Guid("54a130d2-502f-4cf1-a376-63edeb000002"), null, null, 2, "{}", "Alexstr.", "mr", null, null, null }
                });

            migrationBuilder.InsertData(
                schema: "WebApidbo",
                table: "cities",
                columns: new[] { "id", "country_id", "name" },
                values: new object[,]
                {
                    { new Guid("52a130d2-502f-4cf1-a376-63edeb000001"), "de", "Cologne" },
                    { new Guid("52a130d2-502f-4cf1-a376-63edeb000002"), "de", "Bonn" },
                    { new Guid("52a130d2-502f-4cf1-a376-63edeb000003"), "de", "Berlin" },
                    { new Guid("52a130d2-502f-4cf1-a376-63edeb000004"), "us", "New Yourk" }
                });

            migrationBuilder.CreateIndex(
                name: "accounts_gender_id_foreign",
                schema: "WebApidbo",
                table: "accounts",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "accounts_locale_id_foreign",
                schema: "WebApidbo",
                table: "accounts",
                column: "locale_id");

            migrationBuilder.CreateIndex(
                name: "accounts_nationality_id_foreign",
                schema: "WebApidbo",
                table: "accounts",
                column: "nationality_id");

            migrationBuilder.CreateIndex(
                name: "accounts_profession_id_foreign",
                schema: "WebApidbo",
                table: "accounts",
                column: "profession_id");

            migrationBuilder.CreateIndex(
                name: "accounts_title_id_foreign",
                schema: "WebApidbo",
                table: "accounts",
                column: "title_id");

            migrationBuilder.CreateIndex(
                name: "cities_country_id_foreign",
                schema: "WebApidbo",
                table: "cities",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "FK_RefreshToken_Accounts_AccountId",
                schema: "WebApidbo",
                table: "refresh_tokens",
                column: "account_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cities",
                schema: "WebApidbo");

            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "WebApidbo");

            migrationBuilder.DropTable(
                name: "countries",
                schema: "WebApidbo");

            migrationBuilder.DropTable(
                name: "accounts",
                schema: "WebApidbo");

            migrationBuilder.DropTable(
                name: "genders",
                schema: "WebApidbo");

            migrationBuilder.DropTable(
                name: "locales",
                schema: "WebApidbo");

            migrationBuilder.DropTable(
                name: "nationalities",
                schema: "WebApidbo");

            migrationBuilder.DropTable(
                name: "professions",
                schema: "WebApidbo");

            migrationBuilder.DropTable(
                name: "titles",
                schema: "WebApidbo");
        }
    }
}