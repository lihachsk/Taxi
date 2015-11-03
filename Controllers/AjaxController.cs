using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Taxi.Controllers
{
    using Taxi.Models;
    public partial class AjaxController : Controller
    {
        //Ajax
        public JsonResult AjaxRequestCity(String cityName)
        {
            try
            {
                if (cityName != null && cityName != "")
                {
                    var result = getListAdress(cityName);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult RoutPath(string list)
        {
            try
            {
                if (list != null && list != "")
                {
                    var data = new JsonModels.data(list);
                    if (data != null)
                    {
                        var url = getUrl(data);
                        if (url != null)
                        {
                            var responseFromServer = getRoutParametrs(url);
                            if (responseFromServer.Contains("error"))
                            {
                                return Json(new { status = 0, type = -4, textError = responseFromServer }, JsonRequestBehavior.AllowGet);//Неведомая ошибка
                            }
                            var parther = new JsonModels.parserItem(responseFromServer);

                            if (parther != null && parther.status == "OK")
                            {
                                var points = getPoints(parther);
                                var distance = getDistance(parther);
                                var cost = getCost(distance);
                                var array = getErrors(points, data);
                                var te = totalError(array);
                                return Json(new { status = 1, data = array, cost = cost, te = te }, JsonRequestBehavior.AllowGet);//Ошибка расчета пути на стороннем сервере Google
                            }
                            return Json(new { status = 0, type = -4 }, JsonRequestBehavior.AllowGet);//Ошибка расчета пути на стороннем сервере Google
                        }
                        return Json(new { status = 0, type = -3 }, JsonRequestBehavior.AllowGet);//пунктов меньше двух
                    }
                    return Json(new { status = 0, type = -2 }, JsonRequestBehavior.AllowGet);//не удалось преобразовать параметры
                }
                return Json(new { status = 0, type = -1 }, JsonRequestBehavior.AllowGet);//переменная с параметрами пуста
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, type = 0 }, JsonRequestBehavior.AllowGet);//переменная с параметрами пуста
            }
        }
        public JsonResult OrderPath(string list)
        {
            try
            {
                if (list != null && list != "")
                {
                    var data = new JsonModels.data(list);
                    if (data != null)
                    {
                        if (data.city != null && data.city != "")
                        {
                            if (data.points.Count() > 1)
                            {
                                var url = getUrl(data);
                                if (url != null)
                                {
                                    var responseFromServer = getRoutParametrs(url);
                                    var parther = new JsonModels.parserItem(responseFromServer);

                                    if (parther != null && parther.status == "OK")
                                    {
                                        var points = getPoints(parther);
                                        var distance = getDistance(parther);
                                        var cost = getCost(distance);
                                        var array = getErrors(points, data);
                                        var te = totalError(array);
                                        if (te == 1)
                                        {
                                            return Json(new { status = 1, type = 0, data = array, cost = cost, error = "" }, JsonRequestBehavior.AllowGet);//
                                        }
                                        else
                                        {
                                            return Json(new { status = 0, type = -7, data = array, error = "Ошибка в адресе." }, JsonRequestBehavior.AllowGet);//Ошибка расчета пути на стороннем сервере Google
                                        }
                                    }
                                    return Json(new { status = 0, type = -6, error = "Ошибка построения пути." }, JsonRequestBehavior.AllowGet);//Ошибка расчета пути на стороннем сервере Google
                                }
                                else
                                {
                                    return Json(new { status = 0, type = -5, error = "Не удалось построить маршрут. Параметров меньше двух." }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json(new { status = 0, type = -4, error = "Количество пунктов в пути меньше двух" }, JsonRequestBehavior.AllowGet);//пунктов меньше двух
                            }
                        }
                        else
                        {
                            return Json(new { status = 0, type = -3, error = "Не задан основной город. Выберите город из списка." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { status = 0, type = -2, error = "Не удалось разобрать параметры пути" }, JsonRequestBehavior.AllowGet);//не удалось преобразовать параметры
                }
                return Json(new { status = 0, type = -1, error = "Не задан путь" }, JsonRequestBehavior.AllowGet);//переменная с параметрами пуста
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, type = 0, error = "Неизвестная ошибка" }, JsonRequestBehavior.AllowGet);//переменная с параметрами пуста
            }
        }
        public JsonResult cityServer(String city)
        {
            var status = "";
            try
            {
                city = getCity(city);
                status = getStatus(city);
                return Json(new { status = status, city = city }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = status, city = city }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult auth(String tel)
        {
            try
            {
                var status = checkUser(tel);
                return Json(new { status = "" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                writeLog(ex.Message);
                return Json(new { status = "" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}