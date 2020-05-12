using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PGWLib.CustomObjects;

namespace PGWLib
{
    public class Interop
    {


        //======================================================================================================================================= =
        /*PW_iInit : 

            Esta função é utilizada para inicializar a biblioteca, e retorna imediatamente.
            Deve ser garantido que uma chamada dela retorne PWRET_OK antes de chamar qualquer outra função.

               Entradas:
               pszWorkingDir Diretório de trabalho (caminho completo, com final nulo) para uso exclusivo do Pay&Go Web
               Saídas: Nenhuma.

              Retorno: PWRET_OK .................................. Operação bem sucedida.
                       PWRET_WRITERR ....................... Falha de gravação no diretório informado.
                       PWRET_INVCALL ......................... Já foi efetuada uma chamada à função PW_iInit após o carregamento da biblioteca.
                       Outro ..................................Outro erro de execução (ver “10. Códigos de retorno”, página 78 do Manual).
                                                               Uma mensagem de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //============================================================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iInit(string pszWorkingDir);



        //=============================================================================================================================================
        /*  PW_iNewTransac :
            
             Esta função deve ser chamada para iniciar uma nova transação através do Pay&Go Web, e retorna imediatamente.

             Entradas:
             iOper Tipo de transação a ser realizada (PWOPER_xxx, conforme tabela).

             Saídas: Nenhuma

             Retorno:
                       PWRET_OK .................................. Transação inicializada.
                       PWRET_DLLNOTINIT ................... Não foi executado PW_iInit.
                       PWRET_NOTINST ........................ É necessário efetuar uma transação de Instalação.
                       Outro ................................ Outro erro de execução (ver “10. Códigos de retorno”, página 78 Manual).
                                                              Uma mensagem de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //==============================================================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iNewTransac(byte bOper);



        //=============================================================================================================================================
        /* PW_iAddParam:

           Esta função é utilizada para alimentar a biblioteca com as informações da transação a ser realizada,
           e retorna imediatamente. Estas informações podem ser:
           - Pré-fixadas na Automação;
           - Capturadas do operador pela Automação antes do acionamento do Pay&Go Web;
           - Capturadas do operador após solicitação pelo Pay&Go Web (retorno PW_MOREDATA por PW_iExecTransac).


          Entradas:
          wParam   : Identificador do parâmetro (PWINFO_xxx, ver lista completa em “9. Dicionário de dados”, página 72).
          pszValue : Valor do parâmetro (string ASCII com final nulo).

        Saídas: Nenhuma

        Retorno:
                  PWRET_OK .................................. Parametro Acrescentado com sucesso.
                  PWRET_INVPARAM .................... O valor do parâmetro é inválido
                  PWRET_DLLNOTINIT ................... Não foi executado PW_iInit
                  PWRET_TRNNOTINIT .................. Não foi executado PW_iNewTransac (ver página 14).
                  PWRET_NOTINST ........................ É necessário efetuar uma transação de Instalação
                  Outro ........................................... Outro erro de execução (ver “10. Códigos de retorno”, página 78). Uma
                                                                    mensagem de erro pode ser obtida através da função PW_iGetResult
                                                                    (PWINFO_RESULTMSG).
        */
        //=============================================================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iAddParam(ushort wParam, string pszValue);


