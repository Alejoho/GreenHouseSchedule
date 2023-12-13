using System.Windows;
using System.Windows.Controls;

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
        private string _Tip;
        private Visibility _tipVisibility = Visibility.Collapsed;

        public TextBoxWithLabel()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string FieldLabel { get => _fieldLabel; set => _fieldLabel = value; }
        public string FieldContent
        {
            get => _fieldContent;
            set => _fieldContent = value;
        }
        public string Tip { get => _Tip; set => _Tip = value; }
        public Visibility TipVisibility { get => _tipVisibility; set => _tipVisibility = value; }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }
    }
}
