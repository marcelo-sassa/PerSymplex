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
            ViewData["RenderPage"] = "Index";
            return View();
        }

        [HttpGet]
        public ActionResult TesteIndex()
        {
            ViewData["PosicionaPagina"] = false;
            ViewData["RenderPage"] = "Index";
            return View();
        }

        [HttpPost]
        public ActionResult Index([ModelBinder(typeof(TwoDimensionalArrayBinder<string>))] string[,] MatrixA, HeaderSimplex Model)
        {
            if (Model == null)
            {
                Model = new HeaderSimplex();
            }

            List<HeaderSimplex> ListaTabelas = new List<HeaderSimplex>();

            bool Minimizacao = false;

            try
            {
                if (MatrixA != null)
                {
                    if(Model.Operacao == Operacao.Minimizar)
                    {
                        Minimizacao = true;
                        for(int i = 0; i < Model.FO.GetLength(0); i++)
                        {
                            if (string.IsNullOrEmpty(Model.FO[i]))
                                Model.FO[i] = "0";
                            else
                                Model.FO[i] = (decimal.Parse(Model.FO[i]) * (-1)).ToString();
                        }
                    }

                    if (Model.Matriz == null)
                    {
                        Model.Matriz = new string[(Model.NRest + 2), (Model.NRest + Model.NVar + 2)];
                    }
                    for (int i = 0; i < Model.Matriz.GetLength(0); i++)
                    {
                        for (int j = 0; j < Model.Matriz.GetLength(1); j++)
                        {
                            if (i == 0)
                            {
                                if (j == 0)
                                {
                                    Model.Matriz[i, j] = "Base";
                                }
                                else if (j > 0 & j <= Model.NVar)
                                {
                                    Model.Matriz[i, j] = "X" + j;
                                }
                                else if (j > Model.NVar & j < (Model.Matriz.GetLength(1) - 1))
                                {
                                    Model.Matriz[i, j] = "F" + (j - Model.NVar);
                                }
                                else
                                {
                                    Model.Matriz[i, j] = "b";
                                }
                            }
                            else if (j == 0)
                            {
                                if (i <= Model.NRest)
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
                                if (i <= Model.NRest)
                                {
                                    if (j <= Model.NVar)
                                    {
                                        string valor = MatrixA[(i - 1), (j - 1)];
                                        Model.Matriz[i, j] = (!string.IsNullOrEmpty(valor) ? valor : "0");
                                    }
                                    else if (j > Model.NVar & j < (Model.Matriz.GetLength(1) - 1))
                                    {
                                        if (i == (j - Model.NVar))
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

                if (MatrixA == null)
                {
                    ViewData["PosicionaPagina"] = true;
                    ViewData["RenderPage"] = "Index";
                    //return View((Url.Content("TesteIndex") + "#simplex"),Model);
                    return View("TesteIndex",Model);
                }
                else
                {
                    Model.Titulo = "Tabela Inicial";
                    //int PCol = findPCol(Model.Matriz);
                    //bool SolLimitada = false;
                    //for (int i = 1; i < Model.Matriz.GetLength(0); i++)
                    //    if (decimal.Parse(Model.Matriz[i, PCol]) > 0)
                    //        SolLimitada = true;

                    //if (SolLimitada)
                    //{
                    //    int PLin = findPLin(Model.Matriz, PCol);
                    //}
                    //else
                    //{
                    //    throw new Exception("Não foi possível resolver pois a solução é ilimitada!");
                    //}

                    ListaTabelas.Add(Model);

                    //HeaderSimplex ModelTopZera = new HeaderSimplex();
                    //ModelTopZera = Model;
                    int contador = 1;
                    string[,] Tabela = Model.Matriz;

                    while (!CondicaoDeParada(Tabela) & contador <= 20)
                    {
                        //Tabela = SolveSimplex(Tabela);

                        HeaderSimplex NovoModelo = SolveSimplex(Tabela, Minimizacao);
                        Tabela = NovoModelo.Matriz;

                        //NovoModelo.Matriz = Tabela;
                        NovoModelo.Titulo = contador + "ª Iteração";

                        //DEFINIR LIMITE DE ITERAÇÕES
                        contador++;

                        ListaTabelas.Add(NovoModelo);
                    }

                    if(contador > 20)
                    {
                        throw new Exception("Limite de iterações excedido!");
                    }
                    ViewData["PosicionaPagina"] = true;
                    ViewData["RenderPage"] = "Resultado";
                    return View("TesteIndex", ListaTabelas);
                }
            }
            catch (Exception ex)
            {
                ViewData["PosicionaPagina"] = true;
                ViewData["RenderPage"] = "Erro";
                ViewData["Exception"] = ex.Message;
                return View("TesteIndex");
            }
        }

        public HeaderSimplex SolveSimplex(string[,] Tabela, bool Minimizacao)
        {
            HeaderSimplex ModeloTeporario = new HeaderSimplex();

            int PCol, PLin;
            string[,] NovaTabela = new string[Tabela.GetLength(0), Tabela.GetLength(1)];
            
            PCol = findPCol(Tabela);

            bool SolLimitada = false;
            for (int i = 1; i < Tabela.GetLength(0); i++)
                if (decimal.Parse(Tabela[i, PCol]) > 0)
                    SolLimitada = true;

            if (SolLimitada)
            {
                PLin = findPLin(Tabela, PCol);

                //Passa os valores da primeira linha da tabela, para a nova tabela
                for (int i = 0; i < Tabela.GetLength(0); i++)
                    NovaTabela[i, 0] = Tabela[i, 0];

                //Passa os valores da primeira coluna da tabela, para a nova tabela
                for (int j = 0; j < Tabela.GetLength(1); j++)
                    NovaTabela[0, j] = Tabela[0, j];

                //Coloca a váriavel que deve entrar na base no seu respectivo lugar
                NovaTabela[PLin, 0] = Tabela[0, PCol];

                for (int j = 1; j < Tabela.GetLength(1); j++)
                    NovaTabela[PLin, j] = (Math.Round((decimal.Parse(Tabela[PLin, j]) / decimal.Parse(Tabela[PLin, PCol])),2)).ToString();

                for (int i = 1; i < Tabela.GetLength(0); i++)
                {
                    if (i == PLin)
                        continue;

                    for (int j = 1; j < Tabela.GetLength(1); j++)
                        NovaTabela[i, j] = (Math.Round((decimal.Parse(NovaTabela[PLin, j]) * (decimal.Parse(Tabela[i, PCol]) * (-1)) + decimal.Parse(Tabela[i, j])),2)).ToString();
                    //NovaTabela[i, j] = (decimal.Parse(Tabela[i, j]) - (decimal.Parse(Tabela[i, PCol]) * decimal.Parse(NovaTabela[PLin, j]))).ToString();
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
                ModeloTeporario.Matriz = NovaTabela;

                string[] Solucao = new string[(NovaTabela.GetLength(0)-1)];
                if(Minimizacao)
                    Solucao[0] = NovaTabela[(NovaTabela.GetLength(0) - 1), 0] + " = " + (decimal.Parse(NovaTabela[(NovaTabela.GetLength(0) - 1), (NovaTabela.GetLength(1) - 1)]) * (-1));
                else
                    Solucao[0] = NovaTabela[(NovaTabela.GetLength(0) - 1), 0] + " = " + NovaTabela[(NovaTabela.GetLength(0) - 1), (NovaTabela.GetLength(1) - 1)];

                for (int i = 1; i < (NovaTabela.GetLength(0) - 1); i++)
                    Solucao[i] = NovaTabela[i,0] + " = " + NovaTabela[i, (NovaTabela.GetLength(1) - 1)];

                ModeloTeporario.Solução = Solucao;

                return ModeloTeporario;
            }
            else
            {
                throw new Exception("Não foi possível resolver pois a solução é ilimitada!");
            }
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
    }
}