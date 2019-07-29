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

        public class MenuItem
        {
            public string value { get; set; }
            public string texto { get; set; }

            public override string ToString()
            {
                return string.Format("{0} - {1}", this.value, this.texto);
            }
        }

        public FormMenu(PW_GetData expectedData)
        {
            InitializeComponent();

            LblHeader.Text = "";
            _expectedData = expectedData;

            if(expectedData.szPrompt.Trim() != "") LblHeader.Text = expectedData.szPrompt.Replace("\r  ", "");

            populateMenu(expectedData.stMenu, expectedData.bNumOpcoesMenu, expectedData.wIdentificador, expectedData.bTeclasDeAtalho);

            if (_expectedData.bItemInicial >= 0 && _expectedData.bItemInicial < expectedData.bNumOpcoesMenu)
                LstMenu.SelectedIndex = _expectedData.bItemInicial;
        }

        public void ShowDialog(ref bool abort, ref string value)
        {
            this.ShowDialog();

            if (LstMenu.SelectedIndex < 0) userAborted = true;

            abort = userAborted;
            value = _ret;
        }

        private void populateMenu(PW_Menu menu, byte bNumOpcoesMenu, ushort wIdentificador, byte bTeclasDeAtalho)
        {
            List<MenuItem> menuItems = new List<MenuItem>();
            MenuItem menuItem = new MenuItem();
            
            string tempText, tempValue;
            int aux;
            for (byte b = 0; b < bNumOpcoesMenu; b++)
            {
                switch (b)
                {
                    case (0):
                        tempText = menu.szTexto1;
                        tempValue = menu.szValor1;
                        break;

                    case (1):
                        tempText = menu.szTexto2;
                        tempValue = menu.szValor2;
                        break;

                    case (2):
                        tempText = menu.szTexto3;
                        tempValue = menu.szValor3;
                        break;

                    case (3):
                        tempText = menu.szTexto4;
                        tempValue = menu.szValor4;
                        break;

                    case (4):
                        tempText = menu.szTexto5;
                        tempValue = menu.szValor5;
                        break;

                    case (5):
                        tempText = menu.szTexto6;
                        tempValue = menu.szValor6;
                        break;

                    case (6):
                        tempText = menu.szTexto7;
                        tempValue = menu.szValor7;
                        break;

                    case (7):
                        tempText = menu.szTexto8;
                        tempValue = menu.szValor8;
                        break;

                    case (8):
                        tempText = menu.szTexto9;
                        tempValue = menu.szValor9;
                        break;

                    case (9):
                        tempText = menu.szTexto10;
                        tempValue = menu.szValor10;
                        break;

                    default:
                        tempText = "ERR";
                        tempValue = "ERR";
                        break;
                }
                aux = Convert.ToInt32(b) + 1;

                if(bTeclasDeAtalho == 1)
                {
                    menuItem.texto = string.Format("{0} - {1}", aux, tempText);
                }
                else
                {
                    menuItem.texto = string.Format("{1}", aux, tempText);
                }
                
                menuItem.value = tempValue;

                menuItems.Add(menuItem);
                menuItem = new MenuItem();
            }

            LstMenu.DataSource = menuItems;
            LstMenu.Focus();
        }

        private void LstMenu_MouseClick(object sender, MouseEventArgs e)
        {
            if (LstMenu.SelectedIndex == -1) return;
            _ret = ((MenuItem)(LstMenu.SelectedItem)).value;
            this.Close();
        }

        private void respondToPressedKey(Keys key)
        {
            //if(key == Keys.Up)
            //{
            //    if (LstMenu.SelectedIndex > 0) LstMenu.SelectedIndex = LstMenu.SelectedIndex - 1;
            //}

            //if (key == Keys.Down)
            //{
            //    if (LstMenu.SelectedIndex < LstMenu.Items.Count - 1) LstMenu.SelectedIndex = LstMenu.SelectedIndex + 1;
            //}

            if(key == Keys.Escape)
            {
                userAborted = true;
                this.Close();
            }

            if(key== Keys.Enter)
            {
                if (LstMenu.SelectedIndex == -1) return;
                _ret = ((MenuItem)(LstMenu.SelectedItem)).value;
                this.Close();
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            respondToPressedKey(e.KeyCode);
        }

        private void FormMenu_Load(object sender, EventArgs e)
        {
            LstMenu.SelectedIndex = -1;
        }
    }
}
