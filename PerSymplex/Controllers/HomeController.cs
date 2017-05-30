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

        [HttpPost]
        public ActionResult Index([ModelBinder(typeof(TwoDimensionalArrayBinder<string>))] string[,] MatrixA, HeaderSimplex Model)
        {
            //var teste = Model.NVar;
            //var teste2 = Model.NRest;
            //string[] ArrayTeste = new string[3];
            //ArrayTeste[0] = "tt01";
            //ArrayTeste[1] = "tt02";
            //ArrayTeste[2] = "tt03";
            if (Model == null)
            {
                Model = new HeaderSimplex();
            }
            
            //int counter = 0;
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
                                    Model.Matriz[i, j] = MatrixA[(i - 1), (j - 1)];
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
                                    Model.Matriz[i, j] = MatrixA[(i-1),(MatrixA.GetLength(1)-1)];
                                }
                            }
                            else
                            {
                                if (j <= Model.NVar)
                                {
                                    Model.Matriz[i, j] = "-" + Model.FO[j-1];//ArrayTeste[(j-1)];
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
            //Model.Matriz[0, 0] = 1;
            //Model.Matriz[0, 1] = 2;
            //Model.Matriz[1, 0] = 3;
            //Model.Matriz[1, 1] = 4;
            //Model.Matriz[2, 0] = 5;
            //Model.Matriz[2, 1] = 6;

            if(MatrixA == null)
            {
                return View(Model);
            }
            else
            {
                return View("MatrizPreenchida",Model);
            }
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}