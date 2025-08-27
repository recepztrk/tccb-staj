using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetLineApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicatePetHotel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eski Pet Hotel kayıtlarını sil (en son eklenen hariç)
            migrationBuilder.Sql(@"
                DELETE FROM services 
                WHERE title = 'Pet Hotel' 
                AND service_id NOT IN (
                    SELECT service_id 
                    FROM services 
                    WHERE title = 'Pet Hotel' 
                    ORDER BY service_id DESC 
                    LIMIT 1
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
