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

        [DisplayName("Objetivo da função")]
        public Operacao Operacao { get; set; }

        [DisplayName("Váriável que entra na base")]
        public string VarEntra { get; set; }

        [DisplayName("Váriável que sai da base")]
        public string VarSai { get; set; }

        [DisplayName("Pivô")]
        public string[] CalcLinhaPivo { get; set; }

        [DisplayName("Cálculos")]
        public string[] Calculos { get; set; }

        [DisplayName("Solução")]
        public string[] Solução { get; set; }
    }

    public enum Operacao
    {
        Maximizar,
        Minimizar
    }
}