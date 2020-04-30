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
    public partial class SelectUserDataMessageWindow : Window
    {
        public SelectUserDataMessageWindow()
        {
            InitializeComponent();
            foreach (string item in Enum.GetNames(typeof(E_PWUserDataMessages)))
            {
                cmbParam.Items.Add(item);
            }
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public void ShowDialog(ref string param, ref short min, ref short max)
        {
            this.ShowDialog();

            param = (string)cmbParam.SelectedValue;
            min = (short)(cmbQtdMin.SelectedIndex + 1);
            max = (short)(cmbQtdMax.SelectedIndex + 1);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (!validateFields())
            {
                MessageBox.Show("Preencha os campos corretamente.");
            }
            else
            {
                this.Close();
            }
        }

        private bool validateFields()
        {
            if (cmbQtdMax.SelectedIndex >= cmbQtdMin.SelectedIndex && cmbParam.SelectedIndex != -1) return true;
            return false;
        }

            
    }
}
