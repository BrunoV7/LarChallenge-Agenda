using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agenda.Migrations
{
    /// <inheritdoc />
    public partial class RenomeiaPropriedadesPessoa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Pessoas",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "Pessoas",
                newName: "DataNascimento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Pessoas",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DataNascimento",
                table: "Pessoas",
                newName: "BirthDate");
        }
    }
}
