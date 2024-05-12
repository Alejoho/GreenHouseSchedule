using System.Windows.Controls;

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
