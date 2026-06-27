using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SafeBarrio.Web.Data;

namespace SafeBarrio.Web.Controllers
{
    public class AdminController : Controller
    {
        private SafeBarrioContext db = new SafeBarrioContext();

        public ActionResult Dashboard()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            bool esAdmin = Session["EsAdmin"] != null && Convert.ToBoolean(Session["EsAdmin"]);

            if (!esAdmin)
                return RedirectToAction("Dashboard", "Home");

            ViewBag.TotalUsuarios = db.Usuarios.Count();
            ViewBag.TotalIncidentes = db.Incidentes.Count();
            ViewBag.TotalSOSActivos = db.AlertasSOS.Count(x => x.Estado == "Activa");
            ViewBag.TotalSOSResueltos = db.AlertasSOS.Count(x => x.Estado == "Resuelta");

            ViewBag.UltimosIncidentes = db.Incidentes
                .OrderByDescending(x => x.FechaReporte)
                .Take(5)
                .ToList();

            ViewBag.UltimosSOS = db.AlertasSOS
                .OrderByDescending(x => x.FechaAlerta)
                .Take(5)
                .ToList();

            return View();
        }
    }
}