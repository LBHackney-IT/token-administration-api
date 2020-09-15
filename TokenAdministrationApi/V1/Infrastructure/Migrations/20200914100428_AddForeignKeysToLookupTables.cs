using Microsoft.EntityFrameworkCore.Migrations;

namespace TokenAdministrationApi.V1.Infrastructure.Migrations
{
    public partial class AddForeignKeysToLookupTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tokens_api_endpoint_lookup_id",
                table: "tokens",
                column: "api_endpoint_lookup_id");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_api_lookup_id",
                table: "tokens",
                column: "api_lookup_id");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_consumer_type_lookup",
                table: "tokens",
                column: "consumer_type_lookup");

            migrationBuilder.AddForeignKey(
                name: "FK_tokens_api_endpoint_lookup_api_endpoint_lookup_id",
                table: "tokens",
                column: "api_endpoint_lookup_id",
                principalTable: "api_endpoint_lookup",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tokens_api_lookup_api_lookup_id",
                table: "tokens",
                column: "api_lookup_id",
                principalTable: "api_lookup",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tokens_consumer_type_lookup_consumer_type_lookup",
                table: "tokens",
                column: "consumer_type_lookup",
                principalTable: "consumer_type_lookup",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tokens_api_endpoint_lookup_api_endpoint_lookup_id",
                table: "tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_tokens_api_lookup_api_lookup_id",
                table: "tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_tokens_consumer_type_lookup_consumer_type_lookup",
                table: "tokens");

            migrationBuilder.DropIndex(
                name: "IX_tokens_api_endpoint_lookup_id",
                table: "tokens");

            migrationBuilder.DropIndex(
                name: "IX_tokens_api_lookup_id",
                table: "tokens");

            migrationBuilder.DropIndex(
                name: "IX_tokens_consumer_type_lookup",
                table: "tokens");
        }
    }
}
