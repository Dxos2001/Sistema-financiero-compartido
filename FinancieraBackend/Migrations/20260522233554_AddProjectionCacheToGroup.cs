using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancieraBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectionCacheToGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CachedProjection",
                table: "FinancialGroups",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "LastProjectionBalance",
                table: "FinancialGroups",
                type: "decimal(15,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CachedProjection",
                table: "FinancialGroups");

            migrationBuilder.DropColumn(
                name: "LastProjectionBalance",
                table: "FinancialGroups");
        }
    }
}
