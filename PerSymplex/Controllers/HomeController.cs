using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PerSymplex.Models;

namespace PerSymplex.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult TesteIndex()
        {
            return View();
        }

        #region Testes TabelaInicial
        //[HttpPost]
        //public ActionResult Index([ModelBinder(typeof(TwoDimensionalArrayBinder<string>))] string[,] MatrixA, HeaderSimplex Model)
        //{
        //    if (Model == null)
        //    {
        //        Model = new HeaderSimplex();
        //    }

        //    if (MatrixA != null)
        //    {
        //        if (Model.Matriz == null)
        //        {
        //            Model.Matriz = new string[(Model.NRest + 2), (Model.NRest + Model.NVar + 2)];
        //        }
        //        for (int i = 0; i < Model.Matriz.GetLength(0); i++)
        //        {
        //            for (int j = 0; j < Model.Matriz.GetLength(1); j++)
        //            {
        //                if(i == 0)
        //                {
        //                    if(j == 0)
        //                    {
        //                        Model.Matriz[i, j] = "Base";
        //                    }
        //                    else if(j > 0 & j <= Model.NVar)
        //                    {
        //                        Model.Matriz[i, j] = "X" + j;
        //                    }
        //                    else if(j > Model.NVar & j < (Model.Matriz.GetLength(1) - 1))
        //                    {
        //                        Model.Matriz[i, j] = "F" + (j-Model.NVar);
        //                    }
        //                    else
        //                    {
        //                        Model.Matriz[i, j] = "b";
        //                    }
        //                }
        //                else if(j == 0)
        //                {
        //                    if(i <= Model.NRest)
        //                    {
        //                        Model.Matriz[i, j] = "F" + i;
        //                    }
        //                    else
        //                    {
        //                        Model.Matriz[i, j] = "Z";
        //                    }
        //                }
        //                else
        //                {
        //                    if(i <= Model.NRest)
        //                    {
        //                        if(j <= Model.NVar)
        //                        {
        //                            string valor = MatrixA[(i - 1), (j - 1)];
        //                            Model.Matriz[i, j] = (!string.IsNullOrEmpty(valor) ? valor : "0");
        //                        }
        //                        else if(j > Model.NVar & j < (Model.Matriz.GetLength(1) - 1))
        //                        {
        //                            if(i == (j-Model.NVar))
        //                            {
        //                                Model.Matriz[i, j] = "1";
        //                            }
        //                            else
        //                            {
        //                                Model.Matriz[i, j] = "0";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            string valor = MatrixA[(i - 1), (MatrixA.GetLength(1) - 1)];
        //                            Model.Matriz[i, j] = (!string.IsNullOrEmpty(valor) ? valor : "0");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (j <= Model.NVar)
        //                        {
        //                            string valor = (!string.IsNullOrEmpty(Model.FO[j - 1]) ? Model.FO[j - 1] : "0");

        //                            decimal valorConv = decimal.Parse(valor);
        //                            valorConv = valorConv * (-1);
        //                            Model.Matriz[i, j] = valorConv.ToString();
        //                        }
        //                        else 
        //                        {
        //                            Model.Matriz[i, j] = "0";
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if(MatrixA == null)
        //    {
        //        return View(Model);
        //    }
        //    else
        //    {
        //        return View("MatrizPreenchida",Model);
        //    }
        //}

        #endregion

    }
}