using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pump.Model.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Pump");

            migrationBuilder.CreateTable(
                name: "CalculationRequestInfo",
                schema: "Pump",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Request = table.Column<string>(nullable: true),
                    Response = table.Column<string>(nullable: true),
                    HasError = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationRequestInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PumpCalculation",
                schema: "Pump",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(nullable: false),
                    CalcUuid = table.Column<Guid>(nullable: false),
                    CalcState = table.Column<int>(nullable: false),
                    Temperature = table.Column<decimal>(nullable: false),
                    WallTemperature = table.Column<decimal>(nullable: false),
                    Pressure = table.Column<decimal>(nullable: false),
                    CalcRequestInfoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumpCalculation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PumpCalculation_CalculationRequestInfo_CalcRequestInfoId",
                        column: x => x.CalcRequestInfoId,
                        principalSchema: "Pump",
                        principalTable: "CalculationRequestInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PumpCalculation_CalcRequestInfoId",
                schema: "Pump",
                table: "PumpCalculation",
                column: "CalcRequestInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PumpCalculation",
                schema: "Pump");

            migrationBuilder.DropTable(
                name: "CalculationRequestInfo",
                schema: "Pump");
        }
    }
}
