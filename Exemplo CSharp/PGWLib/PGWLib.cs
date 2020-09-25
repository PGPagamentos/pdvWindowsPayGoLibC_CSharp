using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PGWLib.CustomObjects;
using static PGWLib.Enums;

namespace PGWLib
{
    public class PGWLib
    {
        public PGWLib()
        {
            // Cria o diretório que será utilizado pela função PW_iInit
            System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\PGWebLib\\");
            String path = "";
            path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\PGWebLib\\";

            // Inicializa a biblioteca, indicando a pasta de trabalho a ser utilizada para gravação
            // de logs e arquivos
            int ret = Interop.PW_iInit(path);

            // Caso ocorra um erro no processo de inicialização da biblioteca, dispara uma exceção
            if (ret != (int)E_PWRET.PWRET_OK)
                throw new System.Exception(string.Format("Erro {0} ao executar PW_iInit", ret.ToString()));
        }

        #region MÉTODOS PUBLICOS

        // Executa a uma operação utilizando uma lista de parâmetros pre definidos pela automação
        public int StartTransaction(E_PWOPER operation, List<PW_Parameter> paramList)
        {
            int ret;

            // Antes de iniciar uma nova transação, resolve possíveis pendência existentes
            ret = PendencyResolve();

            // Caso ocorra algum problema na resolução de pendência retorna o erro
            if (ret != (int)E_PWRET.PWRET_OK)
                return ret;

            // Inicializa a transação solicitada
            ret = Interop.PW_iNewTransac((byte)operation);

            // Caso ocorra algum problema na inicialização retorna o erro
            if (ret != (int)E_PWRET.PWRET_OK)
                return ret;

            // Loop adicionando todos os parâmetros já capturados pela automação
            // Parâmetros já capturados devem ser adicionados, caso a biblioteca precise de qualquer
            // parâmetro que não foi recebido para finalizar a transação, ela irá solicitá-lo no fluxo
            // implementado em "executeTransaction"
            foreach (PW_Parameter item in paramList)
            {
                // Adiciona o parâmetro
                ret = Interop.PW_iAddParam(item.parameterCode, item.parameterValue);

                // Registra na janela de debug o resultado da adição do parâmetro
                Debug.Print(string.Format("PW_iAddParam ({0},{1})={2}", item.parameterName,
                    item.parameterValue, ret.ToString()));
            }

            // Inicia o processo de execução da transação
            ret = ExecuteTransaction();

            return ret;
        }

        // Retorna uma lista com todas as informações disponíveis da transação atual
        public List<PW_Parameter> GetTransactionResult()
        {
            // Lista com os dados a serem retornados
            List<PW_Parameter> returnList = new List<PW_Parameter>();

            // Percorre todos os dados possíveis
            foreach (E_PWINFO item in Enum.GetValues(typeof(E_PWINFO)).Cast<E_PWINFO>())
            {

                StringBuilder getResultValue = new StringBuilder(10000);

                // Tenta obter o dado
                int getInfoRet = Interop.PW_iGetResult((short)item, getResultValue, 10001);

                // Caso o dado exista, adiciona na lista de retorno
                if (getInfoRet == (int)E_PWRET.PWRET_OK)
                {
                    returnList.Add(new PW_Parameter(item.ToString(), (ushort)item, getResultValue.ToString()));

                    // Registra na janela de debug cada parâmetro obtido com sucesso
                    Debug.Print(string.Format("PW_iGetResult ({0})={1}", item.ToString(), getResultValue));
                }
            }

            return returnList;
        }

