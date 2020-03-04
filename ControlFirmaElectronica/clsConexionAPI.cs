using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ControlFirmaElectronica.NotificacionElectronica;

namespace ControlFirmaElectronica
{
    public class clsConexionAPI
    {
        // SE CREA EL CLIENTE HTTP PARA HACER LA PETICIÓN 
        public HttpClient client = new HttpClient();

        // ESTRUCTURA DE LOS METODOS PARA LLAMAR AL API (PUEDEN O NO SER STATICS), SIEMPRE SON ASYNC PARA MANEJAR LAS TAREAS ASYNCRONAS
        // ESPERA UNA TAREA DE UN TIPO DEFINIDO EN LOS < TIPO ESPERADO >
        public async Task<UsuarioExMin> Autenticar(UsuarioExMin us)
        {

            // OBJETO DE RESPUESTA
            UsuarioExMin product = null;

            // LLAMADA TIPO GET A API
            HttpResponseMessage response = await client.PostAsJsonAsync("https://plataforma.poderjudicial-gto.gob.mx/SV_P3/api/Dependencias/Login", us);

            // SE EVALUA LA RESPUESTA, SI ES EXITOSA, SE PROCESA, DE LO CONTRARIO SE MANEJA COMO ERROR, LA RESPUESTA PUEDE SER DE CUALQUIER TIPO DE RESPUESTA HTTP (400, 404, 302, 500, ETC)
            if (response.IsSuccessStatusCode)
            {

                // LEE LA RESPUESTA

                product = await response.Content.ReadAsAsync<UsuarioExMin>();
            }

            // RETORNA
            return product;
        }

        // IGUAL QUE EL ANTERIOR SOLO QUE ESTE METODO ES POST, RECIBE UN OBJETO COMO PARAMETRO
        public async Task<NotificacionElectronicaInformacion> RealizarNotificacion(ReqRealizarNotificacion product, string token)
        {
            // OBJETO DE RESPUESTA
            NotificacionElectronicaInformacion bandera = new NotificacionElectronicaInformacion();

            // LLAMADA TIPO POST
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.PostAsJsonAsync("https://plataforma.poderjudicial-gto.gob.mx/SV_P3_clon/api/NotificacionElectronica/RealizarNotificacion", product);
            //HttpResponseMessage response = await client.PostAsJsonAsync("http://localhost:52152/api/NotificacionElectronica/RealizarNotificacion", product);

            if (response.IsSuccessStatusCode)
            {
                bandera = await response.Content.ReadAsAsync<NotificacionElectronicaInformacion>();
            }
            return bandera;
        }

        // LOS METODOS QUE HAGAN USO DE LAS LLAMADAS API, DEBERAN SER ASYNCRONOS Y UTILIZAR EL METODO AWAIT
        //private async void llamar()
        //{
        //    string[] product = await GetTransacciones();
        //}
    }

    public class ReqRealizarNotificacion
    { 
        public long Clave { get; set; } 
        public long Credencial { get; set; } 
        public  NotificacionElectronicaUploader Notificacion { get; set; } 
    }
    public class UsuarioExMin
    {
        public long Buzon { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
