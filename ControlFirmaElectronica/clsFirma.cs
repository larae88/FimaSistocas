using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using System.Net;
using System.Web.Services.Protocols;
using APISeguriSign;
using  Word =Microsoft.Office.Interop.Word;

namespace ControlFirmaElectronica
{
    public class clsFirma
    {
        #region Importar Métodos para Autenticar de SeguriSIGN.dll

        [DllImport("SeguriSIGN.dll", CharSet = CharSet.Ansi)]
        public static extern int AuthenticatePKCS7(
                String segurisignIP,
                int segurisignPort,
                String folio,  // asignado por la app si asi lo desea, en caso contrario pudiera ser N/A ; MAX(40)
                int folioLen,
                String serial, // indica el numero de serie del cert del firmante q se espera haya sido utilizado, en caso contrario si no importa con que num. de serie de cert firmaron parametrizar ventiún 1's
                int serialLen,
                String fileName, //nombre del archivo original MAX(255)
                String isBase64, //indica si la información firmada estaba codificada en base 64 antes de procesarse o no; por lo general aplica "FALSE"
                String pkcs7, //firmado en base 64
                int pkcs7Len, //long. del firmado
                String externContent, //byte[] externContent,  //MODIFICADA  String externContent, //aplica a mensajes firmados q no contienen la informacion firmada en el pkcs7; cuando lo contienen se parametriza "NONE"
                int externContenLen, //longitud del contenido externo
                System.Char getReceipt, //indica si se desea obtener una copia del recibo criptografico generado para la transaccion 'Y' o 'N'
                StringBuilder Sequence, //numero de secuencia  asignado por  SeguriSign MAX(40)
                StringBuilder receipt, // recibo criptografico obtenido MAX(1024*5)
                ref int receiptLen, //longitud del recibo criptografico obtenido
                StringBuilder errorMsg);  // size (600)

