﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace SupportLayer.Models;

[DebuggerDisplay(null, Name = "{Id} - {Client.Name,nq} - {Product.Variety,nq}")]
public partial class Order : INotifyPropertyChanged
{
    [NotMapped]
    private int _trigger;

    [NotMapped]
    public int UIColorsUpdateTrigger
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
            return _orderLocationView ?? null;
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

    [NotMapped]
    private ObservableCollection<Block> _blocksView;

    [NotMapped]
    public ObservableCollection<Block> BlocksView
    {
        get
        {
            return _blocksView ?? null;
        }

        set
        {
            _blocksView = value;
        }
    }
}
