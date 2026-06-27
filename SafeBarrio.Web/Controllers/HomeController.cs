using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SafeBarrio.Web.Data;


namespace SafeBarrio.Web.Controllers
{
    public class HomeController : Controller
    {
        private SafeBarrioContext db = new SafeBarrioContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            ViewBag.UltimosSOS = db.AlertasSOS
                .Where(x => x.Estado == "Activa")
                .OrderByDescending(x => x.FechaAlerta)
                .Take(3)
                .ToList();

            ViewBag.UltimosIncidentes = db.Incidentes
                .OrderByDescending(x => x.FechaReporte)
                .Take(5)
                .ToList();

            ViewBag.IncidentesMapa = db.Incidentes
                .OrderByDescending(x => x.FechaReporte)
                .Take(30)
                .Select(x => new
                {
                    tipo = x.TipoIncidente,
                    descripcion = x.Descripcion,
                    lat = x.Latitud,
                    lng = x.Longitud,
                    referencia = x.DireccionReferencia
                })
                .ToList();

            return View();
        }

        public ActionResult Soporte()
        {
            return View();
        }

        public ActionResult Mapa()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            ViewBag.IncidentesMapa = db.Incidentes
                .OrderByDescending(x => x.FechaReporte)
                .Take(80)
                .Select(x => new
                {
                    tipo = x.TipoIncidente,
                    descripcion = x.Descripcion,
                    lat = x.Latitud,
                    lng = x.Longitud,
                    referencia = x.DireccionReferencia
                })
                .ToList();

            return View();
        }
    }
}