using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesafioPolo.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Indicadores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Indicador = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    DataReferencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Media = table.Column<double>(type: "float", nullable: false),
                    Mediana = table.Column<double>(type: "float", nullable: false),
                    DesvioPadrao = table.Column<double>(type: "float", nullable: false),
                    Minimo = table.Column<double>(type: "float", nullable: false),
                    Maximo = table.Column<double>(type: "float", nullable: false),
                    NumeroRespondentes = table.Column<int>(type: "int", nullable: true),
                    BaseCalculo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indicadores", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Indicadores");
        }
    }
}
