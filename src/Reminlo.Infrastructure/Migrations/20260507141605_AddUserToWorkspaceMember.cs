using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reminlo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToWorkspaceMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_UserId",
                table: "WorkspaceMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_AspNetUsers_UserId",
                table: "WorkspaceMembers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_AspNetUsers_UserId",
                table: "WorkspaceMembers");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceMembers_UserId",
                table: "WorkspaceMembers");
        }
    }
}
