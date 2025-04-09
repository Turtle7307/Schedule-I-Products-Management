using System;
using ReactiveUI;
using Schedule_I_Products_Management.Data;

namespace Schedule_I_Products_Management.Models;

public class ProductEffectWrapper(ProductEffect effect) : ReactiveObject
{
    private ProductEffect _effect = effect;
    
    public Guid Id => _effect.Id;
    
    public string Name
    {
        get => _effect.Name;
        set
        {
            _effect.Name = value;
            this.RaisePropertyChanged();
        }
    }
    
    public static implicit operator ProductEffectWrapper(ProductEffect mixable) => new(mixable);
    public static implicit operator ProductEffect(ProductEffectWrapper mixableWrapper) => mixableWrapper._effect;
    
    public override string ToString()
    {
        return Name;
    }
}