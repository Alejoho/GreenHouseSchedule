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
    /// Interaction logic for ComboBoxWithLabelNoButton.xaml
    /// </summary>
    public partial class ComboBoxWithLabelNoButton : UserControl
    {
        public ComboBoxWithLabelNoButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private string _fieldLabel;

        public string FieldLabel { get => _fieldLabel; set => _fieldLabel = value; }
    }
}
