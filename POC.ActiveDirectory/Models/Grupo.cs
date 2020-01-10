using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.ActiveDirectory.Models
{
    public class Grupo
    {
        public Grupo(string nome)
        {
            this.Nome = nome;
        }

        public string Nome { get; set; }

    }
}