        // Resolve a pendência de uma transação finalizada com sucesso
        public int ConfirmUndoNormalTransaction(E_PWCNF transactionStatus, List<PW_Parameter> transactionResponse)
        {
            int ret;

            string pszReqNum = string.Empty;
            string pszLocRef = string.Empty;
            string pszExtRef = string.Empty;
            string pszVirtMerch = string.Empty;
            string pszAuthSyst = string.Empty;

            // Obtém os dados necéssários para a resolução de pendência
            foreach (PW_Parameter item in transactionResponse)
            {
                switch (item.parameterCode)
                {
                    case (ushort)E_PWINFO.PWINFO_REQNUM:
                        pszReqNum = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_AUTLOCREF:
                        pszLocRef = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_AUTEXTREF:
                        pszExtRef = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_VIRTMERCH:
                        pszVirtMerch = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_AUTHSYST:
                        pszAuthSyst = item.parameterValue;
                        break;

                    default:
                        break;
                }
            }

            // Executa a confirmação
            ret = Interop.PW_iConfirmation((uint)transactionStatus, pszReqNum, pszLocRef, pszLocRef, pszVirtMerch, pszAuthSyst);

            // Registra na janela de debug a confirmação executada
            Debug.Print(string.Format("PW_iConfirmationNormal(ReqNum={0},LocRef={1},ExtRef={2},VirtMerch={3},AuthSyst={4})={5}",
                                                              pszReqNum, pszLocRef, pszExtRef, pszVirtMerch, pszAuthSyst, ret.ToString()));

            // Conforme a arquitetura utilizada pela automação, esse ponto poderá estar rodando em 
            // uma thread. Portanto o tratamento abaixo é feito para que a thread não seja 
            // interrompida até que a confirmação seja enviada ao servidor.
            // Para versões de biblioteca iguais ou superiores a 4.0.96.0, poderá ser utilizada a 
            // função: PW_iWaitConfirmation
            int loopRet;
            StringBuilder displayMessage = new StringBuilder(1000);

            for (; ; )
            {
                loopRet = Interop.PW_iPPEventLoop(displayMessage, (uint)1000);
                if (loopRet != (int)E_PWRET.PWRET_NOTHING)
                    break;
                Thread.Sleep(500);
            }

            // Caso a confirmação tenha sido executada com sucesso, remove o desfazimento pendente
            if (ret != (int)E_PWRET.PWRET_OK)
                PendencyDelete();
            // Caso ocorra alguma falha na confirmação
            else
            {
                // Apaga o desfazimento
                PendencyDelete();

                // Armazena o status recebido para repetição do processo antes da próxima transação
                PendencyWrite(transactionStatus);
            }

            return ret;
        }

        // Resolve a pendência de uma transação que, por algum motibo fora do fluxo previsto, ficou
        // pendente no sistema e impediu o ponto de captura de efetuar novas transações
        public int ConfirmUndoPendingTransaction(E_PWCNF transactionStatus, List<PW_Parameter> transactionResponse)
        {
            int ret;

            string pszReqNum = string.Empty;
            string pszLocRef = string.Empty;
            string pszExtRef = string.Empty;
            string pszVirtMerch = string.Empty;
            string pszAuthSyst = string.Empty;

            // Obtém os dados necéssários para a resolução de pendência
            foreach (PW_Parameter item in transactionResponse)
            {
                switch (item.parameterCode)
                {
                    case (ushort)E_PWINFO.PWINFO_PNDREQNUM:
                        pszReqNum = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_PNDAUTLOCREF:
                        pszLocRef = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_PNDAUTEXTREF:
                        pszExtRef = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_PNDVIRTMERCH:
                        pszVirtMerch = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_PNDAUTHSYST:
                        pszAuthSyst = item.parameterValue;
                        break;

                    default:
                        break;
                }
            }

            // Executa a confirmação
            ret = Interop.PW_iConfirmation((uint)transactionStatus, pszReqNum, pszLocRef, pszLocRef, pszVirtMerch, pszAuthSyst);

            // Registra na janela de debug a confirmação executada
            Debug.Print(string.Format("PW_iConfirmationPending(ReqNum={0},LocRef={1},ExtRef={2},VirtMerch={3},AuthSyst={4})={5}",
                                                              pszReqNum, pszLocRef, pszExtRef, pszVirtMerch, pszAuthSyst, ret.ToString()));
            return ret;
        }

