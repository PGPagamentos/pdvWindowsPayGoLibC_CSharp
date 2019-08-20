# Exemplo de integração em C# com a biblioteca PGWebLibC da plataforma de transações com cartão PayGo Web

### Funcionalidades implementadas neste exemplo:
  - Instalação
  - Venda
  - Confirmação
  - Cancelamento

  
### Pré-requisitos
  - Visual Studio 2017 
  - Windows
  - Cadastro no ambiente de testes/sandbox do PayGo Web
    - código do Ponto de Captura (PdC)
    - PIN-Pad

#### Instruções de uso do exemplo C#

- Antes de utilizar qualquer função da PGWebLib.dll o primeiro passo é inicializa-la. 
  Para fazer isso é utilizado o método PW_iINit - Inicializar da PGWebLib.dll. 

- A primeira transação a ser efetuada é a de INSTALAÇÃO. Esta pode ser acionada diretamente
  selecionando PWOPER_INSTALL que executa o método PW_iNewTransac da PGWebLib.dll.
  
- No decorrer da transação de instalação são solicitadas as configurações necessárias para operação
  do Ponto de Captura. Para alterar estas configurações após isso, acionar a transação de CONFIGURAÇÃO,
  diretamente (PWOPER_CONFIG).
  Para se instalar um novo ponto de captura devem ser deletados os diretórios Data e Log que estão em  
  C:\ProgramData\PGWebLib 
  
- A primeira comunicação com o PIN-pad pode demorar até 1 minuto, devido à coleta de todas as suas
  informações(mensagem="Atualizando Tabelas").

- Após realizar a transação de instalação com sucesso, um comprovante será gerado, listando os
  sistemas de autorização configurados para o Ponto de Captura. A partir deste momento, outras transações
  poderão ser realizadas (venda, recarga, etc.).
  
  ### Observações
- Os Logs da DLL PGWebLib.dll ficam em  c:\ProgramData\PGWebLib\Log  
  
