using System;
using System.Windows.Controls;

namespace Presentation.CustomControls
{
    /// <summary>
    /// Interaction logic for ComboBoxWithLabelNoButton.xaml
    /// </summary>
    public partial class ComboBoxWithLabelNoButton : UserControl
    {
        // TODO: Change the Selection_Changed name to SelectionChange
        public SelectionChangedEventHandler Selection_Changed { get; set; }

        public EventHandler DropDownOpened { get; set; }

        private string _fieldLabel;

        public ComboBoxWithLabelNoButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string FieldLabel { get => _fieldLabel; set => _fieldLabel = value; }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selection_Changed?.Invoke(sender, e);
        }

        public string ComboBoxTip { get; set; }

        private void ComboBox_DropDownOpened(object sender, System.EventArgs e)
        {
            DropDownOpened?.Invoke(sender, e);
        }
    }
}
