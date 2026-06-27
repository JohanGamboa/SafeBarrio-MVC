using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SafeBarrio.Web.Data;
using SafeBarrio.Web.Helpers;
using SafeBarrio.Web.Entities;
using System.IO;
using System.Globalization;


namespace SafeBarrio.Web.Controllers
{
    public class ApiController : Controller
    {
        private SafeBarrioContext db = new SafeBarrioContext();
        private bool EsAdmin(int usuarioId)
        {
            var usuario = db.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
            return usuario != null && usuario.EsAdmin;
        }

        [HttpPost]
        public JsonResult Login(string correo, string password)
        {
            string passwordHash = PasswordHelper.HashPassword(password);

            var usuario = db.Usuarios.FirstOrDefault(u =>
                u.Correo == correo &&
                u.PasswordHash == passwordHash);

            if (usuario == null)
                return Json(new { ok = false, mensaje = "Correo o contraseña incorrectos." });

            return Json(new
            {
                ok = true,
                id = usuario.Id,
                nombre = usuario.Nombre,
                correo = usuario.Correo,
                esAdmin = usuario.EsAdmin
            });
        }

        [HttpGet]
        public JsonResult ObtenerIncidente(int id, int usuarioId)
        {
            var incidente = db.Incidentes
                .FirstOrDefault(i => i.Id == id && i.UsuarioId == usuarioId);

            if (incidente == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                id = incidente.Id,
                tipoIncidente = incidente.TipoIncidente,
                descripcion = incidente.Descripcion,
                direccionReferencia = incidente.DireccionReferencia,
                estado = incidente.Estado,
                fechaReporte = incidente.FechaReporte.ToString("dd/MM/yyyy HH:mm"),
                imagenRuta = incidente.ImagenRuta,
                latitud = incidente.Latitud,
                longitud = incidente.Longitud
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditarIncidente()
        {
            try
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                int usuarioId = Convert.ToInt32(Request.Form["usuarioId"]);

                var incidente = db.Incidentes
                    .FirstOrDefault(i => i.Id == id && i.UsuarioId == usuarioId);

                if (incidente == null)
                    return Json(new { ok = false, mensaje = "No se encontró el reporte." });

                incidente.TipoIncidente = Request.Form["tipoIncidente"];
                incidente.Descripcion = Request.Form["descripcion"];
                incidente.DireccionReferencia = Request.Form["direccionReferencia"];
                incidente.Latitud = double.Parse(Request.Form["latitud"], System.Globalization.CultureInfo.InvariantCulture);
                incidente.Longitud = double.Parse(Request.Form["longitud"], System.Globalization.CultureInfo.InvariantCulture);
                incidente.Estado = "Actualizado";

                var imagenEvidencia = Request.Files["imagenEvidencia"];

                if (imagenEvidencia != null && imagenEvidencia.ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(imagenEvidencia.FileName);
                    string nombreArchivo = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N") + extension;

                    string carpeta = Server.MapPath("~/Content/Evidencias/");

                    if (!System.IO.Directory.Exists(carpeta))
                        System.IO.Directory.CreateDirectory(carpeta);

                    string rutaCompleta = System.IO.Path.Combine(carpeta, nombreArchivo);
                    imagenEvidencia.SaveAs(rutaCompleta);

                    incidente.ImagenRuta = "/Content/Evidencias/" + nombreArchivo;
                }

                db.SaveChanges();

                return Json(new { ok = true, mensaje = "Reporte actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "Error al editar reporte: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult Perfil(int usuarioId)
        {
            var usuario = db.Usuarios.FirstOrDefault(u => u.Id == usuarioId);

            if (usuario == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                id = usuario.Id,
                nombre = usuario.Nombre,
                apellido = usuario.Apellido,
                correo = usuario.Correo,
                telefono = usuario.Telefono,
                ubicacion = usuario.Ubicacion,
                esAdmin = usuario.EsAdmin
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult MapaDashboard()
        {
            var incidentes = db.Incidentes
                .Where(i => i.Estado != "Eliminado")
                .ToList()
                .Select(i => new
                {
                    tipo = "incidente",
                    titulo = i.TipoIncidente,
                    descripcion = i.Descripcion,
                    latitud = i.Latitud,
                    longitud = i.Longitud,
                    estado = i.Estado
                });

            var sos = db.AlertasSOS
                .Where(s => s.Estado == "Activa")
                .ToList()
                .Select(s => new
                {
                    tipo = "sos",
                    titulo = "SOS",
                    descripcion = s.Mensaje,
                    latitud = s.Latitud,
                    longitud = s.Longitud,
                    estado = s.Estado
                });

            return Json(
                incidentes.Concat(sos),
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditarPerfil(
            int usuarioId,
            string nombre,
            string apellido,
            string telefono,
            string ubicacion)
        {
            var usuario = db.Usuarios.FirstOrDefault(u => u.Id == usuarioId);

            if (usuario == null)
                return Json(new { ok = false, mensaje = "No se encontró el usuario." });

            usuario.Nombre = nombre;
            usuario.Apellido = apellido;
            usuario.Telefono = telefono;
            usuario.Ubicacion = ubicacion;

            db.SaveChanges();

            return Json(new
            {
                ok = true,
                mensaje = "Perfil actualizado correctamente."
            });
        }



        [HttpGet]
        public JsonResult Notificaciones(int usuarioId)
        {
            var notificacionesBD = db.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.Fecha)
                .Take(20)
                .ToList();

            var notificaciones = notificacionesBD.Select(n => new
            {
                id = n.Id,
                titulo = n.Titulo,
                mensaje = n.Mensaje,
                fecha = n.Fecha.ToString("dd/MM/yyyy HH:mm"),
                leida = n.Leida
            }).ToList();

            return Json(notificaciones, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarIncidente(int id, int usuarioId)
        {
            var incidente = db.Incidentes
                .FirstOrDefault(i => i.Id == id && i.UsuarioId == usuarioId);

            if (incidente == null)
                return Json(new { ok = false, mensaje = "No se encontró el reporte." });

            db.Incidentes.Remove(incidente);
            db.SaveChanges();

            return Json(new { ok = true, mensaje = "Reporte eliminado correctamente." });
        }

        [HttpPost]
        public JsonResult GuardarSOS()
        {
            try
            {
                int usuarioId = Convert.ToInt32(Request.Form["usuarioId"]);

                double latitud = double.Parse(
                    Request.Form["latitud"],
                    System.Globalization.CultureInfo.InvariantCulture);

                double longitud = double.Parse(
                    Request.Form["longitud"],
                    System.Globalization.CultureInfo.InvariantCulture);

                var alerta = new AlertaSOS
                {
                    UsuarioId = usuarioId,
                    Latitud = latitud,
                    Longitud = longitud,
                    FechaAlerta = DateTime.Now,
                    Estado = "Activa",
                    Mensaje = "Alerta SOS enviada desde la app móvil."
                };

                db.AlertasSOS.Add(alerta);

                db.Notificaciones.Add(new Notificacion
                {
                    UsuarioId = usuarioId,
                    Titulo = "Alerta SOS enviada",
                    Mensaje = "Tu alerta SOS fue registrada desde la app móvil.",
                    Fecha = DateTime.Now,
                    Leida = false
                });

                db.SaveChanges();

                return Json(new { ok = true, mensaje = "Alerta SOS enviada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "Error SOS: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult MiSOSActivo(int usuarioId)
        {
            var sos = db.AlertasSOS
                .Where(a => a.UsuarioId == usuarioId && a.Estado == "Activa")
                .OrderByDescending(a => a.FechaAlerta)
                .FirstOrDefault();

            if (sos == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                id = sos.Id,
                latitud = sos.Latitud,
                longitud = sos.Longitud,
                fechaAlerta = sos.FechaAlerta.ToString("dd/MM/yyyy HH:mm"),
                estado = sos.Estado,
                mensaje = sos.Mensaje
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FinalizarSOS(int usuarioId, int alertaId)
        {
            var alerta = db.AlertasSOS
                .FirstOrDefault(a => a.Id == alertaId && a.UsuarioId == usuarioId);

            if (alerta == null)
                return Json(new { ok = false, mensaje = "No se encontró la alerta SOS." });

            alerta.Estado = "Resuelta";
            alerta.Mensaje = "La emergencia fue finalizada desde la app móvil.";

            db.Notificaciones.Add(new Notificacion
            {
                UsuarioId = usuarioId,
                Titulo = "Alerta SOS finalizada",
                Mensaje = "Tu alerta SOS fue marcada como resuelta correctamente.",
                Fecha = DateTime.Now,
                Leida = false
            });

            db.SaveChanges();

            return Json(new { ok = true, mensaje = "Alerta SOS finalizada correctamente." });
        }

        [HttpGet]
        public JsonResult AlertasRecientes()
        {
            var alertasBD = db.AlertasSOS
                .Where(a => a.Estado == "Activa")
                .OrderByDescending(a => a.FechaAlerta)
                .Take(5)
                .ToList();

            var alertas = alertasBD.Select(a => new
            {
                id = a.Id,
                latitud = a.Latitud,
                longitud = a.Longitud,
                fechaAlerta = a.FechaAlerta.ToString("dd/MM/yyyy HH:mm"),
                estado = a.Estado,
                mensaje = a.Mensaje
            }).ToList();

            return Json(alertas, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult MisReportes(int usuarioId)
        {
            var reportesBD = db.Incidentes
                .Where(i => i.UsuarioId == usuarioId)
                .OrderByDescending(i => i.FechaReporte)
                .ToList();

            var reportes = reportesBD.Select(i => new
            {
                id = i.Id,
                tipoIncidente = i.TipoIncidente,
                descripcion = i.Descripcion,
                direccionReferencia = i.DireccionReferencia,
                estado = i.Estado,
                fechaReporte = i.FechaReporte.ToString("dd/MM/yyyy HH:mm"),
                imagenRuta = i.ImagenRuta
            }).ToList();

            return Json(reportes, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public JsonResult ReportarIncidente()
        {
            try
            {
                int usuarioId = Convert.ToInt32(Request.Form["usuarioId"]);
                string tipoIncidente = Request.Form["tipoIncidente"];
                string descripcion = Request.Form["descripcion"];
                string direccionReferencia = Request.Form["direccionReferencia"];

                double latitud = double.Parse(Request.Form["latitud"], CultureInfo.InvariantCulture);
                double longitud = double.Parse(Request.Form["longitud"], CultureInfo.InvariantCulture);

                var incidente = new Incidente
                {
                    UsuarioId = usuarioId,
                    TipoIncidente = tipoIncidente,
                    Descripcion = descripcion,
                    DireccionReferencia = direccionReferencia,
                    Latitud = latitud,
                    Longitud = longitud,
                    FechaReporte = DateTime.Now,
                    Estado = "Reportado"
                };

                var imagenEvidencia = Request.Files["imagenEvidencia"];

                if (imagenEvidencia != null && imagenEvidencia.ContentLength > 0)
                {
                    string extension = Path.GetExtension(imagenEvidencia.FileName);
                    string nombreArchivo = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N") + extension;

                    string carpeta = Server.MapPath("~/Content/Evidencias/");

                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    string rutaCompleta = Path.Combine(carpeta, nombreArchivo);
                    imagenEvidencia.SaveAs(rutaCompleta);

                    incidente.ImagenRuta = "/Content/Evidencias/" + nombreArchivo;
                }

                db.Incidentes.Add(incidente);

                db.Notificaciones.Add(new Notificacion
                {
                    UsuarioId = usuarioId,
                    Titulo = "Incidente reportado",
                    Mensaje = "Tu reporte de " + tipoIncidente + " fue publicado desde la app móvil.",
                    Fecha = DateTime.Now,
                    Leida = false
                });

                db.SaveChanges();

                return Json(new { ok = true, mensaje = "Incidente reportado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "Error al guardar reporte: " + ex.Message });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> Asistencia(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return Json(new { ok = false, respuesta = "Escribe una pregunta para poder ayudarte." });
            }

            string prompt = "Responde en español, máximo 4 líneas, como asistente de seguridad comunitaria SafeBarrio. Pregunta: " + mensaje;

            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(8);

                    string url = "https://text.pollinations.ai/" + Uri.EscapeDataString(prompt);
                    string respuestaIA = await client.GetStringAsync(url);

                    return Json(new { ok = true, respuesta = respuestaIA });
                }
            }
            catch
            {
                return Json(new
                {
                    ok = true,
                    respuesta = "No pude conectar con la IA ahora. Si estás en peligro, activa SOS, busca un lugar seguro y llama al 911."
                });
            }
        }
        [HttpGet]
        public JsonResult AdminDashboard(int usuarioId)
        {
            if (!EsAdmin(usuarioId))
                return Json(new { ok = false, mensaje = "No autorizado." }, JsonRequestBehavior.AllowGet);

            var ultimosIncidentesBD = db.Incidentes
                .OrderByDescending(i => i.FechaReporte)
                .Take(5)
                .ToList();

            var ultimasAlertasBD = db.AlertasSOS
                .OrderByDescending(a => a.FechaAlerta)
                .Take(5)
                .ToList();

            return Json(new
            {
                ok = true,
                totalUsuarios = db.Usuarios.Count(),
                totalIncidentes = db.Incidentes.Count(),
                sosActivos = db.AlertasSOS.Count(a => a.Estado == "Activa"),
                sosResueltos = db.AlertasSOS.Count(a => a.Estado == "Resuelta"),

                ultimosIncidentes = ultimosIncidentesBD.Select(i => new
                {
                    id = i.Id,
                    tipoIncidente = i.TipoIncidente,
                    descripcion = i.Descripcion,
                    direccionReferencia = i.DireccionReferencia,
                    estado = i.Estado,
                    fechaReporte = i.FechaReporte.ToString("dd/MM/yyyy HH:mm")
                }).ToList(),

                ultimasAlertas = ultimasAlertasBD.Select(a => new
                {
                    id = a.Id,
                    latitud = a.Latitud,
                    longitud = a.Longitud,
                    estado = a.Estado,
                    mensaje = a.Mensaje,
                    fechaAlerta = a.FechaAlerta.ToString("dd/MM/yyyy HH:mm")
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UsuariosAdmin(int usuarioId)
        {
            if (!EsAdmin(usuarioId))
                return Json(new { ok = false, mensaje = "No autorizado." }, JsonRequestBehavior.AllowGet);

            var usuarios = db.Usuarios
                .OrderBy(u => u.Nombre)
                .ToList()
                .Select(u => new
                {
                    id = u.Id,
                    nombre = u.Nombre,
                    correo = u.Correo,
                    telefono = u.Telefono,
                    direccion = u.Ubicacion,
                    esAdmin = u.EsAdmin
                })
                .ToList();

            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IncidentesAdmin(int usuarioId)
        {
            if (!EsAdmin(usuarioId))
                return Json(new { ok = false, mensaje = "No autorizado." }, JsonRequestBehavior.AllowGet);

            var incidentes = db.Incidentes
                .OrderByDescending(i => i.FechaReporte)
                .ToList()
                .Select(i => new
                {
                    id = i.Id,
                    tipoIncidente = i.TipoIncidente,
                    descripcion = i.Descripcion,
                    direccionReferencia = i.DireccionReferencia,
                    estado = i.Estado,
                    fechaReporte = i.FechaReporte.ToString("dd/MM/yyyy HH:mm"),
                    latitud = i.Latitud,
                    longitud = i.Longitud,
                    imagenRuta = i.ImagenRuta
                })
                .ToList();

            return Json(incidentes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AlertasSOSAdmin(int usuarioId)
        {
            if (!EsAdmin(usuarioId))
                return Json(new { ok = false, mensaje = "No autorizado." }, JsonRequestBehavior.AllowGet);

            var alertas = db.AlertasSOS
                .OrderByDescending(a => a.FechaAlerta)
                .ToList()
                .Select(a => new
                {
                    id = a.Id,
                    latitud = a.Latitud,
                    longitud = a.Longitud,
                    estado = a.Estado,
                    mensaje = a.Mensaje,
                    fechaAlerta = a.FechaAlerta.ToString("dd/MM/yyyy HH:mm")
                })
                .ToList();

            return Json(alertas, JsonRequestBehavior.AllowGet);
        }

    }

}