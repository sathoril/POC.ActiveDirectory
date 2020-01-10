using POC.ActiveDirectory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.ActiveDirectory.Interfaces
{
    public interface IAuthenticationService
    {
        Usuario Login(string login, string senha);
        List<Grupo> ListarGruposPorUsuario(string usuario);
    }
}
