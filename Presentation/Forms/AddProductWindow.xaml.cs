using Domain.Processors;
using SupportLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for AddProductWindow.xaml
/// </summary>
public partial class AddProductWindow : Window, ISpeciesRequester
{
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

    }

    public void SpeciesComplete(Species model)
    {
        throw new System.NotImplementedException();
    }
}
