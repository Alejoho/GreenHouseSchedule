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
    /// Interaction logic for TextBoxWithLabel.xaml
    /// </summary>
    public partial class TextBoxWithLabel : UserControl
    {
        public event TextChangedEventHandler TextChanged;
        private string _fieldLabel;
        private string _fieldContent;

        public TextBoxWithLabel()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string FieldLabel { get => _fieldLabel; set => _fieldLabel = value; }
        public string FieldContent { get => _fieldContent; 
            set => _fieldContent = value; }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }
    }
}
