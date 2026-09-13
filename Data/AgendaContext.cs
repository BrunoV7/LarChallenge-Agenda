using Agenda.Models;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Data
{

    public class AgendaContext : DbContext
    {
        public AgendaContext(DbContextOptions<AgendaContext> options)
        : base(options)
        {
        }

        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Telefone> Telefones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Telefone>()
                .Property(t => t.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<Telefone>()
                .HasOne(t => t.Pessoa)
                .WithMany(p => p.Telefones)
                .HasForeignKey(t => t.IdPessoa);

            modelBuilder.Entity<Pessoa>()
                .HasIndex(p => p.CPF)
                .IsUnique();

            modelBuilder.Entity<Telefone>()
                .HasIndex(t => new { t.Numero, t.IdPessoa })
                .IsUnique();
        }

    }
}