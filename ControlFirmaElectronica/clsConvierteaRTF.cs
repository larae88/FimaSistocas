using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using System.IO;
using System.Runtime.InteropServices;

namespace Prueba_rtf
{
    class clsConviertea_RTF
    {
        //Microsoft.Office.Interop.Word.a wrdApp = new Microsoft.Office.Interop.Word.Application();

        Microsoft.Office.Interop.Word.Application wrdApp;



        public Boolean ConvierteTexto(string Ruta, string texto)
        {
            Boolean EjecutaQuit = false;

            try
            {
                wrdApp = (Microsoft.Office.Interop.Word.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Word.Application");
            }
            catch (Exception ex)
            {
                wrdApp = new Microsoft.Office.Interop.Word.Application();
                EjecutaQuit = true;

            }

            try
            {
                //Genera un Archivo tipo html en la ruta q pase el piter
                string strfileName = Ruta + "\\Texto";
                if (System.IO.File.Exists(strfileName + ".html"))
                    System.IO.File.Delete(strfileName + ".html");

                FileStream stream = new FileStream(strfileName + ".html", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8);


                writer.WriteLine(texto);
                writer.Close();
                //Oculta mensajes
                wrdApp.Application.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsNone;
                //wrdApp.Application.DisplayAlerts = 0; 
                //abre el archivo  html y lo carga en word para posteriormente  guardarlo como rtf
                object missing = System.Reflection.Missing.Value;

                //Get the reference to Word.Application from the ROT.


                //Display the name.



                wrdApp.Documents.Open(strfileName + ".html");

                object filename = Ruta + "\\metamorfosis\\Texto.rtf";

                if (System.IO.File.Exists(strfileName + ".rtf"))
                {
                    System.IO.File.Delete(strfileName + ".rtf");
                }

                wrdApp.Application.ActiveDocument.SaveAs(ref filename, 6, ref missing, ref missing, ref missing, ref missing,
                                                            ref missing, ref missing, ref missing, ref missing, ref missing,
                                                            ref missing, ref missing, ref missing, ref missing, ref missing);

                System.IO.File.Copy(filename.ToString(), strfileName + ".rtf", true);
                wrdApp.Application.ActiveDocument.Close();

                if (EjecutaQuit == true)
                {
                    wrdApp.Application.Quit();
                    //wrdApp.Application.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsAll;
                    wrdApp = null;
                }


                return true;
            }
            catch (Exception ex)
            {
                wrdApp.Application.ActiveDocument.Close();
                if (EjecutaQuit == true)
                {

                    wrdApp.Application.Quit();
                    //wrdApp.Application.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsAll;
                    wrdApp = null;
                }

                return false;
            }
        }
    }
}
