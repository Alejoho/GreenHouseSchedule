using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace SupportLayer.Models;

public partial class SeedTray : INotifyPropertyChanged
{

    private bool isSelected = false;

    [NotMapped]
    public bool IsSelected
    {
        get { return isSelected; }

        set
        {
            if(value != IsSelected)
            {
                isSelected = value;
                NotifyPropertyChanged();
            }   
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
