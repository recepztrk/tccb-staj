using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VetLineApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServicesAndAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pet_vaccinations");

            migrationBuilder.DropTable(
                name: "vaccines");

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "services",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "service_id",
                table: "appointments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "completed_services",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    animal_id = table.Column<int>(type: "integer", nullable: false),
                    service_id = table.Column<int>(type: "integer", nullable: false),
                    appointment_id = table.Column<int>(type: "integer", nullable: false),
                    completed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_completed_services", x => x.id);
                    table.ForeignKey(
                        name: "FK_completed_services_animals_animal_id",
                        column: x => x.animal_id,
                        principalTable: "animals",
                        principalColumn: "animal_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_completed_services_appointments_appointment_id",
                        column: x => x.appointment_id,
                        principalTable: "appointments",
                        principalColumn: "appointment_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_completed_services_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "service_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_service_id",
                table: "appointments",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_completed_services_animal_id",
                table: "completed_services",
                column: "animal_id");

            migrationBuilder.CreateIndex(
                name: "IX_completed_services_appointment_id",
                table: "completed_services",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_completed_services_service_id",
                table: "completed_services",
                column: "service_id");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_services_service_id",
                table: "appointments",
                column: "service_id",
                principalTable: "services",
                principalColumn: "service_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_services_service_id",
                table: "appointments");

            migrationBuilder.DropTable(
                name: "completed_services");

            migrationBuilder.DropIndex(
                name: "IX_appointments_service_id",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "category",
                table: "services");

            migrationBuilder.DropColumn(
                name: "service_id",
                table: "appointments");

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    BlogPostId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Author = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    Summary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.BlogPostId);
                });

            migrationBuilder.CreateTable(
                name: "vaccines",
                columns: table => new
                {
                    vaccine_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccines", x => x.vaccine_id);
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
                name: "IX_pet_vaccinations_animal_id",
                table: "pet_vaccinations",
                column: "animal_id");

            migrationBuilder.CreateIndex(
                name: "IX_pet_vaccinations_vaccine_id",
                table: "pet_vaccinations",
                column: "vaccine_id");
        }
    }
}
