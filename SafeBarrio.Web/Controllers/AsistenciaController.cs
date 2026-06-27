using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;


namespace SafeBarrio.Web.Controllers
{
    public class AsistenciaController : Controller
    {
        public ActionResult Chat()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Responder(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return Json(new { respuesta = "Escribe una pregunta para poder ayudarte." });
            }

            string prompt = "Responde en español, de forma breve y segura, como asistente de seguridad comunitaria SafeBarrio. Pregunta del usuario: " + mensaje;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://text.pollinations.ai/" + Uri.EscapeDataString(prompt);

                    string respuestaIA = await client.GetStringAsync(url);

                    if (string.IsNullOrWhiteSpace(respuestaIA))
                    {
                        respuestaIA = "Te recomiendo mantener la calma, buscar un lugar seguro y llamar al 911 si estás en peligro.";
                    }

                    return Json(new { respuesta = respuestaIA });
                }
            }
            catch
            {
                string respuestaLocal = "No pude conectar con la IA en este momento. Si estás en peligro, activa SOS, busca un lugar seguro y llama al 911.";
                return Json(new { respuesta = respuestaLocal });
            }
        }
    }
}