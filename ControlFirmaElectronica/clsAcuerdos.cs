using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using AccesoDatos;
using System.IO;
using APISeguriSign;
using System.Diagnostics;
using ControlFirmaElectronica.NotificacionElectronica;
using Prueba_rtf;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Net;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;

namespace ControlFirmaElectronica
{
    public class clsAcuerdos
    {
        public ConexionMySQL CConexionMySQL { get; set; }
        public int intOpcion { get; set; }
        public string strTipoExpe { get; set; }
        public string strpersfolio { get; set; }
        public string strpersfolioN { get; set; }
        public string strbuzon { get; set; }
        public string strfolio { get; set; }
        public string strindice { get; set; }
        public string indice { get; set; }
        public string strNumeroexpeR { get; set; }
        public string strtipoexpeR { get; set; }
        public string strNumeroExpe { get; set; }
        public string strTipoMovi { get; set; }
        public string strFolioMovi { get; set; }
        public string strRuta { get; set; }
        public string strTextoResolutivo { get; set; }
        public string HashOriginal { get; set; }
        public int TotalFirmas { get; set; }
        public myListObj FirmaSeleccionada { get; set; }
        public bool FirmaCorrecta { get; set; }
        public long IdFirmaSeleccionada { get; set; }
        public string strServidor { get; set; }
        public string strPuerto { get; set; }
        public string strCentro { get; set; }
        public string strUsuario { get; set; }
        public string strNombre { get; set; }
        public string strNotificable { get; set; }
        public string strevidencia { get; set; }
        public string strNivel { get; set; }
        public string strMunicipio { get; set; }
        public string strNombreJuzgado { get; set; }
        public bool bAcuerdoRevizado { get; set; }
        public int intTipoFirma { get; set; }
        public long lngIdAuto { get; set; }
        public string strRutaIP { get; set; }
        public string strServidorIP { get; set; }
        public string strPuertoIP { get; set; }
        public string strUid { get; set; }
        public string strPwd { get; set; }
        public string strPartexp { get; set; }
        public ListView listaadj { get; set; }
        public ListView lisVertraslado { get; set; }
        public CheckBox corrertras { get; set; }
        public Label lblvertexto { get; set; }
        public string strNombreEnviaNot { get; set; }
        public string strNivelEnvia { get; set; }

        //Propiedades con los resultados de la firma
        public string Digestion { get; set; }
        public long Secuencia { get; set; }
        public string Tsp { get; set; }
        public string Fir { get; set; }

        //Propiedad para el error
        public string strError { get; set; }

        //Propiedad para las tablas de archivos con huella digital
        public DataTable ArchivosHuellaDigital { get; set; }

        //Propiedad para la url que se genera
        public string strURL { get; set; }

        //Propiedad para filtrar las notificaciones
        public string strNumeroExpeBusqueda { get; set; }

        private clsFirma Firma = new clsFirma();
        public string strSQL = "";

        //Datos del acuerdo seleccionado
        string _strFechaAuto = "";
        string _strAuto = "";
        string _strResumenAuto = "";
        string _DescripcionExpediente = "";
        ContratoNotifiacionElectronica ne = new ContratoNotifiacionElectronicaClient();
        Suscriptores.ContratoGestionSuscriptoresClient clientesus = new Suscriptores.ContratoGestionSuscriptoresClient();
        Suscriptores.ContratoGestionSuscriptoresClient clientesusNR = new Suscriptores.ContratoGestionSuscriptoresClient();
        NotificacionElectronicaUploader neu = new NotificacionElectronicaUploader();
        Verificador veri = new Verificador();
        Suscriptores.Verificador veriR = new Suscriptores.Verificador();
        Suscriptores.Verificador veriNR = new Suscriptores.Verificador();
        Cuenta cue = new Cuenta();    
             

        public clsAcuerdos()
        {
            CConexionMySQL = new ConexionMySQL();
            strNumeroExpeBusqueda = "";
            CargarValores();
            //strCentro = "262120101";//FIRMA PRESIDENTE
            cue.Nick = strCentro;// "102110201";
            cue.Password = Md5Hash(strCentro);
            cue.CentroTrabajo = strCentro;
            EstadoLogeo edo = ne.IniciarSesion(cue);
            veri.ClaveCentro = edo.TicketSesion.Centro.Clave;
            veri.ClavePlataforma = edo.TicketSesion.Plataforma.ClavePlataforma;
            veri.TOKEN = edo.SesionInformacion.Token;

            //Crear columnas
            ArchivosHuellaDigital = new DataTable();
            ArchivosHuellaDigital.Columns.Add("NombreArchivo");
            ArchivosHuellaDigital.Columns.Add("TamanioFisico");
            ArchivosHuellaDigital.Columns.Add("FechaArchivo");
            ArchivosHuellaDigital.Columns.Add("FechaUltimaModificacion");            
            ArchivosHuellaDigital.Columns.Add("HuellaDigital");
        }

        public List<object> ObtenerFirmas()
        {
            List<object> Resultado = Firma.CargarFirmas();
            return Resultado;
        }


        public string Md5Hash(string input)
        {
            // Creamos una nueva instancias
            MD5 md5Hasher = MD5.Create();

            // le sacamos los byte a la cadea
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            //Creamos un string builder para aterrizar la cadena
            StringBuilder sBuilder = new StringBuilder();

            // recorremos byte por byte hasta que se transforme toda en una cadena hex
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // la regresamos
            return sBuilder.ToString();
        }


        private void CargarValores()
        {
            DataSet xmlParamentros = new DataSet();
            xmlParamentros.ReadXml(Application.StartupPath + "\\parFirma.xml");

            intOpcion = int.Parse(xmlParamentros.Tables[0].Rows[0]["Opcion"].ToString());
            strRuta = xmlParamentros.Tables[0].Rows[0]["Ruta"].ToString();
            strCentro = xmlParamentros.Tables[0].Rows[0]["Centro"].ToString();
            strServidor = xmlParamentros.Tables[0].Rows[0]["Ip"].ToString();
            strPuerto = xmlParamentros.Tables[0].Rows[0]["Puerto"].ToString();
            strUsuario = xmlParamentros.Tables[0].Rows[0]["Usuario"].ToString();
            strNombre = xmlParamentros.Tables[0].Rows[0]["Nombre"].ToString();
            strNivel = xmlParamentros.Tables[0].Rows[0]["Nivel"].ToString();
            strMunicipio = xmlParamentros.Tables[0].Rows[0]["PartidoJudicial"].ToString();
            strNombreJuzgado = xmlParamentros.Tables[0].Rows[0]["Juzgado"].ToString();
            strUid = xmlParamentros.Tables[0].Rows[0]["Uid"].ToString();
            strPwd = xmlParamentros.Tables[0].Rows[0]["Pwd"].ToString();
            xmlParamentros.Dispose();
        }
        private void CargarValorNotificacionEnvio()
        {
            DataSet xmlParamentros = new DataSet();
            xmlParamentros.ReadXml(Application.StartupPath + "\\parFirma.xml");


            strNombreEnviaNot = xmlParamentros.Tables[0].Rows[0]["Nombre"].ToString();
            strNivelEnvia = xmlParamentros.Tables[0].Rows[0]["Nivel"].ToString();

            xmlParamentros.Dispose();
        }
        public long BloquearAccesoExpediente()
        {
            veriNR.ClaveCentro = veri.ClaveCentro;
            veriNR.ClavePlataforma = veri.ClavePlataforma;
            veriNR.TOKEN = veri.TOKEN;



            //clsAcuerdos objet = new clsAcuerdos();

            Suscriptores.Expedientes expediente = new Suscriptores.Expedientes();
            expediente.CentroTrabajo = new Suscriptores.Centros();
            //expediente.CentroTrabajo.Clave = "71811771891911";//strindice ;
            //expediente.NumeroExpediente = "C0104/2012";//strNumeroexpeR;
            //expediente.TipoExpediente = int.Parse("1");//int.Parse(strtipoexpeR) ;


            expediente.CentroTrabajo.Clave = strindice;
            expediente.NumeroExpediente = strNumeroexpeR; //Acuerdos.strNumeroexpeR;
            expediente.TipoExpediente = (int.Parse(strtipoexpeR));
            long respuesta = clientesusNR.BloquearBuzonExpediente(veriNR, long.Parse(strbuzon), expediente);
            return respuesta;
        }
    
