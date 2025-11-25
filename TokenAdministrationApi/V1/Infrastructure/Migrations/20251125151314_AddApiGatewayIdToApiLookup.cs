using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TokenAdministrationApi.V1.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApiGatewayIdToApiLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "api_gateway_id",
                table: "api_lookup",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "api_gateway_id",
                table: "api_lookup");
        }
    }
}
