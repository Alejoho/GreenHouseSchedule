﻿using System;
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
        private string _fieldContent;

        public string FieldLabel { get => _fieldLabel; set => _fieldLabel = value; }
        public string FieldContent { get => _fieldContent; set => _fieldContent = value; }
    }
}
