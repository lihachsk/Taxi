﻿using System;
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
            if (cityName != null && cityName != "")
            {
                var result = getListAdress(cityName);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult RoutPath(string list)
        {
            if (list != null && list != "")
            {
                var data = TaxiModels.Serializer.DeseriaizeData(list);
                if (data != null)
                {
                    var url = getUrl(data);
                    if (url != null)
                    {
                        var responseFromServer = getRoutParametrs(url);
                        var parther = TaxiModels.Serializer.Deseriaize(responseFromServer);

                        if (parther != null && parther.status == "OK")
                        {
                            var points = getPoints(parther);
                            var distance = getDistance(parther);
                            var cost = getCost(distance);
                            var array = getErrors(points, data);
                            var te = totalError(array);
                            return Json(new { status = 1, data = array, cost = cost, te = te });//Ошибка расчета пути на стороннем сервере Google
                        }
                        return Json(new { status = 0, type = -4 });//Ошибка расчета пути на стороннем сервере Google
                    }
                    return Json(new { status = 0, type = -3 });//пунктов меньше двух
                }
                return Json(new { status = 0, type = -2 });//не удалось преобразовать параметры
            }
            return Json(new { status = 0, type = -1 });//переменная с параметрами пуста
        }
        public JsonResult OrderPath(string list)
        {
            if (list != null && list != "")
            {
                var data = TaxiModels.Serializer.DeseriaizeData(list);
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
                                var parther = TaxiModels.Serializer.Deseriaize(responseFromServer);

                                if (parther != null && parther.status == "OK")
                                {
                                    var points = getPoints(parther);
                                    var distance = getDistance(parther);
                                    var cost = getCost(distance);
                                    var array = getErrors(points, data);
                                    var te = totalError(array);
                                    return Json(new { status = 1, data = array, cost = cost, te = te });//Ошибка расчета пути на стороннем сервере Google
                                }
                                return Json(new { status = 0, type = -4 });//Ошибка расчета пути на стороннем сервере Google
                            }
                        }
                        else
                        {
                            return Json(new { status = 0, type = -6 });
                        }
                    }
                    else
                    {
                        return Json(new { status = 0, type = -5 });
                    }
                    return Json(new { status = 0, type = -3 });//пунктов меньше двух
                }
                return Json(new { status = 0, type = -2 });//не удалось преобразовать параметры
            }
            return Json(new { status = 0, type = -1 });//переменная с параметрами пуста
        }
        public JsonResult cityServer(String city)
        {
            city = getCity(city);
            var status = getStatus(city);
            return Json(new { status = status, city = city });
        }
    }
}