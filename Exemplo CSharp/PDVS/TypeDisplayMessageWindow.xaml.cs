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
    public partial class TypeDisplayMessageWindow : Window
    {
        public TypeDisplayMessageWindow()
        {
            InitializeComponent();        
        }

        public bool ShowDialog(ref string message)
        {
            this.ShowDialog();

            message = tb1.Text + tb2.Text;

            return ValidateFields();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                MessageBox.Show("A duas linhas devem possuir 16 caracteres cada");
            }
            else
            {
                this.Close();
            }
        }

        private bool ValidateFields()
        {
            if (tb1.Text.Length == 16 && tb2.Text.Length == 16)
                return true;
            return false;
        }

            
    }
}