        //=============================================================================================================================================
        /*  PW_iExecTransac:
        
            Esta função tenta realizar uma transação através do Pay&Go Web, utilizando os parâmetros
            previamente definidos através de PW_iAddParam. Caso algum dado adicional precise ser informado,
            o retorno será PWRET_MOREDATA e o parâmetro pvstParam retornará informações dos dados que
            ainda devem ser capturados.

            Esta função, por se comunicar com a infraestrutura Pay&Go Web, pode demorar alguns segundos
            para retornar.


            Entradas:
              piNumParam : Quantidade máxima de dados que podem ser capturados de uma vez, caso o retorno
              seja PW_MOREDATA. (Deve refletir o tamanho da área de memória apontada por
              pvstParam.) Valor sugerido: 9.

            Saídas:
              pvstParam : Lista e características dos dados que precisam ser informados para executar a transação.
              Consultar “8.Captura de dados” (página 65) para a descrição da estrutura
              e instruções para a captura de dados adicionais. piNumParam Quantidade de dados adicionais que precisam ser capturados
              (quantidade de ocorrências preenchidas em pvstParam

              Retorno:
                  PWRET_OK .................................. Transação realizada com sucesso. Os resultados da transação devem ser obtidos através da função PW_iGetResult.
                  PWRET_NOTHING ....................... Nada a fazer, fazer as validações locais necessárias e chamar a função PW_iExecTransac novamente.
                  PWRET_MOREDATA ................... Mais dados são requeridos para executar a transação.
                  PWRET_DLLNOTINIT ................... Não foi executado PW_iInit.
                  PWRET_TRNNOTINIT .................. Não foi executado PW_iNewTransac (ver página 14).
                  PWRET_NOTINST ........................ É necessário efetuar uma transação de Instalação.
                  PWRET_NOMANDATORY ........... Algum dos parâmetros obrigatórios não foi adicionado (ver página 17).
                  Outro ........................................... Outro erro de execução (ver “10. Códigos de retorno”, página 78 Manual).
                                                                    Uma mensagem de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=============================================================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iExecTransac([Out] PW_GetData[] vstParam, ref short piNumParam);

  
  
  
        //=========================================================================================================
        /*  Funcao     :  PW_iGetResult

             Descricao  :  Esta função pode ser chamada para obter informações que resultaram da transação efetuada,
                           independentemente de ter sido bem ou mal sucedida, e retorna imediatamente.

             Entradas   :  iInfo:	   Código da informação solicitada sendo requisitada (PWINFO_xxx, ver lista completa
                                       em “9. Dicionário de dados”, página 36).
                           ulDataSize:	Tamanho (em bytes) da área de memória apontada por pszData. Prever um tamanho maior
                                       que o máximo previsto para o dado solicitado.


             Saidas     :  pszData:	   Valor da informação solicitada (string ASCII com terminador nulo).

             Retorno    :  PWRET_OK	         Sucesso. pszData contém o valor solicitado.
                           PWRET_NODATA	   A informação solicitada não está disponível.
                           PWRET_BUFOVFLW 	O valor da informação solicitada não cabe em pszData.
                           PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                           PWRET_TRNNOTINIT	Não foi executado PW_iNewTransac (ver página 10).
                           PWRET_NOTINST	   É necessário efetuar uma transação de Instalação.
                           Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                         de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
                                         
        */
        //=========================================================================================================*/
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iGetResult(short iInfo, StringBuilder pszData, uint ulDataSize);