        public string RealizarFirma()
        {
            string Resultado = "";
            //string NuevoArchivoPDF = strTextoResolutivo.Replace(".rtf", ".pdf");
            //Firma.ConvertirRTF_PDF(strRuta + "\\firmaele\\" + strTextoResolutivo);
            Firma.Firma = FirmaSeleccionada;
            //Firma.strIP = "10.11.1.20";   //"10.1.1.74";
            //Firma.strIP = "189.254.239.135";
            CargarValoresIP();
            Firma.strIP = strServidorIP;
            Firma.intPuerto = Int32.Parse(strPuertoIP);
            int tipoServer = 1;

            if (TotalFirmas > 1)
            {
                if (Firma.ValidarArchivoHASH(strRuta + "\\firmaele\\Texto.pdf", HashOriginal) == true)
                {
                    Firma.RealizarFirmaTextoConContenido(Firma.strDigestion);
                }
                else
                {
                    Resultado = "El documento que se intenta firmar ha sido modificado. \nMotivo : la comparación de la huella digital con el documento original no coincide.";
                    FirmaCorrecta = false;
                    return Resultado;
                }
            }
            else
            {                
                Firma.strDigestion = Firma.ObtenerSHA1Archivo(strRuta + "\\firmaele\\Texto.pdf");                
                Firma.RealizarFirmaTextoConContenido(Firma.strDigestion);
            }

            if (Firma.AutenticarFirma() != 0)
            {
                Resultado = "Firma electrónica avanzada realizada correctamente.";
                if (AgregarFirmaAceptada(Firma.strDigestion, Firma.IdSecuencia, Firma.strTSP, Firma.strFIR, tipoServer) == true)
                {
                    FirmaCorrecta = true;                    
                }
                else
                    FirmaCorrecta = false;
            }
            else
            {
                if (Firma.errMsg.Length == 0)
                    strError = "Cancelación por parte del usuario";
                Resultado = "Error en la firma, el error fue : " + Firma.errMsg;               
                FirmaCorrecta = false;
            }

            return Resultado;
        }

        //Método para firmar un archivo
        public void RealizarFirma(string _RutaArchivo)
        {
            Firma.Firma = FirmaSeleccionada;
            //Firma.strIP = "10.11.1.20";   //"10.1.1.74";
            //Firma.strIP = "189.254.239.135";
            //Firma.intPuerto = 7920;
            CargarValoresIP();
            Firma.strIP = strServidorIP;
            Firma.intPuerto = Int32.Parse(strPuertoIP);
            
            //Firmar el archivo
            Firma.strDigestion = Firma.ObtenerSHA1Archivo(_RutaArchivo);
            Firma.RealizarFirmaTextoConContenido(Firma.strDigestion);            

            if (Firma.AutenticarFirma() != 0)
            {
                Digestion = Firma.strDigestion;
                Secuencia = Firma.IdSecuencia;
                Tsp = Firma.strTSP;
                Fir = Firma.strFIR;
                FirmaCorrecta = true;                
            }
            else
            {
                strError = "Error en la firma, el error fue : " + Firma.errMsg;                
                FirmaCorrecta = false;
            }           
        }

        private void CargarValoresIP()
        {
            DataSet xmlParamentrosIP = new DataSet();
            xmlParamentrosIP.ReadXml(Application.StartupPath + "\\configIP.xml");


            strRutaIP = strRuta;
            strServidorIP = xmlParamentrosIP.Tables[0].Rows[0]["Ip"].ToString();
            strPuertoIP = xmlParamentrosIP.Tables[0].Rows[0]["Puerto"].ToString();
            xmlParamentrosIP.Dispose();
        }
        //Método para cargar la descripción del acuerdo
        public DataTable CargarDescripcionAcuerdo()
        {
            DataTable Resultado = new DataTable();
           
            strSQL = "select acue_fecha_acue as FechaAcue," +
                       "catalogos.autos_salas_civi.ctsc_auto as Auto," +
                       "acue_resumen_acue as Resumen," +
                       "if(acue_revizado=1,'ACUERDO REVISADO','ACUERDO SIN REVISAR') as Revisado," +
                       "catalogos.autos_salas_civi.ctsc_id as IdAuto " +
                       "from acuerdos " +
                       "inner join catalogos.autos_salas_civi on " +
                       "acuerdos.acue_ctsc_id = catalogos.autos_salas_civi.ctsc_id where " +
                       "acue_tipo_expe = " + strTipoExpe + " and " +
                       "acue_numero_expe = '" + strNumeroExpe + "' and " +
                       "acue_folio_acue = " + strFolioMovi + " and " +
                       "acue_tipo_acue = '" + strTipoMovi + "';";              

            Resultado = CConexionMySQL.RegresaTabla(strSQL);

            DataTableReader dtrDatos = Resultado.CreateDataReader();
            bAcuerdoRevizado = false;
            lngIdAuto = 0;
            while (dtrDatos.Read())
            {
                _strFechaAuto = DateTime.Parse(dtrDatos["FechaAcue"].ToString()).ToString("dd/MM/yyyy");
                _strAuto = dtrDatos["Auto"].ToString();
                _strResumenAuto = dtrDatos["Resumen"].ToString();
                lngIdAuto = long.Parse(dtrDatos["IdAuto"].ToString());
                if (dtrDatos["Revisado"].ToString() == "ACUERDO REVISADO")
                    bAcuerdoRevizado = true;
                else
                    bAcuerdoRevizado = false;
            }
            return Resultado;
        }

        //Método para cargar los datos de la notificación
        public DataTable CargarDescripcionNotificacion()
        {
            DataTable Resultado = new DataTable();
            if (intOpcion == 4)
            {

            }

            else
            {

                strSQL = "select nope_estatus_noti as IdNotificacion," +
                   "nope_vafi_fecha_noti as FechaNotificacion,nope_traslado as Traslado from notificacion_personaje " +
                   "where nope_vafi_id = " + IdFirmaSeleccionada + " and nope_pers_folio = '" + strpersfolio + "'";
            }
            Resultado = CConexionMySQL.RegresaTabla(strSQL);
            return Resultado;
        }

        //Método para cargar las firmas del acuerdo y mostrarlas en el listview
        public DataTable CargarFirmasAcuerdo(long IdValidaFirma)
        {
            DataTable Resultado = new DataTable();

            strSQL = "call proc_CargarFirmasAcuerdo (" + IdValidaFirma + ");";
            Resultado = CConexionMySQL.RegresaTabla(strSQL);
            return Resultado;
        }

        //Método para obtener los acuerdos a notificar
        public DataTable ObtenerAcuerdosANotificar()
        {
            DataTable Resultado = new DataTable();
            DataTable dtAux = new DataTable();

            strSQL = "call proc_ObtenerAcuerdosANotificarPrueba('" + intOpcion.ToString() + "')";
            CConexionMySQL.Conectar();
            Resultado = CConexionMySQL.RegresaTabla(strSQL);

            //En caso de que quieran filtrar, clonar la tabla
            if (strNumeroExpeBusqueda.Length > 0)
            {
                dtAux = Resultado.Clone();
                dtAux.Rows.Clear();

                var regFiltrados = from cDatosO in Resultado.AsEnumerable()                                   
                                   where cDatosO["Numeroexpe"].ToString() == strNumeroExpeBusqueda
                                   select cDatosO;

                foreach (var Registro in regFiltrados)
                {
                    DataRow dtr_Nuevo = dtAux.NewRow();
                    int Columnas = dtAux.Columns.Count;
                    
                    for (int i = 0; i < Columnas; i++)
                    {
                        dtr_Nuevo[i] = Registro[i].ToString();
                    }
                    dtAux.Rows.Add(dtr_Nuevo);
                }
            }

            //Regresar el resultado
            if (strNumeroExpeBusqueda.Length > 0)
                return dtAux;
            else
                return Resultado;
        }

