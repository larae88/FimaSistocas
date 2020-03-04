namespace ControlFirmaElectronica
{
    partial class wfFirmaArchivos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(wfFirmaArchivos));
            this.fbdRuta = new System.Windows.Forms.FolderBrowserDialog();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.gbSeleccionarRuta = new System.Windows.Forms.GroupBox();
            this.btnVerTextoResolutivo = new System.Windows.Forms.Button();
            this.bgRealizarFirma = new System.Windows.Forms.GroupBox();
            this.btnFirmarArchivos = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lvArchivos = new System.Windows.Forms.ListView();
            this.panel2.SuspendLayout();
            this.gbSeleccionarRuta.SuspendLayout();
            this.bgRealizarFirma.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(529, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(463, 46);
            this.panel2.TabIndex = 31;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(14, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(433, 29);
            this.label2.TabIndex = 0;
            this.label2.Text = "Firmar archivos del video";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbSeleccionarRuta
            // 
            this.gbSeleccionarRuta.Controls.Add(this.btnVerTextoResolutivo);
            this.gbSeleccionarRuta.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSeleccionarRuta.Location = new System.Drawing.Point(531, 62);
            this.gbSeleccionarRuta.Name = "gbSeleccionarRuta";
            this.gbSeleccionarRuta.Size = new System.Drawing.Size(461, 77);
            this.gbSeleccionarRuta.TabIndex = 32;
            this.gbSeleccionarRuta.TabStop = false;
            this.gbSeleccionarRuta.Text = "Seleccionar la carpeta o ruta de los archivos";
            // 
            // btnVerTextoResolutivo
            // 
            this.btnVerTextoResolutivo.Image = ((System.Drawing.Image)(resources.GetObject("btnVerTextoResolutivo.Image")));
            this.btnVerTextoResolutivo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVerTextoResolutivo.Location = new System.Drawing.Point(304, 38);
            this.btnVerTextoResolutivo.Name = "btnVerTextoResolutivo";
            this.btnVerTextoResolutivo.Size = new System.Drawing.Size(153, 23);
            this.btnVerTextoResolutivo.TabIndex = 37;
            this.btnVerTextoResolutivo.Text = "Seleccionar archivos";
            this.btnVerTextoResolutivo.UseVisualStyleBackColor = true;
            this.btnVerTextoResolutivo.Click += new System.EventHandler(this.btnVerTextoResolutivo_Click);
            // 
            // bgRealizarFirma
            // 
            this.bgRealizarFirma.Controls.Add(this.btnFirmarArchivos);
            this.bgRealizarFirma.Controls.Add(this.btnSalir);
            this.bgRealizarFirma.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bgRealizarFirma.Location = new System.Drawing.Point(531, 145);
            this.bgRealizarFirma.Name = "bgRealizarFirma";
            this.bgRealizarFirma.Size = new System.Drawing.Size(461, 77);
            this.bgRealizarFirma.TabIndex = 38;
            this.bgRealizarFirma.TabStop = false;
            this.bgRealizarFirma.Text = "Acciones";
            // 
            // btnFirmarArchivos
            // 
            this.btnFirmarArchivos.Image = ((System.Drawing.Image)(resources.GetObject("btnFirmarArchivos.Image")));
            this.btnFirmarArchivos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFirmarArchivos.Location = new System.Drawing.Point(304, 19);
            this.btnFirmarArchivos.Name = "btnFirmarArchivos";
            this.btnFirmarArchivos.Size = new System.Drawing.Size(153, 23);
            this.btnFirmarArchivos.TabIndex = 39;
            this.btnFirmarArchivos.Text = "Firmar archivos";
            this.btnFirmarArchivos.UseVisualStyleBackColor = true;
            this.btnFirmarArchivos.Click += new System.EventHandler(this.btnFirmarArchivos_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.Image")));
            this.btnSalir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalir.Location = new System.Drawing.Point(304, 48);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(153, 23);
            this.btnSalir.TabIndex = 38;
            this.btnSalir.Text = "Cerrar";
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // lvArchivos
            // 
            this.lvArchivos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvArchivos.Location = new System.Drawing.Point(0, 0);
            this.lvArchivos.MultiSelect = false;
            this.lvArchivos.Name = "lvArchivos";
            this.lvArchivos.Size = new System.Drawing.Size(530, 568);
            this.lvArchivos.TabIndex = 39;
            this.lvArchivos.UseCompatibleStateImageBehavior = false;
            // 
            // wfFirmaArchivos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(992, 568);
            this.Controls.Add(this.lvArchivos);
            this.Controls.Add(this.bgRealizarFirma);
            this.Controls.Add(this.gbSeleccionarRuta);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "wfFirmaArchivos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Firma Electrónica Avanzada - Archivos de video";
            this.panel2.ResumeLayout(false);
            this.gbSeleccionarRuta.ResumeLayout(false);
            this.bgRealizarFirma.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog fbdRuta;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox gbSeleccionarRuta;
        private System.Windows.Forms.GroupBox bgRealizarFirma;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.ListView lvArchivos;
        public System.Windows.Forms.Button btnFirmarArchivos;
        public System.Windows.Forms.Button btnVerTextoResolutivo;
        public System.Windows.Forms.Label label2;
    }
}