        //=========================================================================================================
        /*
         Funcao     :  PW_iConfirmation

         Descricao  :  Esta função informa ao Pay&Go Web o status final da transação em curso (confirmada ou desfeita).
                       Consultar “7. Confirmação de transação” (página 28) para informações adicionais.

         Entradas   :  ulStatus:   	Resultado da transação (PWCNF_xxx, ver lista abaixo).
                       pszReqNum:  	Referência local da transação, obtida através de PW_iGetResult (PWINFO_REQNUM).
                       pszLocRef:  	Referência da transação para a infraestrutura Pay&Go Web, obtida através de PW_iGetResult (PWINFO_AUTLOCREF).
                       pszExtRef:  	Referência da transação para o Provedor, obtida através de PW_iGetResult (PWINFO_AUTEXTREF).
                       pszVirtMerch:	Identificador do Estabelecimento, obtido através de PW_iGetResult (PWINFO_VIRTMERCH).
                       pszAuthSyst:   Nome do Provedor, obtido através de PW_iGetResult (PWINFO_AUTHSYST).

         Saidas     :  não há.

         Retorno    :  PWRET_OK	         O status da transação foi atualizado com sucesso.
                       PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                       PWRET_NOTINST	   É necessário efetuar uma transação de Instalação.
                       Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                         de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iConfirmation(uint ulResult, string pszReqNum, string pszLocRef,
                                               string pszExtRef, string pszVirtMerch, string pszAuthSyst);


        //=========================================================================================================
        /*   Funcao     :  PW_iIdleProc

            Descricao  :  Para o correto funcionamento do sistema, a biblioteca do Pay&Go Web precisa de tempos em tempos
                          executar tarefas automáticas enquanto não está realizando nenhuma transação a pedido da Automação.

            Entradas   :  não há.

            Saidas     :  não há.

            Retorno    :  PWRET_OK	         Operação realizada com êxito.
                          PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                          PWRET_NOTINST	   É necessário efetuar uma transação de Instalação.
                          Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                            de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iIdleProc();


        //=========================================================================================================
        /*
         Funcao     :  PW_iGetOperations

         Descricao  :  Esta função pode ser chamada para obter quais operações o Pay&Go WEB disponibiliza no momento,
                       sejam elas administrativas, de venda ou ambas.

         Entradas   :              bOperType	      Soma dos tipos de operação a serem incluídos na estrutura de
                                                     retorno (PWOPTYPE_xxx).
                                   piNumOperations	Número máximo de operações que pode ser retornado. (Deve refletir
                                                     o tamanho da área de memória apontada por pvstOperations).

         Saídas     :              piNumOperations	Número de operações disponíveis no Pay&Go WEB.
                                   vstOperations	   Lista das operações disponíveis e suas características.


         Retorno    :  PWRET_OK	         Operação realizada com êxito.
                       PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                       PWRET_NOTINST	   É necessário efetuar uma transação de Instalação.
                       Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                         de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iGetOperations(byte bOperType,[Out] PW_Operations[] vstOperations, ref short piNumOperations);


        //=========================================================================================================
        /* Funcao     :  PW_iPPAbort

            Descricao  :  Esta função pode ser utilizada pela Automação para interromper uma captura de dados no PIN-pad
                         em curso, e retorna imediatamente.

            Entradas   :  não há.

            Saidas     :  não há.

            Retorno    :  PWRET_OK	         Operação interrompida com sucesso.
                         PWRET_PPCOMERR	   Falha na comunicação com o PIN-pad.
                         PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                         Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                           de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPAbort();

        //=========================================================================================================
        /* Funcao     :  PW_iPPEventLoop

            Descricao  :  Esta função deverá ser chamada em “loop” até que seja retornado PWRET_OK (ou um erro fatal). Nesse
                         “loop”, caso o retorno seja PWRET_DISPLAY o ponto de captura deverá atualizar o “display” com as
                         mensagens recebidas da biblioteca.

            Entradas   :  ulDisplaySize	Tamanho (em bytes) da área de memória apontada por pszDisplay.
                                        Tamanho mínimo recomendado: 100 bytes.

            Saidas     :  pszDisplay	   Caso o retorno da função seja PWRET_DISPLAY, contém uma mensagem de texto
                                        (string ASCII com terminal nulo) a ser apresentada pela Automação na interface com
                                        o usuário principal. Para o formato desta mensagem, consultar “4.3.Interface com o
                                        usuário”, página 8.

            Retorno    :  PWRET_NOTHING	   Nada a fazer, continuar aguardando o processamento do PIN-pad.
                         PWRET_DISPLAY	   Apresentar a mensagem recebida em pszDisplay e continuar aguardando o processamento do PIN-pad.
                         PWRET_OK	         Captura de dados realizada com êxito, prosseguir com a transação.
                         PWRET_CANCEL	   A operação foi cancelada pelo Cliente no PIN-pad (tecla [CANCEL]).
                         PWRET_TIMEOUT	   O Cliente não realizou a captura no tempo limite.
                         PWRET_FALLBACK	   Ocorreu um erro na leitura do cartão, passar a aceitar a digitação do número do cartão, 
                                           caso já não esteja aceitando.
                         PWRET_PPCOMERR	   Falha na comunicação com o PIN-pad.
                         PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                         PWRET_INVCALL	   Não há captura de dados no PIN-pad em curso.
                         Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                           de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================        
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPEventLoop( StringBuilder pszDisplay, uint ulDisplaySize);

        //=========================================================================================================
        /* Funcao     :  PW_iPPGetCard

            Descricao  :  Esta função é utilizada para realizar a leitura de um cartão (magnético, com chip com contato,
                     ou sem contato) no PIN-pad.

            Entradas   :  uiIndex	Índice (iniciado em 0) do dado solicitado na última execução de PW_iExecTransac
                              (índice do dado no vetor pvstParam).

            Saidas     :  não há.

            Retorno    :  PWRET_OK	         Captura iniciada com sucesso, chamar PW_iPPEventLoop para obter o resultado.
                     PWRET_INVPARAM	   O valor de uiIndex informado não corresponde a uma captura de dados deste tipo.
                     PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                     Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                       de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGetCard(ushort uiIndex);


        //=========================================================================================================
        /* Funcao     :  PW_iPPGetPIN

            Descricao  :  Esta função é utilizada para realizar a captura no PIN-pad da senha (ou outro dado criptografado)
                         do Cliente.

            Entradas   :  uiIndex	Índice (iniciado em 0) do dado solicitado na última execução de PW_iExecTransac
                                  (índice do dado no vetor pvstParam).

            Saidas     :  não há.

            Retorno    :  PWRET_OK	         Captura iniciada com sucesso, chamar PW_iPPEventLoop para obter o resultado.
                         PWRET_INVPARAM	   O valor de uiIndex informado não corresponde a uma captura de dados deste tipo.
                         PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                         Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                           de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGetPIN(ushort uiIndex);

        
        
        //=========================================================================================================
        /* Funcao     :  PW_iPPGetData

            Descricao  :  Esta função é utilizada para fazer a captura no PIN-pad de um dado não sensível do Cliente..

            Entradas   :  uiIndex	Índice (iniciado em 0) do dado solicitado na última execução de PW_iExecTransac
                              (índice do dado no vetor pvstParam).

            Saidas     :  nao ha.

            Retorno    :  PWRET_OK	         Captura iniciada com sucesso, chamar PW_iPPEventLoop para obter o resultado.
                     PWRET_INVPARAM	   O valor de uiIndex informado não corresponde a uma captura de dados deste tipo.
                     PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                     Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                       de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGetData(ushort uiIndex);


        //=========================================================================================================
        /* Funcao     :  PW_iGetUserData

              Descricao  :  Esta função é utilizada para obter um dado digitado pelo portador do cartão no PIN-pad.

              Entradas   :  uiMessageId : Identificador da mensagem a ser exibida como prompt para a captura.
                            bMinLen     : Tamanho mínimo do dado a ser digitado.
                            bMaxLen     : Tamanho máximo do dado a ser digitado.
                            iToutSec    : Tempo limite para a digitação do dado em segundos.

              Saídas     :  pszData     : Dado digitado pelo portador do cartão no PIN-pad.

              Retorno    :  PWRET_OK	         Operação realizada com êxito.
                             PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                             PWRET_NOTINST	   É necessário efetuar uma transação de Instalação.
                             PWRET_CANCEL	   A operação foi cancelada pelo Cliente no PIN-pad (tecla [CANCEL]).
                             PWRET_TIMEOUT	   O Cliente não realizou a captura no tempo limite.
                             PWRET_PPCOMERR	   Falha na comunicação com o PIN-pad.
                             PWRET_INVCALL	   Não é possível capturar dados em um PIN-pad não ABECS.
                             Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                           de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGetUserData(short uiMessageId, short bMinLen, short bMaxLen, short iToutSec, StringBuilder pszData);

        /*
                // PW_iPPGetUserData(Uint16 uiMessageId, Byte bMinLen, Byte bMaxLen, Int16 iToutSec, char *pszData);
                function PW_iPPGetUserData
                (uiMessageId:UInt16; bMinLen:Byte; bMaxLen:Byte; iToutSec:Int16; var pszData: PSZ_GetDado):Int16; StdCall; External 'PGWebLib.dll';

                // conversao de tipos
                UInt16 -> short
                Byte -> short
                Int16 -> short
                var pszData: PSZ_GetDado  -> StringBuilder pszData
        */




        
        
