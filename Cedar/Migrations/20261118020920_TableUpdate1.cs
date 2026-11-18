using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cedar.Migrations;

/// <inheritdoc />
public partial class _20261118020920_TableUpdate1 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Stores_Name",
            table: "Stores",
            column: "Name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Stores_Name",
            table: "Stores");
    }
}
