using System;
using ReactiveUI;
using Schedule_I_Products_Management.Data;

namespace Schedule_I_Products_Management.Models;

public class MixableWrapper(Mixable mixable) : ReactiveObject
{
    private Mixable _mixable = mixable;
    
    public Guid Id
    {
        get => _mixable.Id;
        set
        {
            _mixable.Id = value;
            this.RaisePropertyChanged();
        }
    }
    
    public string Name
    {
        get => _mixable.Name;
        set
        {
            _mixable.Name = value;
            this.RaisePropertyChanged();
        }
    }
    
    public int Cost
    {
        get => _mixable.Cost;
        set
        {
            _mixable.Cost = value;
            this.RaisePropertyChanged();
        }
    }
    
    public static implicit operator MixableWrapper(Mixable mixable) => new(mixable);
    public static implicit operator Mixable(MixableWrapper mixableWrapper) => mixableWrapper._mixable;
    
    public override string ToString()
    {
        return Name;
    }
}