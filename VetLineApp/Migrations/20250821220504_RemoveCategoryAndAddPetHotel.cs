using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetLineApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategoryAndAddPetHotel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Category sütununu kaldır
            migrationBuilder.DropColumn(
                name: "category",
                table: "services");

            // Pet Hotel hizmetini ekle
            migrationBuilder.InsertData(
                table: "services",
                columns: new[] { "title", "short_description", "details" },
                values: new object[,]
                {
                    {
                        "Pet Hotel",
                        "Evcil dostunuz için güvenli ve konforlu konaklama hizmeti",
                        "Yürüyüş ve oyun, beslenme takibi, 24 saat gözetim. Evcil dostunuz tatildeyken veya iş seyahatindeyken güvenle bırakabileceğiniz profesyonel konaklama hizmeti. Her hayvan için özel bakım ve dikkat."
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Pet Hotel hizmetini kaldır
            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "title",
                keyValue: "Pet Hotel");

            // Category sütununu geri ekle
            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "services",
                type: "text",
                nullable: true);
        }
    }
}
