using Aplicacao.Interfaces;
using Dominio.Interfaces;
using System.Threading.Tasks;

namespace Aplicacao.Aplicacoes
{
    public class AplicacaoUsuario : IAplicacaoUsuario
    {
        IUsuario _IUsuario;

        public AplicacaoUsuario(IUsuario IUsuario)
        {
            _IUsuario = IUsuario;
        }

        public async Task<bool> AdicionarUsuario(string email, string senha, int idade, string celular)
        {
            return await _IUsuario.AdicionarUsuario(email, senha, idade, celular);
        }

        public async Task<bool> ExisteUsuario(string email, string senha)
        {
            return await _IUsuario.ExisteUsuario(email, senha);
        }
    }
}
