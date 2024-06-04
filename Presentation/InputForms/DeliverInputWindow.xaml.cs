using Presentation.IRequesters;
using System.Windows;

namespace Presentation.InputForms;

/// <summary>
/// Interaction logic for DeliverInputWindow.xaml
/// </summary>
public partial class DeliverInputWindow : Window
{
    //NEXT - Desing this window
    //NEXT - Make the logic of this window
    private IDeliveredBlockRequester _requester;
    public DeliverInputWindow(IDeliveredBlockRequester requestingWindow)
    {
        InitializeComponent();
        _requester = requestingWindow;
    }
}
