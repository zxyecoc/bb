using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAB1.Migrations
{
    /// <inheritdoc />
    public partial class authorsid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Manga");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Manga",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Manga_AuthorId",
                table: "Manga",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Manga_Authors_AuthorId",
                table: "Manga",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Manga_Authors_AuthorId",
                table: "Manga");

            migrationBuilder.DropIndex(
                name: "IX_Manga_AuthorId",
                table: "Manga");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Manga");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Manga",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
