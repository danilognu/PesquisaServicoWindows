using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace PesquisaSatisfacaoEmail
{
    class OrdemServicoRepository
    {

        public List<OrdemServicoDTO> VerificaOSParaEnvio()
        {

            using (SqlConnection conexao = new SqlConnection(ConnectionString.connectionString))
            {

                try
                {

                    List<OrdemServicoDTO> listOrdemServicoDTO = new List<OrdemServicoDTO>();

                    //OS já finalizadas a 2 dias.
                    string _sqlVerificaOS;
                    _sqlVerificaOS = " SELECT " +
                                    "        T513.A513_num_agendamento " +
                                    "        ,T124.A124_dta_inclusao " +
                                    "        ,T124.A124_dta_fechamento " +
                                    "        ,DATEDIFF(DAY,T124.A124_dta_fechamento,GETDATE()) as DiasAprovacao " +
                                    "        ,T124.A124_cod_ord_serv " +
                                    "        ,T009.A009_placa_veiculo " +
                                    "    FROM T124_ORDEM_SERV T124 " +
                                    "    INNER JOIN T513_AGENDAMENTO T513 ON T513.A513_cod_agendamento = T124.A513_cod_agendamento " +
                                    "    INNER JOIN T009_VEICULO T009 ON T009.A009_cd_veiculo = T513.A009_cd_veiculo  " +
                                    "    WHERE " +
                                    "        DATEDIFF(DAY,T124.A124_dta_fechamento,GETDATE()) = 6 ";
                                    /*"        AND (SELECT COUNT(S.A513_num_agendamento) FROM T654_RESPOSTA_PESQUISA_SATISFACAO S WHERE S.A513_num_agendamento = T513.A513_num_agendamento ) = 0 " +
                                    "        AND CONVERT(DATE,T124.A124_dta_inclusao) >= ( SELECT TOP 1 CONVERT(DATE,A652_dt_liberecao_email_for) FROM T652_PERGUNTA_PESQUISA_SATISFACAO WHERE A653_cd_tipo_pergunta = 2 AND A652_ind_ativo = 1 AND ISNULL(A652_libera_envio,0) = 1 ) " +
                                    "        AND A513_manualmente = 0  " +
                                    "        AND T124.A124_ind_pesquisa_envida IS NULL " +
                                    "    ORDER BY T513.A513_num_agendamento DESC ";*/
                    conexao.Open();
                    SqlCommand cmd = new SqlCommand(_sqlVerificaOS, conexao);
                    SqlDataReader resultVerificaOS = cmd.ExecuteReader();

                    while (resultVerificaOS.Read())
                    {

                        OrdemServicoDTO ordemServico = new OrdemServicoDTO();
                        ordemServico.NumeroAgendamento = resultVerificaOS.GetValue(0).ToString();
                        ordemServico.DataInclusao = resultVerificaOS.GetValue(1).ToString();
                        ordemServico.DataFechamento = resultVerificaOS.GetValue(2).ToString();
                        ordemServico.DiasAprovado = resultVerificaOS.GetValue(3).ToString();
                        ordemServico.CodigoOS = resultVerificaOS.GetValue(4).ToString();
                        ordemServico.Placa = resultVerificaOS.GetValue(5).ToString();
                        listOrdemServicoDTO.Add(ordemServico);
                    }

                    return listOrdemServicoDTO;

                }
                catch (Exception e)
                {
                    Utils.WriteLogArchive(e.Message.ToString());
                    throw;
                }

            }
        }

        public List<OrdemServicoDTO> CountOSOpenEnded(OrdemServicoDTO dataOS)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {

                    List<OrdemServicoDTO> listOrdemServicoDTO = new List<OrdemServicoDTO>();

                    string _sqlCountOS = " SELECT " +
                                        " ( " +
                                        "    SELECT COUNT(*) conta_os  " +
                                        "    FROM T124_ORDEM_SERV  " +
                                        "    WHERE A513_cod_agendamento IN( " +
                                        "        SELECT A513_cod_agendamento  " +
                                        "        FROM T513_AGENDAMENTO  " +
                                        "        WHERE A513_num_agendamento = T513.A513_num_agendamento  " +
                                        "    )  " +
                                        " ) as contaOS  " +
                                        " ,(  " +
                                        "    SELECT COUNT(*) conta_os  " +
                                        "    FROM T124_ORDEM_SERV   " +
                                        "    WHERE A513_cod_agendamento IN(  " +
                                        "        SELECT A513_cod_agendamento   " +
                                        "        FROM T513_AGENDAMENTO   " +
                                        "        WHERE A513_num_agendamento = T513.A513_num_agendamento " +
                                        "    ) " +
                                        "    AND A124_dta_fechamento IS NOT NULL " +
                                        " ) as contaFinalizadasOS " +
                                        " FROM T513_AGENDAMENTO T513 " +
                                        " WHERE T513.A513_num_agendamento = " + dataOS.NumeroAgendamento + "" +
                                        " GROUP BY T513.A513_num_agendamento ";

                    conexao.Open();
                    SqlCommand cmd = new SqlCommand(_sqlCountOS, conexao);
                    SqlDataReader resultCountOS = cmd.ExecuteReader();

                    while (resultCountOS.Read())
                    {

                        int _countOS = Convert.ToInt32(resultCountOS.GetValue(0).ToString());
                        int _countOSFinished = Convert.ToInt32(resultCountOS.GetValue(1).ToString());

                        if (_countOS == _countOSFinished)
                        {
                            OrdemServicoDTO ordemServico = new OrdemServicoDTO();
                            ordemServico.CodigoOS = dataOS.CodigoOS;
                            listOrdemServicoDTO.Add(ordemServico);
                        }


                    }

                    return listOrdemServicoDTO;


                }
                catch (Exception e)
                {
                    Utils.WriteLogArchive(e.Message.ToString());
                    throw;
                }
            }


        }

        public string SearchEmail(int codeOS)
        {
            string _emailUser = "";

            using (SqlConnection conexao = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    string _Sql = " SELECT TOP 1 " +
                                "	T088.A088_email  " +
                                " FROM T513_AGENDAMENTO  " +
                                "	INNER JOIN T088_MOTORISTA T088 ON T088.A088_cd_motorista = T513_AGENDAMENTO.A088_cd_motorista_contato  " +
                                " WHERE A124_cod_ord_serv = " + codeOS + "  " +
                                " AND A009_cd_veiculo = (SELECT A009_cd_veiculo FROM T124_ORDEM_SERV WHERE A124_cod_ord_serv =  " + codeOS + ") ";

                    conexao.Open();
                    SqlCommand cmd = new SqlCommand(_Sql, conexao);
                    SqlDataReader result = cmd.ExecuteReader();

                    while (result.Read())
                    {
                        _emailUser = result.GetValue(0).ToString();
                    }

                    return _emailUser;


                }
                catch (Exception e)
                {
                    Utils.WriteLogArchive(e.Message.ToString());
                    throw;
                }
            }

        }

        public string SearchEmailCopy(int codeOS)
        {
            string _emailUser = "";

            using (SqlConnection conexao = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    string _Sql = " SELECT ISNULL(A513_email_cc,'') A513_email_cc FROM T513_AGENDAMENTO  " +
                                  "	WHERE A513_num_agendamento in(SELECT T513.A513_num_agendamento FROM T124_ORDEM_SERV T124  " +
                                  " 	INNER JOIN T513_AGENDAMENTO T513 ON T513.A513_cod_agendamento = T124.A513_cod_agendamento " +
                                  "   WHERE T124.A124_cod_ord_serv =  " + codeOS + ")  " +
                                  "   AND A513_ind_ativo = 1 ";
                    conexao.Open();
                    SqlCommand cmd = new SqlCommand(_Sql, conexao);
                    SqlDataReader result = cmd.ExecuteReader();

                    while (result.Read())
                    {

                        if (String.IsNullOrEmpty(result.GetValue(0).ToString()) == false)
                            _emailUser = result.GetValue(0).ToString();
                    }

                    return _emailUser;


                }
                catch (Exception e)
                {
                    Utils.WriteLogArchive(e.Message.ToString());
                    throw;
                }
            }
        }

        public string SearchEmailCopy2(int codeOS)
        {
            string _emailUser = "";

            using (SqlConnection conexao = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    string _Sql = " SELECT ISNULL(T088.A088_email,'') A088_email FROM T513_AGENDAMENTO T513  " +
                            "		LEFT JOIN T088_MOTORISTA T088 ON T088.A088_cd_motorista = T513.A088_cd_motorista    " +
                            "	WHERE   " +
                            "		T513.A513_num_agendamento IN( " +
                            "			SELECT T513.A513_num_agendamento FROM T124_ORDEM_SERV T124  " +
                            "				INNER JOIN T513_AGENDAMENTO T513 ON T513.A513_cod_agendamento = T124.A513_cod_agendamento " +
                            "			WHERE T124.A124_cod_ord_serv =  " + codeOS + " " +
                            "		)    " +
                            "	AND T513.A513_ind_ativo = 1 ";
                    conexao.Open();
                    SqlCommand cmd = new SqlCommand(_Sql, conexao);
                    SqlDataReader result = cmd.ExecuteReader();

                    while (result.Read())
                    {
                        if (String.IsNullOrEmpty(result.GetValue(0).ToString()) == false)
                             _emailUser = result.GetValue(0).ToString();
                    }

                    return _emailUser;


                }
                catch (Exception e)
                {
                    Utils.WriteLogArchive(e.Message.ToString());
                    throw;
                }
            }

        }


    }
}
