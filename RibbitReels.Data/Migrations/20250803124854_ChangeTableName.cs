using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RibbitReels.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedBranches_Branches_BranchId",
                table: "AssignedBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedBranches_Users_UserId",
                table: "AssignedBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_Branches_BranchId",
                table: "Leaves");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProgress_Branches_BranchId",
                table: "UserProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProgress_Users_UserId",
                table: "UserProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProgress",
                table: "UserProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Leaves",
                table: "Leaves");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssignedBranches",
                table: "AssignedBranches");

            migrationBuilder.RenameTable(
                name: "UserProgress",
                newName: "LearningProgress");

            migrationBuilder.RenameTable(
                name: "Leaves",
                newName: "Leafs");

            migrationBuilder.RenameTable(
                name: "AssignedBranches",
                newName: "UserBranchAssignment");

            migrationBuilder.RenameIndex(
                name: "IX_UserProgress_UserId",
                table: "LearningProgress",
                newName: "IX_LearningProgress_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProgress_BranchId",
                table: "LearningProgress",
                newName: "IX_LearningProgress_BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_Leaves_BranchId",
                table: "Leafs",
                newName: "IX_Leafs_BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedBranches_UserId",
                table: "UserBranchAssignment",
                newName: "IX_UserBranchAssignment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedBranches_BranchId",
                table: "UserBranchAssignment",
                newName: "IX_UserBranchAssignment_BranchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningProgress",
                table: "LearningProgress",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Leafs",
                table: "Leafs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBranchAssignment",
                table: "UserBranchAssignment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Leafs_Branches_BranchId",
                table: "Leafs",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgress_Branches_BranchId",
                table: "LearningProgress",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgress_Users_UserId",
                table: "LearningProgress",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranchAssignment_Branches_BranchId",
                table: "UserBranchAssignment",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranchAssignment_Users_UserId",
                table: "UserBranchAssignment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leafs_Branches_BranchId",
                table: "Leafs");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgress_Branches_BranchId",
                table: "LearningProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgress_Users_UserId",
                table: "LearningProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBranchAssignment_Branches_BranchId",
                table: "UserBranchAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBranchAssignment_Users_UserId",
                table: "UserBranchAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBranchAssignment",
                table: "UserBranchAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningProgress",
                table: "LearningProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Leafs",
                table: "Leafs");

            migrationBuilder.RenameTable(
                name: "UserBranchAssignment",
                newName: "AssignedBranches");

            migrationBuilder.RenameTable(
                name: "LearningProgress",
                newName: "UserProgress");

            migrationBuilder.RenameTable(
                name: "Leafs",
                newName: "Leaves");

            migrationBuilder.RenameIndex(
                name: "IX_UserBranchAssignment_UserId",
                table: "AssignedBranches",
                newName: "IX_AssignedBranches_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserBranchAssignment_BranchId",
                table: "AssignedBranches",
                newName: "IX_AssignedBranches_BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgress_UserId",
                table: "UserProgress",
                newName: "IX_UserProgress_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgress_BranchId",
                table: "UserProgress",
                newName: "IX_UserProgress_BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_Leafs_BranchId",
                table: "Leaves",
                newName: "IX_Leaves_BranchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssignedBranches",
                table: "AssignedBranches",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProgress",
                table: "UserProgress",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Leaves",
                table: "Leaves",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedBranches_Branches_BranchId",
                table: "AssignedBranches",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedBranches_Users_UserId",
                table: "AssignedBranches",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_Branches_BranchId",
                table: "Leaves",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProgress_Branches_BranchId",
                table: "UserProgress",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProgress_Users_UserId",
                table: "UserProgress",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
