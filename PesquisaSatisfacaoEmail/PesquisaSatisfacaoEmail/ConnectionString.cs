using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesquisaSatisfacaoEmail
{
    class ConnectionString
    {
        public static String connectionString = "Server=" + Properties.Settings.Default.Servidor + "" +
                        ";Database=" + Properties.Settings.Default.NomeBanco + "" +
                        ";Uid=" + Properties.Settings.Default.Usuario + "" +
                        ";Pwd=" + Properties.Settings.Default.Senha + "" + ";";
    }
}
