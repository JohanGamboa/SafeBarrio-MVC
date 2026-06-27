using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SafeBarrio.Web.Data;
using SafeBarrio.Web.Entities;

namespace SafeBarrio.Web.Controllers
{
    public class ContactosEmergenciaController : Controller
    {
        private SafeBarrioContext db = new SafeBarrioContext();

        public ActionResult Index()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var contactos = db.ContactosEmergencia
                .Where(c => c.UsuarioId == usuarioId)
                .ToList();

            return View(contactos);
        }

        public ActionResult Crear()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(ContactoEmergencia contacto)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            if (ModelState.IsValid)
            {
                contacto.UsuarioId = Convert.ToInt32(Session["UsuarioId"]);

                db.ContactosEmergencia.Add(contacto);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(contacto);
        }

        public ActionResult Editar(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var contacto = db.ContactosEmergencia
                .FirstOrDefault(c => c.Id == id && c.UsuarioId == usuarioId);

            if (contacto == null)
                return HttpNotFound();

            return View(contacto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(ContactoEmergencia contacto)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            if (ModelState.IsValid)
            {
                contacto.UsuarioId = Convert.ToInt32(Session["UsuarioId"]);

                db.Entry(contacto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(contacto);
        }

        public ActionResult Eliminar(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var contacto = db.ContactosEmergencia
                .FirstOrDefault(c => c.Id == id && c.UsuarioId == usuarioId);

            if (contacto == null)
                return HttpNotFound();

            return View(contacto);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            var contacto = db.ContactosEmergencia
                .FirstOrDefault(c => c.Id == id && c.UsuarioId == usuarioId);

            if (contacto == null)
                return HttpNotFound();

            db.ContactosEmergencia.Remove(contacto);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}