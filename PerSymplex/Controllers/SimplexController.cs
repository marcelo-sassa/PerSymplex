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
            HeaderSimplex Model = new HeaderSimplex();
            //ViewData["RenderPage"] = "Index";
            return View(Model);
        }

        [HttpGet]
        public ActionResult TesteIndex()
        {
            //ViewData["PosicionaPagina"] = false;
            //ViewData["RenderPage"] = "Index";
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

                    //Insere no array de restricões do modelo o valor limitante de cada restrição
                    string[] Restricoes = new string[MatrixA.GetLength(0)];
                    for (int i = 0; i < MatrixA.GetLength(0); i++)
                        Restricoes[i] = MatrixA[i, (MatrixA.GetLength(1) - 1)];
                    //Coloca as restricoes no modelo
                    Model.Restricoes = Restricoes;

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
                    //ViewData["PosicionaPagina"] = true;
                    //ViewData["RenderPage"] = "Index";
                    //return View((Url.Content("TesteIndex") + "#simplex"),Model);
                    return View("Index",Model);
                }
                else
                {
                    Model.Titulo = "Tabela Inicial";
                    Model.Solucao = MontaSolucao(Model.Matriz, Minimizacao);
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
                
                    int contador = 1;
                    string[,] Tabela = Model.Matriz;

                    while (!CondicaoDeParada(Tabela) & contador <= 20)
                    {
                        HeaderSimplex NovoModelo = SolveSimplex(Tabela, Minimizacao);
                        Tabela = NovoModelo.Matriz;

                        var TabelaAnterior = ListaTabelas.Last();
                        TabelaAnterior.VarEntra = NovoModelo.VarEntra;
                        TabelaAnterior.VarSai = NovoModelo.VarSai;

                        NovoModelo.Titulo = contador + "ª Iteração";

                        //DEFINIR LIMITE DE ITERAÇÕES
                        contador++;

                        ListaTabelas.Add(NovoModelo);
                    }

                    if(contador > 20)
                    {
                        throw new Exception("Limite de iterações excedido!");
                    }

                    if(ListaTabelas.Count > 1)
                    {
                        var TabelaFinal = ListaTabelas.Last();
                        
                        TabelaFinal.CustoReduzido = MontaTabelaCustoReduzido(TabelaFinal.Matriz, Model.FO, TabelaFinal.Solucao);
                        TabelaFinal.PrecoSombra = MontaTabelaPrecoSombra(TabelaFinal.Matriz, Model.Restricoes, TabelaFinal.Solucao);
                        TabelaFinal.VarSai = "";
                        TabelaFinal.VarEntra = "";

                        bool SolucaoMultipla = ValidaSolucaoMultipla(TabelaFinal.CustoReduzido);
                        if(SolucaoMultipla)
                        {
                            TabelaFinal.MsgSolucaoMultipla = "Existem múltiplas soluções que satisfazem as restrições do problema. \nAbaixo é apresentada uma delas.";
                        }
                    }
                    //ViewData["PosicionaPagina"] = true;
                    //ViewData["RenderPage"] = "Resultado";

                    List<HeaderSimplex> ListaFinal = new List<HeaderSimplex>();
                    if (Model.TipoSolucao == TipoSolucao.Direta)
                    {
                        //ListaFinal.Add(ListaTabelas.First());
                        //if(ListaTabelas.Count > 1)
                        ListaFinal.Add(ListaTabelas.Last());
                        ViewData["TipoSolucao"] = "Direta";
                    }
                    else
                    {
                        ListaFinal = ListaTabelas;
                        ViewData["TipoSolucao"] = "Detalhada";
                    }
                    return View("Resultado", ListaFinal);
                }
            }
            catch (Exception ex)
            {
                //ViewData["PosicionaPagina"] = true;
                //ViewData["RenderPage"] = "Erro";
                ViewData["Exception"] = ex.Message;
                return View("Erro");
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

                ModeloTeporario.VarEntra = Tabela[0,PCol];
                ModeloTeporario.VarSai = Tabela[PLin, 0];

                //Passa os valores da primeira linha da tabela, para a nova tabela
                for (int i = 0; i < Tabela.GetLength(0); i++)
                    NovaTabela[i, 0] = Tabela[i, 0];

                //Passa os valores da primeira coluna da tabela, para a nova tabela
                for (int j = 0; j < Tabela.GetLength(1); j++)
                    NovaTabela[0, j] = Tabela[0, j];

                //Coloca a váriavel que deve entrar na base no seu respectivo lugar
                NovaTabela[PLin, 0] = Tabela[0, PCol];

                //Cria um array de strings para armezenar os cálculos da linha do pivô
                string[] linhaPivo = new string[Tabela.GetLength(1)];
                //Insere na primeira linha o "título" daquele bloco de cálculos
                linhaPivo[0] = "Linha do Pivô (Linha " + PLin + "):";
                //Loop usado para efetuar os cálculos da linha do pivô
                for (int j = 1; j < Tabela.GetLength(1); j++)
                {
                    //Divide as células da linha do pivô pelo próprio pivô
                    NovaTabela[PLin, j] = (Math.Round((decimal.Parse(Tabela[PLin, j]) / decimal.Parse(Tabela[PLin, PCol])), 2)).ToString();
                    //Insere no array os cálculos feitos com cada célula
                    linhaPivo[j] = Tabela[PLin, j] + " / " + Tabela[PLin, PCol] + " = " + NovaTabela[PLin, j];
                }

                //Adciona os cálculos ao modelo que vai reunir todos os cálculos
                //executados para a tabela em questão
                ModeloTeporario.Calculos.Add(linhaPivo);
                //Loop usado para efetuar os cálculos das demais linhas da tabela
                for (int i = 1; i < Tabela.GetLength(0); i++)
                {
                    //Não efetua os cálculos se for a linha do pivô
                    if (i == PLin)
                        continue;
                    //Cria um array de strings para armezenar os cálculos das linhas
                    string[] calculos = new string[Tabela.GetLength(1)];
                    //Insere na primeira linha o "título" daquele bloco de cálculos
                    calculos[0] = "Linha " + i + ":";
                    //Loop iterando coluna a coluna das linhas exceto a do pivô para que
                    //sejam feitos os devidos cálculos para zerar a coluna do pivô
                    for (int j = 1; j < Tabela.GetLength(1); j++)
                    {
                        //Efetua os cálculos propriamente ditos
                        NovaTabela[i, j] = (Math.Round((decimal.Parse(NovaTabela[PLin, j]) * (decimal.Parse(Tabela[i, PCol]) * (-1)) + decimal.Parse(Tabela[i, j])), 2)).ToString();
                        //Insere no array os cálculos feitos com cada célula
                        calculos[j] = NovaTabela[PLin, j] + " * ("+ (decimal.Parse(Tabela[i, PCol]) * (-1)) + ") + " + Tabela[i, j] + " = " + NovaTabela[i, j];
                    }
                    //Insere o array de cálculos no modelo que armazena todos os cálculos
                    ModeloTeporario.Calculos.Add(calculos);
                    //Padrão de cálculo diferenciado
                    //NovaTabela[i, j] = (decimal.Parse(Tabela[i, j]) - (decimal.Parse(Tabela[i, PCol]) * decimal.Parse(NovaTabela[PLin, j]))).ToString();
                }

                //Insere a nova tabela no modelo
                ModeloTeporario.Matriz = NovaTabela;

                //Finalmente coloca no modelo o array com todas as linhas que compões a solução
                ModeloTeporario.Solucao = MontaSolucao(NovaTabela, Minimizacao);

                //Retorna o modelo que foi criado
                return ModeloTeporario;
            }
            else
            {
                throw new Exception("Não foi possível resolver pois a solução é ilimitada!");
            }
        }

        public string[] MontaSolucao(string[,] Tabela, bool Minimizacao)
        {
            //SOLUÇÃO
            //Cria um array de strings para armazenar cada linha da solução
            string[] Solucao = new string[(Tabela.GetLength(0) - 1)];
            if (Minimizacao)
                //No caso de minização multiplica o Z por (-1) antes de armazenar na soluçao
                Solucao[0] = Tabela[(Tabela.GetLength(0) - 1), 0] + " = " + (decimal.Parse(Tabela[(Tabela.GetLength(0) - 1), (Tabela.GetLength(1) - 1)]) * (-1));
            else
                //NO caso de maximização armazena o Z da forma que ele é retirado da tabela
                Solucao[0] = Tabela[(Tabela.GetLength(0) - 1), 0] + " = " + Tabela[(Tabela.GetLength(0) - 1), (Tabela.GetLength(1) - 1)];
            //Loop usado para obter os resultados das outras váriaveis que estão base exceto Z
            for (int i = 1; i < (Tabela.GetLength(0) - 1); i++)
                Solucao[i] = Tabela[i, 0] + " = " + Tabela[i, (Tabela.GetLength(1) - 1)];

            return Solucao;
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

        //ANÁLISE DE SENSIBILIDADE
        public string[,] MontaTabelaCustoReduzido(string[,] Tabela, string[] FO, string[] Solucao)
        {
            string[,] TabelaCustoReduzido = new string[(FO.GetLength(0) + 1), 6];
            TabelaCustoReduzido[0, 0] = "Variável de decisão";
            TabelaCustoReduzido[0, 1] = "Valor na F.O.";
            TabelaCustoReduzido[0, 2] = "Valor Final";
            TabelaCustoReduzido[0, 3] = "Custo Reduzido";
            TabelaCustoReduzido[0, 4] = "Limite Superior";
            TabelaCustoReduzido[0, 5] = "Limite Inferior";

            //Insere as Váriáveis e seus respectivos valores da FO na tabela de análise
            for (int i = 1; i <= FO.GetLength(0); i++)
            {
                TabelaCustoReduzido[i, 0] = "X" + (i);
                TabelaCustoReduzido[i, 1] = FO[i - 1];
            }

            //Insere o valor final das váriáveis de decisão retirado do arry da solucao
            for(int i = 1; i < TabelaCustoReduzido.GetLength(0); i++)
            {
                for(int j = 1; j < Solucao.GetLength(0); j++)
                {
                    if (TabelaCustoReduzido[i,0] == (Solucao[j].Split(' ')[0]))
                    {
                        TabelaCustoReduzido[i, 2] = (Solucao[j].Split(' ')[2]);
                    }
                }
                if(string.IsNullOrEmpty(TabelaCustoReduzido[i,2]))
                {
                    TabelaCustoReduzido[i, 2] = "0";
                }
            }

            //Insere o valor do custo reduzido retirado da tabela principal
            for (int i = 1; i <= FO.GetLength(0); i++)
            {
                TabelaCustoReduzido[i, 3] = Tabela[Tabela.GetLength(0)-1,i];
            }

            return TabelaCustoReduzido;
        }//<--End of MontaTabelaCustoReduzido-->

        public string[,] MontaTabelaPrecoSombra(string[,] Tabela, string[] Restricoes, string[] Solucao)
        {
            string[,] TabelaPrecoSombra = new string[(Restricoes.GetLength(0) + 1), 6];
            TabelaPrecoSombra[0, 0] = "Var. representa a restição";
            TabelaPrecoSombra[0, 1] = "Valor original da restrição";
            TabelaPrecoSombra[0, 2] = "Valor Final";
            TabelaPrecoSombra[0, 3] = "Preço Sombra";
            TabelaPrecoSombra[0, 4] = "Limite Superior";
            TabelaPrecoSombra[0, 5] = "Limite Inferior";

            //Insere as Váriáveis e seus respectivos valores de restrição na tabela de análise
            for (int i = 1; i <= Restricoes.GetLength(0); i++)
            {
                TabelaPrecoSombra[i, 0] = "F" + (i);
                TabelaPrecoSombra[i, 1] = Restricoes[i - 1];
            }

            //Insere o valor final das váriáveis de folga retirado do arry da solucao
            for (int i = 1; i < TabelaPrecoSombra.GetLength(0); i++)
            {
                for (int j = 1; j < Solucao.GetLength(0); j++)
                {
                    if (TabelaPrecoSombra[i, 0] == (Solucao[j].Split(' ')[0]))
                    {
                        TabelaPrecoSombra[i, 2] = (Solucao[j].Split(' ')[2]);
                    }
                }
                if (string.IsNullOrEmpty(TabelaPrecoSombra[i, 2]))
                {
                    TabelaPrecoSombra[i, 2] = "0";
                }
            }

            int QtdVar = Tabela.GetLength(1) - (Restricoes.GetLength(0) + 2); 
            //Insere o valor do preço sombra retirado da tabela principal
            for (int i = 1; i <= Restricoes.GetLength(0); i++)
            {
                TabelaPrecoSombra[i, 3] = Tabela[Tabela.GetLength(0) - 1, (i + QtdVar)];
            }

            return TabelaPrecoSombra;
        }//<--End of MontaTabelaPrecoSombra-->

        public bool ValidaSolucaoMultipla(string[,] TabelaCustoReduzido)
        {
            for(int i = 1; i < TabelaCustoReduzido.GetLength(0); i++)
            {
                if((decimal.Parse(TabelaCustoReduzido[i,2]) == 0) & (decimal.Parse(TabelaCustoReduzido[i, 3]) == 0))
                {
                    return true;
                }
            }
            return false;
        }

    }//<--End of Class-->
}//<--End of Namespace-->