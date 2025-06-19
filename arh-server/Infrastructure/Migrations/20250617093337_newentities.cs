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
                name: "SessionDispenseTeam",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    SessionId = table.Column<int>(type: "integer", nullable: false),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_SessionDispenseTeam", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionDoctors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    SessionId = table.Column<int>(type: "integer", nullable: false),
                    DoctorId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_SessionDoctors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionSetup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    SessionName = table.Column<string>(type: "text", nullable: false),
                    SessionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_SessionSetup", x => x.Id);
                });

             }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropTable(
                name: "SessionDispenseTeam");

            migrationBuilder.DropTable(
                name: "SessionDoctors");

            migrationBuilder.DropTable(
                name: "SessionSetup");

        }
    }
}