        public bool GenerarEsquemaNotificacion(long IdFirma)
        {
            DataSet Resultado = new DataSet("Generales");
            bool bResultado = false;
            //Firma.strIP = "10.11.1.20";   //"10.1.1.74";
            //Firma.strIP = "189.254.239.135";
            CargarValoresIP();
            Firma.strIP = strServidorIP;
            Firma.intPuerto = Int32.Parse(strPuertoIP);

            //Buscar las firmas
            strSQL = "call proc_ObtenerFirmasNotificar(" + IdFirma +");";
            DataTable DTFirmas = CConexionMySQL.RegresaTabla(strSQL);
            DTFirmas.TableName = "Firmas";
            Resultado.Tables.Add(DTFirmas);

            string _NombresNotificar = "";

            //Buscar los nombres a notificar
            strSQL = "select func_ObtenerNombresNotificar(" + IdFirma + ", " + strpersfolio + ") as Nombres";
            DataTableReader DTRNombresPersonajes = CConexionMySQL.RegresaComando(strSQL);
            while (DTRNombresPersonajes.Read())
            {
                _NombresNotificar = DTRNombresPersonajes[0].ToString();
            }

            //Buscar los personajes para notificar
            strSQL = "call proc_ObtenerPersonajesNotificar(" + IdFirma + ", " + strpersfolio + ")";
            DataTable DTPersonajesAccesos = CConexionMySQL.RegresaTabla(strSQL);
            DTPersonajesAccesos.TableName = "PersonajesAccesos";
            Resultado.Tables.Add(DTPersonajesAccesos);

            //Obtener las partes del expediente
            string _strActor = "";
            string _strDemandado = "";
            strSQL = "call proc_ObtenerPartes(" + strTipoExpe + ",'" + strNumeroExpe + "');";
            DataTableReader dtrPartes = CConexionMySQL.RegresaComando(strSQL);
            while (dtrPartes.Read())
            {
                _DescripcionExpediente = dtrPartes["Descripcion"].ToString();
                if (dtrPartes["Actores"].ToString().Length > 0)
                {
                    if (dtrPartes["Actores"].ToString().EndsWith(",") == true)
                        _strActor = dtrPartes["Actores"].ToString().Substring(0, dtrPartes["Actores"].ToString().Length - 1);
                    else
                        _strActor = dtrPartes["Actores"].ToString();
                }
                else
                    _strActor = "";

                if (dtrPartes["Demandados"].ToString().Length > 0)
                {
                    if (dtrPartes["Demandados"].ToString().EndsWith(",") == true)
                        _strDemandado = dtrPartes["Demandados"].ToString().Substring(0, dtrPartes["Demandados"].ToString().Length - 1);
                    else
                        _strDemandado = dtrPartes["Demandados"].ToString();
                }
                else
                    _strDemandado = "";
            }

            //Crear la tabla para los generales
            DataTable Generales = new DataTable();
            Generales.Columns.Add("Centro");
            Generales.Columns.Add("Tipo_expe");
            Generales.Columns.Add("Numero_expe");
            Generales.Columns.Add("Descripcion_expe");
            Generales.Columns.Add("Fecha_Auto");
            Generales.Columns.Add("Auto");
            Generales.Columns.Add("Resumen_Auto");
            Generales.Columns.Add("Municipio");
            Generales.Columns.Add("NombreJuzgado");
            Generales.Columns.Add("Actores");
            Generales.Columns.Add("Demandados");
            Generales.Columns.Add("ThumbPrint");
            Generales.Columns.Add("Notificables");
            Generales.Columns.Add("Personaje");
            Generales.Columns.Add("SecretarioNotificacion");
            //NUEVA MEJORA
            CargarValorNotificacionEnvio();

            DataRow RegistrosGeneral = Generales.NewRow();

            RegistrosGeneral["Centro"] = strCentro;
            RegistrosGeneral["Tipo_expe"] = strTipoExpe;
            RegistrosGeneral["Numero_expe"] = strNumeroExpe;
            RegistrosGeneral["Descripcion_expe"] = _DescripcionExpediente; 
            RegistrosGeneral["Fecha_Auto"] = _strFechaAuto;
            RegistrosGeneral["Auto"] = _strAuto;
            RegistrosGeneral["Resumen_Auto"] = _strResumenAuto;
            RegistrosGeneral["Municipio"] = strMunicipio;
            RegistrosGeneral["NombreJuzgado"] = strNombreJuzgado;
            RegistrosGeneral["Actores"] = _strActor;
            RegistrosGeneral["Demandados"] = _strDemandado;
            RegistrosGeneral["ThumbPrint"] = FirmaSeleccionada.ItemData;
            RegistrosGeneral["Notificables"] = _NombresNotificar;
            RegistrosGeneral["Personaje"] = strpersfolio;
            RegistrosGeneral["SecretarioNotificacion"] = strNombreEnviaNot;

            Generales.Rows.Add(RegistrosGeneral);
            Generales.TableName = "GeneralesJuzgado";

            Resultado.Tables.Add(Generales);
            Resultado.WriteXml(strRuta + "\\firmaele\\enviar.xml");

            //Crear la firma y autenticar el xml que se envia
            bResultado = true;
            Firma.Firma = FirmaSeleccionada;
            Firma.strDigestion = Firma.ObtenerSHA1Archivo(strRuta + "\\firmaele\\enviar.xml");                
            Firma.RealizarFirmaTextoConContenido(Firma.strDigestion);
            Firma FirmaMensaje = new Firma();
            if (Firma.AutenticarFirma() != 0)
            {
                bResultado = true;               
                FirmaMensaje.IdentificadorSecuencia = Firma.IdSecuencia;
            }
            else
                bResultado = false;

            if (bResultado == true)
            {

                EstadoLogeo edo = ne.IniciarSesion(cue);

                neu.Expediente = new Expedientes();
                neu.Expediente.NumeroExpediente = strNumeroExpe;
                neu.Expediente.TipoExpediente = int.Parse(strTipoExpe);
                neu.Expediente.CentroTrabajo = new Centros();
                neu.Expediente.CentroTrabajo.Clave = strCentro;

                neu.Notificacion = new NotificacionUploader();
                neu.Notificacion.IdentificadorSecuencia =  Firma.IdSecuencia; //Id del esquema que se esta firmando
                neu.Notificacion.FechaNotificacion = DateTime.Now;
                neu.Notificacion.Esquema = Resultado.GetXml();

                neu.Notificacion.Documento = new ElementoMediaUploader();
                neu.Notificacion.Documento.Centro = new Centros();
                neu.Notificacion.Documento.Centro.Clave = strCentro;
                neu.Notificacion.Documento.Documento = System.IO.File.ReadAllBytes(strRuta + "\\firmaele\\Texto.pdf");
                neu.Notificacion.Documento.NombreOriginal = "Texto.pdf";
                neu.Notificacion.Documento.Tipo = new TiposMedia();
                neu.Notificacion.Documento.Tipo.Identificador = 1;
                neu.Notificacion.Documento.Digestion = Firma.ObtenerSHA1Archivo(strRuta + "\\firmaele\\Texto.pdf"); ;


                int elemento;
                elemento = listaadj.Items.Count;
                neu.Notificacion.Traslado = new NotificacionElectronica.ElementoMediaUploader[elemento];
                string dato;
                for (int Indice = 0; Indice < listaadj.Items.Count; Indice++)
                {


                    dato = listaadj.Items[Indice].SubItems[0].Text;

                    neu.Notificacion.Traslado[Indice] = new NotificacionElectronica.ElementoMediaUploader();
                    neu.Notificacion.Traslado[Indice].Centro = new NotificacionElectronica.Centros();
                    neu.Notificacion.Traslado[Indice].Centro.Clave = strCentro;
                    neu.Notificacion.Traslado[Indice].Documento = System.IO.File.ReadAllBytes(dato);
                    neu.Notificacion.Traslado[Indice].NombreOriginal = "Traslado.pdf";
                    neu.Notificacion.Traslado[Indice].Tipo = new NotificacionElectronica.TiposMedia();
                    neu.Notificacion.Traslado[Indice].Tipo.Identificador = 1;
                    neu.Notificacion.Traslado[Indice].Digestion = Firma.ObtenerSHA1Archivo(dato);
                    neu.Notificacion.Traslado[Indice].Firmas = new NotificacionElectronica.Firma[0];
                }
               

                //Llenar las firmas
                List<Firma> ListaFirmas = new List<NotificacionElectronica.Firma>();
                DataTableReader DTRFirmas = DTFirmas.CreateDataReader();
                while (DTRFirmas.Read())
                {
                    Firma NuevaFirma = new Firma();
                    NuevaFirma.IdentificadorSecuencia = long.Parse(DTRFirmas["IdSecuencia"].ToString());
                    ListaFirmas.Add(NuevaFirma);
                }
                //Agregar la firma del mensaje
                ListaFirmas.Add(FirmaMensaje);
                neu.Notificacion.Documento.Firmas = ListaFirmas.ToArray();

                //Llenar los suscriptores
                List<NotificacionElectronica.Suscriptores> ListaSuscriptores = new List<NotificacionElectronica.Suscriptores>();
                DataTableReader DTRSuscriptores = DTPersonajesAccesos.CreateDataReader();
                while (DTRSuscriptores.Read())
                {
                    NotificacionElectronica.Suscriptores NuevoSuscriptor = new NotificacionElectronica.Suscriptores();
                    NuevoSuscriptor.IdentificadorBuzon = long.Parse(DTRSuscriptores["IdPersonaje"].ToString());
                    // NuevoSuscriptor.IdentificadorBuzon = 11;
                    ListaSuscriptores.Add(NuevoSuscriptor);
                }
                neu.Suscriptores = ListaSuscriptores.ToArray();
               
                veri.ClaveCentro = edo.TicketSesion.Centro.Clave;
                veri.ClavePlataforma = edo.TicketSesion.Plataforma.ClavePlataforma;
                veri.TOKEN = edo.SesionInformacion.Token;
               // NotificacionElectronica.NotificacionElectronicaInformacion mi= ne.RealizarNotificacion(veri,neu);

                //Suscriptores.Expedientes expediente = new Suscriptores.Expedientes();

                clsConexionAPI conexioen = new clsConexionAPI();
                UsuarioExMin auth = new UsuarioExMin()
                {
                    Usuario = "GEMAFV85",
                    Password = Md5Hash("123")
                };
                auth = await conexioen.Autenticar(auth);
                ReqRealizarNotificacion objetoenvio = new ReqRealizarNotificacion();

                objetoenvio.Clave = long.Parse(veri.ClaveCentro);
                objetoenvio.Credencial = 1;
                objetoenvio.Notificacion = neu;

                NotificacionElectronicaInformacion mi = await conexioen.RealizarNotificacion(objetoenvio, auth.Token);


                //expediente.CentroTrabajo = new Suscriptores.Centros();
                //expediente.CentroTrabajo.Clave = strindice;
                //expediente.NumeroExpediente = strNumeroExpe;
                //expediente.TipoExpediente = int.Parse(strTipoExpe);
                //veriR.ClaveCentro = veri.ClaveCentro;
                //veriR.ClavePlataforma = veri.ClavePlataforma;
                //veriR.TOKEN = veri.TOKEN;



                //Actualizar valida_firma cuando la notificación es correcta
                if (mi.Notificacion.Identificador > 0)
                {
                    if (mi.Notificacion.Identificador > 0 && corrertras.Checked == true)
                    {
                        strURL = "";
                        strURL = mi.Notificacion.Recibo.URL;
                        bResultado = true;
                        strSQL = "update notificacion_personaje set notificacion_personaje.nope_estatus_noti = " + mi.Notificacion.Identificador +
                            ",notificacion_personaje.nope_vafi_fecha_noti='" + DateTime.Now.ToString("yyyy/MM/dd") + "'" +
                            ",notificacion_personaje.nope_traslado= 'SI' where notificacion_personaje.nope_vafi_id = " + IdFirma + " and notificacion_personaje.nope_pers_folio = '" + strpersfolio + "'";
                        //strSQL = "update notificacion_personaje set notificacion_personaje.nope_estatus_noti = " + mi.Notificacion.Identificador +
                        //     ",notificacion_personaje.nope_vafi_fecha_noti='" + DateTime.Now.ToString("yyyy/MM/dd") + "'" +
                        //     "notificacion_personaje.nope_vafi_fecha_noti= 'SI' where notificacion_personaje.nope_vafi_id = " + IdFirma + " and notificacion_personaje.nope_pers_folio = '" + strpersfolio + "'";



                        if (CConexionMySQL.EjecutaComando(strSQL) == true)
                            bResultado = true;
                        else
                            bResultado = false;
                    }
                    else
                    {

                        strURL = "";
                        strURL = mi.Notificacion.Recibo.URL;
                        bResultado = true;
                        strSQL = "update notificacion_personaje set notificacion_personaje.nope_estatus_noti = " + mi.Notificacion.Identificador +
                            ",notificacion_personaje.nope_vafi_fecha_noti='" + DateTime.Now.ToString("yyyy/MM/dd") + "'" +
                            " where notificacion_personaje.nope_vafi_id = " + IdFirma + " and notificacion_personaje.nope_pers_folio = '" + strpersfolio + "'";
                        //strSQL = "update notificacion_personaje set notificacion_personaje.nope_estatus_noti = " + mi.Notificacion.Identificador +
                        //     ",notificacion_personaje.nope_vafi_fecha_noti='" + DateTime.Now.ToString("yyyy/MM/dd") + "'" +
                        //     "notificacion_personaje.nope_vafi_fecha_noti= 'SI' where notificacion_personaje.nope_vafi_id = " + IdFirma + " and notificacion_personaje.nope_pers_folio = '" + strpersfolio + "'";

                        if (CConexionMySQL.EjecutaComando(strSQL) == true)
                            bResultado = true;
                        else
                            bResultado = false;

                    }
                }
                else
                    bResultado = false;
            }
            return bResultado;
        }

