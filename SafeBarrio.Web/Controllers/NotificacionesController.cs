using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SafeBarrio.Web.Data;

namespace SafeBarrio.Web.Controllers
{
    public class NotificacionesController : Controller
    {
        private SafeBarrioContext db = new SafeBarrioContext();

        public ActionResult Index()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var lista = db.Notificaciones
                .Where(x => x.UsuarioId == usuarioId)
                .OrderByDescending(x => x.Fecha)
                .ToList();

            return View(lista);
        }
    }
}