        // Obtém uma entrada digitada no PIN-pad (necessário PIN-pad ABECS)
        public int GetInputFromPP(ref string userTypedValue, E_PWUserDataMessages messageToDisplay, short minLength, short maxLength, short timeout = 30)
        {
            StringBuilder value = new StringBuilder(10000);
            int ret;

            // Executa a função de btenção de dados no PIN-pad
            ret = Interop.PW_iPPGetUserData((short)messageToDisplay, minLength, maxLength, timeout, value);

            // Registra na janela de debug o comando
            Debug.Print(string.Format("PW_iPPGetUserData={0}", ret.ToString()));

            // Caso tenha ocorrido algum erro, retorna
            if (ret != (int)E_PWRET.PWRET_OK)
                return ret;

            // Atribui o valor a ser retornado
            userTypedValue = value.ToString();

            // Registra na janela de debug o valor digitado
            Debug.Print(string.Format("Valor digitado={0}", userTypedValue));

            return ret;
        }

        // Exibe uma mensagem no display do PIN-pad
        public int DisplayOnPP(string messageToDisplay)
        {
            int ret;

            // Executa o comando que exibe a mensagem no PIN-pad
            ret = Interop.PW_iPPDisplay(messageToDisplay);

            // Registra na janela de debug o comando
            Debug.Print(string.Format("PW_iPPDisplay({0}={1}", messageToDisplay, ret.ToString()));

            // Caso tenha ocorrido algum erro, retorna
            if (ret != (int)E_PWRET.PWRET_OK)
                return ret;

            // Entra no loop aguardando a finalização do comando
            ret = LoopPP();

            // Registra na janela de debug o retorno do loop
            Debug.Print(string.Format("Retorno={0}", ret.ToString()));

            // Caso tenha ocorrido algum erro, retorna
            if (ret != (int)E_PWRET.PWRET_OK)
                return ret;

            return ret;
        }

        // Aguarda um evento no PIN-pad
        public E_PWPPEVT WaitEventOnPP(E_PWPPEVTIN events)
        {
            int ret;
            uint eventOut = (uint)events;

            // Executa o comando que inicia a espera por evento no PIN-pad
            ret = Interop.PW_iPPWaitEvent(ref eventOut);

            // Registra na janela de debug o comando
            Debug.Print(string.Format("PW_iPPWaitEvent({0}{1}={2}", events, eventOut,
                ret.ToString()));

            // Caso tenha ocorrido algum erro, retorna
            if (ret != (int)E_PWRET.PWRET_OK)
                return E_PWPPEVT.PWPPEVT_NONE;

            // Entra no loop aguardando a finalização do comando
            ret = LoopPP();

            // Registra na janela de debug o retorno do loop
            Debug.Print(string.Format("Retorno={0}", ret.ToString()));

            // Caso tenha ocorrido algum erro, retorna
            if (ret != (int)E_PWRET.PWRET_OK)
                return E_PWPPEVT.PWPPEVT_NONE;

            return (E_PWPPEVT)eventOut;
        }

        // Captura uma senha no PIN-pad, retornando seu valor criptografado
        public int GetCriptoOnPP(byte keyID, string workingKey, short minLen, short maxLen, 
            string prompt, ref string resultado, short timeout=30)
        {
            // Com essa função é possível validar se uma senha foi digitada corretamente, mesmo que
            // não se tenha o conhecimento dela. Duas chamadas da função PW_iPPGetPINBlock
            // utilizando a mesma pszWorkingKey e bKeyID, retornarão o mesmo criptograma para senhas
            // iguais digitadas, com isso é possível fazer cadastro e validação de usuários
            // utilizando o PIN-pad, sem ter o conhecimento das senhas
            StringBuilder cripto = new StringBuilder(100);
            int ret;

            // Executa a função de btenção de dados no PIN-pad
            ret = Interop.PW_iPPGetPINBlock( keyID, workingKey, minLen, maxLen, timeout,
                prompt, cripto);

            // Registra na janela de debug o comando
            Debug.Print(string.Format("PW_iPPGetPINBlock={0}", ret.ToString()));

            // Caso tenha ocorrido algum erro, retorna
            if (ret != (int)E_PWRET.PWRET_OK)
                return ret;

            // Atribui o valor a ser retornado
            resultado = cripto.ToString();

            // Registra na janela de debug o valor digitado
            Debug.Print(string.Format("Criptograma={0}", resultado));

            return ret;

        }

