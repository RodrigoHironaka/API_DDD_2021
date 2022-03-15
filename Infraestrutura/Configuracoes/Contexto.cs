using Entidades.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestrutura.Configuracoes
{
    public class Contexto : IdentityDbContext<ApplicationUser>
    {
        public Contexto(DbContextOptions<Contexto> opcoes) : base(opcoes)
        {
        }

        public DbSet<Noticia> Noticia { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)//se a string de conexao nao estiver configurada na startup
            {
                optionsBuilder.UseSqlServer(ObterStringConexao()); // pega minha conexao criada no metodo "ObterStringConexao()"
                base.OnConfiguring(optionsBuilder);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers").HasKey(t => t.Id); // meu application user esta sobrescrevendo na tabela AspNetusers com os campos que criei na classe
            base.OnModelCreating(builder);
        }

        public string ObterStringConexao()
        {
            string strCon = "Data Source=DESKTOP-NVU34N7\\SQLEXPRESS;Initial Catalog=AULA_API_DDD_2021; Integrated Security=False; User ID=sa;Password=123456;Connect Timeout=15; Encrypt=False;TrustServerCertificate=False";
            return strCon;
        }

    }
}
