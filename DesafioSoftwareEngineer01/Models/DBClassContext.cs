using Microsoft.EntityFrameworkCore;

namespace DesafioSoftwareEngineer01.Models
{
    public class DBClassContext : DbContext
    {

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //substitua pela sua string de conexão
            optionsBuilder.UseSqlServer(connectionString: @"Data Source=LEONARDO;Initial Catalog=cliente_endereco;Integrated Security=True;TrustServerCertificate=True");

        }
    }
}