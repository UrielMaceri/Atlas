using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Back.Migrations
{
    /// <inheritdoc />
    public partial class AddSortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Item",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Category",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                WITH Ranked AS (
                    SELECT Id, ROW_NUMBER() OVER (
                        PARTITION BY WorkspaceId ORDER BY Id
                    ) AS Position
                    FROM Category
                )
                UPDATE Category
                SET SortOrder = (
                    SELECT Position FROM Ranked WHERE Ranked.Id = Category.Id
                );
                """);

            migrationBuilder.Sql("""
                WITH Ranked AS (
                    SELECT Id, ROW_NUMBER() OVER (
                        PARTITION BY CategoryId ORDER BY Id
                    ) AS Position
                    FROM Item
                )
                UPDATE Item
                SET SortOrder = (
                    SELECT Position FROM Ranked WHERE Ranked.Id = Item.Id
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Category");
        }
    }
}
