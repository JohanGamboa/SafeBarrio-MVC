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
        public ActionResult Registro(Usuario usuario, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "La contraseña es obligatoria.");
                return View(usuario);
            }

            usuario.PasswordHash = PasswordHelper.HashPassword(password);
            usuario.FechaRegistro = DateTime.Now;

            ModelState.Remove("PasswordHash");

            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuario);
                db.SaveChanges();

                return RedirectToAction("Login", "Usuarios");
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

            int idUsuario = Convert.ToInt32(Session["UsuarioId"]);

            var usuarioBD = db.Usuarios.Find(idUsuario);

            if (usuarioBD == null)
            {
                return HttpNotFound();
            }

            usuarioBD.Nombre = usuario.Nombre;
            usuarioBD.Apellido = usuario.Apellido;
            usuarioBD.Telefono = usuario.Telefono;
            usuarioBD.Ubicacion = usuario.Ubicacion;

            db.SaveChanges();

            Session["UsuarioNombre"] = usuarioBD.Nombre;

            return RedirectToAction("Perfil");
        }

        // EDITAR USUARIO GET
        public ActionResult Editar(int id)
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
        public ActionResult Editar(Usuario usuario, string password)
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

            var usuarioBD = db.Usuarios.Find(usuario.Id);

            if (usuarioBD == null)
            {
                return HttpNotFound();
            }

            usuarioBD.Nombre = usuario.Nombre;
            usuarioBD.Apellido = usuario.Apellido;
            usuarioBD.Correo = usuario.Correo;
            usuarioBD.Telefono = usuario.Telefono;
            usuarioBD.Ubicacion = usuario.Ubicacion;
            usuarioBD.EsAdmin = usuario.EsAdmin;

            if (!string.IsNullOrWhiteSpace(password))
            {
                usuarioBD.PasswordHash = PasswordHelper.HashPassword(password);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ELIMINAR USUARIO GET
        public ActionResult Eliminar(int id)
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

            bool esAdmin = Session["EsAdmin"] != null && Convert.ToBoolean(Session["EsAdmin"]);

            if (!esAdmin)
            {
                return RedirectToAction("Perfil");
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