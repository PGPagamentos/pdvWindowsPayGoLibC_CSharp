using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PGWLib
{
    public partial class FormDisplayMessage : Form
    {
        Timer timer;
        bool _userAborted;

        public FormDisplayMessage()
        {
            InitializeComponent();
            _userAborted = false;
        }

        // Inicia a exibição da janela de mensagens no modo permamente
        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                this.ShowDialog();
            });
        }

        // Para a exibição da janela de mensagens no modo permanente
        public void Stop()
        {
            if (this.InvokeRequired)
            {
                Invoke((Action)delegate
                {
                    this.Close();
                });
            }
            else
            {
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Control is already disposed.");
                else
                {
                    this.Close();
                }
            }
        }

        // Atualiza o texto exibido da janela de mensagens no modo permanente
        public void ChangeText(string newText)
        {
            if (this.InvokeRequired)
            {
                Invoke((Action)delegate
                {
                    // Atribui o valor do prompt a ser exibido, substituindo a quebra de linha utilizada
                    // pela biblioteca pela quebra de linha utilizada nos forms
                    // Remove a centralização vertical para melhor adaptação da mensagem na janela
                    this.LblMessage.Text = newText.TrimStart('\r').Replace("\r", "\n");
                    this.Focus();
                });
            }
            else
            {
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Control is already disposed.");
                else
                {
                    // Atribui o valor do prompt a ser exibido, substituindo a quebra de linha utilizada
                    // pela biblioteca pela quebra de linha utilizada nos forms
                    // Remove a centralização vertical para melhor adaptação da mensagem na janela
                    this.LblMessage.Text = newText.TrimStart('\r').Replace("\r", "\n");
                    this.Focus();
                }
            }
        }

        // Exibe a janela de mensagem com timeout
        public void ShowDialog(string message, int timeout)
        {
            // Caso não tenha mensagem, retorna
            if (message == "")
                return;

            // Atribui o valor do prompt a ser exibido, substituindo a quebra de linha utilizada
            // pela biblioteca pela quebra de linha utilizada nos forms
            this.LblMessage.Text = message.Replace("\r", "\n");

            // Define ume timeout para fechamento automatico da janela
            timer = new Timer();
            timer.Interval = timeout;
            timer.Tick += Timer_Tick;          
            timer.Start();
            this.ShowDialog();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Dispose();
            this.Close();
        }

        // Caso o operador pressione a tecla ESC para abortar a operação
        private void FormDisplayMessage_KeyUp(object sender, KeyEventArgs e)
        {
            // ESC pressionado, operação abortada
            if (e.KeyCode == Keys.Escape)
            {
                _userAborted = true;
                this.Close();
            }
        }

        // Checa se a tecla ESC foi pressionada
        public bool isAborted()
        {
            return _userAborted;
        }
    }
}
