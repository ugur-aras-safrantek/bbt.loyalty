using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bbt.Campaign.EntityFrameworkCore.Migrations
{
    public partial class mig1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementFrequencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementFrequencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AchievementTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessLines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CampaignStartTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignStartTerms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReportView",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JoinDateStr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    CampaignCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CampaignName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsContinuingCampaign = table.Column<int>(type: "int", nullable: false),
                    CampaignStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CampaignStartDateStr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsBundle = table.Column<bool>(type: "bit", nullable: false),
                    JoinTypeId = table.Column<int>(type: "int", nullable: false),
                    JoinTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerTypeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CampaignStartTermId = table.Column<int>(type: "int", nullable: false),
                    CampaignStartTermName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessLineId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessLineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AchievementTypeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AchievementTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReportView", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JoinTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParticipationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceConstants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceConstants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetOperations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Targets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    RefId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Targets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetViewTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetViewTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TriggerTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggerTimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerificationTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationTimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViewOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TopLimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AchievementFrequencyId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MaxTopLimitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: true),
                    MaxTopLimitRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxTopLimitUtilization = table.Column<decimal>(type: "decimal(18,2)", maxLength: 250, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    RefId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopLimits_AchievementFrequencies_AchievementFrequencyId",
                        column: x => x.AchievementFrequencyId,
                        principalTable: "AchievementFrequencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopLimits_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleAuthorizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleTypeId = table.Column<int>(type: "int", nullable: false),
                    ModuleTypeId = table.Column<int>(type: "int", nullable: false),
                    AuthorizationTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleAuthorizations_AuthorizationTypes_AuthorizationTypeId",
                        column: x => x.AuthorizationTypeId,
                        principalTable: "AuthorizationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleAuthorizations_ModuleTypes_ModuleTypeId",
                        column: x => x.ModuleTypeId,
                        principalTable: "ModuleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleAuthorizations_RoleTypes_RoleTypeId",
                        column: x => x.RoleTypeId,
                        principalTable: "RoleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleTypeId = table.Column<int>(type: "int", nullable: false),
                    LastProcessDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_RoleTypes_RoleTypeId",
                        column: x => x.RoleTypeId,
                        principalTable: "RoleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TargetGroupLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetGroupId = table.Column<int>(type: "int", nullable: false),
                    TargetId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetGroupLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetGroupLines_TargetGroups_TargetGroupId",
                        column: x => x.TargetGroupId,
                        principalTable: "TargetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetGroupLines_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TargetDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetId = table.Column<int>(type: "int", nullable: false),
                    TargetSourceId = table.Column<int>(type: "int", nullable: false),
                    TargetViewTypeId = table.Column<int>(type: "int", nullable: false),
                    TriggerTimeId = table.Column<int>(type: "int", nullable: true),
                    VerificationTimeId = table.Column<int>(type: "int", nullable: true),
                    FlowName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NumberOfTransaction = table.Column<int>(type: "int", nullable: true),
                    FlowFrequency = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AdditionalFlowTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Query = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetDetailEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetDetailTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetDetails_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetDetails_TargetSources_TargetSourceId",
                        column: x => x.TargetSourceId,
                        principalTable: "TargetSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetDetails_TargetViewTypes_TargetViewTypeId",
                        column: x => x.TargetViewTypeId,
                        principalTable: "TargetViewTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetDetails_TriggerTimes_TriggerTimeId",
                        column: x => x.TriggerTimeId,
                        principalTable: "TriggerTimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetDetails_VerificationTimes_VerificationTimeId",
                        column: x => x.VerificationTimeId,
                        principalTable: "VerificationTimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionTr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: true),
                    MaxNumberOfUser = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsBundle = table.Column<bool>(type: "bit", nullable: false),
                    IsContract = table.Column<bool>(type: "bit", nullable: false),
                    ContractId = table.Column<int>(type: "int", nullable: true),
                    SectorId = table.Column<int>(type: "int", nullable: true),
                    ViewOptionId = table.Column<int>(type: "int", nullable: true),
                    ProgramTypeId = table.Column<int>(type: "int", nullable: false),
                    ParticipationTypeId = table.Column<int>(type: "int", nullable: false),
                    ApproveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaigns_ParticipationTypes_ParticipationTypeId",
                        column: x => x.ParticipationTypeId,
                        principalTable: "ParticipationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaigns_ProgramTypes_ProgramTypeId",
                        column: x => x.ProgramTypeId,
                        principalTable: "ProgramTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaigns_Sectors_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaigns_ViewOptions_ViewOptionId",
                        column: x => x.ViewOptionId,
                        principalTable: "ViewOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignAchievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxUtilization = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    AchievementTypeId = table.Column<int>(type: "int", nullable: false),
                    ActionOptionId = table.Column<int>(type: "int", nullable: true),
                    DescriptionTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignAchievements_AchievementTypes_AchievementTypeId",
                        column: x => x.AchievementTypeId,
                        principalTable: "AchievementTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignAchievements_ActionOptions_ActionOptionId",
                        column: x => x.ActionOptionId,
                        principalTable: "ActionOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignAchievements_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignAchievements_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignChannelCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    ChannelCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignChannelCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignChannelCodes_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    CampaignListImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CampaignDetailImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SummaryTr = table.Column<string>(type: "ntext", nullable: true),
                    SummaryEn = table.Column<string>(type: "ntext", nullable: true),
                    ContentTr = table.Column<string>(type: "ntext", nullable: true),
                    ContentEn = table.Column<string>(type: "ntext", nullable: true),
                    DetailTr = table.Column<string>(type: "ntext", nullable: true),
                    DetailEn = table.Column<string>(type: "ntext", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignDetails_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignDocuments_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    JoinTypeId = table.Column<int>(type: "int", nullable: false),
                    CampaignStartTermId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignRules_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignRules_CampaignStartTerms_CampaignStartTermId",
                        column: x => x.CampaignStartTermId,
                        principalTable: "CampaignStartTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignRules_JoinTypes_JoinTypeId",
                        column: x => x.JoinTypeId,
                        principalTable: "JoinTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTargets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    TargetOperationId = table.Column<int>(type: "int", nullable: false),
                    TargetId = table.Column<int>(type: "int", nullable: false),
                    TargetGroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_TargetGroups_TargetGroupId",
                        column: x => x.TargetGroupId,
                        principalTable: "TargetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_TargetOperations_TargetOperationId",
                        column: x => x.TargetOperationId,
                        principalTable: "TargetOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTopLimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopLimitId = table.Column<int>(type: "int", nullable: false),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTopLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTopLimits_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignTopLimits_TopLimits_TopLimitId",
                        column: x => x.TopLimitId,
                        principalTable: "TopLimits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCampaigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsJoin = table.Column<bool>(type: "bit", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCampaigns_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignRuleBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignRuleId = table.Column<int>(type: "int", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignRuleBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignRuleBranches_CampaignRules_CampaignRuleId",
                        column: x => x.CampaignRuleId,
                        principalTable: "CampaignRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignRuleBusinesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignRuleId = table.Column<int>(type: "int", nullable: false),
                    BusinessLineId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignRuleBusinesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignRuleBusinesses_BusinessLines_BusinessLineId",
                        column: x => x.BusinessLineId,
                        principalTable: "BusinessLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignRuleBusinesses_CampaignRules_CampaignRuleId",
                        column: x => x.CampaignRuleId,
                        principalTable: "CampaignRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignRuleCustomerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignRuleId = table.Column<int>(type: "int", nullable: false),
                    CustomerTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignRuleCustomerTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignRuleCustomerTypes_CampaignRules_CampaignRuleId",
                        column: x => x.CampaignRuleId,
                        principalTable: "CampaignRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignRuleCustomerTypes_CustomerTypes_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "CustomerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignRuleIdentities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignRuleId = table.Column<int>(type: "int", nullable: false),
                    Identities = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignRuleIdentities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignRuleIdentities_CampaignRules_CampaignRuleId",
                        column: x => x.CampaignRuleId,
                        principalTable: "CampaignRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AchievementFrequencies",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2762), false, null, null, "Anlık" },
                    { 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2763), false, null, null, "Aylık" },
                    { 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2764), false, null, null, "Yıllık" }
                });

            migrationBuilder.InsertData(
                table: "AchievementTypes",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(762), false, null, null, "Mevduat" },
                    { 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(1259), false, null, null, "Kredi" },
                    { 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(1434), false, null, null, "Cashback" }
                });

            migrationBuilder.InsertData(
                table: "ActionOptions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2048), false, null, null, "Ödeme Cashback" },
                    { 2, "2", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2204), false, null, null, "Fatura Cashback" }
                });

            migrationBuilder.InsertData(
                table: "AuthorizationTypes",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3044), false, null, null, "Insert" },
                    { 2, "2", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3046), false, null, null, "Update" },
                    { 3, "3", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3047), false, null, null, "View" },
                    { 4, "4", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3049), false, null, null, "Approve" }
                });

            migrationBuilder.InsertData(
                table: "BusinessLines",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 231, DateTimeKind.Local).AddTicks(7653), false, null, null, "Bireysel (B)" },
                    { 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 231, DateTimeKind.Local).AddTicks(8045), false, null, null, "Ticari (T)" },
                    { 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 231, DateTimeKind.Local).AddTicks(8185), false, null, null, "Dijital (X)" },
                    { 4, "1", new DateTime(2022, 5, 30, 13, 8, 16, 231, DateTimeKind.Local).AddTicks(8526), false, null, null, "Ticari 1 (I)" },
                    { 5, "1", new DateTime(2022, 5, 30, 13, 8, 16, 231, DateTimeKind.Local).AddTicks(8680), false, null, null, "Ticari 2 (P)" },
                    { 6, "1", new DateTime(2022, 5, 30, 13, 8, 16, 231, DateTimeKind.Local).AddTicks(8849), false, null, null, "Ticari 3 (M)" },
                    { 7, "1", new DateTime(2022, 5, 30, 13, 8, 16, 231, DateTimeKind.Local).AddTicks(8931), false, null, null, "Kurumsal (K)" },
                    { 8, "1", new DateTime(2022, 5, 30, 13, 8, 16, 231, DateTimeKind.Local).AddTicks(8975), false, null, null, "Kurumsal 1 (A)" }
                });

            migrationBuilder.InsertData(
                table: "CampaignStartTerms",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2628), false, null, null, "Katılım Anında" },
                    { 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2629), false, null, null, "Dönem Başlangıcı" }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "TRY", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2824), false, null, null, "TRY" },
                    { 2, "GBP", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2826), false, null, null, "GBP" },
                    { 3, "EUR", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2827), false, null, null, "EUR" },
                    { 4, "USD", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2828), false, null, null, "USD" }
                });

            migrationBuilder.InsertData(
                table: "CustomerTypes",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 231, DateTimeKind.Local).AddTicks(9906), false, null, null, "Gerçek" },
                    { 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(79), false, null, null, "Tüzel" },
                    { 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(157), false, null, null, "Ortak" },
                    { 4, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(236), false, null, null, "Reşit Olmayan" },
                    { 5, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(307), false, null, null, "Adi Ortaklık" }
                });

            migrationBuilder.InsertData(
                table: "JoinTypes",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "SK", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2549), false, null, null, "Tüm Müşteriler" },
                    { 2, "SK", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2551), false, null, null, "Müşteri Özelinde" },
                    { 3, "SK", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2563), false, null, null, "İş Kolu Özelinde" },
                    { 4, "SK", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2575), false, null, null, "Şube Özelinde" },
                    { 5, "SK", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2576), false, null, null, "Müşteri Tipi Özelinde" }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "TR", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2385), false, null, null, "Türkçe" },
                    { 2, "EN", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2386), false, null, null, "İngilizce" }
                });

            migrationBuilder.InsertData(
                table: "ModuleTypes",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3024), false, null, null, "Campaign" },
                    { 2, "2", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3025), false, null, null, "TopLimit" },
                    { 3, "3", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3027), false, null, null, "Target" },
                    { 4, "4", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3028), false, null, null, "Report" }
                });

            migrationBuilder.InsertData(
                table: "ParticipationTypes",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2979), false, null, null, "Otomatik Katılım" },
                    { 2, "2", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2981), false, null, null, "Müşteri Seçimi" },
                    { 3, "3", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2983), false, null, null, "Operatör Seçimi" }
                });

            migrationBuilder.InsertData(
                table: "ProgramTypes",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2853), false, null, null, "Sadakat" },
                    { 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2854), false, null, null, "Kampanya" },
                    { 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2855), false, null, null, "Kazanım" }
                });

            migrationBuilder.InsertData(
                table: "RoleTypes",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2999), false, null, null, "isLoyaltyCreator" },
                    { 2, "2", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3000), false, null, null, "isLoyaltyApprover" },
                    { 3, "3", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3001), false, null, null, "isLoyaltyReader" },
                    { 4, "4", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3002), false, null, null, "isLoyaltyRuleCreator" },
                    { 5, "5", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3005), false, null, null, "isLoyaltyRuleApprover" }
                });

            migrationBuilder.InsertData(
                table: "Sectors",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "Akr", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2494), false, null, null, "Akaryakıt" },
                    { 2, "Chl", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2496), false, null, null, "Giyim" },
                    { 3, "Edu", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2497), false, null, null, "Eğitim" }
                });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3181), false, null, null, "Taslak" },
                    { 2, "2", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3180), false, null, null, "Onaya Gönderildi" },
                    { 3, "3", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3178), false, null, null, "Tarihçe" },
                    { 4, "4", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3177), false, null, null, "Onaylandı" }
                });

            migrationBuilder.InsertData(
                table: "TargetOperations",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2658), false, null, null, "ve" },
                    { 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2659), false, null, null, "veya" },
                    { 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2660), false, null, null, "kesişim" },
                    { 4, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2662), false, null, null, "fark" }
                });

            migrationBuilder.InsertData(
                table: "TargetSources",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2954), false, null, null, "Akış" },
                    { 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2955), false, null, null, "Sorgu" }
                });

            migrationBuilder.InsertData(
                table: "TargetViewTypes",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2877), false, null, null, "Progress Bar" },
                    { 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2879), false, null, null, "Bilgi" },
                    { 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2880), false, null, null, "Görüntülenmeyecek" }
                });

            migrationBuilder.InsertData(
                table: "TriggerTimes",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2906), false, null, null, "Hedefe Ulaşıldığı Anda" },
                    { 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2907), false, null, null, "Tamamlandıktan Sonra" }
                });

            migrationBuilder.InsertData(
                table: "VerificationTimes",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2931), false, null, null, "İlk Kontrol Edildiğinde" },
                    { 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2932), false, null, null, "Her Kontrol Edildiğinde" }
                });

            migrationBuilder.InsertData(
                table: "ViewOptions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, "SK", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2520), false, null, null, "Sürekli Kampanyalar" },
                    { 2, "DK", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2521), false, null, null, "Dönemsel Kampanyalar" },
                    { 3, "AK", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2522), false, null, null, "Genel Kampanyalar" },
                    { 4, "NG", "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(2524), false, null, null, "Görüntülenmeyecek" }
                });

            migrationBuilder.InsertData(
                table: "RoleAuthorizations",
                columns: new[] { "Id", "AuthorizationTypeId", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "ModuleTypeId", "RoleTypeId" },
                values: new object[,]
                {
                    { 1, 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3076), false, null, null, 1, 1 },
                    { 2, 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3082), false, null, null, 1, 1 },
                    { 3, 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3084), false, null, null, 2, 1 },
                    { 4, 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3085), false, null, null, 2, 1 },
                    { 5, 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3087), false, null, null, 3, 1 },
                    { 6, 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3088), false, null, null, 4, 1 },
                    { 7, 4, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3090), false, null, null, 1, 2 },
                    { 8, 4, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3092), false, null, null, 2, 2 },
                    { 9, 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3094), false, null, null, 1, 3 },
                    { 10, 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3097), false, null, null, 2, 3 },
                    { 11, 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3098), false, null, null, 3, 3 },
                    { 12, 3, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3099), false, null, null, 4, 3 },
                    { 13, 1, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3101), false, null, null, 3, 4 },
                    { 14, 2, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3102), false, null, null, 3, 4 },
                    { 15, 4, "1", new DateTime(2022, 5, 30, 13, 8, 16, 232, DateTimeKind.Local).AddTicks(3103), false, null, null, 3, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAchievements_AchievementTypeId",
                table: "CampaignAchievements",
                column: "AchievementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAchievements_ActionOptionId",
                table: "CampaignAchievements",
                column: "ActionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAchievements_CampaignId",
                table: "CampaignAchievements",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAchievements_CurrencyId",
                table: "CampaignAchievements",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignChannelCodes_CampaignId",
                table: "CampaignChannelCodes",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignDetails_CampaignId",
                table: "CampaignDetails",
                column: "CampaignId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignDocuments_CampaignId",
                table: "CampaignDocuments",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRuleBranches_CampaignRuleId",
                table: "CampaignRuleBranches",
                column: "CampaignRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRuleBusinesses_BusinessLineId",
                table: "CampaignRuleBusinesses",
                column: "BusinessLineId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRuleBusinesses_CampaignRuleId",
                table: "CampaignRuleBusinesses",
                column: "CampaignRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRuleCustomerTypes_CampaignRuleId",
                table: "CampaignRuleCustomerTypes",
                column: "CampaignRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRuleCustomerTypes_CustomerTypeId",
                table: "CampaignRuleCustomerTypes",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRuleIdentities_CampaignRuleId",
                table: "CampaignRuleIdentities",
                column: "CampaignRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRules_CampaignId",
                table: "CampaignRules",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRules_CampaignStartTermId",
                table: "CampaignRules",
                column: "CampaignStartTermId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRules_JoinTypeId",
                table: "CampaignRules",
                column: "JoinTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_ParticipationTypeId",
                table: "Campaigns",
                column: "ParticipationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_ProgramTypeId",
                table: "Campaigns",
                column: "ProgramTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_SectorId",
                table: "Campaigns",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_ViewOptionId",
                table: "Campaigns",
                column: "ViewOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_CampaignId",
                table: "CampaignTargets",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_TargetGroupId",
                table: "CampaignTargets",
                column: "TargetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_TargetId",
                table: "CampaignTargets",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_TargetOperationId",
                table: "CampaignTargets",
                column: "TargetOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTopLimits_CampaignId",
                table: "CampaignTopLimits",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTopLimits_TopLimitId",
                table: "CampaignTopLimits",
                column: "TopLimitId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCampaigns_CampaignId",
                table: "CustomerCampaigns",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAuthorizations_AuthorizationTypeId",
                table: "RoleAuthorizations",
                column: "AuthorizationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAuthorizations_ModuleTypeId",
                table: "RoleAuthorizations",
                column: "ModuleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAuthorizations_RoleTypeId",
                table: "RoleAuthorizations",
                column: "RoleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetDetails_TargetId",
                table: "TargetDetails",
                column: "TargetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetDetails_TargetSourceId",
                table: "TargetDetails",
                column: "TargetSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetDetails_TargetViewTypeId",
                table: "TargetDetails",
                column: "TargetViewTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetDetails_TriggerTimeId",
                table: "TargetDetails",
                column: "TriggerTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetDetails_VerificationTimeId",
                table: "TargetDetails",
                column: "VerificationTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetGroupLines_TargetGroupId",
                table: "TargetGroupLines",
                column: "TargetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetGroupLines_TargetId",
                table: "TargetGroupLines",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_TopLimits_AchievementFrequencyId",
                table: "TopLimits",
                column: "AchievementFrequencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TopLimits_CurrencyId",
                table: "TopLimits",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleTypeId",
                table: "UserRoles",
                column: "RoleTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignAchievements");

            migrationBuilder.DropTable(
                name: "CampaignChannelCodes");

            migrationBuilder.DropTable(
                name: "CampaignDetails");

            migrationBuilder.DropTable(
                name: "CampaignDocuments");

            migrationBuilder.DropTable(
                name: "CampaignRuleBranches");

            migrationBuilder.DropTable(
                name: "CampaignRuleBusinesses");

            migrationBuilder.DropTable(
                name: "CampaignRuleCustomerTypes");

            migrationBuilder.DropTable(
                name: "CampaignRuleIdentities");

            migrationBuilder.DropTable(
                name: "CampaignTargets");

            migrationBuilder.DropTable(
                name: "CampaignTopLimits");

            migrationBuilder.DropTable(
                name: "CustomerCampaigns");

            migrationBuilder.DropTable(
                name: "CustomerReportView");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "RoleAuthorizations");

            migrationBuilder.DropTable(
                name: "ServiceConstants");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "TargetDefinitions");

            migrationBuilder.DropTable(
                name: "TargetDetails");

            migrationBuilder.DropTable(
                name: "TargetGroupLines");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "AchievementTypes");

            migrationBuilder.DropTable(
                name: "ActionOptions");

            migrationBuilder.DropTable(
                name: "BusinessLines");

            migrationBuilder.DropTable(
                name: "CustomerTypes");

            migrationBuilder.DropTable(
                name: "CampaignRules");

            migrationBuilder.DropTable(
                name: "TargetOperations");

            migrationBuilder.DropTable(
                name: "TopLimits");

            migrationBuilder.DropTable(
                name: "AuthorizationTypes");

            migrationBuilder.DropTable(
                name: "ModuleTypes");

            migrationBuilder.DropTable(
                name: "TargetSources");

            migrationBuilder.DropTable(
                name: "TargetViewTypes");

            migrationBuilder.DropTable(
                name: "TriggerTimes");

            migrationBuilder.DropTable(
                name: "VerificationTimes");

            migrationBuilder.DropTable(
                name: "TargetGroups");

            migrationBuilder.DropTable(
                name: "Targets");

            migrationBuilder.DropTable(
                name: "RoleTypes");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "CampaignStartTerms");

            migrationBuilder.DropTable(
                name: "JoinTypes");

            migrationBuilder.DropTable(
                name: "AchievementFrequencies");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "ParticipationTypes");

            migrationBuilder.DropTable(
                name: "ProgramTypes");

            migrationBuilder.DropTable(
                name: "Sectors");

            migrationBuilder.DropTable(
                name: "ViewOptions");
        }
    }
}
