using Domain.Processors;
using log4net;
using Presentation.AddEditForms;
using Presentation.Resources;
using SupportLayer;
using SupportLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for SeedTrays.xaml
    /// </summary>
    public partial class SeedTraysWindow : Window
    {
        private static readonly ILog _log = LogHelper.GetLogger();

        List<SeedTray> _seedTrays;
        SeedTrayProcessor _processor;
        public SeedTraysWindow()
        {
            InitializeComponent();
            _processor = new SeedTrayProcessor();
            LoadData();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNewSeedTray_Click(object sender, RoutedEventArgs e)
        {
            AddEditSeedTrayWindow window = new AddEditSeedTrayWindow();
            window.ShowDialog();
            LoadData();
        }

        private void btnEditSeedTray_Click(object sender, RoutedEventArgs e)
        {
            EditSeedTray();
        }

        private void dgSeedTrays_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditSeedTray();
        }

        private void btnDeleteSeedTray_Click(object sender, RoutedEventArgs e)
        {
            if (dgSeedTrays.SelectedItem is SeedTray seedTray)
            {
                if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                    , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _processor.DeleteSeedTray(seedTray.Id);
                        _seedTrays.Remove(seedTray);
                        RefreshData();

                        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(seedTray);
                        _log.Info("A SeedTray record was deleted from the DB");
                        log4net.GlobalContext.Properties["Model"] = "";
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.IsForeignKeyViolation())
                    {
                        _log.Error("Attend to delete a related Seedtray record from the DB", ex);

                        MessageBox.Show("El registro seleccionado no se puede borrar, porque esta relacionado " +
                            "con otros registros.", "Operación Invalida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                    {
                        _log.Error("An unexpected database error occurred while deleting an organization.", ex);

                        MessageBox.Show("Ocurrió un error al intentar eliminar el registro.",
                            "Error de Base de Datos",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar el registro que desea eliminar."
                    , "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadData()
        {
            _seedTrays = _processor.GetAllSeedTrays().ToList();
            dgSeedTrays.ItemsSource = _seedTrays;
        }

        private void RefreshData()
        {
            dgSeedTrays.ItemsSource = null;
            dgSeedTrays.ItemsSource = _seedTrays;
        }

        private void EditSeedTray()
        {
            if (dgSeedTrays.SelectedItem is SeedTray seedTray)
            {
                AddEditSeedTrayWindow window = new AddEditSeedTrayWindow(seedTray);
                window.ShowDialog();
                RefreshData();
            }
            else
            {
                MessageBox.Show("Debe seleccionar el registro que desea editar."
                    , "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
