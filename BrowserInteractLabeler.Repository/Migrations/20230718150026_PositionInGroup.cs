﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrowserInteractLabeler.Repository.Migrations
{
    /// <inheritdoc />
    public partial class PositionInGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PositionInGroup",
                table: "Points",
                type: "INTEGER",
                nullable: false,
                defaultValue: -1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionInGroup",
                table: "Points");
        }
    }
}
