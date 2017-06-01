using PerSymplex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PerSymplex.Controllers
{
    public class SimplexController : Controller
    {
        // GET: Simplex
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index([ModelBinder(typeof(TwoDimensionalArrayBinder<string>))] string[,] MatrixA, HeaderSimplex Model)
        {
            if (Model == null)
            {
                Model = new HeaderSimplex();
            }
            
            if (MatrixA != null)
            {
                if (Model.Matriz == null)
                {
                    Model.Matriz = new string[(Model.NRest + 2), (Model.NRest + Model.NVar + 2)];
                }
                for (int i = 0; i < Model.Matriz.GetLength(0); i++)
                {
                    for (int j = 0; j < Model.Matriz.GetLength(1); j++)
                    {
                        if(i == 0)
                        {
                            if(j == 0)
                            {
                                Model.Matriz[i, j] = "Base";
                            }
                            else if(j > 0 & j <= Model.NVar)
                            {
                                Model.Matriz[i, j] = "X" + j;
                            }
                            else if(j > Model.NVar & j < (Model.Matriz.GetLength(1) - 1))
                            {
                                Model.Matriz[i, j] = "F" + (j-Model.NVar);
                            }
                            else
                            {
                                Model.Matriz[i, j] = "b";
                            }
                        }
                        else if(j == 0)
                        {
                            if(i <= Model.NRest)
                            {
                                Model.Matriz[i, j] = "F" + i;
                            }
                            else
                            {
                                Model.Matriz[i, j] = "Z";
                            }
                        }
                        else
                        {
                            if(i <= Model.NRest)
                            {
                                if(j <= Model.NVar)
                                {
                                    string valor = MatrixA[(i - 1), (j - 1)];
                                    Model.Matriz[i, j] = (!string.IsNullOrEmpty(valor) ? valor : "0");
                                }
                                else if(j > Model.NVar & j < (Model.Matriz.GetLength(1) - 1))
                                {
                                    if(i == (j-Model.NVar))
                                    {
                                        Model.Matriz[i, j] = "1";
                                    }
                                    else
                                    {
                                        Model.Matriz[i, j] = "0";
                                    }
                                }
                                else
                                {
                                    string valor = MatrixA[(i - 1), (MatrixA.GetLength(1) - 1)];
                                    Model.Matriz[i, j] = (!string.IsNullOrEmpty(valor) ? valor : "0");
                                }
                            }
                            else
                            {
                                if (j <= Model.NVar)
                                {
                                    string valor = (!string.IsNullOrEmpty(Model.FO[j - 1]) ? Model.FO[j - 1] : "0");
                                    
                                    decimal valorConv = decimal.Parse(valor);
                                    valorConv = valorConv * (-1);
                                    Model.Matriz[i, j] = valorConv.ToString();
                                }
                                else 
                                {
                                    Model.Matriz[i, j] = "0";
                                }
                            }
                        }
                    }
                }
            }

            if(MatrixA == null)
            {
                return View(Model);
            }
            else
            {
                HeaderSimplex ModelTopZera = new HeaderSimplex();
                ModelTopZera = Model;
                while (!CondicaoDeParada(ModelTopZera.Matriz))
                {
                    ModelTopZera.Matriz = SolveSimplex(ModelTopZera);
                }

                return View("TabelaInicial", ModelTopZera);
            }
        }

        public string[,] SolveSimplex(HeaderSimplex Model)
        {
            int PCol, PLin;
            string[,] NovaTabela = new string[Model.Matriz.GetLength(0), Model.Matriz.GetLength(1)];
            
            PCol = findPCol(Model.Matriz);
            PLin = findPLin(Model.Matriz, PCol);
            //basis[PLin] = PCol;

            for (int i = 0; i < Model.Matriz.GetLength(0); i++)
                NovaTabela[i, 0] = Model.Matriz[i, 0];

            for (int j = 0; j < Model.Matriz.GetLength(1); j++)
                NovaTabela[0, j] = Model.Matriz[0,j];

            NovaTabela[PLin, 0] = Model.Matriz[0, PCol];

            for (int j = 1; j < Model.Matriz.GetLength(1); j++)
                NovaTabela[PLin, j] = (decimal.Parse(Model.Matriz[PLin, j]) / decimal.Parse(Model.Matriz[PLin, PCol])).ToString();

            for (int i = 1; i < Model.Matriz.GetLength(0); i++)
            {
                if (i == PLin)
                    continue;

                for (int j = 1; j < Model.Matriz.GetLength(1); j++)
                    //NovaTabela[i, j] = (decimal.Parse(NovaTabela[PLin, j]) * (decimal.Parse(Model.Matriz[i, PCol]) * (-1)) + decimal.Parse(Model.Matriz[i,j])).ToString();
                NovaTabela[i, j] = (decimal.Parse(Model.Matriz[i, j]) - (decimal.Parse(Model.Matriz[i, PCol]) * decimal.Parse(NovaTabela[PLin, j]))).ToString();
            }
            //table = NovaTabela;

            // Count a result found by the X
            //for (int i = 0; i < result.Length; i++)
            //{
            //    int k = basis.IndexOf(i + 1);
            //    if (k != -1)
            //        result[i] = table[k, 0];
            //    else
            //        result[i] = 0;
            //}
            return NovaTabela;
        }

        public bool CondicaoDeParada(string[,] Tabela)
        {
            for (int i = (Tabela.GetLength(0) - 1); i < Tabela.GetLength(0); i++)
            {
                for (int j = 1; j < Tabela.GetLength(1); j++)
                {
                    if(decimal.Parse(Tabela[i,j]) < 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private int findPCol(string[,] Tabela)
        {
            int PCol = 1;

            for (int j = 2; j < (Tabela.GetLength(1) - 1); j++)
                if (decimal.Parse(Tabela[(Tabela.GetLength(0) - 1), j]) < decimal.Parse(Tabela[(Tabela.GetLength(0) - 1), PCol]))
                    PCol = j;

            return PCol;
        }

        private int findPLin(string[,] Tabela,int PCol)
        {
            int PLin = 1;

            for (int i = 1; i < (Tabela.GetLength(0) - 1); i++)
                if (decimal.Parse(Tabela[i, PCol]) > 0)
                {
                    PLin = i;
                    break;
                }

            for (int i = PLin + 1; i < (Tabela.GetLength(0) - 1); i++)
            {
                if ((decimal.Parse(Tabela[i, PCol]) > 0))
                    if (((decimal.Parse(Tabela[i, (Tabela.GetLength(1) - 1)]) / decimal.Parse(Tabela[i, PCol])) < (decimal.Parse(Tabela[PLin, (Tabela.GetLength(1) - 1)]) / decimal.Parse(Tabela[PLin, PCol]))))
                        PLin = i;
            }
                

            return PLin;
        }

        [HttpGet]
        public ActionResult TesteIndex()
        {
            return View();
        }
    }
}