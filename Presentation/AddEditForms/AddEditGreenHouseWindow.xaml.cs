using Domain.Processors;
using log4net;
using SupportLayer;
using SupportLayer.Models;
using System;
using System.Windows;

namespace Presentation.AddEditForms;

/// <summary>
/// Interaction logic for GreenHousesWindow.xaml
/// </summary>
public partial class AddEditGreenHouseWindow : Window
{
    private static readonly ILog _log = LogHelper.GetLogger();
    private GreenHouseProcessor _processor;
    private GreenHouse _model;

    public AddEditGreenHouseWindow()
    {
        InitializeComponent();
        _processor = new GreenHouseProcessor();
        _model = new GreenHouse();

        _log.Info("The AddEditGreenHouseWindow was opened to add a new GreenHouse");
    }

    public AddEditGreenHouseWindow(GreenHouse model)
    {
        InitializeComponent();
        _processor = new GreenHouseProcessor();
        this._model = model;
        PopulateData();

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_model);
        _log.Info("The AddEditGreenHouseWindow was opened to edit a GreenHouse");
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
            if (_processor.SaveGreenHouse(_model) == true)
            {
                MessageBox.Show("Registro salvado");

                log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_model);
                _log.Info("A GreenHouse record was saved to the DB");
                log4net.GlobalContext.Properties["Model"] = "";

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

    private bool ValidateDataType()
    {
        decimal width = -1;
        decimal length = -1;

        _model.Name = tbtxtName.FieldContent;

        _model.Description = txtDescription.Text;

        //LATER Change all the evaluation of these type to this current example
        if (string.IsNullOrEmpty(tbtxtWidth.FieldContent) == false)
        {
            if (decimal.TryParse(tbtxtWidth.FieldContent, out width))
            {
                _model.Width = width;
            }
            else
            {
                MessageBox.Show("Ancho inválido");
                return false;
            }
        }

        if (string.IsNullOrEmpty(tbtxtLength.FieldContent) == false)
        {
            if (decimal.TryParse(tbtxtLength.FieldContent, out length))
            {
                _model.Length = length;
            }
            else
            {
                MessageBox.Show("Largo inválido");
                return false;
            }
        }

        if (width != -1 && length != -1)
        {
            _model.GreenHouseArea = Math.Round(width * length, 2);
        }

        if (decimal.TryParse(tbtxtSeedTrayArea.FieldContent, out decimal seedTrayArea))
        {
            _model.SeedTrayArea = seedTrayArea;
        }
        else
        {
            MessageBox.Show("Área de bandejas inválido");
            return false;
        }

        if (byte.TryParse(tbtxtAmountOfBlocks.FieldContent, out byte amountOfBlocks))
        {
            _model.AmountOfBlocks = amountOfBlocks;
        }
        else
        {
            MessageBox.Show("Cantidad de bloques inválido");
            return false;
        }

        _model.Active = chkActive.IsChecked ?? false;

        return true;
    }

    private void PopulateData()
    {
        tbtxtName.FieldContent = _model.Name;
        tbtxtWidth.FieldContent = _model.Width.ToString();
        tbtxtLength.FieldContent = _model.Length.ToString();
        tbtxtSeedTrayArea.FieldContent = _model.SeedTrayArea.ToString();
        tbtxtAmountOfBlocks.FieldContent = _model.AmountOfBlocks.ToString();
        txtDescription.Text = _model.Description;
        chkActive.IsChecked = _model.Active;
    }
}