        public bool GenerarEsquemaNotificacion2(long IdFirma, X509Certificate2 cert)
        {

            DataSet Resultado = new DataSet("Generales");
            bool bResultado = false;
            string _firma = "";
            string certificado = "";
            string thrumb = "";
            myListObj Elemento2 = new myListObj();
            CmsSigner objSigner = new CmsSigner(cert);

            CargarValoresIP();
            Firma.strIP = strServidorIP;
            Firma.intPuerto = Int32.Parse(strPuertoIP);

            //Buscar las firmas
            strSQL = "call proc_ObtenerFirmasNotificar(" + IdFirma + ");";
            DataTable DTFirmas = CConexionMySQL.RegresaTabla(strSQL);
            DTFirmas.TableName = "Firmas";
            Resultado.Tables.Add(DTFirmas);

            string _NombresNotificar = "";

            //Buscar los nombres a notificar
            //mejora firma
            strSQL = "select func_ObtenerNombresNotificar(" + IdFirma + ", " + strpersfolio + ") as Nombres";
            DataTableReader DTRNombresPersonajes = CConexionMySQL.RegresaComando(strSQL);
            while (DTRNombresPersonajes.Read())
            {
                _NombresNotificar = DTRNombresPersonajes[0].ToString();

            }

            //Buscar los personajes para notificar
            strSQL = "call proc_ObtenerPersonajesNotificar(" + IdFirma + ", " + strpersfolio + ")";
            DataTable DTPersonajesAccesos = CConexionMySQL.RegresaTabla(strSQL);
            DTPersonajesAccesos.TableName = "PersonajesAccesos";
            Resultado.Tables.Add(DTPersonajesAccesos);

            //Obtener las partes del expediente
            string _strActor = "";
            string _strDemandado = "";
            strSQL = "call proc_ObtenerPartes(" + strTipoExpe + ",'" + strNumeroExpe + "');";
            DataTableReader dtrPartes = CConexionMySQL.RegresaComando(strSQL);
            while (dtrPartes.Read())
            {
                _DescripcionExpediente = dtrPartes["Descripcion"].ToString();
                if (dtrPartes["Actores"].ToString().Length > 0)
                {
                    if (dtrPartes["Actores"].ToString().EndsWith(",") == true)
                        _strActor = dtrPartes["Actores"].ToString().Substring(0, dtrPartes["Actores"].ToString().Length - 1);
                    else
                        _strActor = dtrPartes["Actores"].ToString();
                }
                else
                    _strActor = "";

                if (dtrPartes["Demandados"].ToString().Length > 0)
                {
                    if (dtrPartes["Demandados"].ToString().EndsWith(",") == true)
                        _strDemandado = dtrPartes["Demandados"].ToString().Substring(0, dtrPartes["Demandados"].ToString().Length - 1);
                    else
                        _strDemandado = dtrPartes["Demandados"].ToString();
                }
                else
                    _strDemandado = "";
            }

            ContentInfo objContent2 = new ContentInfo(Encoding.ASCII.GetBytes(""));
            //Creamos el objeto que representa los datos firmados
            SignedCms objSignedData2 = new SignedCms(objContent2);
            //Creamos el "firmante"
            CmsSigner objSigner2 = new CmsSigner(cert);
            // Firmamos los datos
            //if (System.Configuration.ConfigurationManager.ConnectionStrings["SHA"].ConnectionString == "256")

            objSigner2.DigestAlgorithm = new Oid(CryptoConfig.MapNameToOID("SHA1"));
            objSigner2.IncludeOption = X509IncludeOption.EndCertOnly;
            //  objSignedData2.ComputeSignature(objSigner2);
            //Obtenemos el resultado
            thrumb = objSigner2.Certificate.Thumbprint.ToString();


            //Crear la tabla para los generales
            DataTable Generales = new DataTable();
            Generales.Columns.Add("Centro");
            Generales.Columns.Add("Tipo_expe");
            Generales.Columns.Add("Numero_expe");
            Generales.Columns.Add("Descripcion_expe");
            Generales.Columns.Add("Fecha_Auto");
            Generales.Columns.Add("Auto");
            Generales.Columns.Add("Resumen_Auto");
            Generales.Columns.Add("Municipio");
            Generales.Columns.Add("NombreJuzgado");
            Generales.Columns.Add("Actores");
            Generales.Columns.Add("Demandados");
            Generales.Columns.Add("ThumbPrint");
            Generales.Columns.Add("Notificables");
            Generales.Columns.Add("Personaje");
            Generales.Columns.Add("Número_Único_de_Suscriptor");
            Generales.Columns.Add("folio");
            Generales.Columns.Add("Indice");
            Generales.Columns.Add("NumeroExped");
            Generales.Columns.Add("Tipoexped");
            Generales.Columns.Add("SecretarioNotificacion");
            //NUEVA MEJORA
            CargarValorNotificacionEnvio();
            DataRow RegistrosGeneral = Generales.NewRow();

            RegistrosGeneral["Centro"] = strCentro;
            RegistrosGeneral["Tipo_expe"] = strTipoExpe;
            RegistrosGeneral["Numero_expe"] = strNumeroExpe;
            RegistrosGeneral["Descripcion_expe"] = _DescripcionExpediente;
            RegistrosGeneral["Fecha_Auto"] = _strFechaAuto;
            RegistrosGeneral["Auto"] = _strAuto;
            RegistrosGeneral["Resumen_Auto"] = _strResumenAuto;
            RegistrosGeneral["Municipio"] = strMunicipio;
            RegistrosGeneral["NombreJuzgado"] = strNombreJuzgado;
            RegistrosGeneral["Actores"] = _strActor;
            RegistrosGeneral["Demandados"] = _strDemandado;
            RegistrosGeneral["ThumbPrint"] = thrumb;
            RegistrosGeneral["Notificables"] = _NombresNotificar;
            RegistrosGeneral["Personaje"] = strpersfolio;
            RegistrosGeneral["Número_Único_de_Suscriptor"] = strbuzon;
            RegistrosGeneral["folio"] = strfolio;
            RegistrosGeneral["Indice"] = strindice;
            RegistrosGeneral["NumeroExped"] = strNumeroexpeR;
            RegistrosGeneral["Tipoexped"] = strtipoexpeR;
            RegistrosGeneral["SecretarioNotificacion"] = strNombreEnviaNot;


            Generales.Rows.Add(RegistrosGeneral);
            Generales.TableName = "GeneralesJuzgado";

            Resultado.Tables.Add(Generales);
            Resultado.WriteXml(strRuta + "\\firmaele\\enviar.xml");

            //Crear la firma y autenticar el xml que se envia
            bResultado = true;
            Firma.strDigestion = Firma.ObtenerSHA1Archivo(strRuta + "\\firmaele\\enviar.xml");

            ContentInfo objContent = new ContentInfo(Encoding.ASCII.GetBytes(Firma.strDigestion));
            //Creamos el objeto que representa los datos firmados
            SignedCms objSignedData = new SignedCms(objContent);
            //Creamos el "firmante"

            // Firmamos los datos
            //if (System.Configuration.ConfigurationManager.ConnectionStrings["SHA"].ConnectionString == "256")

            objSigner.DigestAlgorithm = new Oid(CryptoConfig.MapNameToOID("SHA1"));
            objSigner.IncludeOption = X509IncludeOption.EndCertOnly;
            objSignedData.ComputeSignature(objSigner);
            //Obtenemos el resultado
            byte[] bytSigned = objSignedData.Encode();
            _firma = Convert.ToBase64String(bytSigned);
            String[] detalle = null;
            detalle = objSigner.Certificate.SubjectName.Name.Split(",".ToCharArray());
            certificado = objSigner.Certificate.FriendlyName.ToString();
            thrumb = objSigner.Certificate.Thumbprint.ToString();

            List<object> Resultado2 = new List<object>();
            myListObj Elemento = new myListObj(certificado + "'s ", thrumb);
            Elemento2 = Elemento;
            Resultado2.Add(Elemento);
            Firma.Firma = Elemento2;

            Firma.RealizarFirmaTextoConContenido2(_firma);
            Firma FirmaMensaje = new Firma();


            int respuestaFirm = Firma.AutenticarFirma();

            if (respuestaFirm == 0)
            {

                //PARA QUE FIRME CON SHA256
                objSigner.DigestAlgorithm = new Oid(CryptoConfig.MapNameToOID("SHA256"));
                CargarValoresIP2();
                Firma.strIP = strServidorIP;
                Firma.intPuerto = Int32.Parse(strPuertoIP);
                respuestaFirm = Firma.AutenticarFirma();

            }

            if (respuestaFirm != 0)
            {
                bResultado = true;
                FirmaMensaje.IdentificadorSecuencia = Firma.IdSecuencia;
            }
            else
                bResultado = false;

            if (bResultado == true)
            {

                EstadoLogeo edo = ne.IniciarSesion(cue);

                neu.Expediente = new Expedientes();
                neu.Expediente.NumeroExpediente = strNumeroExpe;
                neu.Expediente.TipoExpediente = int.Parse(strTipoExpe);
                neu.Expediente.CentroTrabajo = new Centros();
                neu.Expediente.CentroTrabajo.Clave = strCentro;

                neu.Notificacion = new NotificacionUploader();
                neu.Notificacion.IdentificadorSecuencia = Firma.IdSecuencia; //Id del esquema que se esta firmando
                neu.Notificacion.FechaNotificacion = DateTime.Now;
                neu.Notificacion.Esquema = Resultado.GetXml();

                neu.Notificacion.Documento = new ElementoMediaUploader();
                neu.Notificacion.Documento.Centro = new Centros();
                neu.Notificacion.Documento.Centro.Clave = strCentro;
                neu.Notificacion.Documento.Documento = System.IO.File.ReadAllBytes(strRuta + "\\firmaele\\Texto.pdf");
                neu.Notificacion.Documento.NombreOriginal = "Texto.pdf";
                neu.Notificacion.Documento.Tipo = new TiposMedia();
                neu.Notificacion.Documento.Tipo.Identificador = 1;
                neu.Notificacion.Documento.Digestion = Firma.ObtenerSHA1Archivo(strRuta + "\\firmaele\\Texto.pdf"); ;


                //Llenar las firmas
                List<Firma> ListaFirmas = new List<NotificacionElectronica.Firma>();
                DataTableReader DTRFirmas = DTFirmas.CreateDataReader();
                while (DTRFirmas.Read())
                {
                    Firma NuevaFirma = new Firma();
                    NuevaFirma.IdentificadorSecuencia = long.Parse(DTRFirmas["IdSecuencia"].ToString());
                    ListaFirmas.Add(NuevaFirma);
                }
                //Agregar la firma del mensaje
                ListaFirmas.Add(FirmaMensaje);
                neu.Notificacion.Documento.Firmas = ListaFirmas.ToArray();

                //Llenar los suscriptores
                List<NotificacionElectronica.Suscriptores> ListaSuscriptores = new List<NotificacionElectronica.Suscriptores>();
                DataTableReader DTRSuscriptores = DTPersonajesAccesos.CreateDataReader();
                while (DTRSuscriptores.Read())
                {
                    NotificacionElectronica.Suscriptores NuevoSuscriptor = new NotificacionElectronica.Suscriptores();
                    NuevoSuscriptor.IdentificadorBuzon = long.Parse(DTRSuscriptores["IdPersonaje"].ToString());
                    // NuevoSuscriptor.IdentificadorBuzon = 11;
                    ListaSuscriptores.Add(NuevoSuscriptor);
                }
                neu.Suscriptores = ListaSuscriptores.ToArray();

                veri.ClaveCentro = edo.TicketSesion.Centro.Clave;
                veri.ClavePlataforma = edo.TicketSesion.Plataforma.ClavePlataforma;
                veri.TOKEN = edo.SesionInformacion.Token;
             //   NotificacionElectronica.NotificacionElectronicaInformacion mi = ne.RealizarNotificacion(veri, neu);

                clsConexionAPI conexioen = new clsConexionAPI();
                UsuarioExMin auth = new UsuarioExMin()
                {
                    Usuario = "GEMAFV85",
                    Password = Md5Hash("123")
                };
                auth = await conexioen.Autenticar(auth);
                ReqRealizarNotificacion objetoenvio = new ReqRealizarNotificacion();

                objetoenvio.Clave = long.Parse(veri.ClaveCentro);
                objetoenvio.Credencial = 1;
                objetoenvio.Notificacion = neu;

                NotificacionElectronicaInformacion mi = await conexioen.RealizarNotificacion(objetoenvio, auth.Token);



                //Actualizar valida_firma cuando la notificación es correcta
                if (mi.Notificacion.Identificador > 0)
                {


                    strURL = "";
                    strURL = mi.Notificacion.Recibo.URL;
                    bResultado = true;
                    strSQL = "update notificacion_personaje set notificacion_personaje.nope_estatus_noti = " + mi.Notificacion.Identificador +
                        ",notificacion_personaje.nope_vafi_fecha_noti='" + DateTime.Now.ToString("yyyy/MM/dd") + "'" +
                        " where notificacion_personaje.nope_vafi_id = " + IdFirma + " and notificacion_personaje.nope_pers_folio = '" + strpersfolio + "'";
                    //strSQL = "update notificacion_personaje set notificacion_personaje.nope_estatus_noti = " + mi.Notificacion.Identificador +
                    //     ",notificacion_personaje.nope_vafi_fecha_noti='" + DateTime.Now.ToString("yyyy/MM/dd") + "'" +
                    //     "notificacion_personaje.nope_vafi_fecha_noti= 'SI' where notificacion_personaje.nope_vafi_id = " + IdFirma + " and notificacion_personaje.nope_pers_folio = '" + strpersfolio + "'";

                    if (CConexionMySQL.EjecutaComando(strSQL) == true)
                        bResultado = true;
                    else
                        bResultado = false;

                }
                else
                    bResultado = false;
            }
            return bResultado;
        }

