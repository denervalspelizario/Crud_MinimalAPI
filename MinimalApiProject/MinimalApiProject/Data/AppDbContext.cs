using Microsoft.EntityFrameworkCore;
using MinimalApiProject.Model;

namespace MinimalApiProject.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // tabela que será CRIADA chamada de Usuarios baseado em UsuarioModel
        public DbSet<UsuarioModel> Usuarios { get; set; }
    }
}
