using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetLineApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Mevcut hizmetleri temizle
            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "service_id",
                keyValues: new object[] { 1, 2, 3, 4 });

            // Yeni hizmetleri ekle
            migrationBuilder.InsertData(
                table: "services",
                columns: new[] { "title", "short_description", "details", "category" },
                values: new object[,]
                {
                    {
                        "Genel Muayene",
                        "Evcil dostunuzun genel sağlık durumunun değerlendirilmesi",
                        "Rutin sağlık kontrolü, hastalık teşhisi, genel sağlık tavsiyeleri ve koruyucu hekimlik hizmetleri. Veteriner hekimimiz evcil dostunuzu detaylı şekilde muayene ederek genel sağlık durumunu değerlendirir.",
                        "Muayene"
                    },
                    {
                        "Aşılama",
                        "Koruyucu aşı hizmetleri",
                        "Karma aşı, kuduz aşısı ve diğer koruyucu aşılar. Evcil dostunuzun yaşına ve sağlık durumuna uygun aşı programı hazırlanır. Aşı sonrası takip ve gerekli öneriler verilir.",
                        "Aşı"
                    },
                    {
                        "Evcil Hayvan Bakım",
                        "Profesyonel bakım ve tıraş hizmetleri",
                        "Kedi ve köpek tıraşı, tırnak kesimi, kulak temizliği, göz bakımı ve genel hijyen hizmetleri. Evcil dostunuzun hem sağlığı hem de görünümü için profesyonel bakım.",
                        "Bakım"
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eklenen hizmetleri geri al
            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "title",
                keyValues: new object[] { "Genel Muayene", "Aşılama", "Evcil Hayvan Bakım" });
        }
    }
}
