using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAB1.Migrations
{
    /// <inheritdoc />
    public partial class illid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Illustrator",
                table: "Manga");

            migrationBuilder.AddColumn<int>(
                name: "IllustratorId",
                table: "Manga",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Manga_IllustratorId",
                table: "Manga",
                column: "IllustratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Manga_Authors_IllustratorId",
                table: "Manga",
                column: "IllustratorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Manga_Authors_IllustratorId",
                table: "Manga");

            migrationBuilder.DropIndex(
                name: "IX_Manga_IllustratorId",
                table: "Manga");

            migrationBuilder.DropColumn(
                name: "IllustratorId",
                table: "Manga");

            migrationBuilder.AddColumn<string>(
                name: "Illustrator",
                table: "Manga",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
