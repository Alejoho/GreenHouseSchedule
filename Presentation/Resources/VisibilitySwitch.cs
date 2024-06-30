using System.Windows;
using System.Windows.Controls;

namespace Presentation.Resources;

internal static class VisibilitySwitch
{
    internal static void Switch(object sender)
    {
        var row = DataGridRow.GetRowContainingElement((Button)sender);

        row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ?
        Visibility.Collapsed : Visibility.Visible;
    }
}
