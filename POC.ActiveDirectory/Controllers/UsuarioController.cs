using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using POC.ActiveDirectory.Interfaces;
using POC.ActiveDirectory.ViewModels;

namespace POC.ActiveDirectory.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IAuthenticationService authService;
        private readonly ILogger<UsuarioController> _logger;
        private string ldapHost;

        public UsuarioController(ILogger<UsuarioController> logger, IAuthenticationService authService)
        {
            _logger = logger;
            this.ldapHost = "conexao LDAP aqui";
            this.authService = authService;
        }

        [HttpPost]
        [Route("autenticar")]
        public ActionResult AutenticarNovell([FromBody]LoginViewModel viewModel)
        {
            try
            {
                using (var adConnection = new LdapConnection())
                {
                    adConnection.Connect(this.ldapHost, LdapConnection.DEFAULT_PORT);

                    adConnection.Bind(LdapConnection.Ldap_V3, viewModel.login, viewModel.senha);

                    return Ok("Usuário autenticado com sucesso");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "error");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel viewModel)
        {
            if (String.IsNullOrEmpty(viewModel.login) || String.IsNullOrEmpty(viewModel.senha))
                return BadRequest();

            try
            {
                var user = authService.Login(viewModel.login, viewModel.senha);
                if (null != user)
                {
                    return Ok(user);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("grupo")]
        public async Task<IActionResult> ListarGruposPorUsuario(string usuario)
        {
            if (String.IsNullOrEmpty(usuario))
                return BadRequest();

            try
            {
                var grupos = authService.ListarGruposPorUsuario(usuario);
                if(grupos != null && grupos.Count > 0)
                {
                    return Ok(grupos);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
