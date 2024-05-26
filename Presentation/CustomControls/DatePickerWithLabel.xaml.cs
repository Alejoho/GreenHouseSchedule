using System;
using System.Windows.Controls;

namespace Presentation.CustomControls
{
    /// <summary>
    /// Interaction logic for DateTimePickerWithLabel.xaml
    /// </summary>
    public partial class DatePickerWithLabel : UserControl
    {
        public DatePickerWithLabel()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private string _fieldLabel;

        public string FieldLabel { get => _fieldLabel; set => _fieldLabel = value; }
        //NEXT - Look up how to change the format of a datetimepicker to dd/mm/yyyy
        public DateOnly SelectedDateOnly { get => DateOnly.FromDateTime((DateTime)TimePicker.SelectedDate); }

    }
}
