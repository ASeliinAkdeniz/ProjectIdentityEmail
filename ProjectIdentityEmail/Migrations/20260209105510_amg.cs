using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectIdentityEmail.Migrations
{
    /// <inheritdoc />
    public partial class amg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educations_AspNetUsers_AppUserId",
                table: "Educations");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Educations",
                newName: "AppUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Educations_AppUserId",
                table: "Educations",
                newName: "IX_Educations_AppUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_AspNetUsers_AppUserID",
                table: "Educations",
                column: "AppUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educations_AspNetUsers_AppUserID",
                table: "Educations");

            migrationBuilder.RenameColumn(
                name: "AppUserID",
                table: "Educations",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Educations_AppUserID",
                table: "Educations",
                newName: "IX_Educations_AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_AspNetUsers_AppUserId",
                table: "Educations",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
