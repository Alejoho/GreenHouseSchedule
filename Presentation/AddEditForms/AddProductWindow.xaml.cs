using Domain.Processors;
using log4net;
using Presentation.IRequesters;
using SupportLayer;
using SupportLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Presentation.AddEditForms;

/// <summary>
/// Interaction logic for AddProductWindow.xaml
/// </summary>
public partial class AddProductWindow : Window, ISpeciesRequester
{
    private static readonly ILog _log = LogHelper.GetLogger();
    private ProductProcessor _processor;
    private Product _model;
    private List<Species> _species;
    IProductRequester _requester;

    public AddProductWindow(IProductRequester requestingWindow)
    {
        InitializeComponent();
        _processor = new ProductProcessor();
        _model = new Product();
        _requester = requestingWindow;
        LoadData();

        _log.Info("The AddProductWindow was opened by NewOrderWindow to add a new Product");
    }

    private void LoadData()
    {
        SpeciesProcessor speciesProcessor = new SpeciesProcessor();
        _species = speciesProcessor.GetAllSpecies().ToList();
        lblcmbbtnSpecies.ComboBox.ItemsSource = _species;
        lblcmbbtnSpecies.ComboBox.DisplayMemberPath = "Name";
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateDataType() == true)
        {
            if (_processor.SaveProduct(_model) == true)
            {
                MessageBox.Show("Registro salvado");

                log4net.GlobalContext.Properties["Model"] = _model;
                _log.Info("A Product record was saved to the DB");
                log4net.GlobalContext.Properties["Model"] = "";

                _requester?.ProductComplete(_model);
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
        bool output = true;
        _model.Variety = lbltxtVariety.FieldContent;

        if (lblcmbbtnSpecies.ComboBox.SelectedItem != null)
        {
            _model.SpecieId = ((Species)lblcmbbtnSpecies.ComboBox.SelectedItem).Id;
        }

        return output;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void lblcmbbtnSpecies_ButtonClick(object sender, RoutedEventArgs e)
    {
        AddEditSpeciesWindow window = new AddEditSpeciesWindow(this);
        window.ShowDialog();
    }

    public void SpeciesComplete(Species model)
    {
        _species.Add(model);
        lblcmbbtnSpecies.ComboBox.ItemsSource = _species.OrderBy(x => x.Name);
        lblcmbbtnSpecies.ComboBox.SelectedItem = model;
    }
}
