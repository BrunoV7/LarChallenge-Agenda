using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agenda.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguraFkTelefone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Telefones_Pessoas_PessoaId",
                table: "Telefones");

            migrationBuilder.DropIndex(
                name: "IX_Telefones_PessoaId",
                table: "Telefones");

            migrationBuilder.DropColumn(
                name: "PessoaId",
                table: "Telefones");

            migrationBuilder.CreateIndex(
                name: "IX_Telefones_IdPessoa",
                table: "Telefones",
                column: "IdPessoa");

            migrationBuilder.AddForeignKey(
                name: "FK_Telefones_Pessoas_IdPessoa",
                table: "Telefones",
                column: "IdPessoa",
                principalTable: "Pessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Telefones_Pessoas_IdPessoa",
                table: "Telefones");

            migrationBuilder.DropIndex(
                name: "IX_Telefones_IdPessoa",
                table: "Telefones");

            migrationBuilder.AddColumn<Guid>(
                name: "PessoaId",
                table: "Telefones",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Telefones_PessoaId",
                table: "Telefones",
                column: "PessoaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Telefones_Pessoas_PessoaId",
                table: "Telefones",
                column: "PessoaId",
                principalTable: "Pessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