        // Obtem todas as operações disponíveis na biblioteca
        public List<PW_Operations> GetOperations()
        {
            int ret;

            // Lista com as operações a serem retornadas
            List<PW_Operations> returnList = new List<PW_Operations>();

            // Estruturas auxiliares para acionamento da biblioteca
            PW_Operations[] operations = new PW_Operations[50];
            short operationsNum=50;
            byte operType = (byte)E_PWOperType.ADMIN + (byte)E_PWOperType.SALE;

            // Executa o comando que obtem as operações
            ret = Interop.PW_iGetOperations(operType, operations, ref operationsNum);

            // Registra na janela de debug o comando
            Debug.Print(string.Format("PW_iGetOperations={0}", ret.ToString()));

            // Caso tenham sido obtidas perações, monta a lista de retorno
            if (ret == (int)E_PWRET.PWRET_OK)
            {
                // Percorre todas as operações possíveis
                for (short n=0; n<operationsNum; n++)
                {
                    // Adiciona a operação
                    returnList.Add(operations[n]);

                    // Registra na janela de debug cada operação permitida
                    Debug.Print(string.Format("PW_Operations ({0},{1},{2}", operations[n].bOperType.ToString(),
                        operations[n].szText, operations[n].szValue));
                }
            }

            return returnList;
        }

        #endregion

        #region MÉTODOS PRIVADOS

        // Executa o loop da transação até que ela seja aprovada ou ocorra algum erro
        private int ExecuteTransaction()
        {
            int ret;
            FormDisplayQRcode fdqr = new FormDisplayQRcode();

            // Loop que só será interrompido em caso da finalização da transação, seja ela por algum
            // tipo de erro ou com o sucesso
            for (; ; )
            {
                // Cria a estrutura necessária para executar a função PW_iExecTransac, caso seja 
                // necessário capturar algum dado, essa estrutura terá detalhes de como deverá ser feita
                // essa captura
                PW_GetData[] structParam = new PW_GetData[10];

                // Parâmetro que, na entrada, indica quantos registros possui a estrutura PW_GetData e
                // na saída indica quantos dados precisam ser capturados
                short numDados = 10;

                // Desmarca o desfazimento marcado por segurança, pois a transação irá para 
                // controle pela biblioteca
                PendencyDelete();

                // Chama a função que executa um passo da transação
                ret = (int)Interop.PW_iExecTransac(structParam, ref numDados);

                // Marca um desfazimento por segurança, caso a automação seja fechada abruptamente
                // durante qualquer passo abaixo, o desfazimento já estará armazenado em disco para
                // ser executado por PendencyResolve antes da próxima transação
                // Esse desfazimento será desmarcado em duas situações:
                // 1-) O loop foi executado novamente e PW_iExecTransac será chamada
                // 2-) Algum erro ocorreu durante o loop
                // 3-) A transação foi finalizada com sucesso, nesse caso o desfazimento permanecerá
                //     gravado até a execução da resolução de pendência da transação em 
                //     "ConfirmUndoNormalTransaction"
                PendencyWrite(E_PWCNF.PWCNF_REV_PWR_AUT);

                // Registra na janela de debug o resultado da execução
                Debug.Print(string.Format("PW_iExecTransac={0}", ret.ToString()));

                // Faz o tratamento correto de acordo com o retorno recebido em PW_iExecTransac
                switch (ret)
                {
                    // Caso a biblioteca tenha solicitado a captura de mais dados, chama a função que
                    // faz a captura de acordo com as informações contidas em structParam
                    case (int)E_PWRET.PWRET_MOREDATA:
                        int ret2 = ShowCorrespondingWindow(structParam, ref fdqr);
                        if (ret2 != (int)E_PWRET.PWRET_OK)
                        {
                            if (ret2 == (int)E_PWRET.PWRET_CANCEL)
                            {
                                // Apaga o status de desfazimento anterior por desligamento abrupto da
                                // automação
                                PendencyDelete();

                                // Escreve o novo desfazimento a ser executado por transação abortada
                                // pela automação durante uma captura de dados
                                PendencyWrite(E_PWCNF.PWCNF_REV_ABORT);

                            }
                            return ret2;
                        }
                        break;

                    // Caso a biblioteca tenha retornado que existe uma transação pendente.
                    // Esse retorno só irá acontecer em caso de alguma falha de tratamento da resolução
                    // de pendência da transação por parte da automação, ou alguma falha de sistema
                    // do Pay&Go WEB, caso um ponto de captura fique com uma transação pendente ele não
                    // irá poder realizar novas transações até que essa pendência seja resolvida
                    case (int)E_PWRET.PWRET_FROMHOSTPENDTRN:
                        // Desmarca o desfazimento marcado por segurança, pois a transação não foi 
                        // finalizada com sucesso
                        PendencyDelete();

                        return ret;

                    // Esse retorno indica que nada deve ser feito e PW_iExecTransac deve ser chamada 
                    // novamente para prosseguir o fluxo
                    case (int)E_PWRET.PWRET_NOTHING:
                        break;

                    // Esse retorno indica que a transação foi executada com sucesso
                    case (int)E_PWRET.PWRET_OK:
                        // Para de exibir o QRcode, caso exista um sendo exibido
                        fdqr.Stop();
                        return ret;

                    // Qualquer outro código de retorno representa um erro
                    default:
                        // Para de exibir o QRcode, caso exista um sendo exibido
                        fdqr.Stop();
                        // Desmarca o desfazimento marcado por segurança, pois a transação não foi 
                        // finalizada com sucesso
                        PendencyDelete();

                        return ret;
                }
            }
        }
        
