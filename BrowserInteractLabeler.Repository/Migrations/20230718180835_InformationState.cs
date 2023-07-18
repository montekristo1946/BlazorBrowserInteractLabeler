using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrowserInteractLabeler.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InformationState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InformationState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryInformation = table.Column<int>(type: "INTEGER", nullable: false),
                    Information = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformationState", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InformationState");
        }
    }
}
