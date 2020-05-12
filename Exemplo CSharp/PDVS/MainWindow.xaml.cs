//Change log

//Data              Responsavel         Versão      Mudança
//18/10/18          Thiago Santos       3.0         1) Ajustes gerais
//
//16/10/18          Thiago Santos       2.0         1) Nova funcionalidade, de captura de dado via PINPad
//
//10/06/18          Thiago Santos       1.0         1) Versão inicial da aplicação

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using static PGWLib.CustomObjects;
using static PGWLib.Enums;

namespace PDVS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PGWLib.PGWLib eft;

        public MainWindow()
        {
            InitializeComponent();

            // Inicializa a instancia de acesso a biblioteca
            eft = new PGWLib.PGWLib();

            // Preenche o combo com as opções de operação
            foreach (string item in Enum.GetNames(typeof(E_PWOPER)))
            {
                cmbOper.Items.Add(item);
            }

            // Define a operação de venda como default
            cmbOper.SelectedItem = E_PWOPER.PWOPER_SALE.ToString();

            AddMandatoryParameters();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        // Adiciona um novo parâmetro antes do início da transação
        private void btnAddParam_Click(object sender, RoutedEventArgs e)
        {
            OpenParamWindow();
        }

        // Remove um parâmetro já adicionado
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lstParameters.SelectedIndex == -1) 
                return;
            lstParameters.Items.RemoveAt(lstParameters.SelectedIndex);
        }

        // Edita um parâmetro já adicionado
        private void lstParameters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstParameters.SelectedIndex == -1)
                return;
            PW_Parameter param = (PW_Parameter)lstParameters.SelectedItem;
            OpenParamWindow(param.parameterName, param.parameterValue, lstParameters.SelectedIndex);
        }

        // Limpa todos os parâmetros não obrigatórios 
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lstParameters.Items.Clear();
            AddMandatoryParameters();
        }

        // Executa uma transação 
        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {

            if (!ValidateFields())
            {
                MessageBox.Show(this, "Preencha os campos corretamente.");
                return;
            }

            ExecuteTransaction();
        }

        // Faz a captura de um dado digitado no PIN-pad
        private void btnCaptureWithPinpad_Click(object sender, RoutedEventArgs e)
        {
            string param = "";
            short minLength = 0;
            short maxLength = 0;

            // Cria a janela para obter as característica de captura do dado
            SelectUserDataMessageWindow selectUserDataWindow = new SelectUserDataMessageWindow();
            selectUserDataWindow.ShowDialog(ref param, ref minLength, ref maxLength);

            if (minLength > 0)
            {
                E_PWUserDataMessages message = (E_PWUserDataMessages)Enum.Parse(typeof(E_PWUserDataMessages), param);

                string userTypedValue = "";

                // Executa a captura
                int ret = eft.GetInputFromPP(ref userTypedValue, message, minLength, maxLength);
                if (ret != 0)
                {
                    MessageBox.Show(this, string.Format("Erro ao executar a captura de dado no PINPad: {0}{1}{2}", ret, Environment.NewLine, ((E_PWRET)ret).ToString()));
                    return;
                }

                MessageBox.Show(this, string.Format("Dado capturado no PINPad: {0}{1}", Environment.NewLine, userTypedValue));

            }
        }

        // Exibe uma mensagem no PIN-pad
        private void btnMsgPinpad_Click(object sender, RoutedEventArgs e)
        {
            string message = "";

            // Cria a janela para capturar a mensagem a ser exibida
            TypeDisplayMessageWindow typeDisplayWindow = new TypeDisplayMessageWindow();
            if (typeDisplayWindow.ShowDialog(ref message))
            {

                // Exibe a mensagem
                int ret = eft.DisplayOnPP(message);
                if (ret != 0)
                {
                    MessageBox.Show(this, string.Format("Erro ao exibir mensagem no PINPad: {0}{1}{2}",
                        ret, Environment.NewLine, ((E_PWRET)ret).ToString()));
                    return;
                }

                MessageBox.Show(this, string.Format("Mensagem exibida !!!"));
            }
        }

        // Captura uma senha no PIN-pad, retornando o identificador dela criptografado
        private void btnGetPINBlock_Click(object sender, RoutedEventArgs e)
        {
            string cripto = "";

            // Executa a captura da senha no PIN-pad, retornando o criptograma gerado, baseado nas
            // informações passadas como parâmetros:
            // keyID: 12 (chave SETIS)
            // workingKey: "12345678901234567890123456789012" (valor aleatório)
            int ret = eft.GetCriptoOnPP(12, "12345678901234567890123456789012", 6, 6,
                " DIGITE A SENHA " + "   NO PINPAD    ", ref cripto);
            if (ret != 0)
            {
                MessageBox.Show(this, string.Format("Erro ao executar a captura no PINPad: {0}{1}{2}",
                    ret, Environment.NewLine, ((E_PWRET)ret).ToString()));
                return;
            }

            MessageBox.Show(this, string.Format("Criptograma da senha digitada: {0}", cripto));
        }

        // Aguarda qualquer evento que ocorra no PIN-pad
        private void btnWaitEvent_Click(object sender, RoutedEventArgs e)
        {
            E_PWPPEVTIN eventoEntrada = E_PWPPEVTIN.PWPPEVTIN_ICC | E_PWPPEVTIN.PWPPEVTIN_ICCOUT |
                E_PWPPEVTIN.PWPPEVTIN_CTLS | E_PWPPEVTIN.PWPPEVTIN_KEYS | E_PWPPEVTIN.PWPPEVTIN_MAG;
            E_PWPPEVT eventoSaida;

            // Inicia a espera por todos os eventos possíveis
            eventoSaida = eft.WaitEventOnPP(eventoEntrada);
            if (eventoSaida != E_PWPPEVT.PWPPEVT_NONE)
            {
                MessageBox.Show(this, string.Format("Evento acionado={0}", eventoSaida.ToString()));
                return;
            }
        }

        // Obtém todas as operações disponíveis na biblioteca
        private void btnGetOperations_Click(object sender, RoutedEventArgs e)
        {
            List<PW_Operations> operations = new List<PW_Operations>();

            // Obtem os resultado da biblioteca
            operations = eft.GetOperations();
           
            // Limpa os resultados da transação anterior
            listBox.Items.Clear();

            // Escreve na janela
            foreach (PW_Operations item in operations)
            {
                listBox.Items.Add(string.Format("Tipo={0} Nome={1} Valor={2}",
                    item.bOperType.ToString(), item.szText.PadRight(20), item.szValue));
            }
        }

        // Adiciona todos os parâmetros obrigatórios para iniciar uma transação
        private void AddMandatoryParameters()
        {
            int autCap;

            lstParameters.Items.Add(new PW_Parameter(E_PWINFO.PWINFO_AUTNAME.ToString(), (int)E_PWINFO.PWINFO_AUTNAME, "PDVS"));
            lstParameters.Items.Add(new PW_Parameter(E_PWINFO.PWINFO_AUTVER.ToString(), (int)E_PWINFO.PWINFO_AUTVER, "1.0"));
            lstParameters.Items.Add(new PW_Parameter(E_PWINFO.PWINFO_AUTDEV.ToString(), (int)E_PWINFO.PWINFO_AUTDEV, "PayGo Pagamentos"));

            autCap = (int)E_PWAutCapabilities.FIXO + (int)E_PWAutCapabilities.CUPOMRED + (int)E_PWAutCapabilities.CUPOMDIF;
            lstParameters.Items.Add(new PW_Parameter(E_PWINFO.PWINFO_AUTCAP.ToString(), (int)E_PWINFO.PWINFO_AUTCAP, autCap.ToString()));  
        }

        // Função auxiliar para adição/edição de novo parâmetro
        private void OpenParamWindow(string param = "", string value = "", int itemPosition = -1)
        {
            AddParamWindow addParamWindow = new AddParamWindow(param, value);
            addParamWindow.ShowDialog(ref param, ref value);

            if (!string.IsNullOrEmpty(param) & !string.IsNullOrEmpty(value) & !string.IsNullOrWhiteSpace(value))
            {
                PW_Parameter parameterObject = new PW_Parameter();

                E_PWINFO receivedParam = (E_PWINFO)Enum.Parse(typeof(E_PWINFO), param);

                parameterObject.parameterName = param;
                parameterObject.parameterCode = (ushort)receivedParam;
                parameterObject.parameterValue = value;

                foreach (PW_Parameter item in lstParameters.Items)
                {
                    if (item.parameterName == parameterObject.parameterName && itemPosition == -1)
                    {
                        MessageBox.Show(this, "Você não pode inserir duas vezes o mesmo parâmetro");
                        return;
                    }
                }

                if (itemPosition != -1) 
                    lstParameters.Items.RemoveAt(itemPosition);
                lstParameters.Items.Add(parameterObject);
            }
        }

        // Valida os campos antes de executar uma transação
        private bool ValidateFields()
        {
            if (cmbOper.SelectedIndex == -1) 
                return false;
            if (lstParameters.Items.Count == 0) 
                return false;
            return true;
        }

        // Executa uma transação
        private int ExecuteTransaction()
        {
            E_PWOPER operation = (E_PWOPER)Enum.Parse(typeof(E_PWOPER), cmbOper.SelectedValue.ToString());
            List<PW_Parameter> paramList;
            int ret;
            string resultMessage;
            PW_Parameter param;
            E_PWCNF transactionStatus;

            // Executa a transação
            ret = eft.StartTransaction(operation, lstParameters.Items.Cast<PW_Parameter>().ToList());

            // Obtem todos os resultados da transação
            paramList = GetTransactionResult();

            // Caso a operação tenha sido cancelada, obtém a mensagem a ser exibida nesse caso
            if(ret==(int)E_PWRET.PWRET_CANCEL)
                param = paramList.Find(item => item.parameterCode == (ushort)E_PWINFO.PWINFO_CNCDSPMSG);
            else    
                param = paramList.Find(item => item.parameterCode == (ushort)E_PWINFO.PWINFO_RESULTMSG);

            // Caso não seja possível obter uma mensagem de resultado da biblioteca, atribui uma padrão
            if (param != null)
                resultMessage = param.parameterValue;
            else
                resultMessage = "TRANSACAO FINALIZADA";

            // Exibe a mensagem de resultado, substituindo a quebra de linha utilizada
            // pela biblioteca pela quebra de linha utilizada na janela
            MessageBox.Show(this, resultMessage.Replace("\r", "\n"));

            // Transação com erro
            if (ret != 0)
            {
                // A última transação não foi confirmada corretamente
                if (ret == (int)E_PWRET.PWRET_FROMHOSTPENDTRN)
                {
                    // Captura do usuário o que ele deseja fazer com a transação que ficou com problema de integridade
                    // Esse é um ponto que vai ser atingido somente se ocorrer algum erro de tratamento da automação
                    // ou um erro grave de sistema e uma nova transação não poderá ser realizada até que
                    // a última seja resolvida
                    // A captura do que deve ser feita do usuário é somente um exemplo, é possível a automação
                    // obter essa informação em seu banco de dados e saber se a transação deve ser confirmada ou desfeita
                    
                    // Obtem os identificadores da transação pendente
                    PW_Parameter authSyst, virtMerch, reqNum, autLocRef, autExtRef;
                    authSyst = paramList.Find(item => item.parameterCode == (ushort)E_PWINFO.PWINFO_PNDAUTHSYST);
                    virtMerch = paramList.Find(item => item.parameterCode == (ushort)E_PWINFO.PWINFO_PNDVIRTMERCH);
                    reqNum = paramList.Find(item => item.parameterCode == (ushort)E_PWINFO.PWINFO_PNDREQNUM);
                    autLocRef = paramList.Find(item => item.parameterCode == (ushort)E_PWINFO.PWINFO_PNDAUTLOCREF);
                    autExtRef = paramList.Find(item => item.parameterCode == (ushort)E_PWINFO.PWINFO_PNDAUTEXTREF);

                    // Exibe uma mensagem identificando a transação que está pendente
                    MessageBox.Show(this, string.Format("Existe uma transação pendente:\n" +
                        "PNDAUTHSYST={0}\n" +
                        "NDVIRTMERCH={1}\n" +
                        "PNDREQNUM={2}\n" +
                        "PNDAUTLOCREF={3}\n" +
                        "PNDAUTEXTREF={4}\n" +
                        "Será necessário resolvê-la !!!", 
                        authSyst==null ? "" : authSyst.parameterValue,
                        virtMerch == null ? "" : virtMerch.parameterValue,
                        reqNum == null ? "" : reqNum.parameterValue,
                        autLocRef == null ? "" : autLocRef.parameterValue,
                        autExtRef == null ? "" : autExtRef.parameterValue));                    

                    // Pergunta ao usuário qual status de confirmação atribuir para a transação
                    ConfirmationWindow cw = new ConfirmationWindow();
                    transactionStatus = E_PWCNF.PWCNF_REV_ABORT;
                    cw.ShowDialog(ref transactionStatus);

                    // Executa a resolução de pendencia
                    ConfirmUndoTransaction(paramList, transactionStatus, true);
                }
            }
            // Transação com sucesso
            else
            {
                // Verifica se é necessário confirmar a transação
                PW_Parameter confirmacaoNecessaria;
                confirmacaoNecessaria = paramList.Find(item => item.parameterCode == (ushort)E_PWINFO.PWINFO_CNFREQ);
                if (confirmacaoNecessaria != null && int.Parse(confirmacaoNecessaria.parameterValue) == 1)
                {
                    // Pergunta ao usuário qual status de confirmação atribuir para a transação
                    ConfirmationWindow cw = new ConfirmationWindow();
                    transactionStatus = E_PWCNF.PWCNF_REV_ABORT;
                    cw.ShowDialog(ref transactionStatus);

                    // Executa a resolução de pendencia
                    ConfirmUndoTransaction(paramList, transactionStatus);
                }
            }

            // *** IMPORTANTE:
            // Sempre após a finalização de uma transação, é necessário verificar o conteúdo
            // da informação PWINFO_IDLEPROCTIME e agendar uma chamada automática da função
            // PW_iIdleProc no horário apontado por ela
            // Essa implementação é necessária para que a biblioteca seja acionada de tempos
            // em tempos, de acordo com a sua necessidade, para resolver pendências com
            // o sistema, mesmo que nenhuma nova transação seja feita
            // PW_Parameter idleProc = paramList.Find(item => item.parameterCode == (ushort)E_PWINFO.PWINFO_IDLEPROCTIME);

            return ret;
        }

        // Pega todos os resultados da transação e loga na tela
        private List<PW_Parameter> GetTransactionResult()
        {
            List<PW_Parameter> results = new List<PW_Parameter>();
            
            // Obtem os resultado da biblioteca
            results = eft.GetTransactionResult();
           
            // Limpa os resultados da transação anterior
            listBox.Items.Clear();

            // Escreve na janela
            foreach (PW_Parameter item in results)
            {
                // Se é recibo quebra linha por linha e insere no listbox
                // para resolver bug de scroll dos recibos.
                if ((item.parameterCode == (ushort)E_PWINFO.PWINFO_RCPTCHOLDER) ||
                     (item.parameterCode == (ushort)E_PWINFO.PWINFO_RCPTCHSHORT) ||
                     (item.parameterCode == (ushort)E_PWINFO.PWINFO_RCPTFULL) ||
                     (item.parameterCode == (ushort)E_PWINFO.PWINFO_RCPTMERCH) ||
                     (item.parameterCode == (ushort)E_PWINFO.PWINFO_RCPTPRN)
                   )
                {
                    string _input = item.ToString();

                    using (StringReader reader = new StringReader(_input))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            listBox.Items.Add(line);
                        }
                    }
                }
                else
                {
                    listBox.Items.Add(item.ToString());
                }
            }

            return results;
        }

        // Resolve a pendência de uma transação
        private int ConfirmUndoTransaction(List<PW_Parameter> transactionResult, E_PWCNF transactionStatus, bool isPending=false)
        {
            int ret;

            if (isPending)
                ret = eft.ConfirmUndoPendingTransaction(transactionStatus, transactionResult);
            else
                ret = eft.ConfirmUndoNormalTransaction(transactionStatus, transactionResult);

            return ret;

        }
    }
}