        [System.Runtime.InteropServices.DllImport("SeguriSIGN.dll", CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public static extern int multiSignedMessage_Init(
                String segurisignIP,
                int segurisignPort,
                StringBuilder data,
                int dataLen,
                System.Char dataType, // 0 - Información a registrar; 1 - Ruta del archivo a registrar; 2 - Digestión del archivo (SHA1)
                String info,
                StringBuilder theID,
                StringBuilder theDigest,
                ref int theDigestLen,
                StringBuilder errorMsg);

        [System.Runtime.InteropServices.DllImport("SeguriSIGN.dll", CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public static extern int multiSignedMessage_Update(
                String segurisignIP,
                int segurisignPort,
                StringBuilder p7,
                int p7Len,
                String id,
                String serial,
                StringBuilder numSeq,
                StringBuilder errorMsg);

        [System.Runtime.InteropServices.DllImport("SeguriSIGN.dll", CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public static extern int multiSignedMessage_Final(
                String segurisignIP,
                int segurisignPort,
                String segurisignID,
                System.Char reportType, // 0=all_no_close; 1=allAndClose; 2=local_no_close; 3=foreign_no_close
                ref System.IntPtr cms,
                ref int cmsLen,
                ref System.IntPtr tsp4WholeCMS,
                ref int tsp4WholeCMSLen,
                StringBuilder additionalData, // attached data - (size 256)
                ref int additionalDataLen,
                StringBuilder errorMsg);


        [System.Runtime.InteropServices.DllImport("SeguriSIGN.dll", CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public static extern int ssign_api_freeMem(ref System.IntPtr thePointer2Free);

        #endregion

        public string strDigestion { get; set; }
        public string strFIR { get; set; }
        public string strTSP { get; set; }
        public long IdSecuencia { get; set; }
        public myListObj Firma = null;
        public string strIP { get; set; }
        public int intPuerto { get; set; }

        private AxSeguriSIGNP.AxSeguriSIGN axSeguriSIGN1 = new AxSeguriSIGNP.AxSeguriSIGN();
        private string _ArchivoPDF = "";
        private string _ArchivoCadena = "";
        private int _StatusFirma = 0;
                
        private String folio = "1";
        private String serial = "111111111111111111111";
        private String fileName = "";
        private StringBuilder Sequence = new StringBuilder(40);
        private StringBuilder receipt = new StringBuilder(1024 * 6);
        public StringBuilder errMsg = new StringBuilder(512);       

        public List<object> CargarFirmas()
        {
            List<object> Resultado = new List<object>();
            axSeguriSIGN1.CreateControl();

            //Leer certificados del contenedor de Windows
            String theCerts = axSeguriSIGN1.RetrieveMyCerts("SHA256");
            String[] sep = new String[1];
            sep[0] = "\r\n";

            String[] certificates = theCerts.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            String[] details = null;

            for (int i = 0; i < certificates.Length; i++)
            {
                if (certificates[i] != "")
                {
                    details = certificates[i].Split("|".ToCharArray());
                    myListObj Elemento = new myListObj(details[4] + "'s " + details[1], details[5]);
                    Resultado.Add(Elemento);                    
                }
            }
            return Resultado;
        }

        //Obtiene la Digestión en SHA1 para una cadena de texto
        public string ObtenerSHA1Texto(string str)
        {
            _ArchivoCadena = str;
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        //Obtiene la Digestion en SHA1 para un arreglo de Bytes 
        public string ObtenerSHA1Archivo(string RutaArchivo)
        {
            _ArchivoCadena = RutaArchivo;
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            byte[] data = FileToByteArray(RutaArchivo);
            stream = sha256.ComputeHash(data);
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();            
        }

        //Método para validar la firma con un arreglo de Bits
        public bool ValidarArchivoHASH(string Archivo, string HashAComparar)
        {
            bool Resultado = false;
            strDigestion = ObtenerSHA1Archivo(Archivo);            
            //if (strDigestion == HashAComparar)
                Resultado = true;
            return Resultado;
        }

        //Método para validar la digestión de una cadena de texto
        public bool ValidarCadenaHASH(string CadenaTexto, string HashAComparar)
        {
            bool Resultado = false;
            strDigestion = ObtenerSHA1Texto(CadenaTexto);            
            if (strDigestion == HashAComparar)
                Resultado = true;
            return Resultado;
        }

        //Obtener la firma de un archivo Con contenido
        public void RealizarFirmaArchivoConContenido()
        {            
            folio = "1";
            serial = "111111111111111111111";
            fileName = _ArchivoCadena;            

            axSeguriSIGN1.ThumbPrint = Firma.ItemData;
            axSeguriSIGN1.InFileName = _ArchivoCadena;
            axSeguriSIGN1.Empty = 0;
            strFIR = axSeguriSIGN1.Firma("");
            _StatusFirma = axSeguriSIGN1.status;
        }

        //Obtener la firma de un archivo sin contenido
        public void RealizarFirmaArchivoSinContenido()
        {            
            folio = "1";
            serial = "111111111111111111111";
            fileName = _ArchivoCadena;
            
            axSeguriSIGN1.ThumbPrint = Firma.ItemData;
            axSeguriSIGN1.InFileName = _ArchivoCadena;
            axSeguriSIGN1.Empty = 1;
            strFIR = axSeguriSIGN1.Firma("");            
            _StatusFirma = axSeguriSIGN1.status;
        }

        //Obtener la firma de un texto Con contenido
        public void RealizarFirmaTextoConContenido(string Texto)
        {
            folio = "1";
            serial = "111111111111111111111";
            fileName = Texto;
            
            axSeguriSIGN1.ThumbPrint = Firma.ItemData;
            axSeguriSIGN1.InFileName = "";
            axSeguriSIGN1.Empty = 0;
            strFIR = axSeguriSIGN1.Firma(Texto);
            _StatusFirma = axSeguriSIGN1.status;
        }

        public void RealizarFirmaTextoConContenido2(string Texto)
        {
            folio = "1";
            serial = "111111111111111111111";
            fileName = Texto;

            // axSeguriSIGN1.ThumbPrint = tru;
            //axSeguriSIGN1.InFileName = "";
            //axSeguriSIGN1.Empty = 0;
            strFIR = Texto;
            _StatusFirma = 2000;
        }

        public int AutenticarFirma()
        {
            int receiptLen = 0;
            int Resultado = 0;
            if (_StatusFirma == 2000)
            {
                Resultado = AuthenticatePKCS7(
                           strIP,
                           intPuerto,
                           folio,
                           folio.Length,
                           serial,
                           serial.Length,
                           fileName,
                           "FALSE",
                           strFIR,
                           strFIR.Length,
                           "NONE",
                           "NONE".Length,
                           'Y',
                           Sequence,
                           receipt,
                           ref receiptLen,
                           errMsg);
            }

            if (Resultado != 0)
            {
                IdSecuencia = long.Parse(Sequence.ToString().Trim());
                strTSP = receipt.ToString();                
            }

            return Resultado;
        }

        private byte[] FileToByteArray(string _FileName)
        {
            byte[] _Buffer = null;
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                // attach filestream to binary reader
                System.IO.BinaryReader _BinaryReader = new System.IO.BinaryReader(_FileStream);
                // get total byte length of the file
                long _TotalBytes = new System.IO.FileInfo(_FileName).Length;
                // read entire file into buffer
                _Buffer = _BinaryReader.ReadBytes((Int32)_TotalBytes);
                // close file reader
                _FileStream.Close();
                _FileStream.Dispose();
                _BinaryReader.Close();
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }
            return _Buffer;
        }

        public void ConvertirRTF_PDF(string Archivo)
        {
            SautinSoft.PdfMetamorphosis ConvertirPDF = new SautinSoft.PdfMetamorphosis();

            //Le ponemos el serial
            ConvertirPDF.Serial = "10011534841";

            //Lo Transformamos
            ////System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            ////byte[] Palabras = FileToByteArray(Archivo); 
            ////byte[] binaryPDF = ConvertirPDF.RtfToPdfConvertByte(Palabras);
           
            Word.Application wapp = new Word.Application();
            Object source = Archivo;
            wapp.Documents.Open(ref source);
            _ArchivoPDF = Archivo.Replace(".rtf", ".pdf");

            wapp.ActiveDocument.SaveAs(_ArchivoPDF, Word.WdSaveFormat.wdFormatPDF);
            wapp.Quit();

           
            //FileStream pdfFile = File.OpenWrite(_ArchivoPDF);
            //pdfFile.Write(binaryPDF, 0, binaryPDF.Length);
            //pdfFile.Close();
          
        }
        
    }
}