        //=========================================================================================================
        /* Funcao     :  PW_iPPGoOnChip

            Descricao  :  Esta função é utilizada para realizar o processamento off-line (antes da comunicação com o Provedor)
                         de um cartão com chip no PIN-pad.

            Entradas   :  uiIndex	Índice (iniciado em 0) do dado solicitado na última execução de PW_iExecTransac
                                  (índice do dado no vetor pvstParam).

            Saidas     :  não há.

            Retorno    :  PWRET_OK	         Captura iniciada com sucesso, chamar PW_iPPEventLoop para obter o resultado.
                         PWRET_INVPARAM	   O valor de uiIndex informado não corresponde a uma captura de dados deste tipo.
                         PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                         Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                           de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================        
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGoOnChip(ushort uiIndex);

        
        //=========================================================================================================
        /* Funcao     :  PW_iPPFinishChip

            Descricao  :  Esta função é utilizada para finalizar o processamento on-line (após comunicação com o Provedor)
                         de um cartão com chip no PIN-pad.

            Entradas   :  uiIndex	Índice (iniciado em 0) do dado solicitado na última execução de PW_iExecTransac
                                  (índice do dado no vetor pvstParam).

            Saidas     :  não há.

            Retorno    :  PWRET_OK	         Captura iniciada com sucesso, chamar PW_iPPEventLoop para obter o resultado.
                         PWRET_INVPARAM	   O valor de uiIndex informado não corresponde a uma captura de dados deste tipo.
                         PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                         Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                           de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPFinishChip(ushort uiIndex);

        //=========================================================================================================
        /* Funcao     :  PW_iPPConfirmData

            Descricao  :  Esta função é utilizada para obter do Cliente a confirmação de uma informação no PIN-pad.

            Entradas   :  uiIndex	Índice (iniciado em 0) do dado solicitado na última execução de PW_iExecTransac
                                  (índice do dado no vetor pvstParam).

            Saidas     :  não há.

            Retorno    :  PWRET_OK	         Captura iniciada com sucesso, chamar PW_iPPEventLoop para obter o resultado.
                         PWRET_INVPARAM	   O valor de uiIndex informado não corresponde a uma captura de dados deste tipo.
                         PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                         Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                           de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPConfirmData(ushort uiIndex);


        //=========================================================================================================
        /* Funcao     :  PW_iPPRemoveCard

            Descricao  :  Esta função é utilizada para fazer uma remoção de cartão do PIN-pad.

            Entradas   :  não há.

            Saidas     :  não há.

            Retorno    :  PWRET_OK	         Captura iniciada com sucesso, chamar PW_iPPEventLoop para obter o resultado.
                          PWRET_INVPARAM	   O valor de uiIndex informado não corresponde a uma captura de dados deste tipo.
                          PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                          Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                               de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPRemoveCard();



        //=========================================================================================================
        /* Funcao     :  PW_iPPDisplay

            Descricao  :  Esta função é utilizada para apresentar uma mensagem no PIN-pad

            Entradas   :  pszMsg   Mensagem a ser apresentada no PIN-pad. O caractere ‘\r’ (0Dh) indica uma quebra de linha.

            Saidas     :  não há.

            Retorno    :  PWRET_OK	         Captura iniciada com sucesso, chamar PW_iPPEventLoop para obter o resultado.
                          PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                          Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                           de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPDisplay(string pszMsg);


        //=========================================================================================================
        /* Funcao     :  PW_iPPWaitEvent

            Descricao  :  Esta função é utilizada para aguardar a ocorrência de um evento no PIN-pad.

            Entradas   :  não há.

            Saidas     :  pulEvent	         Evento ocorrido.

            Retorno    :  PWRET_OK	         Captura iniciada com sucesso, chamar PW_iPPEventLoop para obter o resultado.
                         PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                         Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40). Uma mensagem
                                           de erro pode ser obtida através da função PW_iGetResult (PWINFO_RESULTMSG).
        */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPWaitEvent(ref uint pulEvent);



