using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cedar.Migrations;

/// <inheritdoc />
public partial class _20261115033402_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Category",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Budget = table.Column<double>(type: "REAL", nullable: false),
                IsIncome = table.Column<bool>(type: "INTEGER", nullable: false),
                Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                SmallestCategory_ParentId = table.Column<int>(type: "INTEGER", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Category", x => x.Id);
                table.ForeignKey(
                    name: "FK_Category_Category_ParentId",
                    column: x => x.ParentId,
                    principalTable: "Category",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Category_Category_SmallestCategory_ParentId",
                    column: x => x.SmallestCategory_ParentId,
                    principalTable: "Category",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PaymentMethods",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PaymentMethods", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Stores",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Stores", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Items",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                SmallestCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                Amount = table.Column<double>(type: "REAL", nullable: false),
                PaymentMethodId = table.Column<int>(type: "INTEGER", nullable: false),
                StoreId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Items", x => x.Id);
                table.ForeignKey(
                    name: "FK_Items_Category_SmallestCategoryId",
                    column: x => x.SmallestCategoryId,
                    principalTable: "Category",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Items_PaymentMethods_PaymentMethodId",
                    column: x => x.PaymentMethodId,
                    principalTable: "PaymentMethods",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Items_Stores_StoreId",
                    column: x => x.StoreId,
                    principalTable: "Stores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ItemDetail",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false),
                ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Quantity = table.Column<double>(type: "REAL", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ItemDetail", x => new { x.ItemId, x.Id });
                table.ForeignKey(
                    name: "FK_ItemDetail_Items_ItemId",
                    column: x => x.ItemId,
                    principalTable: "Items",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Category_ParentId",
            table: "Category",
            column: "ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_Category_SmallestCategory_ParentId",
            table: "Category",
            column: "SmallestCategory_ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_Items_PaymentMethodId",
            table: "Items",
            column: "PaymentMethodId");

        migrationBuilder.CreateIndex(
            name: "IX_Items_SmallestCategoryId",
            table: "Items",
            column: "SmallestCategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Items_StoreId",
            table: "Items",
            column: "StoreId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ItemDetail");

        migrationBuilder.DropTable(
            name: "Items");

        migrationBuilder.DropTable(
            name: "Category");

        migrationBuilder.DropTable(
            name: "PaymentMethods");

        migrationBuilder.DropTable(
            name: "Stores");
    }
}
