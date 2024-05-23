using Presentation.IRequesters;
using System.Windows;

namespace Presentation.InputForms;

//CHECK - What is the property show in the taskbar
/// <summary>
/// Interaction logic for SowInputWindow.xaml
/// </summary>
public partial class SowInputWindow : Window
{

    private IOrderLocationChangeRequester _requester;
    public SowInputWindow(IOrderLocationChangeRequester requestingWindow)
    {
        InitializeComponent();
        _requester = requestingWindow;
    }

    private void btnConfirm_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}