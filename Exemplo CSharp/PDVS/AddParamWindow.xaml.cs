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
using System.Windows.Shapes;
using static PGWLib.Enums;

namespace PDVS
{
    /// <summary>
    /// Interaction logic for AddParamWindow.xaml
    /// </summary>
    public partial class AddParamWindow : Window
    {
        public AddParamWindow(string param = "", string value = "")
        {
            InitializeComponent();
            foreach (string item in Enum.GetNames(typeof(E_PWINFO)))
            {
                cmbParam.Items.Add(item);
            }

            if (!string.IsNullOrEmpty(param))
            {
                cmbParam.SelectedItem = param;
                txtValue.Text = value;
                cmbParam.IsEnabled = false;
            }

        }

        public void ShowDialog(ref string param, ref string value)
        {
            this.ShowDialog();

            param = (string)cmbParam.SelectedValue;
            value = txtValue.Text;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Text = "";
            this.Close();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (!validateFields())
            {
                MessageBox.Show("Preencha os campos corretamente.");
            }else
            {
                this.Close();
            }
        }

        private bool validateFields()
        {
            if (cmbParam.SelectedIndex == -1) return false;
            if (string.IsNullOrEmpty(txtValue.Text) & string.IsNullOrWhiteSpace(txtValue.Text)) return false;
            return true;
        }

    }
}
