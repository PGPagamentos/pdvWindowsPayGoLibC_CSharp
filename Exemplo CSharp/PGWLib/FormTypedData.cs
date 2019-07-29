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
    public partial class FormTypedData : Form
    {
        bool userAborted = false;
        PW_GetData _expectedData;
        private static Timer _timer = new Timer();

        public FormTypedData(PW_GetData expectedData)
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
            if (expectedData.szMascaraDeCaptura != "") txtValue.Mask = expectedData.szMascaraDeCaptura.Replace("@","C");

            //Cursor deve se mover da esquerda para direita ou ao contrário?
            if (expectedData.bIniciaPelaEsquerda == 1)
            {
                txtValue.RightToLeft = RightToLeft.No;
            }
            else
            {
                txtValue.RightToLeft = RightToLeft.Yes;
            }

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
            value = txtValue.Text.Trim();
        }

        private void FormTypedData_Load(object sender, EventArgs e)
        {

        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.Activate();
            txtValue.Focus();
        }

    }
}
