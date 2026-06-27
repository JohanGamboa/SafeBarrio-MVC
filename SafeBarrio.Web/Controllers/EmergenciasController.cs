using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SafeBarrio.Web.Data;
using SafeBarrio.Web.Entities;


namespace SafeBarrio.Web.Controllers
{
    public class EmergenciasController : Controller
    {
        private SafeBarrioContext db = new SafeBarrioContext();

        public ActionResult SOS()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            ViewBag.MiSOSActivo = db.AlertasSOS
                .Where(a => a.UsuarioId == usuarioId && a.Estado == "Activa")
                .OrderByDescending(a => a.FechaAlerta)
                .FirstOrDefault();

            return View();
        }

        [HttpPost]
        public JsonResult GuardarSOS(double latitud, double longitud)
        {
            if (Session["UsuarioId"] == null)
                return Json(new { ok = false, mensaje = "Sesión expirada." });

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var alerta = new AlertaSOS
            {
                UsuarioId = usuarioId,
                Latitud = latitud,
                Longitud = longitud,
                FechaAlerta = DateTime.Now,
                Estado = "Activa",
                Mensaje = "Alerta SOS enviada por el usuario."
            };

            db.AlertasSOS.Add(alerta);

            db.Notificaciones.Add(new Notificacion
            {
                UsuarioId = usuarioId,
                Titulo = "Alerta SOS enviada",
                Mensaje = "Tu alerta de emergencia fue registrada correctamente.",
                Fecha = DateTime.Now,
                Leida = false
            });

            db.SaveChanges();

            return Json(new { ok = true, mensaje = "Alerta SOS enviada correctamente." });
        }

        public ActionResult FinalizarMiSOS(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var alerta = db.AlertasSOS
                .FirstOrDefault(a => a.Id == id && a.UsuarioId == usuarioId);

            if (alerta == null)
                return HttpNotFound();

            alerta.Estado = "Resuelta";
            alerta.Mensaje = "La emergencia fue finalizada por el usuario.";
            db.SaveChanges();

            return RedirectToAction("SOS");
        }

        public ActionResult Activas()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            bool esAdmin = Session["EsAdmin"] != null && Convert.ToBoolean(Session["EsAdmin"]);

            if (!esAdmin)
                return RedirectToAction("Dashboard", "Home");

            var alertas = db.AlertasSOS
                .OrderByDescending(a => a.FechaAlerta)
                .ToList();

            return View(alertas);
        }

        public ActionResult Resolver(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            bool esAdmin = Session["EsAdmin"] != null && Convert.ToBoolean(Session["EsAdmin"]);

            if (!esAdmin)
                return RedirectToAction("Dashboard", "Home");

            var alerta = db.AlertasSOS.Find(id);

            if (alerta == null)
                return HttpNotFound();

            alerta.Estado = "Resuelta";
            alerta.Mensaje = "La emergencia fue atendida o finalizada por administración.";
            db.SaveChanges();

            return RedirectToAction("Activas");
        }
    }
}