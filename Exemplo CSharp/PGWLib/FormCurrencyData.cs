using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PGWLib.CustomObjects;
using static PGWLib.Enums;

namespace PGWLib
{
    /////////////////////////////////////////////////
    // Form criado para entrada de valores em dinheiro
    // 
    public partial class FormCurrencyData : Form
    {
        bool userAborted = false;
        PW_GetData _expectedData;
        private static Timer _timer = new Timer();
        string valorDinheiro;

        public FormCurrencyData(PW_GetData expectedData)
        {
            InitializeComponent();

            _timer.Interval = 4000;
            _timer.Tick += TimerTick;
            _timer.Start();

            _expectedData = expectedData;
            
            //Ocultar os dados digitados
            if (expectedData.wIdentificador == (ushort)E_PWINFO.PWINFO_AUTHMNGTUSER) expectedData.szPrompt = "INSIRA A SENHA DO LOJISTA";
            if (expectedData.wIdentificador == (ushort)E_PWINFO.PWINFO_AUTHTECHUSER) expectedData.szPrompt = "INSIRA A SENHA TÉCNICA";
            if (expectedData.wIdentificador == (ushort)E_PWINFO.PWINFO_AUTHMNGTUSER || 
                expectedData.wIdentificador == (ushort)E_PWINFO.PWINFO_AUTHTECHUSER ||
                expectedData.bOcultarDadosDigitados == 1) txtValue.PasswordChar = '*';

            //Define o tamanho máximo do dado a ser capturado
            if (expectedData.ulValorMaximo > 0) txtValue.MaxLength = expectedData.ulValorMaximo;

            //Define se o dado deve ser mascarado
            //if (expectedData.szMascaraDeCaptura != "") txtValue.Mask = expectedData.szMascaraDeCaptura.Replace("@","C");

            //Cursor deve se mover da esquerda para direita ou ao contrário?
            //if (expectedData.bIniciaPelaEsquerda == 1)
            //{
            //    txtValue.RightToLeft = RightToLeft.No;
            //}
            //else
            //{
            //    txtValue.RightToLeft = RightToLeft.Yes;
            //}

            //Valor inicial informado pelo servidor. Pode ser alterado pelo usuário.
            if (!string.IsNullOrEmpty(expectedData.szValorInicial.Trim())) txtValue.Text = expectedData.szValorInicial;

            LblHeader.Text = expectedData.szPrompt.Replace("  "," ").Replace("\r","").Replace("   "," ").Replace("    ", " ");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(_expectedData.ulValorMinimo > 0 && txtValue.TextLength < _expectedData.ulValorMinimo)
            {
                MessageBox.Show(string.Format("Você deve inserir ao menos {0} caracteres", _expectedData.ulValorMinimo));
                return;
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            userAborted = true;
            txtValue.Text = "";
            this.Close();
        }

        public void ShowDialog(ref bool abort, ref string value)
        {
            this.ShowDialog();

            abort = userAborted;
            //value = txtValue.Text.Trim();
            valorDinheiro = valorDinheiro.Trim();
            value = valorDinheiro.Replace(",", "");
            value = value.Replace(".", "");
           
        }

      
        private void TimerTick(object sender, EventArgs e)
        {
            this.Activate();
            txtValue.Focus();
        }

       
        private void txtValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                if (e.KeyChar == ',')
                {
                    e.Handled = (txtValue.Text.Contains(","));
                }
                else
                    e.Handled = true;
            }
        }

        /////////////////////////////////////////////////////////////
        // Método utilizado para formatar o valor durante a digitação
        private void txtValue_KeyUp(object sender, KeyEventArgs e)
        {
            valorDinheiro = txtValue.Text.Replace("R$", "").Replace(",", "").Replace(" ", "").Replace("00,", "");
            if (valorDinheiro.Length == 0)
            {
                txtValue.Text = "0,00" + valorDinheiro;
            }
            if (valorDinheiro.Length == 1)
            {
                txtValue.Text = "0,0" + valorDinheiro;
            }
            if (valorDinheiro.Length == 2)
            {
                txtValue.Text = "0," + valorDinheiro;
            }
            else if (valorDinheiro.Length >= 3)
            {
                if (txtValue.Text.StartsWith("0,"))
                {
                    txtValue.Text = valorDinheiro.Insert(valorDinheiro.Length - 2, ",").Replace("0,", "");
                }
                else if (txtValue.Text.Contains("00,"))
                {
                    txtValue.Text = valorDinheiro.Insert(valorDinheiro.Length - 2, ",").Replace("00,", "");
                }
                else
                {
                    txtValue.Text = valorDinheiro.Insert(valorDinheiro.Length - 2, ",");
                }
            }
            valorDinheiro = txtValue.Text;
            txtValue.Text = string.Format("{0:C}", Convert.ToDouble(valorDinheiro));
            txtValue.Select(txtValue.Text.Length, 0);
        }

        private void txtValue_Leave(object sender, EventArgs e)
        {
            valorDinheiro = txtValue.Text.Replace("R$", "");
            txtValue.Text = string.Format("{0:C}", Convert.ToDouble(valorDinheiro));
        }
    }
}
