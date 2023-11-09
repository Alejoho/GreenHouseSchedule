using Domain.Processors;
using SupportLayer.Models;
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

namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditSpeciesWindow : Window
    {
        private SpeciesProcessor _processor;
        private Species _model;

        public AddEditSpeciesWindow()
        {
            InitializeComponent();
            _processor = new SpeciesProcessor();
            _model = new Species();
            _model.Id = 0;
        }

        public AddEditSpeciesWindow(Species model)
        {
            InitializeComponent();
            _processor = new SpeciesProcessor();
            this._model = model;
            PopulateData();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(ValidateDataType() == true)
            {
                if(_processor.SaveSpecies(_model)==true) 
                {
                    MessageBox.Show("Registro salvado");
                    this.Close();
                }
                else
                {
                    ShowError();
                }
            }
        }

        private void ShowError()
        {
            MessageBox.Show(_processor.Error);
        }
        //NEXT - do the ValidateDataType method
        private bool ValidateDataType()
        {
            throw new NotImplementedException();
        }
        //NEXT - do the PopulateData method
        private void PopulateData()
        {

        }
    }
}
