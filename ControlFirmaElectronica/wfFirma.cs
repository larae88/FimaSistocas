using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AccesoDatos;
using System.IO;
using System.Runtime.InteropServices;
using System.Net;
using System.Web.Services.Protocols;
using APISeguriSign;
using System.Diagnostics;
using System.Security.Cryptography;
using SeguriSIGNP;
using System.Security.Cryptography.X509Certificates;

//using ControlFirmaElectronica.NotificacionElectronica;




namespace ControlFirmaElectronica
{
    public partial class wfFirma : Form
    {
        //private string _strConexionBD = "";
        //private ConexionMySQL Conexion = new ConexionMySQL();
        private clsAcuerdos Acuerdos = new clsAcuerdos();
        private long _IDNotificacion = 0;
        String[] filePath = null;
        String[] nombresArchivos = null;
        int validador;

        public wfFirma()
        {
            InitializeComponent();
            try
            {
                FormatoListaAcuerdos();
                FormatoListaFirmas();
                FormatoListaNotificaciones();
                CargarValores();
                Acuerdos.CConexionMySQL.ConnectionString = "Server=" + Acuerdos.strServidor + ";Database=" + Acuerdos.strCentro +
                   ";Uid=" + Acuerdos.strUid + ";Pwd=" + Acuerdos.strPwd + ";Connection Timeout=6000;port=" + Acuerdos.strPuerto + ";";
                //";Uid=" + "sistemas" + ";Pwd=" + "sistv4c" + ";Connection Timeout=6000;port=" + Acuerdos.strPuerto + ";";
                dgAcuerdos.DataSource = Acuerdos.ObtenerAcuerdosANotificar();
                //Datos que no se muestran al usuario
                if (Acuerdos.intOpcion == 1)
                {

                    dgAcuerdos.Columns[0].Visible = false;
                    dgAcuerdos.Columns[1].Visible = false;
                    dgAcuerdos.Columns[2].Visible = false;
                    dgAcuerdos.Columns[3].Visible = false;

                }
                //dgAcuerdos.Columns[11].Visible = false;


                if (Acuerdos.intOpcion == 2)
                {

                    dgAcuerdos.Columns[0].Visible = false;
                    dgAcuerdos.Columns[1].Visible = false;
                    dgAcuerdos.Columns[2].Visible = false;
                    dgAcuerdos.Columns[3].Visible = false;
                    dgAcuerdos.Columns[4].Visible = false;
                    dgAcuerdos.Columns[5].Visible = true;
                    dgAcuerdos.Columns[6].Visible = true;
                    dgAcuerdos.Columns[7].Visible = true;
                    dgAcuerdos.Columns[8].Visible = false;
                    dgAcuerdos.Columns[9].Visible = true;
                    dgAcuerdos.Columns[10].Visible = false;
                    dgAcuerdos.Columns[11].Visible = false;
                    dgAcuerdos.Columns[12].Visible = false;
                }

                if (Acuerdos.intOpcion == 3)
                {

                    dgAcuerdos.Columns[0].Visible = false;
                    dgAcuerdos.Columns[1].Visible = false;
                    dgAcuerdos.Columns[2].Visible = false;
                    dgAcuerdos.Columns[3].Visible = false;
                    dgAcuerdos.Columns[4].Visible = false;
                    dgAcuerdos.Columns[5].Visible = true;
                    dgAcuerdos.Columns[6].Visible = true;
                    dgAcuerdos.Columns[7].Visible = true;
                    dgAcuerdos.Columns[8].Visible = false;
                    dgAcuerdos.Columns[9].Visible = true;
                    dgAcuerdos.Columns[10].Visible = false;
                    dgAcuerdos.Columns[11].Visible = false;



                }

                //if (Acuerdos.intOpcion == 2 || Acuerdos.intOpcion == 3)
                //{

                //    dgAcuerdos.Columns[11].Visible = false;
                //}
                if (Acuerdos.intOpcion == 4)
                {
                    dgAcuerdos.Columns[0].Visible = false;
                    dgAcuerdos.Columns[1].Visible = true;
                    dgAcuerdos.Columns[2].Visible = true;
                    dgAcuerdos.Columns[3].Visible = true;
                    dgAcuerdos.Columns[4].Visible = true;
                    dgAcuerdos.Columns[5].Visible = false;
                }


                dgAcuerdos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgAcuerdos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                bool bErrorCargandoCertificados = false;
                try
                {
                    CargarFirmas(Acuerdos.ObtenerFirmas());
                }
                catch
                {
                    bErrorCargandoCertificados = true;
                }

                if (bErrorCargandoCertificados == false)
                {
                    //Opciones de la interfaz
                    if (Acuerdos.intOpcion == 1)
                    {
                        //Opcion para realizar firma
                        label2.Text = "Realizar firma del acuerdo";
                        btnVerificar.Visible = true;
                        btnNotificar.Visible = false;
                        btnpreview.Visible = false;

                    }
                    if (Acuerdos.intOpcion == 2)
                    {
                        //Opción para notificar
                        label2.Text = "Enviar notificación del acuerdo";
                        btnVerificar.Visible = false;
                        btnNotificar.Visible = true;
                        btnpreview.Visible = true;
                        env_trasl.Visible = true;
                        chkb_correr.Visible = true;

                    }
                    if (Acuerdos.intOpcion == 3)
                    {
                        //Opción para mostrar las notificaciones realizadas
                        label2.Text = "Mostrar datos de la notificación";
                        btnRevocarFirma.Enabled = false;
                        btnVerRecibo.Enabled = true;
                        btnVerificar.Visible = false;
                        btnNotificar.Visible = false;
                        linkUrl.Enabled = false;
                        btnpreview.Visible = false;
                        env_trasl.Visible = false;
                        chkb_correr.Visible = false;
                        m_traslado.Visible = true;
                        m_acuerdo.Visible = true;
                    }
                    if (Acuerdos.intOpcion == 4)
                    {
                        //Opción para  enviar  revocar
                        label2.Text = "Revocar Número Unico de Suscriptor";
                        btnVerificar.Visible = false;
                        btnNotificar.Visible = false;
                        cbFirmasDisponibles.Enabled = false;
                        lvAcuerdos.Enabled = false;
                        btnVerTextoResolutivo.Enabled = false;
                        lvFirmas.Enabled = false;
                        btnRevocarFirma.Enabled = false;
                        lvDatosNotificacion.Enabled = false;
                        btnenviarRev.Visible = true;
                        txtNumeroExpe.Enabled = false;
                        btnFiltrar.Enabled = false;
                        btnCancelarFiltro.Enabled = false;
                        btnpreview.Visible = false;
                    }
                    if (Acuerdos.intOpcion == 5)
                    {
                        //Opción para notificar el documento
                        label2.Text = "Enviar notificación del acuerdo";
                        btnVerificar.Visible = false;
                        btnNotificar.Visible = true;
                        btnenviarRev.Visible = false;
                        btnpreview.Visible = false;
                    }
                }
                else
                {
                    //Error cuando se cargan los certificados
                    label2.Text = "No se han encontrado certificados";
                    gbFirmas.Enabled = false;
                    gbAcuerdos.Enabled = false;
                    gbFirma.Enabled = false;
                    gbDatosNotificacion.Enabled = false;
                    btnVerificar.Visible = false;
                    btnNotificar.Visible = false;
                    linkUrl.Enabled = false;
                    dgAcuerdos.Enabled = false;
                    gbFiltro.Enabled = false;
                    btnSalir.Enabled = true;
                }
                //panel2.BackColor = Color.FromArgb(206, 201, 174);
                //label2.ForeColor = Color.FromArgb(75, 93, 97);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void wfFirma_Load(object sender, EventArgs e)
        {
            //timer1.Enabled = true;
        }

        //Método para mandar llamar a firmar y autenticar
        private void btnVerificar_Click(object sender, EventArgs e)
        {

            string accesofirma;

            accesofirma = ObtenerHuella();

            Acuerdos.strSQL = "SELECT vinculacion.huella_vinculacion as huella FROM vinculacion WHERE vinculacion.cargo_vinculacion =  '" + Acuerdos.strNivel + "' AND  vinculacion.status_vinculacion =  '" + 1 + "'";
            Acuerdos.CConexionMySQL.EjecutaComando(Acuerdos.strSQL);


            string _huellatoken;

            DataTableReader dtrResultado = Acuerdos.CConexionMySQL.RegresaComando(Acuerdos.strSQL);
            while (dtrResultado.Read())
            {
                _huellatoken = dtrResultado[0].ToString();
            }


            if (accesofirma != dtrResultado[0].ToString())
            {
                MessageBox.Show("El Token no tiene vinculación con la cuenta de usuario.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (dtrResultado[0].ToString() == null)
            {
                MessageBox.Show("El Token no se ha vinculado.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {


                if (cbFirmasDisponibles.SelectedIndex >= 0)
                {
                    if (dgAcuerdos.Rows.Count > 0)
                    {
                        if (dgAcuerdos.SelectedRows.Count > 0)
                        {
                            Acuerdos.intTipoFirma = 1;
                            if (ContinuarFirmaExistente() == true)
                            {
                                //Checar que el acuerdo ya este revizado
                                if (Acuerdos.bAcuerdoRevizado == true)
                                {
                                    if (Acuerdos.AbrirTextoResolutivo(0) == true)
                                    {
                                        //Inicia espera
                                        btnVerificar.Enabled = false;
                                        Cursor.Current = Cursors.WaitCursor;
                                        Acuerdos.FirmaSeleccionada = (myListObj)cbFirmasDisponibles.SelectedItem;
                                        Acuerdos.TotalFirmas = ObtenerTotalFirmasNotificacion();
                                        Acuerdos.HashOriginal = ObtenerHashAcuerdoSeleccionado();
                                        string Mensaje = Acuerdos.RealizarFirma();
                                        if (Acuerdos.strError != "Cancelación por parte del usuario")
                                            MessageBox.Show(Mensaje, "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        if (Acuerdos.FirmaCorrecta == true)
                                        {
                                            dgAcuerdos.DataSource = Acuerdos.ObtenerAcuerdosANotificar();
                                            lvAcuerdos.Items.Clear();
                                            lvDatosNotificacion.Items.Clear();
                                            lvFirmas.Items.Clear();
                                        }
                                        //Termina espera
                                        Cursor.Current = Cursors.WaitCursor;
                                        btnVerificar.Enabled = true;
                                    }
                                    else
                                        MessageBox.Show("El formato del texto resolutivo esta incorrecto.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                    MessageBox.Show("El acuerdo aún no ha sido analizado y revisado.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                                MessageBox.Show("El acuerdo seleccionado ya ha sido firmado por usted.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                            MessageBox.Show("Debe de seleccionar un acuerdo para firmar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                    MessageBox.Show("Debe de seleccionar una firma del depósito de firmas.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //}
        //Método para obtener el HASH seleccionado
        private string ObtenerHashAcuerdoSeleccionado()
        {
            return dgAcuerdos.SelectedRows[0].Cells[3].Value.ToString();
        }

        //Método para obtener las firmas de documentos
        private int ObtenerTotalFirmasNotificacion()
        {
            int Total = 0;
            for (int intIndice = 0; intIndice < lvFirmas.Items.Count; intIndice++)
            {
                if (lvFirmas.Items[intIndice].SubItems[3].Text == "DOCUMENTO")
                    Total++;
            }
            Total++;
            return Total;
        }

        //Método para validar que ya no firme de nuevo el mismo documento
        private bool ContinuarFirmaExistente()
        {
            bool Resultado = true;

            for (int Indice = 0; Indice < lvFirmas.Items.Count; Indice++)
            {
                if (Acuerdos.strUsuario == lvFirmas.Items[Indice].SubItems[2].Text)
                {
                    if (Acuerdos.intTipoFirma == 1)
                    {
                        if (lvFirmas.Items[Indice].SubItems[3].Text == "DOCUMENTO")
                            Resultado = false;
                    }
                    if (Acuerdos.intTipoFirma == 2)
                    {
                        if (lvFirmas.Items[Indice].SubItems[3].Text == "ARCHIVOS")
                            Resultado = false;
                    }
                }
            }
            return Resultado;
        }

        private void CargarValores()
        {
            DataSet xmlParamentros = new DataSet();
            xmlParamentros.ReadXml(Application.StartupPath + "\\parFirma.xml");
            Acuerdos.intOpcion = int.Parse(xmlParamentros.Tables[0].Rows[0]["Opcion"].ToString());
            Acuerdos.strRuta = xmlParamentros.Tables[0].Rows[0]["Ruta"].ToString();
            Acuerdos.strCentro = xmlParamentros.Tables[0].Rows[0]["Centro"].ToString();
            Acuerdos.strServidor = xmlParamentros.Tables[0].Rows[0]["Ip"].ToString();
            Acuerdos.strPuerto = xmlParamentros.Tables[0].Rows[0]["Puerto"].ToString();
            Acuerdos.strUsuario = xmlParamentros.Tables[0].Rows[0]["Usuario"].ToString();
            Acuerdos.strNombre = xmlParamentros.Tables[0].Rows[0]["Nombre"].ToString();
            Acuerdos.strNivel = xmlParamentros.Tables[0].Rows[0]["Nivel"].ToString();
            Acuerdos.strMunicipio = xmlParamentros.Tables[0].Rows[0]["PartidoJudicial"].ToString();
            Acuerdos.strNombreJuzgado = xmlParamentros.Tables[0].Rows[0]["Juzgado"].ToString();
            Acuerdos.strUid = xmlParamentros.Tables[0].Rows[0]["Uid"].ToString();
            Acuerdos.strPwd = xmlParamentros.Tables[0].Rows[0]["Pwd"].ToString();
            xmlParamentros.Dispose();
        }

        private void CargarFirmas(List<object> Firmas)
        {
            foreach (myListObj Elemento in Firmas)
            {
                cbFirmasDisponibles.Items.Add((myListObj)Elemento);
            }
        }

        ~wfFirma()
        {
            //Conexion.Desconectar();
            //Conexion.Dispose();            
        }

        private void btnNotificar_Click(object sender, EventArgs e)
        {


            string accesofirma;
            accesofirma = ObtenerHuella();

            Acuerdos.strSQL = "SELECT vinculacion.huella_vinculacion as huella FROM vinculacion WHERE vinculacion.cargo_vinculacion =  '" + Acuerdos.strNivel + "' AND  vinculacion.status_vinculacion =  '" + 1 + "'";
            Acuerdos.CConexionMySQL.EjecutaComando(Acuerdos.strSQL);


            string _huellatoken;

            DataTableReader dtrResultado = Acuerdos.CConexionMySQL.RegresaComando(Acuerdos.strSQL);
            while (dtrResultado.Read())
            {
                _huellatoken = dtrResultado[0].ToString();
            }


            if (accesofirma != dtrResultado[0].ToString())
            {
                MessageBox.Show("El Token no tiene vinculación con la cuenta de usuario.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {

                if (dgAcuerdos.Rows.Count > 0)
                {
                    if (dgAcuerdos.SelectedRows.Count > 0)
                    {
                        if (cbFirmasDisponibles.SelectedIndex >= 0)
                        {
                            if (Acuerdos.intOpcion == 2)
                            {
                                CargarValoresAcuerdosN();
                            }
                            if (Acuerdos.intOpcion == 1)
                            {

                                CargarValoresAcuerdos();

                            }

                            if (Acuerdos.intOpcion == 3)
                            {
                                CargarValoresAcuerdosVN();
                            }


                            if (Acuerdos.AbrirTextoResolutivo(0) == true)
                            {
                                Acuerdos.FirmaSeleccionada = (myListObj)cbFirmasDisponibles.SelectedItem;
                                //Inicia espera
                                btnNotificar.Enabled = false;
                                Cursor.Current = Cursors.WaitCursor;
                                Acuerdos.listaadj = lstadjuntar;
                                Acuerdos.corrertras = chkb_correr;
                                if (await Acuerdos.GenerarEsquemaNotificacion(long.Parse(dgAcuerdos.SelectedRows[0].Cells[0].Value.ToString())) == true)
                                {
                                    //Actualizar las notificaciones que falta por enviar
                                    dgAcuerdos.DataSource = Acuerdos.ObtenerAcuerdosANotificar();
                                    lvAcuerdos.Items.Clear();
                                    lvDatosNotificacion.Items.Clear();
                                    lvFirmas.Items.Clear();
                                    linkUrl.Links.Remove(linkUrl.Links[0]);
                                    linkUrl.Links.Add(0, linkUrl.Text.Length, Acuerdos.strURL);
                                    linkUrl.Enabled = true;
                                    MessageBox.Show("Notificación enviada correctamente.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                    MessageBox.Show("Error al enviar la notificación.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                //Termina espera
                                Cursor.Current = Cursors.WaitCursor;
                                btnNotificar.Enabled = true;
                            }
                            else
                                MessageBox.Show("El formato del texto resolutivo es incorrecto.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                            MessageBox.Show("Debe de seleccionar una firma del depósito de firmas.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    if (Acuerdos.intOpcion == 4)
                    {
                        MessageBox.Show("Debe de seleccionar un Número Unico de Suscriptor a Revocar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Debe de seleccionar un acuerdo a notificar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                    MessageBox.Show("No existen acuerdos por notificar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //}
        private void dgAcuerdos_Click(object sender, EventArgs e)
        {
            if (dgAcuerdos.Rows.Count > 0)
            {
                if (dgAcuerdos.SelectedRows.Count > 0)
                {
                    Acuerdos.IdFirmaSeleccionada = long.Parse(dgAcuerdos.SelectedRows[0].Cells[0].Value.ToString());
                    if (Acuerdos.intOpcion == 2)
                    {
                        Acuerdos.strpersfolio = dgAcuerdos.SelectedRows[0].Cells[10].Value.ToString();

                        CargarValoresAcuerdosN();

                    }

                    if (Acuerdos.intOpcion == 3)
                    {
                        Acuerdos.strpersfolio = dgAcuerdos.SelectedRows[0].Cells[10].Value.ToString();

                        CargarValoresAcuerdosVN();

                    }



                    if (Acuerdos.intOpcion == 1)
                    {
                        CargarValoresAcuerdos();
                    }

                    if (Acuerdos.intOpcion == 4)
                    {
                        CargarValoresAcuerdosR();
                    }

                    DataTableReader Resultado = Acuerdos.CargarDescripcionAcuerdo().CreateDataReader();
                    lvAcuerdos.Items.Clear();

                    //Cargar los datos del acuerdo
                    int i = 0;
                    while (Resultado.Read())
                    {
                        for (int cont = 0; cont <= Resultado.FieldCount - 1; cont++)
                        {
                            if (Resultado.GetName(cont) != "IdAuto")
                            {
                                ListViewItem List;
                                List = lvAcuerdos.Items.Add(Resultado.GetName(cont));
                                if (Resultado.GetName(cont) == "FechaAcue")
                                    List.SubItems.Add(DateTime.Parse(Resultado[cont].ToString()).ToString("dd/MM/yyyy"));
                                else
                                    List.SubItems.Add(Resultado[cont].ToString());
                            }
                            i += 1;
                        }
                    }
                    //Cargar los datos de la firma
                    CargarFirmasDelAcuerdo();
                    //Cargar los datos de la notificación
                    CargarDatosNotificacion();
                    //Habilitar o deshabilitar las opciones para firmar el video
                    if (Acuerdos.lngIdAuto == 1090 || Acuerdos.lngIdAuto == 1091)
                    {
                        btnenviarRev.Enabled = true;
                        //Checar que se pueda notificar o nó el auto cuando sea la opción 5
                        if (btnNotificar.Visible == true && Acuerdos.intOpcion == 5)
                        {
                            if (ObtenerTotalFirmaArchivos() == 2)
                            {
                                btnNotificar.Enabled = true;
                            }
                            else
                            {
                                btnNotificar.Enabled = false;
                                MessageBox.Show("Este acuerdo no puede ser notificado por no tener las dos firmas en los archivos de video.", "Mensaje del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                        btnenviarRev.Enabled = true;
                }
                else
                    MessageBox.Show("Debe de seleccionar un acuerdo a notificar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Método para cargar las firmas del acuerdo seleccionado
        private void CargarFirmasDelAcuerdo()
        {
            lvFirmas.Items.Clear();
            //Cargar los valores de las firmas
            DataTableReader ResultadoFirmas = Acuerdos.CargarFirmasAcuerdo(Acuerdos.IdFirmaSeleccionada).CreateDataReader();
            while (ResultadoFirmas.Read())
            {
                ListViewItem ListF;
                ListF = lvFirmas.Items.Add(ResultadoFirmas[0].ToString());
                ListF.SubItems.Add(ResultadoFirmas[1].ToString());
                ListF.SubItems.Add(ResultadoFirmas[2].ToString());
                ListF.SubItems.Add(ResultadoFirmas[3].ToString());
            }
        }

        //Método para cargar los datos de notificación
        private void CargarDatosNotificacion()
        {
            DataTableReader Resultado = Acuerdos.CargarDescripcionNotificacion().CreateDataReader();
            lvDatosNotificacion.Items.Clear();

            //Cargar los datos del acuerdo
            int i = 0;
            while (Resultado.Read())
            {
                for (int cont = 0; cont <= Resultado.FieldCount - 1; cont++)
                {
                    ListViewItem List;
                    List = lvDatosNotificacion.Items.Add(Resultado.GetName(cont));
                    if (Resultado.GetName(cont) == "FechaNotificacion")
                    {
                        if (Resultado[cont].ToString() == "0000-00-00")
                            List.SubItems.Add("");
                        else
                            List.SubItems.Add(DateTime.Parse(Resultado[cont].ToString()).ToString("dd/MM/yyyy"));
                    }


                    if (Resultado.GetName(cont) == "IdNotificacion")
                    {
                        _IDNotificacion = long.Parse(Resultado[cont].ToString());
                        List.SubItems.Add(Resultado[cont].ToString());
                    }


                    if (Resultado.GetName(cont) == "Traslado")
                    {
                        List.SubItems.Add(Resultado[cont].ToString());
                        if (Resultado[cont].ToString() == "SI")
                        {
                            m_traslado.Enabled = true;
                        }
                        else
                        {
                            m_traslado.Enabled = false;
                        }
                    }
                    i += 1;
                }
            }
        }

        //Método para cargar las variables con el auto seleccionado
        private void CargarValoresAcuerdos()
        {
            Acuerdos.strTipoExpe = dgAcuerdos.SelectedRows[0].Cells[1].Value.ToString();
            Acuerdos.strNumeroExpe = dgAcuerdos.SelectedRows[0].Cells[5].Value.ToString();
            Acuerdos.strTipoMovi = dgAcuerdos.SelectedRows[0].Cells[2].Value.ToString();
            Acuerdos.strFolioMovi = dgAcuerdos.SelectedRows[0].Cells[7].Value.ToString();
        }

        private void CargarValoresAcuerdosN()
        {
            Acuerdos.strTipoExpe = dgAcuerdos.SelectedRows[0].Cells[1].Value.ToString();
            Acuerdos.strevidencia = dgAcuerdos.SelectedRows[0].Cells[3].Value.ToString();
            Acuerdos.strNumeroExpe = dgAcuerdos.SelectedRows[0].Cells[5].Value.ToString();
            Acuerdos.strTipoMovi = dgAcuerdos.SelectedRows[0].Cells[2].Value.ToString();
            Acuerdos.strFolioMovi = dgAcuerdos.SelectedRows[0].Cells[7].Value.ToString();
            Acuerdos.strpersfolio = dgAcuerdos.SelectedRows[0].Cells[10].Value.ToString();
            Acuerdos.strNotificable = dgAcuerdos.SelectedRows[0].Cells[9].Value.ToString();
            Acuerdos.strbuzon = dgAcuerdos.SelectedRows[0].Cells[11].Value.ToString();
            Acuerdos.strPartexp = dgAcuerdos.SelectedRows[0].Cells[12].Value.ToString();

        }

        public void CargarValoresAcuerdosVN()
        {
            Acuerdos.strTipoExpe = dgAcuerdos.SelectedRows[0].Cells[1].Value.ToString();
            Acuerdos.strNumeroExpe = dgAcuerdos.SelectedRows[0].Cells[5].Value.ToString();
            Acuerdos.strTipoMovi = dgAcuerdos.SelectedRows[0].Cells[2].Value.ToString();
            Acuerdos.strFolioMovi = dgAcuerdos.SelectedRows[0].Cells[7].Value.ToString();
            Acuerdos.strNotificable = dgAcuerdos.SelectedRows[0].Cells[9].Value.ToString();
            Acuerdos.strpersfolio = dgAcuerdos.SelectedRows[0].Cells[10].Value.ToString();
            Acuerdos.strbuzon = dgAcuerdos.SelectedRows[0].Cells[11].Value.ToString();
        }

        public void CargarValoresAcuerdosR()
        {
            Acuerdos.strindice = dgAcuerdos.SelectedRows[0].Cells[0].Value.ToString();
            Acuerdos.strbuzon = dgAcuerdos.SelectedRows[0].Cells[1].Value.ToString();
            Acuerdos.strpersfolio = dgAcuerdos.SelectedRows[0].Cells[2].Value.ToString();
            Acuerdos.strpersfolioN = dgAcuerdos.SelectedRows[0].Cells[3].Value.ToString();
            Acuerdos.strNumeroexpeR = dgAcuerdos.SelectedRows[0].Cells[4].Value.ToString();
            Acuerdos.strtipoexpeR = dgAcuerdos.SelectedRows[0].Cells[5].Value.ToString();

        }

        private void FormatoListaAcuerdos()
        {
            lvAcuerdos.View = View.Details;

            // Allow the user to edit item text.
            lvAcuerdos.LabelEdit = false;

            // Allow the user to rearrange columns.
            lvAcuerdos.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            lvAcuerdos.FullRowSelect = true;

            // Display grid lines.
            lvAcuerdos.GridLines = true;

            lvAcuerdos.Columns.Add("Dato", 100, HorizontalAlignment.Left);
            lvAcuerdos.Columns.Add("Descripción", 320, HorizontalAlignment.Left);
        }
        public void FormatoAdjuntar()
        {
            lstadjuntar.View = View.Details;

            // Allow the user to edit item text.
            lstadjuntar.LabelEdit = false;

            // Allow the user to rearrange columns.
            lstadjuntar.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            lstadjuntar.FullRowSelect = true;

            // Display grid lines.
            lstadjuntar.GridLines = true;


            lstadjuntar.Columns.Add("Archivos Adjuntos", 340, HorizontalAlignment.Left);


        }

        public void FormatoVertras()
        {
            lstverTras.View = View.Details;

            // Allow the user to edit item text.
            lstverTras.LabelEdit = false;

            // Allow the user to rearrange columns.
            lstverTras.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            lstverTras.FullRowSelect = true;

            // Display grid lines.
            lstverTras.GridLines = true;


            lstverTras.Columns.Add("Archivos Adjuntos", 340, HorizontalAlignment.Left);
            lstverTras.Columns.Add("count", 0, HorizontalAlignment.Left);


        }
        private void FormatoListaFirmas()
        {
            lvFirmas.View = View.Details;

            // Allow the user to edit item text.
            lvFirmas.LabelEdit = false;

            // Allow the user to rearrange columns.
            lvFirmas.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            lvFirmas.FullRowSelect = true;

            // Display grid lines.
            lvFirmas.GridLines = true;

            lvFirmas.Columns.Add("IdUsuario", 0, HorizontalAlignment.Left);
            lvFirmas.Columns.Add("Firmas", 220, HorizontalAlignment.Left);
            lvFirmas.Columns.Add("Usuario", 0, HorizontalAlignment.Left);
            lvFirmas.Columns.Add("Tipo", 200, HorizontalAlignment.Left);
        }

        private void FormatoListaNotificaciones()
        {
            lvDatosNotificacion.View = View.Details;

            // Allow the user to edit item text.
            lvDatosNotificacion.LabelEdit = false;

            // Allow the user to rearrange columns.
            lvDatosNotificacion.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            lvDatosNotificacion.FullRowSelect = true;

            // Display grid lines.
            lvDatosNotificacion.GridLines = true;

            lvDatosNotificacion.Columns.Add("Dato", 100, HorizontalAlignment.Left);
            lvDatosNotificacion.Columns.Add("Descripción", 320, HorizontalAlignment.Left);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgAcuerdos.Rows.Count > 0)
            {
                if (dgAcuerdos.SelectedRows.Count > 0)
                {
                    if (Acuerdos.AbrirTextoResolutivo(1) == false)
                        MessageBox.Show("Formato de texto resolutivo incorrecto.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    MessageBox.Show("Debe de seleccionar un acuerdo para ver su texto resolutivo.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show("No hay acuerdos para mostrar el texto resolutivo.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnRevocarFirma_Click(object sender, EventArgs e)
        {

            string accesofirma;




            accesofirma = ObtenerHuella();

            Acuerdos.strSQL = "SELECT vinculacion.huella_vinculacion as huella FROM vinculacion WHERE vinculacion.cargo_vinculacion =  '" + Acuerdos.strNivel + "' AND  vinculacion.status_vinculacion =  '" + 1 + "'";
            Acuerdos.CConexionMySQL.EjecutaComando(Acuerdos.strSQL);


            string _huellatoken;

            DataTableReader dtrResultado = Acuerdos.CConexionMySQL.RegresaComando(Acuerdos.strSQL);
            while (dtrResultado.Read())
            {
                _huellatoken = dtrResultado[0].ToString();
            }


            if (accesofirma != dtrResultado[0].ToString())
            {
                MessageBox.Show("El Token no tiene vinculación con la cuenta de usuario.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {


                if (lvFirmas.Items.Count > 0)
                {
                    if (lvFirmas.SelectedItems.Count > 0)
                    {
                        if (Acuerdos.strUsuario == lvFirmas.SelectedItems[0].SubItems[2].Text)
                        {
                            if (cbFirmasDisponibles.SelectedIndex >= 0)
                            {
                                if (MessageBox.Show("Esta seguro que desea revocar la firma.", "Mensaje del sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    //Verificar que la firma sea igual
                                    Acuerdos.FirmaSeleccionada = (myListObj)cbFirmasDisponibles.SelectedItem;
                                    Acuerdos.intTipoFirma = ObtenerTipoFirma();
                                    //Inicia espera
                                    btnRevocarFirma.Enabled = false;
                                    Cursor.Current = Cursors.WaitCursor;
                                    if (Acuerdos.intTipoFirma == 1)
                                    {
                                        //Verificar cuando sea un documento
                                        if (Acuerdos.CompararFirmaElectronica() == true)
                                        {
                                            if (Acuerdos.RevocarFirma(long.Parse(lvFirmas.SelectedItems[0].Text)) == true)
                                            {
                                                CargarFirmasDelAcuerdo();
                                                dgAcuerdos.DataSource = Acuerdos.ObtenerAcuerdosANotificar();
                                                MessageBox.Show("Firma electrónica avanzada revocada correctamente.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                        }
                                        else
                                            MessageBox.Show("La comparación de la firma del acuerdo seleccionado con la firma guardada anteriormente no son idénticas.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        //Verificar cuando sea el esquema de los archivos
                                        if (System.IO.File.Exists(Acuerdos.strRuta + "\\firmaele\\HuellasDigitales.xml"))
                                            System.IO.File.Delete(Acuerdos.strRuta + "\\firmaele\\HuellasDigitales.xml");

                                        Acuerdos.ObtenerEsquemaArchivos().WriteXml(Acuerdos.strRuta + "\\firmaele\\HuellasDigitales.xml");
                                        if (Acuerdos.CompararFirmaElectronica(Acuerdos.strRuta + "\\firmaele\\HuellasDigitales.xml") == true)
                                        {
                                            if (Acuerdos.RevocarFirma(long.Parse(lvFirmas.SelectedItems[0].Text)) == true)
                                            {
                                                CargarFirmasDelAcuerdo();
                                                dgAcuerdos.DataSource = Acuerdos.ObtenerAcuerdosANotificar();
                                                MessageBox.Show("Firma electrónica avanzada revocada correctamente.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                        }
                                        else
                                            MessageBox.Show("La comparación de la firma de los archivos comparada con la firma guardada anteriormente no son idénticas.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    //Fin de espera
                                    btnRevocarFirma.Enabled = true;
                                    Cursor.Current = Cursors.Default;
                                }
                            }
                            else
                                MessageBox.Show("Debe de seleccionar una firma del depósito de firmas.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                            MessageBox.Show("La firma que desea revocar no es la de usted.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        MessageBox.Show("Debe de seleccionar una firma para revocar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    MessageBox.Show("Debe de seleccionar una firma para revocar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //}
        private int ObtenerTipoFirma()
        {
            int Resultado = 0;
            if (lvFirmas.SelectedItems[0].SubItems[3].Text == "DOCUMENTO")
                Resultado = 1;
            else
                Resultado = 2;
            return Resultado;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void linkUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(sInfo);
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (txtNumeroExpe.Text.Trim().Length > 0)
            {
                Acuerdos.strNumeroExpeBusqueda = txtNumeroExpe.Text;
                dgAcuerdos.DataSource = Acuerdos.ObtenerAcuerdosANotificar();
            }
            else
                MessageBox.Show("Debe de escribir un número de expediente para filtrar la información.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnCancelarFiltro_Click(object sender, EventArgs e)
        {
            Acuerdos.strNumeroExpeBusqueda = "";
            txtNumeroExpe.Text = "";
            dgAcuerdos.DataSource = Acuerdos.ObtenerAcuerdosANotificar();
        }

        private void btnVerRecibo_Click(object sender, EventArgs e)
        {
            if (_IDNotificacion > 0)
            {
                if (Acuerdos.ObtenerRecibo(_IDNotificacion) == true)
                {
                    linkUrl.Links.Remove(linkUrl.Links[0]);
                    linkUrl.Links.Add(0, linkUrl.Text.Length, Acuerdos.strURL);
                    LinkLabelLinkClickedEventArgs x = new LinkLabelLinkClickedEventArgs(linkUrl.Links[0]);
                    linkUrl_LinkClicked(null, x);
                }
                else
                    MessageBox.Show("Hubo un error al cargar el recibo.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show("Debe de seleccionar el acuerdo.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        //Método para cargar los archivos en caso de que ya exista una firma
        private void Verificar_CargarArchivosFirma(wfFirmaArchivos wFA)
        {
            for (int Indice = 0; Indice < lvFirmas.Items.Count; Indice++)
            {
                if (lvFirmas.Items[Indice].SubItems[3].Text == "ARCHIVOS")
                {
                    wFA.btnVerTextoResolutivo.Enabled = false;
                    //Actualizar solo el contador de la firma
                    wFA.intOpcionFirmar = 2;
                    wFA.CargarArchivosFirmados();
                }
            }
        }

        //Método para obtener el total de firmas de archivos, sirve para la opción 5
        private int ObtenerTotalFirmaArchivos()
        {
            int Resultado = 0;
            for (int Indice = 0; Indice < lvFirmas.Items.Count; Indice++)
            {
                if (lvFirmas.Items[Indice].SubItems[3].Text == "ARCHIVOS")
                {
                    Resultado++;
                }
            }
            return Resultado;
        }
        public string ObtenerHuella()
        {
            string _Respuesta = "";
            string huella;
            try
            {
                CspParameters csp = new CspParameters(1, "SafeSign Standard Cryptographic Service Provider");
                csp.Flags = CspProviderFlags.UseDefaultKeyContainer;
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp);
                _Respuesta = rsa.CspKeyContainerInfo.UniqueKeyContainerName;
                huella = _Respuesta;
            }
            catch
            {
            }
            return _Respuesta;

        }


        private void gbFiltro_Enter(object sender, EventArgs e)
        {

        }

        private void dgAcuerdos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnenviarRev_Click(object sender, EventArgs e)
        {
            ////Suscriptores.ContratoGestionSuscriptoresClient clientesusNR = new Suscriptores.ContratoGestionSuscriptoresClient();
            //// Suscriptores.Verificador veriNR = new Suscriptores.Verificador();
            //  clsAcuerdos objeto = new clsAcuerdos();

            // objeto.BloquearAccesoExpediente();
            //CargarValoresAcuerdosR(); 
            Acuerdos.BloquearAccesoExpediente();
            Acuerdos.EnviarRev();
            MessageBox.Show("Revocación Enviada correctamente.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dgAcuerdos.DataSource = Acuerdos.ObtenerAcuerdosANotificar();

        }

        private void gbNotificaciones_Enter(object sender, EventArgs e)
        {

        }

        private void gbDatosNotificacion_Enter(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
        }

        private void btnpreview_Click(object sender, EventArgs e)
        {

            if (dgAcuerdos.Rows.Count > 0)
            {
                if (dgAcuerdos.SelectedRows.Count > 0)
                {
                    string _DescripcionExpediente = "";
                    string _strActor = "";
                    string _strDemandado = "";
                    string _Secretario = "";



                    Acuerdos.strSQL = "call proc_ObtenerPartes(" + Acuerdos.strTipoExpe + ",'" + Acuerdos.strNumeroExpe + "');";
                    DataTableReader dtrPartes = Acuerdos.CConexionMySQL.RegresaComando(Acuerdos.strSQL);
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


                    Acuerdos.strSQL = "SELECT firmas.firm_vafi_id,firmas.firm_nombre as Nombre,firmas.firm_nivel as nivel FROM firmas WHERE firmas.firm_vafi_id  = '" + Acuerdos.IdFirmaSeleccionada + "' ";
                    DataTableReader dtrfirmas = Acuerdos.CConexionMySQL.RegresaComando(Acuerdos.strSQL);

                    string niveljuez = "1";
                    string nivelsecre = "2";


                    while (dtrfirmas.Read())
                    {
                        _Secretario = dtrfirmas["nivel"].ToString();


                        if (_Secretario == niveljuez)
                        {
                            lbljuez.Text = dtrfirmas["Nombre"].ToString();

                        }


                        if (_Secretario == nivelsecre)
                        {
                            lblsecretario.Text = dtrfirmas["Nombre"].ToString();

                        }
                    }

                    textBox1.Text = _strActor;
                    textBox2.Text = _strDemandado;
                    lblbuzon.Text = Acuerdos.strbuzon;
                    lblnotificable.Text = Acuerdos.strNotificable;
                    lblpar.Text = Acuerdos.strPartexp;
                    panel3.Visible = true;
                }
                else
                {

                    MessageBox.Show("Debe de seleccionar una notificación para ver su vista previa.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
                MessageBox.Show("No existen notificaciónes para ver su vista previa.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);


        }

        private void label5_Click(object sender, EventArgs e)
        {


        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            //clsAcuerdos mic = new clsAcuerdos();
            //mic.Traslado (this);
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = true; //para seleccionar varios archivos a la vez
            file.Filter = "Text Files (.pdf)|*.pdf";
            file.FilterIndex = 1;
            int conteo;

            String[] nombresArchivos = null;

            if (file.ShowDialog() == DialogResult.OK)
            {
                nombresArchivos = file.SafeFileNames;
                filePath = file.FileNames; //guardo archivos en arreglo
            }

            if (filePath == null)
            {

            }

            else
            {

                foreach (string adjunto in filePath)
                {

                    //ListViewItem ListF;
                    //ListF = lvArchivos.Items.Add(Resultado[0].ToString());
                    //ListF.SubItems.Add(Resultado[1].ToString());
                    //ListF.SubItems.Add(Resultado[3].ToString());


                    ListViewItem List;
                    List = lstadjuntar.Items.Add(adjunto);



                    conteo = lstadjuntar.Items.Count;
                    lblcantidad.Text = conteo.ToString();


                    //txtadjuntos.Text += adjunto + ",";
                }
                //foreach (string archivos in filePath)
                //{
                //    ListViewItem List;
                //    List = lstadjuntar.Items.Add(archivos);
                //    List.SubItems.Add(archivos); 
                //    List.SubItems.Add(archivos);
                //}

            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            lstadjuntar.Items.Clear();
            int conteo;


            conteo = lstadjuntar.Items.Count;
            lblcantidad.Text = conteo.ToString();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            string accesofirma;


            if (lstadjuntar.Items.Count == 0)
            {
                MessageBox.Show("No se han anexado archivos para Correr Traslado", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }


            else
            {


                accesofirma = ObtenerHuella();
                //accesofirma = "282C92FB87B235FBB7D1C90056A545DC49727AE7";

                Acuerdos.strSQL = "SELECT vinculacion.huella_vinculacion as huella FROM vinculacion WHERE vinculacion.cargo_vinculacion =  '" + Acuerdos.strNivel + "' AND  vinculacion.status_vinculacion =  '" + 1 + "'";
                Acuerdos.CConexionMySQL.EjecutaComando(Acuerdos.strSQL);


                string _huellatoken;

                DataTableReader dtrResultado = Acuerdos.CConexionMySQL.RegresaComando(Acuerdos.strSQL);

                if (dtrResultado.Read() == false)
                {
                    MessageBox.Show("No se ha vinculado el token", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else
                {
                    while (dtrResultado.Read())
                    {
                        _huellatoken = dtrResultado[0].ToString();
                    }


                    if (accesofirma != dtrResultado[0].ToString())
                    {
                        MessageBox.Show("El Token no tiene vinculación con la cuenta de usuario.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    else
                    {




                        if (dgAcuerdos.Rows.Count > 0)
                        {
                            if (dgAcuerdos.SelectedRows.Count > 0)
                            {
                                if (cbFirmasDisponibles.SelectedIndex >= 0)
                                {
                                    if (Acuerdos.intOpcion == 2)
                                    {
                                        CargarValoresAcuerdosN();
                                    }
                                    if (Acuerdos.intOpcion == 1)
                                    {

                                        CargarValoresAcuerdos();

                                    }

                                    if (Acuerdos.intOpcion == 3)
                                    {
                                        CargarValoresAcuerdosVN();
                                    }


                                    if (Acuerdos.AbrirTextoResolutivo(0) == true)
                                    {
                                        Acuerdos.FirmaSeleccionada = (myListObj)cbFirmasDisponibles.SelectedItem;
                                        //Inicia espera
                                        btnNotificar.Enabled = false;
                                        Cursor.Current = Cursors.WaitCursor;

                                        //if (Acuerdos.intOpcion == 2)
                                        //{
                                        //    CargarValoresAcuerdosR();
                                        //}
                                        Acuerdos.listaadj = lstadjuntar;
                                        Acuerdos.corrertras = chkb_correr;
                                        if (await Acuerdos.GenerarEsquemaNotificacion(long.Parse(dgAcuerdos.SelectedRows[0].Cells[0].Value.ToString())) == true)
                                        {
                                            //clsAcuerdos mic = new clsAcuerdos();
                                            //mic.enviartraslado(this);
                                            //Actualizar las notificaciones que falta por enviar
                                            dgAcuerdos.DataSource = Acuerdos.ObtenerAcuerdosANotificar();
                                            lvAcuerdos.Items.Clear();
                                            lvDatosNotificacion.Items.Clear();
                                            lvFirmas.Items.Clear();
                                            linkUrl.Links.Remove(linkUrl.Links[0]);
                                            linkUrl.Links.Add(0, linkUrl.Text.Length, Acuerdos.strURL);
                                            linkUrl.Enabled = true;
                                            lstadjuntar.Items.Clear();
                                            panel4.Visible = false;
                                            chkb_correr.Checked = false;


                                            MessageBox.Show("Notificación y  Traslado enviado correctamente.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        else
                                            MessageBox.Show("Error al enviar la notificación.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        //Termina espera
                                        Cursor.Current = Cursors.WaitCursor;
                                        btnNotificar.Enabled = true;
                                    }
                                    else
                                        MessageBox.Show("El formato del texto resolutivo es incorrecto.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                    MessageBox.Show("Debe de seleccionar una firma del depósito de firmas.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else

                                if (Acuerdos.intOpcion == 4)
                                {
                                    MessageBox.Show("Debe de seleccionar un Número Unico de Suscriptor a Revocar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }

                        }
                        else
                            MessageBox.Show("No existen acuerdos por notificar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            panel4.Visible = false;
            lstadjuntar.Items.Clear();
            lblcantidad.Text = "0";
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (lstverTras.SelectedItems.Count > 0)
            {
                Acuerdos.lisVertraslado = lstverTras;
                if (Acuerdos.ObtenerTraslado(_IDNotificacion) == true)
                {
                    linkUrl.Links.Remove(linkUrl.Links[0]);
                    linkUrl.Links.Add(0, linkUrl.Text.Length, Acuerdos.strURL);
                    LinkLabelLinkClickedEventArgs x = new LinkLabelLinkClickedEventArgs(linkUrl.Links[0]);
                    linkUrl_LinkClicked(null, x);
                }
            }
            else
            {

                MessageBox.Show("Debe de seleccionar un Archivo para verificarlo.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            panel5.Visible = false;
            lstverTras.Items.Clear();
            lblvert.Text = "0";
        }

        private void m_traslado_Click(object sender, EventArgs e)
        {
            if (dgAcuerdos.Rows.Count > 0)
            {
                if (dgAcuerdos.SelectedRows.Count > 0)
                {
                    if (_IDNotificacion > 0)
                    {
                        FormatoVertras();
                        label18.Text = Acuerdos.strbuzon;
                        label19.Text = Acuerdos.strNotificable;
                        label20.Text = Acuerdos.strNumeroExpe;
                        Acuerdos.lisVertraslado = lstverTras;
                        Acuerdos.lblvertexto = lblvert;
                        if (Acuerdos.ObtenerAcuerdoT(_IDNotificacion) == true)
                        {

                        }
                        else
                            MessageBox.Show("Hubo un error al cargar el  Traslado.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        panel5.Visible = true;

                    }
                    else

                        MessageBox.Show("Debe de seleccionar el acuerdo.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else
                {

                    MessageBox.Show("Debe de seleccionar una notificación para ver su Traslado.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
                MessageBox.Show("No existen notificaciónes para ver su Traslado.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void m_acuerdo_Click(object sender, EventArgs e)
        {
            if (dgAcuerdos.Rows.Count > 0)
            {
                if (dgAcuerdos.SelectedRows.Count > 0)
                {
                    if (_IDNotificacion > 0)
                    {
                        if (Acuerdos.ObtenerAcuerdoP(_IDNotificacion) == true)
                        {
                            linkUrl.Links.Remove(linkUrl.Links[0]);
                            linkUrl.Links.Add(0, linkUrl.Text.Length, Acuerdos.strURL);
                            LinkLabelLinkClickedEventArgs x = new LinkLabelLinkClickedEventArgs(linkUrl.Links[0]);
                            linkUrl_LinkClicked(null, x);
                        }
                        else
                            MessageBox.Show("Hubo un error al cargar el  Acuerdo.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else


                        MessageBox.Show("Debe de seleccionar el acuerdo.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else
                {

                    MessageBox.Show("Debe de seleccionar una notificación para ver su vista previa.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
                MessageBox.Show("No existen notificaciónes para ver su vista previa.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);


        }

        private void chkb_correr_CheckedChanged(object sender, EventArgs e)
        {
            if (chkb_correr.Checked == true)
            {
                env_trasl.Enabled = true;
                btnNotificar.Enabled = false;
            }
            else
            {
                env_trasl.Enabled = false;
                btnNotificar.Enabled = true;

            }

        }

        private void env_trasl_Click(object sender, EventArgs e)
        {
            if (dgAcuerdos.Rows.Count > 0)
            {
                if (dgAcuerdos.SelectedRows.Count > 0)
                {

                    if (cbFirmasDisponibles.SelectedIndex >= 0)
                    {
                        FormatoAdjuntar();
                        lbl_buz.Text = Acuerdos.strbuzon;
                        lbl_noti.Text = Acuerdos.strNotificable;
                        lbl_expe.Text = Acuerdos.strNumeroExpe;
                        panel4.Visible = true;
                    }

                    else
                    {
                        MessageBox.Show("Debe de seleccionar una firma del depósito de firmas.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                }
                else
                {

                    MessageBox.Show("Debe de seleccionar una notificación para enviarlas y Correr traslado.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
                MessageBox.Show("No existen notificaciones para enviarlas y Correr traslado.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void picEnviar_Click(object sender, EventArgs e)
        {
            if (txtcertificado.Text == "" || txtcontraseña.Text == "")
            {
                MessageBox.Show("Falta Agregar Certificado o Cntraseña", "Favor de verificar", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            else
            {

                X509Certificate2 cert2 = new X509Certificate2();
                try
                {
                    X509Certificate2 cert = new X509Certificate2(filePath[0], txtcontraseña.Text);
                    cert2 = cert;
                    validador = 1;
                }
                catch
                {
                    MessageBox.Show("La contraseña es incorrecta", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    validador = 0;
                }

                if (validador == 1)
                {
                    if (dgAcuerdos.Rows.Count > 0)
                    {
                        if (dgAcuerdos.SelectedRows.Count > 0)
                        {
                            if (cbFirmasDisponibles.Text != "")
                            {
                                if (Acuerdos.intOpcion == 2)
                                {
                                    CargarValoresAcuerdosN();
                                }
                                if (Acuerdos.intOpcion == 1)
                                {

                                    CargarValoresAcuerdos();
                                }

                                if (Acuerdos.intOpcion == 3)
                                {
                                    CargarValoresAcuerdosVN();
                                }

                                if (Acuerdos.AbrirTextoResolutivo(0) == true)
                                {

                                    if (Acuerdos.GenerarEsquemaNotificacion2(long.Parse(dgAcuerdos.SelectedRows[0].Cells[0].Value.ToString()), cert2) == true)
                                    {
                                        Cursor.Current = Cursors.WaitCursor;
                                        //Actualizar las notificaciones que falta por enviar
                                        dgAcuerdos.DataSource = Acuerdos.ObtenerAcuerdosANotificar();
                                        lvAcuerdos.Items.Clear();
                                        lvDatosNotificacion.Items.Clear();
                                        lvFirmas.Items.Clear();
                                        linkUrl.Links.Remove(linkUrl.Links[0]);
                                        linkUrl.Links.Add(0, linkUrl.Text.Length, Acuerdos.strURL);
                                        linkUrl.Enabled = true;
                                        txtcertificado.Text = "";
                                        txtcontraseña.Text = "";
                                        lblNombre.Text = "";
                                        MessageBox.Show("Notificación enviada correctamente.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    }
                                    else
                                    {
                                        MessageBox.Show("Error al enviar la notificación.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    //Termina espera
                                    Cursor.Current = Cursors.WaitCursor;
                                    btnNotificar.Enabled = true;
                                }
                                else
                                    MessageBox.Show("El formato del texto resolutivo es incorrecto.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                                MessageBox.Show("Debe de seleccionar una firma del depósito de firmas.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else

                            if (Acuerdos.intOpcion == 4)
                            {
                                MessageBox.Show("Debe de seleccionar un Número Unico de Suscriptor a Revocar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                MessageBox.Show("Debe de seleccionar un acuerdo a notificar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                    }
                    else
                        MessageBox.Show("No existen acuerdos por notificar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            if (txtcertificado.Text == "" || txtcontraseña.Text == "")
            {
                MessageBox.Show("Falta Agregar Certificado o Cntraseña", "Favor de verificar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Cursor.Current = Cursors.WaitCursor;
            }

            else
            {

                X509Certificate2 cert2 = new X509Certificate2();
                try
                {

                    X509Certificate2 cert = new X509Certificate2(filePath[0], txtcontraseña.Text);
                    cert2 = cert;
                    validador = 1;
                }
                catch
                {
                    MessageBox.Show("La contraseña es incorrecta", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    validador = 0;
                }

                if (validador == 1)
                {

                    if (cbFirmasDisponibles.Text != "")
                    {
                        if (dgAcuerdos.Rows.Count > 0)
                        {
                            if (dgAcuerdos.SelectedRows.Count > 0)
                            {
                                Acuerdos.intTipoFirma = 1;
                                if (ContinuarFirmaExistente() == true)
                                {
                                    //Checar que el acuerdo ya este revizado
                                    if (Acuerdos.bAcuerdoRevizado == true)
                                    {
                                        if (Acuerdos.AbrirTextoResolutivo(0) == true)
                                        {
                                            //Inicia espera
                                            btnVerificar.Enabled = false;
                                            Cursor.Current = Cursors.WaitCursor;
                                            //  Acuerdos.FirmaSeleccionada =  ;
                                            Acuerdos.TotalFirmas = ObtenerTotalFirmasNotificacion();
                                            Acuerdos.HashOriginal = ObtenerHashAcuerdoSeleccionado();
                                            string Mensaje = Acuerdos.RealizarFirmaCertificado(cert2);
                                            if (Acuerdos.strError != "Cancelación por parte del usuario")
                                                MessageBox.Show(Mensaje, "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            if (Acuerdos.FirmaCorrecta == true)
                                            {
                                                dgAcuerdos.DataSource = Acuerdos.ObtenerAcuerdosANotificar();
                                                lvAcuerdos.Items.Clear();
                                                lvDatosNotificacion.Items.Clear();
                                                lvFirmas.Items.Clear();
                                                txtcertificado.Text = "";
                                                txtcontraseña.Text = "";
                                                lblNombre.Text = "";
                                            }
                                            //Termina espera
                                            Cursor.Current = Cursors.WaitCursor;
                                            btnVerificar.Enabled = true;
                                        }
                                        else
                                            MessageBox.Show("El formato del texto resolutivo esta incorrecto.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                        MessageBox.Show("El acuerdo aún no ha sido analizado y revisado.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                    MessageBox.Show("El acuerdo seleccionado ya ha sido firmado por usted.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                                MessageBox.Show("Debe de seleccionar un acuerdo para firmar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                        MessageBox.Show("Debe de seleccionar una firma del depósito de firmas.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btncertificado_Click(object sender, EventArgs e)
        {

            if (Acuerdos.intOpcion == 1)
            {
                btnVerificar.Enabled = false;
                pictureBox10.Visible = true;
            }
            if (Acuerdos.intOpcion == 2)
            {
                btnNotificar.Enabled = false;
                picEnviar.Visible = true;
            }

            panel6.Visible = true;

        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            //file.Multiselect = true; //para seleccionar varios archivos a la vez
            file.Filter = "Text Files (.p)|*.p12";
            file.FilterIndex = 1;



            if (file.ShowDialog() == DialogResult.OK)
            {
                nombresArchivos = file.SafeFileNames;
                filePath = file.FileNames; //guardo archivos en arreglo
                lblNombre.Text = nombresArchivos[0];
                //List<object> lista = new List<object>();
                //lista.Add(nombresArchivos[0]); 
                // cbFirmasDisponibles.Items.Add(nombresArchivos[0]);
                cbFirmasDisponibles.Text = nombresArchivos[0];
                txtcertificado.Text = filePath[0];
                txtcontraseña.Focus();


            }

        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {

            if (Acuerdos.intOpcion == 1)
            {
                btnVerificar.Enabled = true;
            }
            if (Acuerdos.intOpcion == 2)
            {
                btnNotificar.Enabled = true;
            }
            panel6.Visible = false;
            txtcertificado.Text = "";
            txtcontraseña.Text = "";
            lblNombre.Text = "";

        } 
    }
}