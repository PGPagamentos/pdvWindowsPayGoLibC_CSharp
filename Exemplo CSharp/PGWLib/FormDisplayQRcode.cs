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
    public partial class FormDisplayQRcode : Form
    {
        bool _userAborted;
        string _lastQRcode;
        bool _isStarted;

        private void AtualizaQRCode(string qrCode)
        {
            //MessagingToolkit.QRCode.Codec.QRCodeEncoder qrCodecEncoder = new MessagingToolkit.QRCode.Codec.QRCodeEncoder
            //{
            //    QRCodeBackgroundColor = System.Drawing.Color.White,
            //    QRCodeForegroundColor = System.Drawing.Color.Black,
            //    CharacterSet = "UTF-8",
            //    QRCodeEncodeMode = MessagingToolkit.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE,
            //    QRCodeScale = 7,
            //    QRCodeVersion = 0,
            //    QRCodeErrorCorrect = MessagingToolkit.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.Q
            //};

            //Image imagemQRCode = qrCodecEncoder.Encode(qrCode);
            //pbQRcode.Image = imagemQRCode;

            // Caso o componente de geração de QRcode não esteja instalado, somente exibe a string com o código na tela
            // Ao descomentar o código acima, comentar esse
            this.tbQRcode.Text = qrCode;
            this.tbQRcode.BringToFront();
        }

        public FormDisplayQRcode()
        {
            InitializeComponent();
            _userAborted = false;
            _isStarted = false;
        }

        // Inicia a exibição da janela de QRcode no modo permamente
        public void Start()
        {
            if (_isStarted)
                return;
            Task.Factory.StartNew(() =>
            {
                this.ShowDialog();
            });
            _isStarted = true;
        }

        // Para a exibição do QRcode
        public void Stop()
        {
            if (!_isStarted)
                return;

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
            _isStarted = false;
        }

        // Atualiza o texto e/ou o QRcode
        public void ChangeText(string text, string qrCode)
        {
            if (qrCode != _lastQRcode)
                AtualizaQRCode(qrCode);
            _lastQRcode = qrCode;
            if (this.InvokeRequired)
            {
                Invoke((Action)delegate
                {
                    // Atribui o valor do prompt a ser exibido, substituindo a quebra de linha utilizada
                    // pela biblioteca pela quebra de linha utilizada nos forms
                    // Remove a centralização vertical para melhor adaptação da mensagem na janela
                    this.lblMessage.Text = text.TrimStart('\r').Replace("\r", "\n");
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
                    this.lblMessage.Text = text.TrimStart('\r').Replace("\r", "\n");
                    this.Focus();
                }
            }
        }

        // Caso o operador pressione a tecla ESC para abortar a operação
        private void FormDisplayQRcode_KeyUp(object sender, KeyEventArgs e)
        {
            // ESC pressionado, operação abortada
            if (e.KeyCode == Keys.Escape)
            {
                _userAborted = true;
                this.Close();
            }
        }

        // Caso o operador pressione a tecla ESC para abortar a operação
        private void tbQRcode_KeyUp(object sender, KeyEventArgs e)
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
