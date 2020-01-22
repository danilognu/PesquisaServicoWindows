using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PesquisaSatisfacaoEmail
{
    class Utils
    {

        public static void EscreveLogArquivo(string mensagem)
        {
            StreamWriter vWriter = new StreamWriter(@"" + Properties.Settings.Default.ArquivoLog, true);
            vWriter.WriteLine(mensagem);
            vWriter.Flush();
            vWriter.Close();
        }

        public static void WriteLogArchive(string text)
        {
            StreamWriter vWriter = new StreamWriter(@"" + Properties.Settings.Default.ArquivoLog, true);
            vWriter.WriteLine(text);
            vWriter.Flush();
            vWriter.Close();
        }

    }
}
