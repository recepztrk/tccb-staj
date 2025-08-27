using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VetLineApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    animal_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    brand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    stock = table.Column<int>(type: "integer", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.product_id);
                });

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    service_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    short_description = table.Column<string>(type: "text", nullable: true),
                    details = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.service_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "vaccines",
                columns: table => new
                {
                    vaccine_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccines", x => x.vaccine_id);
                });

            migrationBuilder.CreateTable(
                name: "animals",
                columns: table => new
                {
                    animal_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    breed = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_animals", x => x.animal_id);
                    table.ForeignKey(
                        name: "FK_animals_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_reviews",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    review_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_reviews", x => x.review_id);
                    table.ForeignKey(
                        name: "FK_user_reviews_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    appointment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    animal_id = table.Column<int>(type: "integer", nullable: false),
                    appointment_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    complaint = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.appointment_id);
                    table.ForeignKey(
                        name: "FK_appointments_animals_animal_id",
                        column: x => x.animal_id,
                        principalTable: "animals",
                        principalColumn: "animal_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pet_vaccinations",
                columns: table => new
                {
                    pet_vaccination_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    animal_id = table.Column<int>(type: "integer", nullable: false),
                    vaccine_id = table.Column<int>(type: "integer", nullable: false),
                    application_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pet_vaccinations", x => x.pet_vaccination_id);
                    table.ForeignKey(
                        name: "FK_pet_vaccinations_animals_animal_id",
                        column: x => x.animal_id,
                        principalTable: "animals",
                        principalColumn: "animal_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pet_vaccinations_vaccines_vaccine_id",
                        column: x => x.vaccine_id,
                        principalTable: "vaccines",
                        principalColumn: "vaccine_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_animals_user_id",
                table: "animals",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_animal_id",
                table: "appointments",
                column: "animal_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_user_id",
                table: "appointments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_pet_vaccinations_animal_id",
                table: "pet_vaccinations",
                column: "animal_id");

            migrationBuilder.CreateIndex(
                name: "IX_pet_vaccinations_vaccine_id",
                table: "pet_vaccinations",
                column: "vaccine_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_reviews_user_id",
                table: "user_reviews",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "pet_vaccinations");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "user_reviews");

            migrationBuilder.DropTable(
                name: "animals");

            migrationBuilder.DropTable(
                name: "vaccines");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
