using Domain.Processors;
using log4net;
using Presentation.IRequesters;
using SupportLayer;
using SupportLayer.Models;
using System.Windows;

namespace Presentation.AddEditForms
{
    /// <summary>
    /// Interaction logic for AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditSpeciesWindow : Window
    {
        private static readonly ILog _log = LogHelper.GetLogger();
        private SpeciesProcessor _processor;
        private Species _model;
        private ISpeciesRequester _requestor;

        public AddEditSpeciesWindow(ISpeciesRequester requestor)
        {
            InitializeComponent();
            _processor = new SpeciesProcessor();
            _model = new Species();
            _requestor = requestor;

            _log.Info("The AddEditSpeciesWindow was opened to add a new Species");
        }

        public AddEditSpeciesWindow(Species model)
        {
            InitializeComponent();
            _processor = new SpeciesProcessor();
            this._model = model;
            PopulateData();

            log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_model);
            _log.Info("The AddEditSpeciesWindow was opened to edit a Species");
            log4net.GlobalContext.Properties["Model"] = "";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateDataType() == true)
            {
                if (_model.Id != 0 && MessageBox.Show("Editar estos registros puede traer resultados inesperados en la " +
                    "aplicación. Es recomendable crear un nuevo registro con los nuevos datos aunque la especie sea la misma " +
                    "y diferenciarlos de alguna manera. Por ejemplo ya existe 'Tomate' crear otro que se llame " +
                    "'Tomate 2'.\n\n ¿Desea guardar los cambios?"
                    , "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (_processor.SaveSpecies(_model) == true)
                    {
                        //MessageBox.Show("Registro salvado");

                        _requestor?.SpeciesComplete(_model);
                        this.Close();
                    }
                    else
                    {
                        ShowError();
                    }
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void ShowError()
        {
            MessageBox.Show(_processor.Error, "", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private bool ValidateDataType()
        {
            _model.Name = lbltxtName.FieldContent;

            if (byte.TryParse(lbltxtProductionDays.FieldContent, out byte productionDays))
            {
                _model.ProductionDays = productionDays;
            }
            else
            {
                MessageBox.Show("Días de producción inválido", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    MessageBox.Show("Peso de 1000 semillas inválido", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }

            if (int.TryParse(lbltxtAmountOfSeedsPerHectare.FieldContent, out int amountOfSeedsPerHectare))
            {
                _model.AmountOfSeedsPerHectare = amountOfSeedsPerHectare;
            }
            else
            {
                MessageBox.Show("Semillas en una hectárea inválido", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (decimal.TryParse(lbltxtWeightOfSeedsPerHectare.FieldContent, out decimal weightOfSeedsPerHectare))
            {
                _model.WeightOfSeedsPerHectare = weightOfSeedsPerHectare;
            }
            else
            {
                MessageBox.Show("Peso de una hectárea de semilla inválido", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}
