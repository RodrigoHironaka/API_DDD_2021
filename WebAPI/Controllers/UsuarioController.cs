using Aplicacao.Interfaces;
using Entidades.Entidades;
using Entidades.Entidades.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Token;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IAplicacaoUsuario _IAplicacaoUsuario;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsuarioController(IAplicacaoUsuario IAplicacaoUsuario, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _IAplicacaoUsuario = IAplicacaoUsuario;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/CriarToken")]
        public async Task<IActionResult> CriarToken([FromBody] Login login)
        {
            if (string.IsNullOrWhiteSpace(login.email) || string.IsNullOrWhiteSpace(login.senha))
                return Unauthorized();

            var resultado = await _IAplicacaoUsuario.ExisteUsuario(login.email, login.senha);
            if (resultado)
            {
                var idUsuario = await _IAplicacaoUsuario.RetornaIdUsuario(login.email);

                //dados da empresa
                var token = new TokenJWTBuilder()
                    .AddSecurityKey(JwtSecurityKey.Create("Secret_Key-12345678"))
                    .AddSubject("Empresa Teste")
                    .AddIssuer("Teste.Security.Bearer")
                    .AddAudience("Teste.Security.Bearer")
                    .AddClaim("idUsuario", idUsuario)
                    .AddExpiry(5) //minutos
                    .Builder();

                return Ok(token.value);
            }
            else
                return Unauthorized();
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/AdicionarUsuario")]
        public async Task<IActionResult> AdicionarUsuario([FromBody] Login login)
        {
            if (string.IsNullOrWhiteSpace(login.email) || string.IsNullOrWhiteSpace(login.senha))
                return Ok("Falta alguns dados");

            var resultado = await _IAplicacaoUsuario.AdicionarUsuario(login.email, login.senha, login.idade, login.celular);

            if(resultado)
                return Ok("Adicionado com sucesso");
            else
                return Ok("Erro ao adiconar usuario");
        }


        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/CriarTokenIdentity")]
        public async Task<IActionResult> CriarTokenIdentity([FromBody] Login login)
        {
            if (string.IsNullOrWhiteSpace(login.email) || string.IsNullOrWhiteSpace(login.senha))
                return Unauthorized();

            var resultado = await _signInManager.PasswordSignInAsync(login.email, login.senha, false, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                var idUsuario = await _IAplicacaoUsuario.RetornaIdUsuario(login.email);

                //dados da empresa
                var token = new TokenJWTBuilder()
                    .AddSecurityKey(JwtSecurityKey.Create("Secret_Key-12345678"))
                    .AddSubject("Empresa Teste")
                    .AddIssuer("Teste.Securiry.Bearer")
                    .AddAudience("Teste.Securiry.Bearer")
                    .AddClaim("idUsuario", idUsuario)
                    .AddExpiry(5) //minutos
                    .Builder();

                return Ok(token.value);
            }
            else
                return Unauthorized();
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/AdicionarUsuarioIdentity")]
        public async Task<IActionResult> AdicionarUsuarioIdentity([FromBody] Login login)
        {
            if (string.IsNullOrWhiteSpace(login.email) || string.IsNullOrWhiteSpace(login.senha))
                return Ok("Falta alguns dados");

            var user = new ApplicationUser
            {
                UserName = login.email,
                Email = login.email,
                Celular = login.celular,
                Tipo = TipoUsuario.Comum,
            };

            var resultado = await _userManager.CreateAsync(user, login.senha);

            if (resultado.Errors.Any())
                return Ok(resultado.Errors);

            //Geração de confirmação case precise
            var usurarioId = await _userManager.GetUserIdAsync(user);
            var codigo = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            codigo = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codigo));

            //Retorno email
            codigo = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(codigo));
            var resultado2 = await _userManager.ConfirmEmailAsync(user, codigo);
            //var StatusMessage = resultado2.Succeeded;

            if(resultado2.Succeeded)
                return Ok("Usuário adicionado com sucesso");
            else
                return Ok("Erro ao confirmar usuário");
        }
    }
}
