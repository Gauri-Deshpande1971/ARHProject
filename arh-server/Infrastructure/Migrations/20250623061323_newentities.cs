using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.CreateTable(
                name: "additionalreports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    reports = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UCode = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNo = table.Column<int>(type: "integer", nullable: false),
                    ExtraId1 = table.Column<int>(type: "integer", nullable: true),
                    ExtraId2 = table.Column<int>(type: "integer", nullable: true),
                    ExtraValue1 = table.Column<string>(type: "text", nullable: true),
                    ExtraValue2 = table.Column<string>(type: "text", nullable: true),
                    LogHistory = table.Column<string>(type: "text", nullable: true),
                    JsonData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_additionalreports", x => x.Id);
                });
           
            migrationBuilder.CreateTable(
                name: "complaints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    Large = table.Column<string>(type: "text", nullable: true),
                    Small = table.Column<string>(type: "text", nullable: true),
                    Medium = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UCode = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNo = table.Column<int>(type: "integer", nullable: false),
                    ExtraId1 = table.Column<int>(type: "integer", nullable: true),
                    ExtraId2 = table.Column<int>(type: "integer", nullable: true),
                    ExtraValue1 = table.Column<string>(type: "text", nullable: true),
                    ExtraValue2 = table.Column<string>(type: "text", nullable: true),
                    LogHistory = table.Column<string>(type: "text", nullable: true),
                    JsonData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_complaints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "family",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    familyhistory = table.Column<string>(type: "text", nullable: false),
                    familysetup = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UCode = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNo = table.Column<int>(type: "integer", nullable: false),
                    ExtraId1 = table.Column<int>(type: "integer", nullable: true),
                    ExtraId2 = table.Column<int>(type: "integer", nullable: true),
                    ExtraValue1 = table.Column<string>(type: "text", nullable: true),
                    ExtraValue2 = table.Column<string>(type: "text", nullable: true),
                    LogHistory = table.Column<string>(type: "text", nullable: true),
                    JsonData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_family", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "investigations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    testname = table.Column<string>(type: "text", nullable: false),
                    findings = table.Column<string>(type: "text", nullable: false),
                    investigationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UCode = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNo = table.Column<int>(type: "integer", nullable: false),
                    ExtraId1 = table.Column<int>(type: "integer", nullable: true),
                    ExtraId2 = table.Column<int>(type: "integer", nullable: true),
                    ExtraValue1 = table.Column<string>(type: "text", nullable: true),
                    ExtraValue2 = table.Column<string>(type: "text", nullable: true),
                    LogHistory = table.Column<string>(type: "text", nullable: true),
                    JsonData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investigations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "medications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    details = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UCode = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNo = table.Column<int>(type: "integer", nullable: false),
                    ExtraId1 = table.Column<int>(type: "integer", nullable: true),
                    ExtraId2 = table.Column<int>(type: "integer", nullable: true),
                    ExtraValue1 = table.Column<string>(type: "text", nullable: true),
                    ExtraValue2 = table.Column<string>(type: "text", nullable: true),
                    LogHistory = table.Column<string>(type: "text", nullable: true),
                    JsonData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medications", x => x.Id);
                });
     
            migrationBuilder.CreateTable(
                name: "pasthistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    history = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UCode = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNo = table.Column<int>(type: "integer", nullable: false),
                    ExtraId1 = table.Column<int>(type: "integer", nullable: true),
                    ExtraId2 = table.Column<int>(type: "integer", nullable: true),
                    ExtraValue1 = table.Column<string>(type: "text", nullable: true),
                    ExtraValue2 = table.Column<string>(type: "text", nullable: true),
                    LogHistory = table.Column<string>(type: "text", nullable: true),
                    JsonData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pasthistory", x => x.Id);
                });
      
            migrationBuilder.CreateTable(
                name: "physicalexam",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    T = table.Column<string>(type: "text", nullable: false),
                    P = table.Column<string>(type: "text", nullable: false),
                    RR = table.Column<string>(type: "text", nullable: false),
                    BP = table.Column<string>(type: "text", nullable: false),
                    Glands = table.Column<string>(type: "text", nullable: false),
                    ENTSkin = table.Column<string>(type: "text", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UCode = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNo = table.Column<int>(type: "integer", nullable: false),
                    ExtraId1 = table.Column<int>(type: "integer", nullable: true),
                    ExtraId2 = table.Column<int>(type: "integer", nullable: true),
                    ExtraValue1 = table.Column<string>(type: "text", nullable: true),
                    ExtraValue2 = table.Column<string>(type: "text", nullable: true),
                    LogHistory = table.Column<string>(type: "text", nullable: true),
                    JsonData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_physicalexam", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "physicalgen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    appetite = table.Column<string>(type: "text", nullable: true),
                    thirst = table.Column<string>(type: "text", nullable: true),
                    cravings = table.Column<string>(type: "text", nullable: true),
                    aversions = table.Column<string>(type: "text", nullable: true),
                    stools = table.Column<string>(type: "text", nullable: true),
                    urine = table.Column<string>(type: "text", nullable: true),
                    perspiration = table.Column<string>(type: "text", nullable: true),
                    sleepdream = table.Column<string>(type: "text", nullable: true),
                    menstrualobs = table.Column<string>(type: "text", nullable: true),
                    themalreaction = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UCode = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNo = table.Column<int>(type: "integer", nullable: false),
                    ExtraId1 = table.Column<int>(type: "integer", nullable: true),
                    ExtraId2 = table.Column<int>(type: "integer", nullable: true),
                    ExtraValue1 = table.Column<string>(type: "text", nullable: true),
                    ExtraValue2 = table.Column<string>(type: "text", nullable: true),
                    LogHistory = table.Column<string>(type: "text", nullable: true),
                    JsonData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_physicalgen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "systemeticexam",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    details = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UCode = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNo = table.Column<int>(type: "integer", nullable: false),
                    ExtraId1 = table.Column<int>(type: "integer", nullable: true),
                    ExtraId2 = table.Column<int>(type: "integer", nullable: true),
                    ExtraValue1 = table.Column<string>(type: "text", nullable: true),
                    ExtraValue2 = table.Column<string>(type: "text", nullable: true),
                    LogHistory = table.Column<string>(type: "text", nullable: true),
                    JsonData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_systemeticexam", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.DropTable(
                name: "additionalreports");

            migrationBuilder.DropTable(
                name: "complaints");
            migrationBuilder.DropTable(
                name: "family");
          
            migrationBuilder.DropTable(
                name: "investigations");

            migrationBuilder.DropTable(
                name: "medications");
migrationBuilder.DropTable(
                name: "pasthistory");
            
            migrationBuilder.DropTable(
                name: "physicalexam");

            migrationBuilder.DropTable(
                name: "physicalgen");
            migrationBuilder.DropTable(
                name: "systemeticexam");
        }
    }
}
