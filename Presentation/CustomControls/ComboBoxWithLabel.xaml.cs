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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Presentation.CustomControls
{
    /// <summary>
    /// Interaction logic for ComboBoxWithLabel.xaml
    /// </summary>
    public partial class ComboBoxWithLabel : UserControl
    {
        public event RoutedEventHandler ButtonClick;
        private string _fieldLabel;

        public ComboBoxWithLabel()
        {
            InitializeComponent();
            this.DataContext= this;
        }

        public string FieldLabel { get => _fieldLabel; set => _fieldLabel = value; }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick?.Invoke(sender, e);
        }
    }
}
