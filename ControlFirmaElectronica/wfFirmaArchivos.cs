using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace ControlFirmaElectronica
{
    public partial class wfFirmaArchivos : Form
    {
        clsAcuerdos Acuerdos = null;    
        DataSet _ResultadoFirmaArchivos = new DataSet("HuellasDigitales");
   
        //Variable pública para la opción del firmado
        //1 -> No existia firma
        //2 -> Ya existia una firma y lista de archivos
        public int intOpcionFirmar = 0;

        public wfFirmaArchivos(clsAcuerdos _Acuerdos)
        {
            InitializeComponent();
            FormatoArchivos();
            Acuerdos = _Acuerdos;          
            //panel2.BackColor = Color.FromArgb(206, 201, 174);
            //label2.ForeColor = Color.FromArgb(75, 93, 97);
        }

        private void btnVerTextoResolutivo_Click(object sender, EventArgs e)
        {
            if (fbdRuta.ShowDialog() == DialogResult.OK)
            {
                Acuerdos.ArchivosHuellaDigital.Rows.Clear();
                //Inicia espera
                btnVerTextoResolutivo.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;
                if (Acuerdos.RegresarHashArchivos(fbdRuta.SelectedPath) == true)
                {                    
                    _ResultadoFirmaArchivos.Tables.Add(Acuerdos.ArchivosHuellaDigital.Copy());
                    _ResultadoFirmaArchivos.WriteXml(Acuerdos.strRuta + "\\firmaele\\HuellasDigitales.xml");
                    MostrarArchivosFirmar();
                    //Actualizar todos los registros
                    intOpcionFirmar = 1;
                }
                else
                    MessageBox.Show("Hubo un fallo al obtener la huella digital de los archivos.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //Termina espera
                Cursor.Current = Cursors.Default;
                btnVerTextoResolutivo.Enabled = true;
            }
            else
                MessageBox.Show("Ruta de los archivos incorrecta.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        //Método para llenar los archivos encontrados        
        private void MostrarArchivosFirmar()
        {
            lvArchivos.Items.Clear();
            //Cargar los valores de las firmas
            DataTableReader Resultado = Acuerdos.ArchivosHuellaDigital.Copy().CreateDataReader();
            while (Resultado.Read())
            {
                ListViewItem ListF;
                ListF = lvArchivos.Items.Add(Resultado[0].ToString());
                ListF.SubItems.Add(Resultado[1].ToString());
                ListF.SubItems.Add(Resultado[3].ToString());
            }
        }

        //Método para llenar los archivos encontrados        
        private void MostrarArchivosFirmar(DataTable DTDatos)
        {
            lvArchivos.Items.Clear();
            //Cargar los valores de las firmas
            DataTableReader Resultado = DTDatos.CreateDataReader();
            while (Resultado.Read())
            {
                ListViewItem ListF;
                ListF = lvArchivos.Items.Add(Resultado[0].ToString());
                ListF.SubItems.Add(Resultado[1].ToString());
                ListF.SubItems.Add(Resultado[3].ToString());
            }
        }  

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormatoArchivos()
        {
            lvArchivos.View = View.Details;

            // Allow the user to edit item text.
            lvArchivos.LabelEdit = false;

            // Allow the user to rearrange columns.
            lvArchivos.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            lvArchivos.FullRowSelect = true;

            // Display grid lines.
            lvArchivos.GridLines = true;

            lvArchivos.Columns.Add("Nombre", 120, HorizontalAlignment.Left);
            lvArchivos.Columns.Add("Tamaño", 190, HorizontalAlignment.Left);
            lvArchivos.Columns.Add("UltimaModificación", 190, HorizontalAlignment.Left);
        }

        private void btnFirmarArchivos_Click(object sender, EventArgs e)
        {
            if (lvArchivos.Items.Count > 0)
            {
                //Inicia espera
                btnFirmarArchivos.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;
                //Código para realizar la firma electrónica avanzada                    
                Acuerdos.RealizarFirma(Acuerdos.strRuta + "\\firmaele\\HuellasDigitales.xml");
                if (Acuerdos.FirmaCorrecta == true)
                {
                    if (Acuerdos.GuardarFirmaArchivos(_ResultadoFirmaArchivos.GetXml(),intOpcionFirmar) == true)
                    {
                        if (Acuerdos.AgregarFirmaAceptadaArchivos() == true)
                        {
                            MessageBox.Show("Proceso de firma electrónica avanzada para archivos realizada correctamente.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                            MessageBox.Show("Hubo un error al registrar la nueva firma.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        MessageBox.Show("Hubo un error al registrar el esquema.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    MessageBox.Show("Hubo un error al firmar los archivos, el error fue : " + Acuerdos.strError, "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //Termina espera
                Cursor.Current = Cursors.Default;
                btnFirmarArchivos.Enabled = true;
            }
            else
                MessageBox.Show("No ha seleccionado los archivos a firmar.", "Mensaje del sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        //Método para cargar los archivos firmados
        public void CargarArchivosFirmados()
        {
            _ResultadoFirmaArchivos = Acuerdos.ObtenerEsquemaArchivos().Copy();
            if (System.IO.File.Exists(Acuerdos.strRuta + "\\firmaele\\HuellasDigitales.xml"))
                System.IO.File.Delete(Acuerdos.strRuta + "\\firmaele\\HuellasDigitales.xml");
            _ResultadoFirmaArchivos.WriteXml(Acuerdos.strRuta + "\\firmaele\\HuellasDigitales.xml");
            MostrarArchivosFirmar(_ResultadoFirmaArchivos.Tables[0]);
        }
    }
}