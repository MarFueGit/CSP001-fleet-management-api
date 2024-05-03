using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FleetManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class TaxiMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Taxis",
                columns: table => new
                {
                    idtaxi = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plate = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxis", x => x.idtaxi);
                });

            migrationBuilder.CreateTable(
                name: "Trajectories",
                columns: table => new
                {
                    idtrajectorie = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    taxi_id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    latitude = table.Column<float>(type: "real", nullable: false),
                    longitude = table.Column<float>(type: "real", nullable: false),
                    plate = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trajectories", x => x.idtrajectorie);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Taxis");

            migrationBuilder.DropTable(
                name: "Trajectories");
        }
    }
}