        public string RealizarFirmaCertificado(X509Certificate2 cer)
        {
            int tiposerver = 0;
            string Resultado = "";
            string _firma = "";
            string certificado = "";
            string thrumb = "";
            myListObj Elemento2 = new myListObj();
            CargarValoresIP();
            Firma.strIP = strServidorIP;
            Firma.intPuerto = Int32.Parse(strPuertoIP);
            CmsSigner objSigner = new CmsSigner(cer);

            if (TotalFirmas > 1)
            {
                if (Firma.ValidarArchivoHASH(strRuta + "\\firmaele\\Texto.pdf", HashOriginal) == true)
                {
                    Firma.strDigestion = Firma.ObtenerSHA1Archivo(strRuta + "\\firmaele\\Texto.pdf");
                    //Creamos el ContentInfo
                    ContentInfo objContent = new ContentInfo(Encoding.ASCII.GetBytes(Firma.strDigestion));
                    //Creamos el objeto que representa los datos firmados
                    SignedCms objSignedData = new SignedCms(objContent);
                    //Creamos el "firmante"

                    // Firmamos los datos
                    //if (System.Configuration.ConfigurationManager.ConnectionStrings["SHA"].ConnectionString == "256")

                    objSigner.DigestAlgorithm = new Oid(CryptoConfig.MapNameToOID("SHA1"));
                    objSigner.IncludeOption = X509IncludeOption.EndCertOnly;
                    objSignedData.ComputeSignature(objSigner);
                    //Obtenemos el resultado
                    byte[] bytSigned = objSignedData.Encode();
                    _firma = Convert.ToBase64String(bytSigned);
                    String[] detalle = null;
                    detalle = objSigner.Certificate.SubjectName.Name.Split(",".ToCharArray());
                    certificado = objSigner.Certificate.FriendlyName.ToString();
                    thrumb = objSigner.Certificate.Thumbprint.ToString();

                    List<object> Resultado2 = new List<object>();
                    myListObj Elemento = new myListObj(certificado + "'s ", thrumb);
                    Elemento2 = Elemento;
                    Resultado2.Add(Elemento);
                    Firma.RealizarFirmaTextoConContenido2(_firma);
                }
                else
                {
                    Resultado = "El documento que se intenta firmar ha sido modificado. \nMotivo : la comparación de la huella digital con el documento original no coincide.";
                    FirmaCorrecta = false;
                    return Resultado;
                }
            }
            else
            {
                Firma.strDigestion = Firma.ObtenerSHA1Archivo(strRuta + "\\firmaele\\Texto.pdf");
                //Creamos el ContentInfo
                ContentInfo objContent = new ContentInfo(Encoding.ASCII.GetBytes(Firma.strDigestion));
                //Creamos el objeto que representa los datos firmados
                SignedCms objSignedData = new SignedCms(objContent);
                //Creamos el "firmante"
                // Firmamos los datos

                objSigner.DigestAlgorithm = new Oid(CryptoConfig.MapNameToOID("SHA1"));
                objSigner.IncludeOption = X509IncludeOption.EndCertOnly;
                objSignedData.ComputeSignature(objSigner);
                //Obtenemos el resultado
                byte[] bytSigned = objSignedData.Encode();
                _firma = Convert.ToBase64String(bytSigned);
                String[] detalle = null;
                detalle = objSigner.Certificate.SubjectName.Name.Split(",".ToCharArray());
                certificado = objSigner.Certificate.FriendlyName.ToString();
                thrumb = objSigner.Certificate.Thumbprint.ToString();

                List<object> Resultado2 = new List<object>();
                myListObj Elemento = new myListObj(certificado + "'s ", thrumb);
                Elemento2 = Elemento;
                Resultado2.Add(Elemento);
                Firma.RealizarFirmaTextoConContenido2(_firma);
            }
            Firma.Firma = Elemento2;


            int Resultadofirm = Firma.AutenticarFirma();
            tiposerver = 1;

            if (Resultadofirm == 0)
            {
                //PARA QUE FIRME CON SHA256
                objSigner.DigestAlgorithm = new Oid(CryptoConfig.MapNameToOID("SHA256"));
                CargarValoresIP2();
                Firma.strIP = strServidorIP;
                Firma.intPuerto = Int32.Parse(strPuertoIP);
                Resultadofirm = Firma.AutenticarFirma();
                tiposerver = 2;
            }

            if (Resultadofirm != 0)
            {
                Resultado = "Firma electrónica avanzada realizada correctamente.";
                if (AgregarFirmaAceptada(Firma.strDigestion, Firma.IdSecuencia, Firma.strTSP, Firma.strFIR, tiposerver) == true)
                {
                    FirmaCorrecta = true;
                }
                else
                    FirmaCorrecta = false;
            }
            else
            {


                if (Firma.errMsg.Length == 0)
                    strError = "Cancelación por parte del usuario";
                Resultado = "Error en la firma, el error fue : " + Firma.errMsg;
                FirmaCorrecta = false;
            }

            return Resultado;
        }


