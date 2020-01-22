using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesquisaSatisfacaoEmail
{
    class OrdemServicoDTO
    {
        public string CodigoOS { get; set; }
        public string NumeroAgendamento { get; set; }
        public string DataInclusao { get; set; }
        public string DataFechamento { get; set; }
        public string DiasAprovado { get; set; }
        public string Placa { get; set; }
    }
}
