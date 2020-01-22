using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PesquisaSatisfacaoEmail
{
    public partial class Service1 : ServiceBase
    {

        DateTime date = DateTime.Now;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Utils.WriteLogArchive("Serviço Pesquisa Iniciada: " + date.ToString("MM/dd/yyyy HH:mm"));

            OrdemServico ordemServico = new OrdemServico();
            ordemServico.BuscaDadosOrdemServico();

        }

        protected override void OnStop()
        {
            Utils.WriteLogArchive("Serviço Pesquisa Parado: " + date.ToString("MM/dd/yyyy HH:mm"));
        }
    }
}