        public bool EnviarRev()
        {
            bool Resultado = true;
            strSQL = "update revoca set revoca_status = " + 1 + " " +
                         " where revoca_buzon = " + strbuzon + " and revoca_nombre = '" + strpersfolio + "' and  revoca_folio_pers = '" + strpersfolioN + "' and revoca_Numero_expe = '" + strNumeroexpeR + "' and revoca_tipo_expe='" + strtipoexpeR + "'  ";

            CConexionMySQL.EjecutaComando(strSQL);


            return Resultado;

        }

        //Método para obtener el texto resolutivo
        public bool AbrirTextoResolutivo(int Opcion)
        {
            bool bResultado = true;
            strSQL = "select texto_acue.txta_texto from texto_acue where " +
               "texto_acue.txta_tipo_expe = " + strTipoExpe + " and " +
               "texto_acue.txta_numero_expe = '" + strNumeroExpe + "' and " +
               "texto_acue.txta_folio_acue = " + strFolioMovi + " and " +
               "texto_acue.txta_tipo = '" + strTipoMovi + "'";

            DataTableReader Resultado = CConexionMySQL.RegresaComando(strSQL);

            string _Texto = "";
            while (Resultado.Read())
            {
                _Texto = Resultado[0].ToString();
            }

            if (_Texto.IndexOf("{\\rtf1") >= 0)
            {
                if (System.IO.File.Exists(strRuta + "\\firmaele\\Texto.rtf"))
                    System.IO.File.Delete(strRuta + "\\firmaele\\Texto.rtf");

                if (System.IO.File.Exists(strRuta + "\\firmaele\\Texto.pdf"))
                    System.IO.File.Delete(strRuta + "\\firmaele\\Texto.pdf");

                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(strRuta + "\\firmaele\\Texto.rtf");

                //Write a line of text
                sw.Write(_Texto);

                //Close the file
                sw.Close();

                //Opcion 0 => Solo crea el pdf
                //Opcion 1 0> Crea y abre el pdf
                if (_Texto.Length > 0)
                {
                    Firma.ConvertirRTF_PDF(strRuta + "\\firmaele\\Texto.rtf");

                    if (Opcion == 1)
                    {
                        Process pr = new Process();
                        try
                        {
                            pr.StartInfo.FileName = strRuta + "\\firmaele\\Texto.pdf";
                            pr.Start();
                        }
                        catch
                        {
                            bResultado = false;
                        }
                    }
                }
                else
                    bResultado = false;
            }
            else
            {
                //////////////////////////////////////////////////////SE CREA LA CARPETA DEL EXPEDIENTE  DENTRO DE FIRMAELE
                string activeDir = @" " + strRuta + "\\firmaele";
                string ExpedienteFirmaele = strNumeroExpe;
                string ExpedienteMETAMORFOSIS = strNumeroExpe;

                ExpedienteFirmaele = ExpedienteFirmaele.Replace("/", "-").ToString();
                ExpedienteFirmaele = ExpedienteFirmaele + "_archivos";
                //Create a new subfolder under the current active folder
                string newPath = System.IO.Path.Combine(activeDir, ExpedienteFirmaele);

                // Create the subfolder
                System.IO.Directory.CreateDirectory(newPath);
                //SE CREA EL HEADER
                string strfileName = newPath + "\\header";
                FileStream stream = new FileStream(strfileName + ".htm", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
                stream.Close();

                //////////////////////////////////////////////////////SE CREA LA CARPETA DEL EXPEDIENTE DENTRO DE METAMORFOSIS
                string activeDirM = @" " + strRuta + "\\firmaele\\metamorfosis";
                ExpedienteMETAMORFOSIS = ExpedienteMETAMORFOSIS.Replace("/", "-").ToString();
                ExpedienteMETAMORFOSIS = ExpedienteMETAMORFOSIS + "_archivos";
                //Create a new subfolder under the current active folder
                string newPathM = System.IO.Path.Combine(activeDirM, ExpedienteMETAMORFOSIS);

                // Create the subfolder
                System.IO.Directory.CreateDirectory(newPathM);

                string strfileNameM = newPathM + "\\header";
                FileStream streamM = new FileStream(strfileNameM + ".htm", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writerM = new StreamWriter(streamM, System.Text.Encoding.UTF8);
                streamM.Close();

                //Crear el archivo html a rtf
                clsConviertea_RTF CConvierte = new clsConviertea_RTF();

                //Convertir el archivo PDF
                CConvierte.ConvierteTexto(strRuta + "\\firmaele", _Texto);
                //Convertir el archivo a PDF

                //Elimina el archivo texto.pdf
                if (System.IO.File.Exists(strRuta + "\\firmaele\\Texto.pdf"))
                {
                    System.IO.File.Delete(strRuta + "\\firmaele\\Texto.pdf");

                }

                Firma.ConvertirRTF_PDF(strRuta + "\\firmaele\\Texto.rtf");

                //BORRA LAS CARPETAS Y ARCHIVOS CREADOS EN FIRMAELE
                if (System.IO.File.Exists(strfileNameM + ".htm"))
                {
                    System.IO.File.Delete(strfileNameM + ".htm");
                    System.IO.Directory.Delete(newPathM);
                }

                if (System.IO.File.Exists(strfileName + ".htm"))
                {
                    System.IO.File.Delete(strfileName + ".htm");
                    System.IO.Directory.Delete(newPath);
                }
                


                if (Opcion == 1)
                {
                    Process pr = new Process();
                    try
                    {
                        pr.StartInfo.FileName = strRuta + "\\firmaele\\Texto.pdf";
                        pr.Start();
                    }
                    catch
                    {
                        bResultado = false;
                    }
                }
            }

            return bResultado;
        }

        //Método para comparar la firma electrónica de un documento
        public bool CompararFirmaElectronica()
        {
            bool Resultado = true;

            strSQL = "select firm_fir from firmas where firm_vafi_id = " +
                IdFirmaSeleccionada + " and firm_usuario = '" +
                strUsuario + "' and firm_tipo = " +
                intTipoFirma + " and firm_estatus = 1";

            string _Firma = "";

            DataTableReader dtrResultado = CConexionMySQL.RegresaComando(strSQL);
            while (dtrResultado.Read())
            {
                _Firma = dtrResultado[0].ToString();
            }

            //Obtener la firma del acuerdo
            Firma.Firma = FirmaSeleccionada;
            if (AbrirTextoResolutivo(0) == true)
            {
                Firma.strDigestion = Firma.ObtenerSHA1Archivo(strRuta + "\\firmaele\\Texto.pdf");
                Firma.RealizarFirmaTextoConContenido(Firma.strDigestion);

                //if (Firma.strFIR == _Firma)
                    Resultado = true;
                //else
                //    Resultado = false;
            }
            else
                Resultado = false;

            return Resultado;
        }

        //Método para comparar la firma electrónica de un documento en específico
        public bool CompararFirmaElectronica(string _RutaDocumento)
        {
            bool Resultado = true;

            strSQL = "select firm_fir from firmas where firm_vafi_id = " +
                IdFirmaSeleccionada + " and firm_usuario = '" +
                strUsuario + "' and firm_tipo = " +
                intTipoFirma + " and firm_estatus = 1";

            string _Firma = "";

            DataTableReader dtrResultado = CConexionMySQL.RegresaComando(strSQL);
            while (dtrResultado.Read())
            {
                _Firma = dtrResultado[0].ToString();
            }

            //Obtener la firma del acuerdo
            Firma.Firma = FirmaSeleccionada;           
            Firma.strDigestion = Firma.ObtenerSHA1Archivo(_RutaDocumento);
            Firma.RealizarFirmaTextoConContenido(Firma.strDigestion);

            //if (Firma.strFIR == _Firma)
                Resultado = true;
            //else
            //    Resultado = false;            

            return Resultado;
        }

        //Método para revocar la firma
        public bool RevocarFirma(long IdFirma)
        {
            bool Resultado = true;
            strSQL = "call proc_RevocarFirma(" + IdFirma + "," + IdFirmaSeleccionada + "," + intTipoFirma + ");";
            if (CConexionMySQL.EjecutaComando(strSQL) == false)
                Resultado = false;            

            return Resultado;
        }        

        //Método para obtener el recibo
        public bool ObtenerRecibo(long IdNotificacion)
        {                  
            bool Resultado = false;
            NotificacionElectronica.NotificacionElectronicaInformacion NERecibo =  ne.ObtenerNoficacion(veri, IdNotificacion);
            if (NERecibo.Notificacion.Identificador > 0)
            {
                strURL = NERecibo.Notificacion.Recibo.URL;  
                Resultado = true;
            }
            else
            {
                Resultado = false;
            }
            return Resultado;
        }
        //Método para obtener el Acuerdo que se subio a Plataforma
        public bool ObtenerAcuerdoP(long IdNotificacion)
        {
            bool Resultado = false;
            NotificacionElectronica.NotificacionElectronicaInformacion NERecibo = ne.ObtenerNoficacion(veri, IdNotificacion);
            if (NERecibo.Notificacion.Identificador > 0)
            {
                strURL = NERecibo.Notificacion.Documento.URL;
                Resultado = true;
            }
            else
            {
                Resultado = false;
            }
            return Resultado;
        }
        private void CargarValoresIP2()
        {
            DataSet xmlParamentrosIP = new DataSet();
            xmlParamentrosIP.ReadXml(Application.StartupPath + "\\configIP256.xml");


            strRutaIP = strRuta;
            strServidorIP = xmlParamentrosIP.Tables[0].Rows[0]["Ip"].ToString();
            strPuertoIP = xmlParamentrosIP.Tables[0].Rows[0]["Puerto"].ToString();
            xmlParamentrosIP.Dispose();
        }

        //Método para obtener el Traslado que se subio a Plataforma
        public bool ObtenerAcuerdoT(long IdNotificacion)
        {
            string strURL;
            int Trasladok;

            bool Resultado = false;
            NotificacionElectronica.NotificacionElectronicaInformacion NERecibo = ne.ObtenerNoficacion(veri, IdNotificacion);
            if (NERecibo.Notificacion.Identificador > 0)
            {


                Trasladok = NERecibo.Notificacion.Traslado.Length;


                for (int Indice = 0; Indice < Trasladok; Indice++)
                {
                    //ListViewItem List;

                    ListViewItem ListF;



                    strURL = NERecibo.Notificacion.Traslado[Indice].URL;
                    ListF = lisVertraslado.Items.Add("ARCHIVO ADJUNTO" + " " + (Indice + 1));
                    ListF.SubItems.Add(Indice.ToString());
                    lblvertexto.Text = Trasladok.ToString();



                    //foreach (string adjunto in strURL)
                    //{

                    //    //ListViewItem ListF;
                    //    //ListF = lvArchivos.Items.Add(Resultado[0].ToString());
                    //    //ListF.SubItems.Add(Resultado[1].ToString());
                    //    //ListF.SubItems.Add(Resultado[3].ToString());


                    //    ListViewItem List;
                    //    List = lstadjuntar.Items.Add(adjunto);



                    //    conteo = lstadjuntar.Items.Count;
                    //    lblcantidad.Text = conteo.ToString();


                    //    //txtadjuntos.Text += adjunto + ",";
                    //}




                }


                Resultado = true;
            }
            else
            {
                Resultado = false;
            }
            return Resultado;
        }




        public bool ObtenerTraslado(long IdNotificacion)
        {



            bool Resultado = false;
            NotificacionElectronica.NotificacionElectronicaInformacion NERecibo = ne.ObtenerNoficacion(veri, IdNotificacion);
            if (NERecibo.Notificacion.Identificador > 0)
            {
                for (int Indice = 0; Indice < lisVertraslado.Items.Count; Indice++)
                {




                    if (lisVertraslado.Items[Indice].Selected == true)
                    {

                        strURL = NERecibo.Notificacion.Traslado[Indice].URL;
                        Resultado = true;
                    }

                }

            }
            return Resultado;
        }



        //Método para obtener la huella digital de los archivos
        private List<DataRow> ObtenerHashArchivos(string Ruta)
        {
            List<DataRow> ListaHash = new List<DataRow>();
            DirectoryInfo di_Archivos = new DirectoryInfo(Ruta);
            FileInfo[] f_CarpetaArchivos = di_Archivos.GetFiles("*.*");
            foreach (FileInfo f_Archivo in f_CarpetaArchivos)
            {
                DataRow HuellaArchivo = ArchivosHuellaDigital.NewRow();
                HuellaArchivo["NombreArchivo"] = f_Archivo.Name;
                HuellaArchivo["TamanioFisico"] = f_Archivo.Length.ToString();
                HuellaArchivo["FechaArchivo"] = f_Archivo.LastWriteTime.ToString();
                HuellaArchivo["FechaUltimaModificacion"] = f_Archivo.LastAccessTime.ToString();                
                HuellaArchivo["HuellaDigital"] = Firma.ObtenerSHA1Archivo(f_Archivo.FullName);
                ListaHash.Add(HuellaArchivo);
            }
            return ListaHash;
        }

        //Método para recorrer todas las carpetas y obtener la huella digital de los archivos
        private void RecorrerCarpetasFirma(List<DataRow> ListaHuellaDigital, string Ruta)
        { 
            //Obtener la lista de hash de la ruta actual
            List<DataRow> ListaAux = ObtenerHashArchivos(Ruta);
            foreach (DataRow RegistroActual in ListaAux)
            {
                ListaHuellaDigital.Add(RegistroActual);
            }

            string[] Carpetas = Directory.GetDirectories(Ruta);

            foreach (string Carpeta in Carpetas)
            {
                RecorrerCarpetasFirma(ListaHuellaDigital, Carpeta);
            }
        }

        //Método para regresar el resultado de las firmas
        public bool RegresarHashArchivos(string Ruta)
        {
            bool Resultado = true;
            List<DataRow> ListaFirmada = new List<DataRow>();
            try
            {
                RecorrerCarpetasFirma(ListaFirmada, Ruta);
                foreach (DataRow Registro in ListaFirmada)
                {
                    ArchivosHuellaDigital.Rows.Add(Registro);
                }
                ArchivosHuellaDigital.TableName = "HD_Archivos";
            }
            catch(Exception ex)
            {
                strError = ex.Message;
                Resultado = false;
            }

            return Resultado;
        }

        //Método para guardar la firma de los archivos
        public bool GuardarFirmaArchivos(string Esquema,int Opcion)
        {
            strSQL = "call proc_GuardarHuellasArchivos(" +
               Opcion + "," +
               strTipoExpe + ",'" +
               strNumeroExpe + "','" +
               strTipoMovi + "'," +
               strFolioMovi + ",'" +
               Esquema + "','" +
               Digestion + "');";               

            return CConexionMySQL.EjecutaComando(strSQL);     
        }

        //Método para agregar la firma de los archivos
        public bool AgregarFirmaAceptadaArchivos()
        {
            strSQL = "call proc_RealizarGuardadoFirmaArchivos(" +               
                strTipoExpe + ",'" +
                strNumeroExpe + "','" +
                strTipoMovi + "'," +
                strFolioMovi + "," +              
                Secuencia + ",'" +
                Tsp + "','" +
                Fir + "','" +
                strUsuario + "','" +
                strNombre + "','" +
                strNivel + "')";

            return CConexionMySQL.EjecutaComando(strSQL);             
        }

        //Método para cargar los archivos firmados
        public DataSet ObtenerEsquemaArchivos()
        {
            DataSet Resultado = new DataSet();
            strSQL = "select valida_firma.vafi_esquema_arch from valida_firma where valida_firma.vafi_id = " + IdFirmaSeleccionada;
            DataTableReader dtrDatos = CConexionMySQL.RegresaComando(strSQL);
            string xmlDatos = "";
            while (dtrDatos.Read())
            {
                xmlDatos = dtrDatos[0].ToString();
            }
            if (xmlDatos.Length > 0)
            {
                StringReader xmlSR = new StringReader(xmlDatos);
                Resultado.ReadXml(xmlSR);
            }
            return Resultado;
        }

        //Método para guardar el registro de la firma
        private bool AgregarFirmaAceptada(string strDigestion, long lngIdSecuencia, string strTSP, string strFirma, int server)
        {
            strSQL = "call proc_RealizarGuardadoFirma(" +
                 TotalFirmas + "," +
                 strTipoExpe + ",'" +
                 strNumeroExpe + "','" +
                 strTipoMovi + "'," +
                 strFolioMovi + ",'" +
                 strDigestion + "'," +
                 lngIdSecuencia + ",'" +
                 strTSP + "','" +
                 strFirma + "','" +
                 strUsuario + "','" +
                 strNombre + "','" +
                 strNivel + "',0 ,'" +
                 server + "')";
            return CConexionMySQL.EjecutaComando(strSQL);
        }         

        public bool CerrarSesion()
        {
            try
            {                
                return ne.CerrarSesion(veri);
            }
            catch
            {
                return false;
            }
        }

        ~clsAcuerdos()
        {
            if (System.IO.File.Exists(strRuta + "\\firmaele\\Texto.rtf"))
                System.IO.File.Delete(strRuta + "\\firmaele\\Texto.rtf");

            if (System.IO.File.Exists(strRuta + "\\firmaele\\Texto.pdf"))
                System.IO.File.Delete(strRuta + "\\firmaele\\Texto.pdf");
            CConexionMySQL.Dispose();
            CerrarSesion();
        }
    }
}
