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

    }
}