        //===========================================================================
        /* Funcao   : PW_iPPGenericCMD

            Descricao  :  Realiza comando genérico de PIN-pad.

            Entradas   :  uiIndex	Índice (iniciado em 0) do dado solicitado na última execução de PW_iExecTransac
                                  (índice do dado no vetor pvstParam).

            Saidas     :  Não há.

            Retorno    :  PWRET_xxx.
        */
        //===========================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGenericCMD(ushort uiIndex);


        ////////
        //===========================================================================
        /* Funcao     : PW_iTransactionInquiry

           Descricao  :  Esta função é utilizada para realizar uma consulta de transações
                         efetuadas por um ponto de captura junto ao Pay&Go WEB.

           Entradas   :  pszXmlRequest	Arquivo de entrada no formato XML, contendo as informações
                                        necessárias para fazer a consulta pretendida.
                         ulXmlResponseLen Tamanho da string pszXmlResponse.

           Saidas     :  pszXmlResponse	Arquivo de saída no formato XML, contendo o resultado da consulta
                                        efetuada, o arquivo de saída tem todos os elementos do arquivo de entrada.

           Retorno    :  PWRET_xxx. 
           @@@@
        */
        //===========================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iTransactionInquiry( byte pszXmlRequest,  byte pszXmlResponse, uint ulXmlResponseLen);
 


