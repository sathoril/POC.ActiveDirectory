using Microsoft.Extensions.Options;
using POC.ActiveDirectory.Configuration;
using POC.ActiveDirectory.Interfaces;
using POC.ActiveDirectory.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;

namespace POC.ActiveDirectory.Services
{
    public class LdapAuthenticationService : IAuthenticationService
    {
        private const string DisplayNameAttribute = "DisplayName";
        private const string SAMAccountNameAttribute = "SAMAccountName";

        private readonly LdapConfig config;

        public LdapAuthenticationService(IOptions<LdapConfig> config)
        {
            this.config = config.Value;
        }

        public Usuario Login(string login, string senha)
        {
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry(config.Path, config.UserDomainName + "\\" + login, senha))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        searcher.Filter = String.Format("({0}={1})", SAMAccountNameAttribute, login);
                        searcher.PropertiesToLoad.Add(DisplayNameAttribute);
                        searcher.PropertiesToLoad.Add(SAMAccountNameAttribute);
                        var result = searcher.FindOne();
                        if (result != null)
                        {
                            var displayName = result.Properties[DisplayNameAttribute];
                            var samAccountName = result.Properties[SAMAccountNameAttribute];

                            return new Usuario
                            {
                                Nome = displayName == null || displayName.Count <= 0 ? null : displayName[0].ToString(),
                                Login = samAccountName == null || samAccountName.Count <= 0 ? null : samAccountName[0].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // if we get an error, it means we have a login failure.
                // Log specific exception 
                throw ex;
            }
            return null;

        }

        public List<Grupo> ListarGruposPorUsuario(string usuario)
        {
            List<Grupo> list = new List<Grupo>();

            try
            {
                using (DirectoryEntry entry = new DirectoryEntry(config.Path, config.UserDomainName + "\\" + "bfcorrea", "Cavalos123##"))
                {
                    entry.AuthenticationType = AuthenticationTypes.Secure;

                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        searcher.Filter = String.Format("({0}={1})", SAMAccountNameAttribute, "bfcorrea");
                        searcher.PropertiesToLoad.Add(DisplayNameAttribute);
                        searcher.PropertiesToLoad.Add(SAMAccountNameAttribute);
                        searcher.PropertiesToLoad.Add("memberOf");
                        searcher.SearchScope = SearchScope.Subtree;

                        var result = searcher.FindOne();

                        ResultPropertyValueCollection resultPropertyValueCollection = result.Properties["memberOf"];
                        if (resultPropertyValueCollection.Count > 0)
                        {
                            foreach (object current in resultPropertyValueCollection)
                            {
                                list.Add(new Grupo(current.ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return list;
        }
    }
}
