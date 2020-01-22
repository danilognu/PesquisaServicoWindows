using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;

namespace PesquisaSatisfacaoEmail
{
    class OrdemServico
    {

        public void BuscaDadosOrdemServico()
        {
            try
            {
                OrdemServicoRepository ordemServicoRepository = new OrdemServicoRepository();
                List<OrdemServicoDTO> ordemServicos = ordemServicoRepository.VerificaOSParaEnvio();

                string _emailForn,_emailCopy,_emailCopy2;

                foreach (OrdemServicoDTO ordemServico in ordemServicos)
                {
                    //Utils.WriteLogArchive(ordemServico.CodigoOS);
                    List<OrdemServicoDTO> ordemServicoFinalizadas = ordemServicoRepository.CountOSOpenEnded(ordemServico);

                    OrdemServicoDTO ordemServicoSend = new OrdemServicoDTO();

                    foreach (OrdemServicoDTO ordemServicoFinalizada in ordemServicoFinalizadas)
                    {                        
                        ordemServicoSend.Placa = ordemServico.Placa;
                        ordemServicoSend.NumeroAgendamento = ordemServico.NumeroAgendamento;

                        //Search e-mail to send
                        int _codOS = Convert.ToInt32(ordemServicoFinalizada.CodigoOS);

                        _emailForn = ordemServicoRepository.SearchEmail(_codOS);

                        if (String.IsNullOrEmpty(_emailForn) == false)
                        {
                            _emailCopy = ordemServicoRepository.SearchEmailCopy(_codOS);
                            _emailCopy2 = ordemServicoRepository.SearchEmailCopy2(_codOS);

                            if (String.IsNullOrEmpty(_emailCopy) == false)
                                _emailForn += ";" + _emailCopy;

                            if (String.IsNullOrEmpty(_emailCopy2) == false)
                                _emailForn += ";" + _emailCopy2;

                            this.SendEmail(ordemServicoSend, _emailForn);
                        }

                    }
                }


            }
            catch(Exception e)
            {
                Utils.WriteLogArchive(e.Message.ToString());
            }
        }

        public void SendEmail(OrdemServicoDTO ordemServico, string emailSend)
        {

            emailSend = "danilo.souza@lets.com.br";

            string _Html = "<span style='font-family:Arial, Helvetica, sans-serif; font-size: 13px;'> " +
                            "  Paix&atilde;o pelo melhor significa melhorarmos nossos servi&ccedil;os a cada dia e por isso que <br />  " +
                            "  gostar&iacute;amos que avaliasse a qualidade da &uacute;ltima manuten&ccedil;&atilde;o realizada na placa " + ordemServico.Placa + ". <br /> <br /> " +
                            "  <a href='" + Properties.Settings.Default.LinkSistema + "?numAgendamento=" + ordemServico.NumeroAgendamento + "&tipoPergunta=2' style='color:#FF8C00'> Clique aqui </a>  " +
                            " </a> para digitar a nota que voc&ecirc; atribui a qualidade da manuten&ccedil;&atilde;o do seu ve&iacute;culo. " +
                            " </span> ";

            Utils.WriteLogArchive(emailSend);
            Utils.WriteLogArchive(_Html);
            Utils.WriteLogArchive("=====================================");

            SmtpClient client = new SmtpClient();

            Utils.WriteLogArchive("Host: " + Properties.Settings.Default.Host);
            Utils.WriteLogArchive("Porta:" + Properties.Settings.Default.Porta);
            Utils.WriteLogArchive("EmailHost:" + Properties.Settings.Default.EmailHost);
            Utils.WriteLogArchive("SenhaEmailHost:" + Properties.Settings.Default.SenhaEmailHost);

            client.Host = Properties.Settings.Default.Host;
            client.EnableSsl = true;
            client.Port = Convert.ToInt32(Properties.Settings.Default.Porta);
            client.Credentials = new NetworkCredential(Properties.Settings.Default.EmailHost, Properties.Settings.Default.SenhaEmailHost);
            MailMessage mail = new MailMessage();

            //Attachment att = new Attachment(@"" + Properties.Settings.Default.caminhoRelatorio);
            //mail.Attachments.Add(att);

            mail.Sender = new MailAddress(Properties.Settings.Default.EmailHost, "ENVIADOR");
            mail.From = new MailAddress("suporteaocliente@lets.com.br", "ENVIADOR");
            mail.To.Add(new MailAddress(emailSend, "RECEBEDOR"));
            mail.Subject = "Pesquisa Continuada Lets <suporteaocliente@lets.com.br>";
            mail.Body = _Html;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            try
            {
                client.Send(mail);
                Utils.WriteLogArchive("Email Enviado");
            }
            catch (Exception e)
            {
                Utils.WriteLogArchive("Email Erro: " + e.Message);
            }

        }

    }
}
