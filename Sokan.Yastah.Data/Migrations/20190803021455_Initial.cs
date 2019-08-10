using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Sokan.Yastah.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Administration");

            migrationBuilder.EnsureSchema(
                name: "Permissions");

            migrationBuilder.EnsureSchema(
                name: "Roles");

            migrationBuilder.EnsureSchema(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "AdministrationActionCategories",
                schema: "Administration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrationActionCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionCategories",
                schema: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    AvatarHash = table.Column<string>(nullable: true),
                    FirstSeen = table.Column<DateTimeOffset>(nullable: false),
                    LastSeen = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdministrationActionTypes",
                schema: "Administration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrationActionTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdministrationActionTypes_AdministrationActionCategories_Ca~",
                        column: x => x.CategoryId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActionCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "Permissions",
                columns: table => new
                {
                    PermissionId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.PermissionId);
                    table.ForeignKey(
                        name: "FK_Permissions_PermissionCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Permissions",
                        principalTable: "PermissionCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdministrationActions",
                schema: "Administration",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TypeId = table.Column<int>(nullable: false),
                    Performed = table.Column<DateTimeOffset>(nullable: false),
                    PerformedById = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrationActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdministrationActions_Users_PerformedById",
                        column: x => x.PerformedById,
                        principalSchema: "Users",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdministrationActions_AdministrationActionTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissionMappings",
                schema: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RoleId = table.Column<long>(nullable: false),
                    PermissionId = table.Column<int>(nullable: false),
                    CreationId = table.Column<long>(nullable: false),
                    DeletionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissionMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissionMappings_AdministrationActions_CreationId",
                        column: x => x.CreationId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissionMappings_AdministrationActions_DeletionId",
                        column: x => x.DeletionId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissionMappings_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "Permissions",
                        principalTable: "Permissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissionMappings_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Roles",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleVersions",
                schema: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RoleId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ActionId = table.Column<long>(nullable: false),
                    PreviousVersionId = table.Column<long>(nullable: true),
                    NextVersionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleVersions_AdministrationActions_ActionId",
                        column: x => x.ActionId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleVersions_RoleVersions_NextVersionId",
                        column: x => x.NextVersionId,
                        principalSchema: "Roles",
                        principalTable: "RoleVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleVersions_RoleVersions_PreviousVersionId",
                        column: x => x.PreviousVersionId,
                        principalSchema: "Roles",
                        principalTable: "RoleVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleVersions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Roles",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefaultPermissionMappings",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    PermissionId = table.Column<int>(nullable: false),
                    CreationId = table.Column<long>(nullable: false),
                    DeletionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultPermissionMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefaultPermissionMappings_AdministrationActions_CreationId",
                        column: x => x.CreationId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultPermissionMappings_AdministrationActions_DeletionId",
                        column: x => x.DeletionId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefaultPermissionMappings_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "Permissions",
                        principalTable: "Permissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefaultRoleMappings",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RoleId = table.Column<long>(nullable: false),
                    CreationId = table.Column<long>(nullable: false),
                    DeletionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultRoleMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefaultRoleMappings_AdministrationActions_CreationId",
                        column: x => x.CreationId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultRoleMappings_AdministrationActions_DeletionId",
                        column: x => x.DeletionId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefaultRoleMappings_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Roles",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissionMappings",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UserId = table.Column<long>(nullable: false),
                    PermissionId = table.Column<int>(nullable: false),
                    IsDenied = table.Column<bool>(nullable: false),
                    CreationId = table.Column<long>(nullable: false),
                    DeletionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissionMappings_AdministrationActions_CreationId",
                        column: x => x.CreationId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissionMappings_AdministrationActions_DeletionId",
                        column: x => x.DeletionId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissionMappings_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "Permissions",
                        principalTable: "Permissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissionMappings_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Users",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleMappings",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UserId = table.Column<long>(nullable: false),
                    RoleId = table.Column<long>(nullable: false),
                    CreationId = table.Column<long>(nullable: false),
                    DeletionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoleMappings_AdministrationActions_CreationId",
                        column: x => x.CreationId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleMappings_AdministrationActions_DeletionId",
                        column: x => x.DeletionId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoleMappings_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Roles",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleMappings_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Users",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Administration",
                table: "AdministrationActionCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "RoleManagement" },
                    { 2, "UserManagement" }
                });

            migrationBuilder.InsertData(
                schema: "Permissions",
                table: "PermissionCategories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "Permissions related to administration of the application", "Administration" });

            migrationBuilder.InsertData(
                schema: "Administration",
                table: "AdministrationActionTypes",
                columns: new[] { "Id", "CategoryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "RoleCreated" },
                    { 2, 1, "RoleModified" },
                    { 3, 1, "RoleDeleted" },
                    { 4, 1, "RoleRestored" },
                    { 5, 1, "PermissionsChanged" },
                    { 20, 2, "UserInitialization" }
                });

            migrationBuilder.InsertData(
                schema: "Permissions",
                table: "Permissions",
                columns: new[] { "PermissionId", "CategoryId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Allows management of application permissions", "ManagePermissions" },
                    { 2, 1, "Allows management of application roles", "ManageRoles" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdministrationActionCategories_Name",
                schema: "Administration",
                table: "AdministrationActionCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdministrationActions_PerformedById",
                schema: "Administration",
                table: "AdministrationActions",
                column: "PerformedById");

            migrationBuilder.CreateIndex(
                name: "IX_AdministrationActions_TypeId",
                schema: "Administration",
                table: "AdministrationActions",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AdministrationActionTypes_CategoryId",
                schema: "Administration",
                table: "AdministrationActionTypes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AdministrationActionTypes_Name",
                schema: "Administration",
                table: "AdministrationActionTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionCategories_Name",
                schema: "Permissions",
                table: "PermissionCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CategoryId_Name",
                schema: "Permissions",
                table: "Permissions",
                columns: new[] { "CategoryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionMappings_CreationId",
                schema: "Roles",
                table: "RolePermissionMappings",
                column: "CreationId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionMappings_DeletionId",
                schema: "Roles",
                table: "RolePermissionMappings",
                column: "DeletionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionMappings_PermissionId",
                schema: "Roles",
                table: "RolePermissionMappings",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionMappings_RoleId",
                schema: "Roles",
                table: "RolePermissionMappings",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleVersions_ActionId",
                schema: "Roles",
                table: "RoleVersions",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleVersions_NextVersionId",
                schema: "Roles",
                table: "RoleVersions",
                column: "NextVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleVersions_PreviousVersionId",
                schema: "Roles",
                table: "RoleVersions",
                column: "PreviousVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleVersions_RoleId",
                schema: "Roles",
                table: "RoleVersions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultPermissionMappings_CreationId",
                schema: "Users",
                table: "DefaultPermissionMappings",
                column: "CreationId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultPermissionMappings_DeletionId",
                schema: "Users",
                table: "DefaultPermissionMappings",
                column: "DeletionId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultPermissionMappings_PermissionId",
                schema: "Users",
                table: "DefaultPermissionMappings",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultRoleMappings_CreationId",
                schema: "Users",
                table: "DefaultRoleMappings",
                column: "CreationId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultRoleMappings_DeletionId",
                schema: "Users",
                table: "DefaultRoleMappings",
                column: "DeletionId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultRoleMappings_RoleId",
                schema: "Users",
                table: "DefaultRoleMappings",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionMappings_CreationId",
                schema: "Users",
                table: "UserPermissionMappings",
                column: "CreationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionMappings_DeletionId",
                schema: "Users",
                table: "UserPermissionMappings",
                column: "DeletionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionMappings_PermissionId",
                schema: "Users",
                table: "UserPermissionMappings",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionMappings_UserId",
                schema: "Users",
                table: "UserPermissionMappings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMappings_CreationId",
                schema: "Users",
                table: "UserRoleMappings",
                column: "CreationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMappings_DeletionId",
                schema: "Users",
                table: "UserRoleMappings",
                column: "DeletionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMappings_RoleId",
                schema: "Users",
                table: "UserRoleMappings",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMappings_UserId",
                schema: "Users",
                table: "UserRoleMappings",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissionMappings",
                schema: "Roles");

            migrationBuilder.DropTable(
                name: "RoleVersions",
                schema: "Roles");

            migrationBuilder.DropTable(
                name: "DefaultPermissionMappings",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "DefaultRoleMappings",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "UserPermissionMappings",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "UserRoleMappings",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "Permissions");

            migrationBuilder.DropTable(
                name: "AdministrationActions",
                schema: "Administration");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Roles");

            migrationBuilder.DropTable(
                name: "PermissionCategories",
                schema: "Permissions");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "AdministrationActionTypes",
                schema: "Administration");

            migrationBuilder.DropTable(
                name: "AdministrationActionCategories",
                schema: "Administration");
        }
    }
}
