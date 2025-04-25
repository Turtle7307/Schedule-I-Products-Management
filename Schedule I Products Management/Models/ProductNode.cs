using System;
using System.Windows.Input;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;

namespace Schedule_I_Products_Management.Models;

public class ProductNode : ReactiveObject, ICommand
{
    public event EventHandler? CanExecuteChanged;
    private IProductWrapper? _product;
    public Action? OnClick { get; set; }
    
    public ProductNode(IProductWrapper? product, Action? onClick = null)
    {
        _product = product;
        OnClick = onClick;
        if(product is not null)
            _product.WhenAnyValue(x => x.Name)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(Name)));
    }
    

    public string Name {
        get
        {
            return _product switch
            {
                null => "???",
                _ => _product.Name
            };
        }
    }

    public IBrush BackgroundColor
    {
        get
        {
            return _product switch
            {
                null => Brushes.Red,
                BaseProductWrapper => Brushes.Cyan,
                _ => Brushes.LimeGreen
            };
        }
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if(OnClick is not null)
            Dispatcher.UIThread.Post(OnClick);
    }
}