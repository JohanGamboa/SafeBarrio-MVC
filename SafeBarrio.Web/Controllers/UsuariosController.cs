using System;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using SafeBarrio.Web.Data;
using SafeBarrio.Web.Entities;
using SafeBarrio.Web.Models;
using SafeBarrio.Web.Helpers;

namespace SafeBarrio.Web.Controllers
{
    public class UsuariosController : Controller
    {
        private SafeBarrioContext db = new SafeBarrioContext();

        // REGISTRO GET
        public ActionResult Registro()
        {
            return View();
        }

        // REGISTRO POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                bool correoExiste = db.Usuarios.Any(u => u.Correo == usuario.Correo);

                if (correoExiste)
                {
                    ModelState.AddModelError("Correo", "Este correo ya está registrado.");
                    return View(usuario);
                }

                usuario.FechaRegistro = DateTime.Now;

                usuario.PasswordHash = PasswordHelper.HashPassword(usuario.PasswordHash);
                db.Usuarios.Add(usuario);
                db.SaveChanges();

                TempData["Mensaje"] = "Cuenta creada correctamente. Ahora inicia sesión.";
                return RedirectToAction("Login");
            }

            return View(usuario);
        }

        // LOGIN GET
        public ActionResult Login()
        {
            return View();
        }

        // LOGIN POST
        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                string passwordHash = PasswordHelper.HashPassword(login.Password);

                var usuario = db.Usuarios.FirstOrDefault(u =>
                    u.Correo == login.Correo &&
                    u.PasswordHash == passwordHash);

                if (usuario != null)
                {
                    Session["UsuarioId"] = usuario.Id;
                    Session["UsuarioNombre"] = usuario.Nombre;
                    Session["UsuarioCorreo"] = usuario.Correo;
                    Session["EsAdmin"] = usuario.EsAdmin;

                    return RedirectToAction("Dashboard", "Home");
                }

                ViewBag.Error = "Correo o contraseña incorrectos.";
            }

            return View(login);
        }

        // CERRAR SESIÓN
        public ActionResult CerrarSesion()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // LISTAR USUARIOS
        public ActionResult Index()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login");
            }

            bool esAdmin = Session["EsAdmin"] != null && Convert.ToBoolean(Session["EsAdmin"]);

            if (!esAdmin)
            {
                return RedirectToAction("Perfil");
            }

            var usuarios = db.Usuarios.ToList();
            return View(usuarios);
        }

        // PERFIL DEL USUARIO LOGEADO
        public ActionResult Perfil()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login");
            }

            int idUsuario = Convert.ToInt32(Session["UsuarioId"]);

            var usuario = db.Usuarios.Find(idUsuario);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        // EDITAR PERFIL GET
        public ActionResult EditarPerfil()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login");
            }

            int idUsuario = Convert.ToInt32(Session["UsuarioId"]);

            var usuario = db.Usuarios.Find(idUsuario);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        // EDITAR PERFIL POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPerfil(Usuario usuario)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();

                Session["UsuarioNombre"] = usuario.Nombre;
                Session["UsuarioCorreo"] = usuario.Correo;

                return RedirectToAction("Perfil");
            }

            return View(usuario);
        }

        // EDITAR USUARIO GET
        public ActionResult Editar(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login");
            }

            var usuario = db.Usuarios.Find(id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        // EDITAR USUARIO POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Usuario usuario)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // ELIMINAR USUARIO GET
        public ActionResult Eliminar(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login");
            }

            var usuario = db.Usuarios.Find(id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        // ELIMINAR USUARIO POST
        [HttpPost]
        [ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login");
            }

            var usuario = db.Usuarios.Find(id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            db.Usuarios.Remove(usuario);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}