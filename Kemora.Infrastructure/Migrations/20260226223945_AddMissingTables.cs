using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReaction_AspNetUsers_UserID",
                table: "CommentReaction");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentReaction_Comments_CommentID",
                table: "CommentReaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentReaction",
                table: "CommentReaction");

            migrationBuilder.RenameTable(
                name: "CommentReaction",
                newName: "CommentReactions");

            migrationBuilder.RenameIndex(
                name: "IX_CommentReaction_UserID",
                table: "CommentReactions",
                newName: "IX_CommentReactions_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentReactions",
                table: "CommentReactions",
                columns: new[] { "CommentID", "UserID" });

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReactions_AspNetUsers_UserID",
                table: "CommentReactions",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReactions_Comments_CommentID",
                table: "CommentReactions",
                column: "CommentID",
                principalTable: "Comments",
                principalColumn: "CommentID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReactions_AspNetUsers_UserID",
                table: "CommentReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentReactions_Comments_CommentID",
                table: "CommentReactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentReactions",
                table: "CommentReactions");

            migrationBuilder.RenameTable(
                name: "CommentReactions",
                newName: "CommentReaction");

            migrationBuilder.RenameIndex(
                name: "IX_CommentReactions_UserID",
                table: "CommentReaction",
                newName: "IX_CommentReaction_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentReaction",
                table: "CommentReaction",
                columns: new[] { "CommentID", "UserID" });

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReaction_AspNetUsers_UserID",
                table: "CommentReaction",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReaction_Comments_CommentID",
                table: "CommentReaction",
                column: "CommentID",
                principalTable: "Comments",
                principalColumn: "CommentID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