        //===========================================================================
        /* Funcao     : PW_iPPPositiveConfirmation

             Descricao  :  Realiza a confirmação positiva de um dado, ou um bloco de dados,
                            no PIN-pad
             
             Entradas   :  uiIndex	Índice (iniciado em 0) do dado solicitado na última execução de PW_iExecTransac 
                                    (índice do dado no vetor pvstParam).

             Saidas     :  Não há.
         
             Retorno    :  PWRET_xxx.
        */
        //===========================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPPositiveConfirmation(ushort uiIndex);


        //=========================================================================================================
        /* Funcao     :  PW_iPPGetPINBlock

            Descricao  :  Esta função é utilizada para obter o PIN block gerado a partir de um dado digitado pelo usuário no PIN-pad.

            Entradas   :  bKeyID	      : Índice da Master Key (para chave PayGo, utilizar o índice “12”).
                         pszWorkingKey	: Sequência 32 caracteres utilizados para a geração do PIN block (dois valores iguais digitados pelo usuário com duas pszWorkingKey diferentes irão gerar dois PIN block diferentes.
                         bMinLen	      : Tamanho mínimo do dado a ser digitado (a partir de 4).
                         bMaxLen     	: Tamanho máximo do dado a ser digitado.
                         iToutSec    	: Tempo limite para a digitação do dado em segundos.
                         pszPrompt	   : Mensagem de 32 caracteres (2 linhas com 16 colunas) para apresentação no momento do pedido do dado do usuário.


            Saídas     :  pszData        : PIN block gerado com base nos dados fornecidos na função combinados com o dado digitado pelo usuário no PIN-pad.

            Retorno    :  PWRET_OK	         Operação realizada com êxito.
                         PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                         PWRET_NOTINST	   É necessário efetuar uma transação de Instalação.
                         PWRET_CANCEL	   A operação foi cancelada pelo Cliente no PIN-pad (tecla [CANCEL]).
                         PWRET_TIMEOUT	   O Cliente não realizou a captura no tempo limite.
                         PWRET_PPCOMERR	   Falha na comunicação com o PIN-pad.
                         Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40).
         
         @@@@
         */
        //=========================================================================================================
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGetPINBlock(short bKeyID,string pszWorkingKey, short bMinLen,
            short bMaxLen, short iToutSec, string pszPrompt, StringBuilder pszData);

        // conversao de tipos
        //        delphi     |   C#
        // 
        //        UInt16     -> short
        //        Byte       -> short
        //        Int16      -> short
        //        char       -> byte
        //        Uint32     -> uint  
        //        AnsiString -> string  
        //        var pszData: PSZ_GetDado  -> StringBuilder pszData

        //===========================================================================
        /* Funcao     :  PW_iWaitConfirmation

             Descricao  :  Esta função é utilizada sincronizar a aplicação com a thread da confirmação.
                           Esta função apenas retorna quando o processo de confirmação é finalizado.

             Entradas   :  Não há.

             Saídas     :  Não há.

             Retorno    :  PWRET_OK	         Operação realizada com êxito.
                           PWRET_DLLNOTINIT	Não foi executado PW_iInit.
                           PWRET_NOTINST	   É necessário efetuar uma transação de Instalação.
                           Outro	            Outro erro de execução (ver “10. Códigos de retorno”, página 40).
        \*=========================================================================================================*/
        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short  PW_iWaitConfirmation();

    }
}
