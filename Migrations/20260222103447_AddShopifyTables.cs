using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntegrasyonMetro.Migrations
{
    public partial class AddShopifyTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "ShopifyProducts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlatformId = table.Column<int>(type: "int", nullable: false),
                    ShopifyProductId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    BodyHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vendor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ProductType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Handle = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    AdminGraphqlApiId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedAtShopify = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAtShopify = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PublishedAtShopify = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopifyProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopifyProductImages",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlatformId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ShopifyImageId = table.Column<long>(type: "bigint", nullable: false),
                    ShopifyProductId = table.Column<long>(type: "bigint", nullable: false),
                    Src = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Alt = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: true),
                    VariantIdsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtShopify = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAtShopify = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopifyProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopifyProductImages_ShopifyProducts_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "ShopifyProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopifyProductVariants",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlatformId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ShopifyVariantId = table.Column<long>(type: "bigint", nullable: false),
                    ShopifyProductId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CompareAtPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InventoryQuantity = table.Column<int>(type: "int", nullable: true),
                    InventoryItemId = table.Column<long>(type: "bigint", nullable: true),
                    InventoryManagement = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    InventoryPolicy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Taxable = table.Column<bool>(type: "bit", nullable: false),
                    RequiresShipping = table.Column<bool>(type: "bit", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    WeightUnit = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Position = table.Column<int>(type: "int", nullable: true),
                    Option1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Option2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Option3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAtShopify = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAtShopify = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopifyProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopifyProductVariants_ShopifyProducts_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "ShopifyProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopifyProductImages_PlatformId_ShopifyImageId",
                schema: "dbo",
                table: "ShopifyProductImages",
                columns: new[] { "PlatformId", "ShopifyImageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopifyProductImages_ProductId",
                schema: "dbo",
                table: "ShopifyProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopifyProducts_PlatformId_ShopifyProductId",
                schema: "dbo",
                table: "ShopifyProducts",
                columns: new[] { "PlatformId", "ShopifyProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopifyProductVariants_PlatformId_ShopifyVariantId",
                schema: "dbo",
                table: "ShopifyProductVariants",
                columns: new[] { "PlatformId", "ShopifyVariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopifyProductVariants_ProductId",
                schema: "dbo",
                table: "ShopifyProductVariants",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopifyProductImages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ShopifyProductVariants",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ShopifyProducts",
                schema: "dbo");
        }
    }
}