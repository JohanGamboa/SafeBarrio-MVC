using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SafeBarrio.Web.Data;
using SafeBarrio.Web.Entities;
using System.IO;


namespace SafeBarrio.Web.Controllers
{
    public class IncidentesController : Controller
    {
        private SafeBarrioContext db = new SafeBarrioContext();

        public ActionResult Reportar()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            return View(new Incidente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reportar(Incidente incidente, HttpPostedFileBase imagenEvidencia)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            string latTexto = Request.Form["Latitud"];
            string lngTexto = Request.Form["Longitud"];

            double latitud;
            double longitud;

            bool latOk = double.TryParse(latTexto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out latitud);
            bool lngOk = double.TryParse(lngTexto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out longitud);

            if (!latOk || !lngOk || latitud == 0 || longitud == 0)
            {
                ModelState.AddModelError("", "Debes buscar o seleccionar una ubicación en el mapa.");
                return View(incidente);
            }

            ModelState.Remove("Latitud");
            ModelState.Remove("Longitud");
            ModelState.Remove("Usuario");
            ModelState.Remove("UsuarioId");
            ModelState.Remove("Estado");
            ModelState.Remove("FechaReporte");
            ModelState.Remove("ImagenRuta");

            incidente.UsuarioId = Convert.ToInt32(Session["UsuarioId"]);
            incidente.FechaReporte = DateTime.Now;
            incidente.Estado = "Reportado";
            incidente.Latitud = latitud;
            incidente.Longitud = longitud;

            if (imagenEvidencia != null && imagenEvidencia.ContentLength > 0)
            {
                string extension = Path.GetExtension(imagenEvidencia.FileName);
                string nombreArchivo = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N") + extension;

                string rutaCarpeta = Server.MapPath("~/Content/Evidencias/");

                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
                imagenEvidencia.SaveAs(rutaCompleta);

                incidente.ImagenRuta = "/Content/Evidencias/" + nombreArchivo;
            }

            if (ModelState.IsValid)
            {
                db.Incidentes.Add(incidente);

                db.Notificaciones.Add(new Notificacion
                {
                    UsuarioId = incidente.UsuarioId,
                    Titulo = "Incidente reportado",
                    Mensaje = "Tu reporte de " + incidente.TipoIncidente + " fue publicado correctamente.",
                    Fecha = DateTime.Now,
                    Leida = false
                });

                db.SaveChanges();

                TempData["Mensaje"] = "Incidente reportado correctamente.";
                return RedirectToAction("MisReportes");
            }

            return View(incidente);
        }

        public ActionResult MisReportes()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var reportes = db.Incidentes
                .Where(i => i.UsuarioId == usuarioId)
                .OrderByDescending(i => i.FechaReporte)
                .ToList();

            return View(reportes);
        }

        public ActionResult Todos()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            bool esAdmin = Session["EsAdmin"] != null && Convert.ToBoolean(Session["EsAdmin"]);

            if (!esAdmin)
                return RedirectToAction("Dashboard", "Home");

            var incidentes = db.Incidentes
                .OrderByDescending(i => i.FechaReporte)
                .ToList();

            return View(incidentes);
        }

        public ActionResult Editar(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var incidente = db.Incidentes
                .FirstOrDefault(i => i.Id == id && i.UsuarioId == usuarioId);

            if (incidente == null)
                return HttpNotFound();

            return View(incidente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Incidente incidente, HttpPostedFileBase imagenEvidencia)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var incidenteBD = db.Incidentes
                .FirstOrDefault(i => i.Id == incidente.Id && i.UsuarioId == usuarioId);

            if (incidenteBD == null)
                return HttpNotFound();

            ModelState.Remove("Usuario");
            ModelState.Remove("UsuarioId");
            ModelState.Remove("Estado");
            ModelState.Remove("FechaReporte");
            ModelState.Remove("ImagenRuta");

            if (ModelState.IsValid)
            {
                incidenteBD.TipoIncidente = incidente.TipoIncidente;
                incidenteBD.Descripcion = incidente.Descripcion;
                incidenteBD.DireccionReferencia = incidente.DireccionReferencia;
                incidenteBD.Latitud = incidente.Latitud;
                incidenteBD.Longitud = incidente.Longitud;
                incidenteBD.Estado = "Actualizado";

                if (imagenEvidencia != null && imagenEvidencia.ContentLength > 0)
                {
                    string extension = Path.GetExtension(imagenEvidencia.FileName);
                    string nombreArchivo = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N") + extension;

                    string rutaCarpeta = Server.MapPath("~/Content/Evidencias/");

                    if (!Directory.Exists(rutaCarpeta))
                    {
                        Directory.CreateDirectory(rutaCarpeta);
                    }

                    string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
                    imagenEvidencia.SaveAs(rutaCompleta);

                    incidenteBD.ImagenRuta = "/Content/Evidencias/" + nombreArchivo;
                }

                db.SaveChanges();

                TempData["Mensaje"] = "Reporte actualizado correctamente.";
                return RedirectToAction("MisReportes");
            }

            return View(incidente);
        }

        public ActionResult Eliminar(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var incidente = db.Incidentes
                .FirstOrDefault(i => i.Id == id && i.UsuarioId == usuarioId);

            if (incidente == null)
                return HttpNotFound();

            return View(incidente);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var incidente = db.Incidentes
                .FirstOrDefault(i => i.Id == id && i.UsuarioId == usuarioId);

            if (incidente == null)
                return HttpNotFound();

            db.Incidentes.Remove(incidente);
            db.SaveChanges();

            TempData["Mensaje"] = "Reporte eliminado correctamente.";
            return RedirectToAction("MisReportes");
        }
    }
}