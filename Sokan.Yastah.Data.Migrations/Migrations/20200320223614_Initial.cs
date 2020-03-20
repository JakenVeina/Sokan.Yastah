using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Sokan.Yastah.Data.Migrations.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Administration");

            migrationBuilder.EnsureSchema(
                name: "Authentication");

            migrationBuilder.EnsureSchema(
                name: "Characters");

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
                name: "CharacterGuilds",
                schema: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterGuilds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterLevelDefinitions",
                schema: "Characters",
                columns: table => new
                {
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterLevelDefinitions", x => x.Level);
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
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
                name: "CharacterGuildDivisions",
                schema: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterGuildDivisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterGuildDivisions_CharacterGuilds_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "Characters",
                        principalTable: "CharacterGuilds",
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
                name: "Characters",
                schema: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Users",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdministrationActions",
                schema: "Administration",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeId = table.Column<int>(nullable: false),
                    Performed = table.Column<DateTimeOffset>(nullable: false),
                    PerformedById = table.Column<long>(nullable: true)
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdministrationActions_AdministrationActionTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationTickets",
                schema: "Authentication",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(nullable: false),
                    CreationId = table.Column<long>(nullable: false),
                    DeletionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationTickets_AdministrationActions_CreationId",
                        column: x => x.CreationId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthenticationTickets_AdministrationActions_DeletionId",
                        column: x => x.DeletionId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthenticationTickets_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Users",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterGuildDivisionVersions",
                schema: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DivisionId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationId = table.Column<long>(nullable: false),
                    PreviousVersionId = table.Column<long>(nullable: true),
                    NextVersionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterGuildDivisionVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterGuildDivisionVersions_AdministrationActions_Creati~",
                        column: x => x.CreationId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterGuildDivisionVersions_CharacterGuildDivisions_Divi~",
                        column: x => x.DivisionId,
                        principalSchema: "Characters",
                        principalTable: "CharacterGuildDivisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterGuildDivisionVersions_NextVersion",
                        column: x => x.NextVersionId,
                        principalSchema: "Characters",
                        principalTable: "CharacterGuildDivisionVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterGuildDivisionVersions_PreviousVersion",
                        column: x => x.PreviousVersionId,
                        principalSchema: "Characters",
                        principalTable: "CharacterGuildDivisionVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterGuildVersions",
                schema: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationId = table.Column<long>(nullable: false),
                    PreviousVersionId = table.Column<long>(nullable: true),
                    NextVersionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterGuildVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterGuildVersions_AdministrationActions_CreationId",
                        column: x => x.CreationId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterGuildVersions_CharacterGuilds_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "Characters",
                        principalTable: "CharacterGuilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterGuildVersions_CharacterGuildVersions_NextVersionId",
                        column: x => x.NextVersionId,
                        principalSchema: "Characters",
                        principalTable: "CharacterGuildVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterGuildVersions_CharacterGuildVersions_PreviousVersi~",
                        column: x => x.PreviousVersionId,
                        principalSchema: "Characters",
                        principalTable: "CharacterGuildVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterLevelDefinitionVersions",
                schema: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Level = table.Column<int>(nullable: false),
                    ExperienceThreshold = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationId = table.Column<long>(nullable: false),
                    PreviousVersionId = table.Column<long>(nullable: true),
                    NextVersionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterLevelDefinitionVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterLevelDefinitionVersions_AdministrationActions_Crea~",
                        column: x => x.CreationId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterLevelDefinitionVersions_CharacterLevelDefinitions_~",
                        column: x => x.Level,
                        principalSchema: "Characters",
                        principalTable: "CharacterLevelDefinitions",
                        principalColumn: "Level",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterLevelDefinitionVersions_NextVersion",
                        column: x => x.NextVersionId,
                        principalSchema: "Characters",
                        principalTable: "CharacterLevelDefinitionVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterLevelDefinitionVersions_PreviousVersion",
                        column: x => x.PreviousVersionId,
                        principalSchema: "Characters",
                        principalTable: "CharacterLevelDefinitionVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterVersions",
                schema: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    DivisionId = table.Column<long>(nullable: false),
                    ExperiencePoints = table.Column<int>(nullable: false),
                    GoldAmount = table.Column<int>(nullable: false),
                    InsanityValue = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationId = table.Column<long>(nullable: false),
                    PreviousVersionId = table.Column<long>(nullable: true),
                    NextVersionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterVersions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "Characters",
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterVersions_AdministrationActions_CreationId",
                        column: x => x.CreationId,
                        principalSchema: "Administration",
                        principalTable: "AdministrationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterVersions_CharacterGuildDivisions_DivisionId",
                        column: x => x.DivisionId,
                        principalSchema: "Characters",
                        principalTable: "CharacterGuildDivisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterVersions_CharacterVersions_NextVersionId",
                        column: x => x.NextVersionId,
                        principalSchema: "Characters",
                        principalTable: "CharacterVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterVersions_CharacterVersions_PreviousVersionId",
                        column: x => x.PreviousVersionId,
                        principalSchema: "Characters",
                        principalTable: "CharacterVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissionMappings",
                schema: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationId = table.Column<long>(nullable: false),
                    PreviousVersionId = table.Column<long>(nullable: true),
                    NextVersionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleVersions_AdministrationActions_CreationId",
                        column: x => x.CreationId,
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                    { 2, "UserManagement" },
                    { 3, "CharacterManagement" }
                });

            migrationBuilder.InsertData(
                schema: "Permissions",
                table: "PermissionCategories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Permissions related to administration of the application", "Administration" },
                    { 2, "Permissions related to administration of game characters", "CharacterAdministration" }
                });

            migrationBuilder.InsertData(
                schema: "Administration",
                table: "AdministrationActionTypes",
                columns: new[] { "Id", "CategoryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "RoleCreated" },
                    { 463, 3, "CharacterRestored" },
                    { 462, 3, "CharacterDeleted" },
                    { 461, 3, "CharacterModified" },
                    { 460, 3, "CharacterCreated" },
                    { 441, 3, "LevelDefinitionsUpdated" },
                    { 440, 3, "LevelDefinitionsInitialized" },
                    { 423, 3, "DivisionRestored" },
                    { 422, 3, "DivisionDeleted" },
                    { 420, 3, "DivisionCreated" },
                    { 421, 3, "DivisionModified" },
                    { 402, 3, "GuildDeleted" },
                    { 401, 3, "GuildModified" },
                    { 400, 3, "GuildCreated" },
                    { 22, 2, "DefaultsModified" },
                    { 21, 2, "UserModified" },
                    { 20, 2, "UserCreated" },
                    { 4, 1, "RoleRestored" },
                    { 3, 1, "RoleDeleted" },
                    { 2, 1, "RoleModified" },
                    { 403, 3, "GuildRestored" }
                });

            migrationBuilder.InsertData(
                schema: "Permissions",
                table: "Permissions",
                columns: new[] { "PermissionId", "CategoryId", "Description", "Name" },
                values: new object[,]
                {
                    { 100, 2, "Allows management of character guilds", "ManageGuilds" },
                    { 1, 1, "Allows management of application permissions", "ManagePermissions" },
                    { 2, 1, "Allows management of application roles", "ManageRoles" },
                    { 3, 1, "Allows management of application users", "ManageUsers" },
                    { 101, 2, "Allows management of character level definitions", "ManageLevels" }
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
                name: "IX_AuthenticationTickets_CreationId",
                schema: "Authentication",
                table: "AuthenticationTickets",
                column: "CreationId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationTickets_DeletionId",
                schema: "Authentication",
                table: "AuthenticationTickets",
                column: "DeletionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationTickets_UserId",
                schema: "Authentication",
                table: "AuthenticationTickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGuildDivisions_GuildId",
                schema: "Characters",
                table: "CharacterGuildDivisions",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGuildDivisionVersions_CreationId",
                schema: "Characters",
                table: "CharacterGuildDivisionVersions",
                column: "CreationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGuildDivisionVersions_DivisionId",
                schema: "Characters",
                table: "CharacterGuildDivisionVersions",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGuildDivisionVersions_NextVersionId",
                schema: "Characters",
                table: "CharacterGuildDivisionVersions",
                column: "NextVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGuildDivisionVersions_PreviousVersionId",
                schema: "Characters",
                table: "CharacterGuildDivisionVersions",
                column: "PreviousVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGuildVersions_CreationId",
                schema: "Characters",
                table: "CharacterGuildVersions",
                column: "CreationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGuildVersions_GuildId",
                schema: "Characters",
                table: "CharacterGuildVersions",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGuildVersions_NextVersionId",
                schema: "Characters",
                table: "CharacterGuildVersions",
                column: "NextVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGuildVersions_PreviousVersionId",
                schema: "Characters",
                table: "CharacterGuildVersions",
                column: "PreviousVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLevelDefinitionVersions_CreationId",
                schema: "Characters",
                table: "CharacterLevelDefinitionVersions",
                column: "CreationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLevelDefinitionVersions_Level",
                schema: "Characters",
                table: "CharacterLevelDefinitionVersions",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLevelDefinitionVersions_NextVersionId",
                schema: "Characters",
                table: "CharacterLevelDefinitionVersions",
                column: "NextVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLevelDefinitionVersions_PreviousVersionId",
                schema: "Characters",
                table: "CharacterLevelDefinitionVersions",
                column: "PreviousVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_OwnerId",
                schema: "Characters",
                table: "Characters",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterVersions_CharacterId",
                schema: "Characters",
                table: "CharacterVersions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterVersions_CreationId",
                schema: "Characters",
                table: "CharacterVersions",
                column: "CreationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterVersions_DivisionId",
                schema: "Characters",
                table: "CharacterVersions",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterVersions_NextVersionId",
                schema: "Characters",
                table: "CharacterVersions",
                column: "NextVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterVersions_PreviousVersionId",
                schema: "Characters",
                table: "CharacterVersions",
                column: "PreviousVersionId",
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
                name: "IX_RoleVersions_CreationId",
                schema: "Roles",
                table: "RoleVersions",
                column: "CreationId");

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
                name: "AuthenticationTickets",
                schema: "Authentication");

            migrationBuilder.DropTable(
                name: "CharacterGuildDivisionVersions",
                schema: "Characters");

            migrationBuilder.DropTable(
                name: "CharacterGuildVersions",
                schema: "Characters");

            migrationBuilder.DropTable(
                name: "CharacterLevelDefinitionVersions",
                schema: "Characters");

            migrationBuilder.DropTable(
                name: "CharacterVersions",
                schema: "Characters");

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
                name: "CharacterLevelDefinitions",
                schema: "Characters");

            migrationBuilder.DropTable(
                name: "Characters",
                schema: "Characters");

            migrationBuilder.DropTable(
                name: "CharacterGuildDivisions",
                schema: "Characters");

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
                name: "CharacterGuilds",
                schema: "Characters");

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
