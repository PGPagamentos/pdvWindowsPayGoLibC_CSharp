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
    /// Interaction logic for ConfirmationWindow.xaml
    /// </summary>
    public partial class ConfirmationWindow : Window
    {
        public ConfirmationWindow()
        {
            InitializeComponent();

            foreach (string item in Enum.GetNames(typeof(E_PWCNF)))
            {
                cmbtransactionStatuses.Items.Add(item);
            }
            cmbtransactionStatuses.SelectedIndex = 0;

        }

        public void ShowDialog(ref E_PWCNF transactionStatus)
        {
            this.ShowDialog();

            transactionStatus = (E_PWCNF)Enum.Parse(typeof(E_PWCNF), cmbtransactionStatuses.SelectedItem.ToString());
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
