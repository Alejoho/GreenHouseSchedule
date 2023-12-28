using Domain.Processors;
using SupportLayer.Models;
using System.Windows;

namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditSpeciesWindow : Window
    {
        private SpeciesProcessor _processor;
        private Species _model;
        private ISpeciesRequester _requestor;

        public AddEditSpeciesWindow(ISpeciesRequester requestor)
        {
            InitializeComponent();
            _processor = new SpeciesProcessor();
            _model = new Species();
            _model.Id = 0;            
            _requestor = requestor;
            LlenarCasillas();
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
            if (ValidateDataType() == true)
            {
                if (_processor.SaveSpecies(_model) == true)
                {
                    MessageBox.Show("Registro salvado");
                    _requestor?.SpeciesComplete(_model);
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

        //TODO - I have a problem here. When a update a record if the model has an error in the
        //validateDataType it launches the error mesagge. The record isn,t save to the 
        //database but the model link to the datagrid changes.
        private bool ValidateDataType()
        {
            _model.Name = lbltxtName.FieldContent;

            if (byte.TryParse(lbltxtProductionDays.FieldContent, out byte productionDays))
            {
                _model.ProductionDays = productionDays;
            }
            else
            {
                MessageBox.Show("Días de produccion inválido");
                return false;
            }

            if (lbltxtWeightOf1000Seeds.FieldContent != string.Empty)
            {
                if (decimal.TryParse(lbltxtWeightOf1000Seeds.FieldContent, out decimal weightOf1000Seeds))
                {
                    _model.WeightOf1000Seeds = weightOf1000Seeds;
                }
                else
                {
                    MessageBox.Show("Peso de 1000 semillas inválido");
                    return false;
                }
            }

            if(int.TryParse( lbltxtAmountOfSeedsPerHectare.FieldContent,out int amountOfSeedsPerHectare))
            {
                _model.AmountOfSeedsPerHectare = amountOfSeedsPerHectare;
            }
            else
            {
                MessageBox.Show("Semillas en una hectárea inválido");
                return false;
            }

            if(decimal.TryParse(lbltxtWeightOfSeedsPerHectare.FieldContent,out decimal weightOfSeedsPerHectare))
            {
                _model.WeightOfSeedsPerHectare = weightOfSeedsPerHectare;
            }
            else
            {
                MessageBox.Show("Peso de una hectárea de semilla inválido");
                return false;
            }

            return true;
        }
        
        private void PopulateData()
        {
            lbltxtName.FieldContent = _model.Name;
            lbltxtProductionDays.FieldContent = _model.ProductionDays.ToString();
            lbltxtWeightOf1000Seeds.FieldContent = _model.WeightOf1000Seeds.ToString();
            lbltxtAmountOfSeedsPerHectare.FieldContent = _model.AmountOfSeedsPerHectare.ToString();
            lbltxtWeightOfSeedsPerHectare.FieldContent = _model.WeightOfSeedsPerHectare.ToString();
        }

        private void LlenarCasillas()
        {
            lbltxtName.FieldContent = "aji picante";
            lbltxtProductionDays.FieldContent = "45";
            lbltxtWeightOf1000Seeds.FieldContent = "678";
            lbltxtAmountOfSeedsPerHectare.FieldContent = "25000";
            lbltxtWeightOfSeedsPerHectare.FieldContent = "32.63";
        }
    }
}
