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

namespace PGWLib
{
    public partial class FormMenu : Form
    {
        bool userAborted = false;
        PW_GetData _expectedData;
        string _ret = string.Empty;

        public FormMenu(PW_GetData expectedData)
        {
            InitializeComponent();

            _expectedData = expectedData;

            // Atribui o valor do prompt a ser exibido, substituindo a quebra de linha utilizada
            // pela biblioteca pela quebra de linha utilizada nos forms
            LblHeader.Text = expectedData.szPrompt.Replace("\r", "\n");

            // Preenche o ListBox com as opções
            PopulateMenu();

            // Coloca o curso em cima do item padrão
            if (_expectedData.bItemInicial >= 0 && _expectedData.bItemInicial < expectedData.bNumOpcoesMenu)
                LstMenu.SelectedIndex = _expectedData.bItemInicial;
        }

        // Exibe o menu para o usuário
        public void ShowDialog(ref bool abort, ref string value)
        {
            this.ShowDialog();

            if (LstMenu.SelectedIndex < 0) 
                userAborted = true;

            abort = userAborted;
            value = _ret;
        }

        // Coloca as opções no menu
        private void PopulateMenu()
        {           
            for (byte b = 0; b < _expectedData.bNumOpcoesMenu; b++)
            {
                if (_expectedData.bTeclasDeAtalho == 1 && b < 10)
                {
                    LstMenu.Items.Add(string.Format("{0} - {1}", b, _expectedData.vszTextoMenu[b].szTextoMenu));
                }
                else
                {
                    LstMenu.Items.Add(string.Format("{0}", _expectedData.vszTextoMenu[b].szTextoMenu));
                }               
            }

            LstMenu.Focus();
        }

        // Tratamento de click de mouse
        private void LstMenu_MouseClick(object sender, MouseEventArgs e)
        {
            // Indica que uma opção não foi selecionada, retorna sem fazer nada
            if (LstMenu.SelectedIndex == -1) 
                return;

            // Atribui o valor a ser retornado para a opção selecionada
            _ret = _expectedData.vszValorMenu[LstMenu.SelectedIndex].szValorMenu;

            this.Close();
        }

        // Tratamento de ESC e ENTER no teclado
        private void KeyPressed(object sender, KeyEventArgs e)
        {
            // ESC pressionado, operação abortada
            if (e.KeyCode == Keys.Escape)
            {
                userAborted = true;
                this.Close();
            }

            // ENTER pressionado
            if (e.KeyCode == Keys.Enter)
            {
                // Se nenhuma opção selecionada, retorna sem fazer nada
                if (LstMenu.SelectedIndex == -1)
                    return;

                // Atribui o valor a ser retornado para a opção selecionada
                _ret = _expectedData.vszValorMenu[LstMenu.SelectedIndex].szValorMenu;
                this.Close();
            }

            // Tecla numérica superior pressionada (0-9)
            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                _ret = _expectedData.vszValorMenu[e.KeyValue-48].szValorMenu;
                this.Close();
            }

            // Tecla numérica keypad pressionada (0-9)
            if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                _ret = _expectedData.vszValorMenu[e.KeyValue - 96].szValorMenu;
                this.Close();
            }            
        }

        // Coloca o foco no menu ao carregar
        private void FormMenu_Load(object sender, EventArgs e)
        {
            this.Focus();
        }
    }
}
