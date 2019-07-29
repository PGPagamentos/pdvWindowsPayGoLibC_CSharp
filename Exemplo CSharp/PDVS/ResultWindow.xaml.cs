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
using static PGWLib.CustomObjects;

namespace PDVS
{
    /// <summary>
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow(List<PW_Parameter> result)
        {
            InitializeComponent();

            foreach (PW_Parameter item in result)
            {
                listBox.Items.Add(item.ToString());
            }
        }
    }
}
