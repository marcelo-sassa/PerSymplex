using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PerSymplex.Models
{
    public class HeaderSimplex
    {
        [DisplayName("Nº de Variáveis")]
        public int NVar { get; set; }
        [DisplayName("Nº de Restrições")]
        public int NRest { get; set; }

        [DisplayName("Matriz")]
        public string[,] Matriz { get; set; }

        [DisplayName("Função Objetivo")]
        public string[] FO { get; set; }

        [DisplayName("Titulo da Tabela")]
        public string Titulo { get; set; }
    }
}