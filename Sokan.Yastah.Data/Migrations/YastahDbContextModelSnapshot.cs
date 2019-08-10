﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Sokan.Yastah.Data;

namespace Sokan.Yastah.Data.Migrations
{
    [DbContext(typeof(YastahDbContext))]
    partial class YastahDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Sokan.Yastah.Data.Administration.AdministrationActionCategoryEntity", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("AdministrationActionCategories","Administration");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "RoleManagement"
                        },
                        new
                        {
                            Id = 2,
                            Name = "UserManagement"
                        });
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Administration.AdministrationActionEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("Performed");

                    b.Property<long>("PerformedById");

                    b.Property<int>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("PerformedById");

                    b.HasIndex("TypeId");

                    b.ToTable("AdministrationActions","Administration");
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Administration.AdministrationActionTypeEntity", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("CategoryId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("AdministrationActionTypes","Administration");

                    b.HasData(
                        new
                        {
                            Id = 20,
                            CategoryId = 2,
                            Name = "UserInitialization"
                        },
                        new
                        {
                            Id = 1,
                            CategoryId = 1,
                            Name = "RoleCreated"
                        },
                        new
                        {
                            Id = 2,
                            CategoryId = 1,
                            Name = "RoleModified"
                        },
                        new
                        {
                            Id = 3,
                            CategoryId = 1,
                            Name = "RoleDeleted"
                        },
                        new
                        {
                            Id = 4,
                            CategoryId = 1,
                            Name = "RoleRestored"
                        },
                        new
                        {
                            Id = 5,
                            CategoryId = 1,
                            Name = "PermissionsChanged"
                        });
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Permissions.PermissionCategoryEntity", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("PermissionCategories","Permissions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Permissions related to administration of the application",
                            Name = "Administration"
                        });
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Permissions.PermissionEntity", b =>
                {
                    b.Property<int>("PermissionId");

                    b.Property<int>("CategoryId");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("PermissionId");

                    b.HasIndex("CategoryId", "Name")
                        .IsUnique();

                    b.ToTable("Permissions","Permissions");

                    b.HasData(
                        new
                        {
                            PermissionId = 1,
                            CategoryId = 1,
                            Description = "Allows management of application permissions",
                            Name = "ManagePermissions"
                        },
                        new
                        {
                            PermissionId = 2,
                            CategoryId = 1,
                            Description = "Allows management of application roles",
                            Name = "ManageRoles"
                        });
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Roles.RoleEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("Roles","Roles");
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Roles.RolePermissionMappingEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CreationId");

                    b.Property<long?>("DeletionId");

                    b.Property<int>("PermissionId");

                    b.Property<long>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("CreationId");

                    b.HasIndex("DeletionId");

                    b.HasIndex("PermissionId");

                    b.HasIndex("RoleId");

                    b.ToTable("RolePermissionMappings","Roles");
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Roles.RoleVersionEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ActionId");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<long?>("NextVersionId");

                    b.Property<long?>("PreviousVersionId");

                    b.Property<long>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("ActionId");

                    b.HasIndex("NextVersionId")
                        .IsUnique();

                    b.HasIndex("PreviousVersionId")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.ToTable("RoleVersions","Roles");
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Users.DefaultPermissionMappingEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CreationId");

                    b.Property<long?>("DeletionId");

                    b.Property<int>("PermissionId");

                    b.HasKey("Id");

                    b.HasIndex("CreationId");

                    b.HasIndex("DeletionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("DefaultPermissionMappings","Users");
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Users.DefaultRoleMappingEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CreationId");

                    b.Property<long?>("DeletionId");

                    b.Property<long>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("CreationId");

                    b.HasIndex("DeletionId");

                    b.HasIndex("RoleId");

                    b.ToTable("DefaultRoleMappings","Users");
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Users.UserEntity", b =>
                {
                    b.Property<long>("Id");

                    b.Property<string>("AvatarHash");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<DateTimeOffset>("FirstSeen");

                    b.Property<DateTimeOffset>("LastSeen");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Users","Users");
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Users.UserPermissionMappingEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CreationId");

                    b.Property<long?>("DeletionId");

                    b.Property<bool>("IsDenied");

                    b.Property<int>("PermissionId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CreationId");

                    b.HasIndex("DeletionId");

                    b.HasIndex("PermissionId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPermissionMappings","Users");
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Users.UserRoleMappingEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CreationId");

                    b.Property<long?>("DeletionId");

                    b.Property<long>("RoleId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CreationId");

                    b.HasIndex("DeletionId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoleMappings","Users");
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Administration.AdministrationActionEntity", b =>
                {
                    b.HasOne("Sokan.Yastah.Data.Users.UserEntity", "PerformedBy")
                        .WithMany()
                        .HasForeignKey("PerformedById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionTypeEntity", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Administration.AdministrationActionTypeEntity", b =>
                {
                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionCategoryEntity", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Permissions.PermissionEntity", b =>
                {
                    b.HasOne("Sokan.Yastah.Data.Permissions.PermissionCategoryEntity", "Category")
                        .WithMany("Permissions")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Roles.RolePermissionMappingEntity", b =>
                {
                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionEntity", "Creation")
                        .WithMany()
                        .HasForeignKey("CreationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionEntity", "Deletion")
                        .WithMany()
                        .HasForeignKey("DeletionId");

                    b.HasOne("Sokan.Yastah.Data.Permissions.PermissionEntity", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sokan.Yastah.Data.Roles.RoleEntity", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Roles.RoleVersionEntity", b =>
                {
                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionEntity", "Action")
                        .WithMany()
                        .HasForeignKey("ActionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sokan.Yastah.Data.Roles.RoleVersionEntity", "NextVersion")
                        .WithOne()
                        .HasForeignKey("Sokan.Yastah.Data.Roles.RoleVersionEntity", "NextVersionId");

                    b.HasOne("Sokan.Yastah.Data.Roles.RoleVersionEntity", "PreviousVersion")
                        .WithOne()
                        .HasForeignKey("Sokan.Yastah.Data.Roles.RoleVersionEntity", "PreviousVersionId");

                    b.HasOne("Sokan.Yastah.Data.Roles.RoleEntity", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Users.DefaultPermissionMappingEntity", b =>
                {
                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionEntity", "Creation")
                        .WithMany()
                        .HasForeignKey("CreationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionEntity", "Deletion")
                        .WithMany()
                        .HasForeignKey("DeletionId");

                    b.HasOne("Sokan.Yastah.Data.Permissions.PermissionEntity", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Users.DefaultRoleMappingEntity", b =>
                {
                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionEntity", "Creation")
                        .WithMany()
                        .HasForeignKey("CreationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionEntity", "Deletion")
                        .WithMany()
                        .HasForeignKey("DeletionId");

                    b.HasOne("Sokan.Yastah.Data.Roles.RoleEntity", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Users.UserPermissionMappingEntity", b =>
                {
                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionEntity", "Creation")
                        .WithMany()
                        .HasForeignKey("CreationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionEntity", "Deletion")
                        .WithMany()
                        .HasForeignKey("DeletionId");

                    b.HasOne("Sokan.Yastah.Data.Permissions.PermissionEntity", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sokan.Yastah.Data.Users.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sokan.Yastah.Data.Users.UserRoleMappingEntity", b =>
                {
                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionEntity", "Creation")
                        .WithMany()
                        .HasForeignKey("CreationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sokan.Yastah.Data.Administration.AdministrationActionEntity", "Deletion")
                        .WithMany()
                        .HasForeignKey("DeletionId");

                    b.HasOne("Sokan.Yastah.Data.Roles.RoleEntity", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sokan.Yastah.Data.Users.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