        // Executa a captura de dado solicitada pela biblioteca
        private int ShowCorrespondingWindow(PW_GetData[] expectedData, ref FormDisplayQRcode fdqr)
        {
            int ret = 0;
            ushort index = 0;
            FormDisplayMessage fdm;

            foreach (PW_GetData item in expectedData)
            {
                // Caso exista uma mensagem a ser exibida ao usuário antes da captura do dado
                if (item.szMsgPrevia.Length > 0)
                {
                    fdm = new FormDisplayMessage();
                    fdm.ShowDialog(item.szMsgPrevia, 3000);
                    fdm.Dispose();
                }

                ret = 0;
                switch (item.bTipoDeDado)
                {
                    case 0:
                        Debug.Print(string.Format("ERRO!!! Item com valor zerado."));
                        return ret;

                    // Caso a automação trabalhe com captura de código de barras, necessário 
                    // implementar os casos que serão aceitos (digitado, leitor...), bem como
                    //  as validações necessárias por tipo de código
                    case (int)E_PWDAT.PWDAT_BARCODE:
                        ret = GetTypedDataFromUser(item);
                        return ret;

                    // Menu de opções
                    case (int)E_PWDAT.PWDAT_MENU:
                        return GetMenuFromUser(item);

                    // Captura de dado digitado
                    case (int)E_PWDAT.PWDAT_TYPED:
                        ret = GetTypedDataFromUser(item);
                        return ret;

                    // Autenticação de permissão de usuário
                    case (int)E_PWDAT.PWDAT_USERAUTH:
                        ret = GetTypedDataFromUser(item);
                        return ret;

                    // Captura de dados do cartão
                    case (int)E_PWDAT.PWDAT_CARDINF:
                        // Caso só seja aceito o modo de entrada de cartão digitado
                        if (item.ulTipoEntradaCartao == 1)
                        {
                            PW_GetData temp = item;
                            temp.wIdentificador = (ushort)E_PWINFO.PWINFO_CARDFULLPAN;
                            ret = GetTypedDataFromUser(temp);
                        }
                        // Caso seja aceito cartão lido pelo PIN-pad
                        else
                        {
                            ret = Interop.PW_iPPGetCard(index);
                            Debug.Print(string.Format("PW_iPPGetCard={0}", ret.ToString()));
                            if (ret == (int)E_PWRET.PWRET_OK) 
                                ret = LoopPP();
                        }
                        return ret;
               
                    // Processamento offline do cartão
                    case (int)E_PWDAT.PWDAT_CARDOFF:
                        ret = Interop.PW_iPPGoOnChip(index);
                        Debug.Print(string.Format("PW_iPPGoOnChip={0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK)
                            ret = LoopPP();
                        return ret;

                    // Processamento online do cartão
                    case (int)E_PWDAT.PWDAT_CARDONL:
                        ret = Interop.PW_iPPFinishChip(index);
                        Debug.Print(string.Format("PW_iPPFinishChip={0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK) 
                            ret = LoopPP();
                        return ret;

                    // Confirmação de dado no PIN-pad
                    case (int)E_PWDAT.PWDAT_PPCONF:
                        ret = Interop.PW_iPPConfirmData(index);
                        Debug.Print(string.Format("PW_iPPConfirmData={0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK) 
                            ret = LoopPP();
                        return ret;

                    // Confirmação positiva PIN-pad
                    case (int)E_PWDAT.PWDAT_PPDATAPOSCNF:
                        ret = Interop.PW_iPPPositiveConfirmation(index);
                        Debug.Print(string.Format("PW_iPPPositiveConfirmation={0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK)
                            ret = LoopPP();
                        return ret;

                    // Comando genérico no PIN-pad
                    case (int)E_PWDAT.PWDAT_PPGENCMD:
                        ret = Interop.PW_iPPGenericCMD(index);
                        Debug.Print(string.Format("PW_iPPGenericCMD={0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK)
                            ret = LoopPP();
                        return ret;

                    // Senha do portador
                    case (int)E_PWDAT.PWDAT_PPENCPIN:
                        ret = Interop.PW_iPPGetPIN(index);
                        Debug.Print(string.Format("PW_iPPGetPIN={0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK) 
                            ret = LoopPP();
                        return ret;

                    // Entrada digitada no PIN-pad
                    case (int)E_PWDAT.PWDAT_PPENTRY:
                        ret = Interop.PW_iPPGetData(index);
                        Debug.Print(string.Format("PW_iPPGetData={0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK) 
                            ret = LoopPP();
                        return ret;

                    // Remoção de cartão do PIN-pad
                    case (int)E_PWDAT.PWDAT_PPREMCRD:
                        ret = Interop.PW_iPPRemoveCard();
                        Debug.Print(string.Format("PW_iPPRemoveCard={0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK) 
                            ret = LoopPP();
                        return ret;

                    // Exibição de mensagem de interface no display da automação
                    case (int)E_PWDAT.PWDAT_DSPCHECKOUT:
                        fdm = new FormDisplayMessage();
                        fdm.Start();
                        fdm.ChangeText(item.szPrompt);
                        Thread.Sleep(1000);
                        // Caso o operador tenha apertado a tecla ESC, cancela a operação e aborta o comando do PINpad
                        if(fdm.isAborted())
                        {
                            // Aborta a operação em curso no PIN-pad
                            Interop.PW_iPPAbort();

                            // Atribui o retorno de aoperação cancelada
                            ret = (int)E_PWRET.PWRET_CANCEL;
                        }
                        fdm.Stop();

                        if(ret != (int)E_PWRET.PWRET_OK)
                            return ret;
                        // Sinaliza a exibição da mensagem para a biblioteca
                        ret = Interop.PW_iAddParam(item.wIdentificador, "");
                        return (int)E_PWRET.PWRET_OK;

                    // Exibição de QRcode no display da automação
                    case (int)E_PWDAT.PWDAT_DSPQRCODE:

                        // Exemplo 2: A string com o QR Code é recebida e um QRcode é gerado utilizando uma biblioteca 
                        // de terceiros, para compilar essa opção é necessário descomentar a função AtualizaQRCode na classe FormDisplayQRcode
                        // e instalar a biblioteca  MessagingToolkit.QRCode em seu Visual studio através do gerenciador de pacotes
                        // com o comando "Install-Package MessagingToolkit.QRCode -ProjectName PGWLib"
                        StringBuilder stringQRcode = new StringBuilder(5001);

                        // Tenta obter o valor do QRcode a ser exibido, caso não ache retorna operação cancelada
                        if (Interop.PW_iGetResult((short)E_PWINFO.PWINFO_AUTHPOSQRCODE, stringQRcode, 5001) != (int)E_PWRET.PWRET_OK)
                            return (int)E_PWRET.PWRET_CANCEL;

                        // Exibe o QRcode e o prompt
                        fdqr.Start();

                        // Para esse caso em específico o QR code é muito grande para exibir no display "padrão"
                        // Somente para esse exemplo, liga o autoSize da janela
                        fdqr.ChangeText(item.szPrompt, stringQRcode.ToString());

                        // Caso o operador tenha apertado a tecla ESC, cancela a operação e aborta o comando do PINpad
                        if (fdqr.isAborted())
                        {
                            // Aborta a operação em curso no PIN-pad
                            Interop.PW_iPPAbort();

                            fdqr.Stop();

                            // Atribui o retorno de aoperação cancelada
                            return (int)E_PWRET.PWRET_CANCEL;
                        }
                        
                        // Sinaliza a exibição do QRcode para a biblioteca
                        ret = Interop.PW_iAddParam(item.wIdentificador, "");
                        return (int)E_PWRET.PWRET_OK;

                    default:
                        break;
                }
                index++;
            }
            return ret;
        }

        // Aguarda em loop a finalização da operação executada no PIN-pad, fazendo
        // os tratamento necessários dos retornos
        private int LoopPP()
        {
            int ret;
            bool isFdmStarted = false;

            // Cria a janela para exibição de mensagens de interface
            FormDisplayMessage fdm = new FormDisplayMessage();
            
            // Loop executando até a finalização do comando de PIN-pad, seja ele com um erro
            // ou com sucesso
            do
            { 
                // Chama o loop de eventos
                StringBuilder displayMessage = new StringBuilder(1000);
                ret = Interop.PW_iPPEventLoop(displayMessage, (uint)displayMessage.Capacity);

                // Caso tenha retornado uma mensagem para exibição, exibe
                if (ret == (int)E_PWRET.PWRET_DISPLAY)
                {
                    if (!isFdmStarted)
                    {
                        fdm.Start();
                        isFdmStarted = true;
                    }
                    fdm.ChangeText(displayMessage.ToString());
                }

                // Verifica se o operador abortou a operação no checkout
                if(isFdmStarted)
                {
                    if(fdm.isAborted())
                    {
                        // Aborta a operação em curso no PIN-pad
                        Interop.PW_iPPAbort();

                        // Atribui o retorno de aoperação cancelada
                        ret = (int)E_PWRET.PWRET_CANCEL;

                        break;
                    }
                }

                // Aguarda 200ms para chamar o loop de eventos novamente
                Thread.Sleep(200);
            } while (ret == (int)E_PWRET.PWRET_NOTHING || ret == (int)E_PWRET.PWRET_DISPLAY);

            // Fecha janela para exibição de mensagem
            if(isFdmStarted)
                fdm.Stop();

            // Registra o resultado final do loop de PIN-pad na janela de Debug
            Debug.Print(string.Format("PW_iPPEventLoop={0}", ret.ToString()));

            return ret;
        }

        // Executa um menu de opções para o usuário
        private int GetMenuFromUser(PW_GetData expectedData)
        {
            bool userAborted = false;
            string value = string.Empty;
            int ret;

            // Caso o menu só tenha uma opção e ela seja a opção default, seleciona automaticamente
            // Caso ela não seja a opção defualt, necessário exibir para confirmação do usuário
            if (expectedData.bNumOpcoesMenu == 1 && expectedData.bItemInicial == 0)
            {
                value = expectedData.vszValorMenu[0].szValorMenu;
            }
            else
            {
                // Executa o menu
                FormMenu window = new FormMenu(expectedData);
                window.ShowDialog(ref userAborted, ref value);
                window.Dispose();

                // Caso o usuário tenha abortado a transação, retorna E_PWRET.PWRET_CANCEL
                if (userAborted)
                    return (int)E_PWRET.PWRET_CANCEL;
            }

            ret = Interop.PW_iAddParam(expectedData.wIdentificador, value);

            // Registra na janela de debug o resultado da adição do parâmetro
            Debug.Print(string.Format("PW_iAddParam({0},{1})={2}", ((E_PWINFO)expectedData.wIdentificador).ToString(), value, ret.ToString()));

            return ret;
        }

        // Obtém um dado digitado do usuário
        private int GetTypedDataFromUser(PW_GetData expectedData)
        {
            bool userAborted = false;
            string value = string.Empty;
            int ret;

            // Exibe a janela para captura do dado digitado 
            FormTypedData window = new FormTypedData(expectedData);
            window.ShowDialog(ref userAborted, ref value);
            window.Dispose();

            // Caso o usuário tenha abortado a transação, retorna E_PWRET.PWRET_CANCEL
            if (userAborted) 
                return (int)E_PWRET.PWRET_CANCEL;

            // Adiciona o dado capturado
            ret = Interop.PW_iAddParam(expectedData.wIdentificador, value);

            // Registra na janela de debug o resultado da adição do parâmetro
            Debug.Print(string.Format("PW_iAddParam({0},{1})={2}", ((E_PWINFO)expectedData.wIdentificador).ToString(), value, ret.ToString()));

            return ret;
        }

        // Resolve uma pendência previamente gravada
        private int PendencyResolve()
        {
            // Nessa função é necessário implementar na automação, de acordo com o tipo de persistência
            // de dados, a checagem se existe alguma transação necessitando de resolução de pendência
            // e, caso positivo, obter os identificadores dela que foram persistidos anteriormente:
            // PWINFO_REQNUM
            // PWINFO_AUTLOCREF
            // PWINFO_AUTEXTREF
            // PWINFO_VIRTMERCH
            // PWINFO_AUTHSYST
            // Após isso, chamar a função PW_iConfirmation para resolver e:
            //      Caso ocorra algum erro nessa chamada, não desmarcar a resolução de pendência 
            //      em disco e retornar erro, abortando a transação em curso.
            //      Caso a chamada retorne PWRET_OK, desmarcar a resolução de pendência em disco e
            //      prosseguir normalmente com a transação em curso.

            return (int)E_PWRET.PWRET_OK;
        }

        // Grava uma pendência para posterior resolução
        private int PendencyWrite(E_PWCNF transactionStatus)
        {
            // Sempre é necessário, antes de marcar este desfazimento, verificar se a transação necessita
            // de resolução de pendência através da obtenção do dado PWINFO_CNFREQ, caso esse valor 
            // seja "0", o tratamento abaixo nã é necessário para a transação corrente.

            // Nessa função é necessário implementar na automação, de acordo com o tipo de persistência
            // de dados, a obtenção da biblioteca dos identificadores da transação através de 
            // PW_iGetResult e armazená-los em disco:
            // PWINFO_REQNUM
            // PWINFO_AUTLOCREF
            // PWINFO_AUTEXTREF
            // PWINFO_VIRTMERCH
            // PWINFO_AUTHSYST
            // Bem como o status a ser utilizado para a resolução de sua pendencia "transactionStatus"

            return (int)E_PWRET.PWRET_OK;
        }

        // Descarta uma pendência que já foi resolvida ou não é mais necessária
        private int PendencyDelete()
        {
            // Nessa função é necessário implementar na automação, de acordo com o tipo de persistência
            // de dados a exclusão de qualquer resolução de pendência que possa estar armazenada.

            return (int)E_PWRET.PWRET_OK;
        }

        #endregion
    }
}
