using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reminlo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkspacesInvitations2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceInvitation_Workspaces_WorkspaceId",
                table: "WorkspaceInvitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkspaceInvitation",
                table: "WorkspaceInvitation");

            migrationBuilder.RenameTable(
                name: "WorkspaceInvitation",
                newName: "WorkspaceInvitations");

            migrationBuilder.RenameIndex(
                name: "IX_WorkspaceInvitation_WorkspaceId",
                table: "WorkspaceInvitations",
                newName: "IX_WorkspaceInvitations_WorkspaceId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkspaceInvitation_Token",
                table: "WorkspaceInvitations",
                newName: "IX_WorkspaceInvitations_Token");

            migrationBuilder.RenameIndex(
                name: "IX_WorkspaceInvitation_Email",
                table: "WorkspaceInvitations",
                newName: "IX_WorkspaceInvitations_Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkspaceInvitations",
                table: "WorkspaceInvitations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceInvitations_Workspaces_WorkspaceId",
                table: "WorkspaceInvitations",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceInvitations_Workspaces_WorkspaceId",
                table: "WorkspaceInvitations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkspaceInvitations",
                table: "WorkspaceInvitations");

            migrationBuilder.RenameTable(
                name: "WorkspaceInvitations",
                newName: "WorkspaceInvitation");

            migrationBuilder.RenameIndex(
                name: "IX_WorkspaceInvitations_WorkspaceId",
                table: "WorkspaceInvitation",
                newName: "IX_WorkspaceInvitation_WorkspaceId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkspaceInvitations_Token",
                table: "WorkspaceInvitation",
                newName: "IX_WorkspaceInvitation_Token");

            migrationBuilder.RenameIndex(
                name: "IX_WorkspaceInvitations_Email",
                table: "WorkspaceInvitation",
                newName: "IX_WorkspaceInvitation_Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkspaceInvitation",
                table: "WorkspaceInvitation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceInvitation_Workspaces_WorkspaceId",
                table: "WorkspaceInvitation",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
