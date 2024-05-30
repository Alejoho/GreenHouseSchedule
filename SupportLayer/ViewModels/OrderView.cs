using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupportLayer.Models
{
    public partial class Order : INotifyPropertyChanged
    {
        [NotMapped]
        private int _trigger;

        [NotMapped]
        public int Trigger
        {
            get => _trigger;
            set
            {
                _trigger = value;
                OnPropertyChanged("OrderLocationsView");
            }
        }

        [NotMapped]
        private ObservableCollection<OrderLocation> _orderLocationView;

        [NotMapped]
        public ObservableCollection<OrderLocation> OrderLocationsView
        {
            get
            {
                return _orderLocationView;
            }

            set
            {
                _orderLocationView = value;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
