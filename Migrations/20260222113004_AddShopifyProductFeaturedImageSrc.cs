using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntegrasyonMetro.Migrations
{
    /// <inheritdoc />
    public partial class AddShopifyProductFeaturedImageSrc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeaturedImageSrc",
                schema: "dbo",
                table: "ShopifyProducts",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeaturedImageSrc",
                schema: "dbo",
                table: "ShopifyProducts");
        }
    }
}
