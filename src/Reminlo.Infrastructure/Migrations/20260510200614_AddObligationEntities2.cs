using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reminlo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddObligationEntities2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obligation_ObligationCategory_CategoryId",
                table: "Obligation");

            migrationBuilder.DropForeignKey(
                name: "FK_ObligationReminder_Obligation_ObligationId",
                table: "ObligationReminder");

            migrationBuilder.DropForeignKey(
                name: "FK_ObligationVisibility_Obligation_ObligationId",
                table: "ObligationVisibility");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ObligationReminder",
                table: "ObligationReminder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ObligationCategory",
                table: "ObligationCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Obligation",
                table: "Obligation");

            migrationBuilder.RenameTable(
                name: "ObligationReminder",
                newName: "ObligationReminders");

            migrationBuilder.RenameTable(
                name: "ObligationCategory",
                newName: "ObligationCategories");

            migrationBuilder.RenameTable(
                name: "Obligation",
                newName: "Obligations");

            migrationBuilder.RenameIndex(
                name: "IX_ObligationReminder_ObligationId",
                table: "ObligationReminders",
                newName: "IX_ObligationReminders_ObligationId");

            migrationBuilder.RenameIndex(
                name: "IX_ObligationCategory_Name",
                table: "ObligationCategories",
                newName: "IX_ObligationCategories_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Obligation_CategoryId",
                table: "Obligations",
                newName: "IX_Obligations_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ObligationReminders",
                table: "ObligationReminders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ObligationCategories",
                table: "ObligationCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Obligations",
                table: "Obligations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObligationReminders_Obligations_ObligationId",
                table: "ObligationReminders",
                column: "ObligationId",
                principalTable: "Obligations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Obligations_ObligationCategories_CategoryId",
                table: "Obligations",
                column: "CategoryId",
                principalTable: "ObligationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ObligationVisibility_Obligations_ObligationId",
                table: "ObligationVisibility",
                column: "ObligationId",
                principalTable: "Obligations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObligationReminders_Obligations_ObligationId",
                table: "ObligationReminders");

            migrationBuilder.DropForeignKey(
                name: "FK_Obligations_ObligationCategories_CategoryId",
                table: "Obligations");

            migrationBuilder.DropForeignKey(
                name: "FK_ObligationVisibility_Obligations_ObligationId",
                table: "ObligationVisibility");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Obligations",
                table: "Obligations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ObligationReminders",
                table: "ObligationReminders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ObligationCategories",
                table: "ObligationCategories");

            migrationBuilder.RenameTable(
                name: "Obligations",
                newName: "Obligation");

            migrationBuilder.RenameTable(
                name: "ObligationReminders",
                newName: "ObligationReminder");

            migrationBuilder.RenameTable(
                name: "ObligationCategories",
                newName: "ObligationCategory");

            migrationBuilder.RenameIndex(
                name: "IX_Obligations_CategoryId",
                table: "Obligation",
                newName: "IX_Obligation_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ObligationReminders_ObligationId",
                table: "ObligationReminder",
                newName: "IX_ObligationReminder_ObligationId");

            migrationBuilder.RenameIndex(
                name: "IX_ObligationCategories_Name",
                table: "ObligationCategory",
                newName: "IX_ObligationCategory_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Obligation",
                table: "Obligation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ObligationReminder",
                table: "ObligationReminder",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ObligationCategory",
                table: "ObligationCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Obligation_ObligationCategory_CategoryId",
                table: "Obligation",
                column: "CategoryId",
                principalTable: "ObligationCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ObligationReminder_Obligation_ObligationId",
                table: "ObligationReminder",
                column: "ObligationId",
                principalTable: "Obligation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObligationVisibility_Obligation_ObligationId",
                table: "ObligationVisibility",
                column: "ObligationId",
                principalTable: "Obligation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
