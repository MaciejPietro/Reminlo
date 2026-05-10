using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reminlo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddObligationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObligationCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Obligation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FrequencyInterval = table.Column<string>(type: "text", nullable: true),
                    FrequencyValue = table.Column<int>(type: "integer", nullable: true),
                    NextDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obligation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obligation_ObligationCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ObligationCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObligationReminder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ObligationId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationOffset = table.Column<int>(type: "integer", nullable: false),
                    NotificationType = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationReminder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObligationReminder_Obligation_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObligationVisibility",
                columns: table => new
                {
                    ObligationId = table.Column<Guid>(type: "uuid", nullable: false),
                    VisibleToId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationVisibility", x => new { x.ObligationId, x.VisibleToId });
                    table.ForeignKey(
                        name: "FK_ObligationVisibility_Obligation_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObligationVisibility_WorkspaceMembers_VisibleToId",
                        column: x => x.VisibleToId,
                        principalTable: "WorkspaceMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Obligation_CategoryId",
                table: "Obligation",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationCategory_Name",
                table: "ObligationCategory",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObligationReminder_ObligationId",
                table: "ObligationReminder",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationVisibility_VisibleToId",
                table: "ObligationVisibility",
                column: "VisibleToId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObligationReminder");

            migrationBuilder.DropTable(
                name: "ObligationVisibility");

            migrationBuilder.DropTable(
                name: "Obligation");

            migrationBuilder.DropTable(
                name: "ObligationCategory");
        }
    }
}
