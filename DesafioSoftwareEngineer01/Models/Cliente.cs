using System.ComponentModel.DataAnnotations.Schema;

namespace DesafioSoftwareEngineer01.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string NumeroTelefone { get; set; }
        public Endereco EnderecoCliente { get; set; }

        public Cliente()
        {
            EnderecoCliente = new Endereco();
        }
    }
}
