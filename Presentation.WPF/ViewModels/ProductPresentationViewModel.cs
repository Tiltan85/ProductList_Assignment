
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using System.Collections.ObjectModel;

namespace Presentation.WPF.ViewModels;

public class ProductPresentationViewModel : ObservableObject
{
    public ObservableCollection<Product> Products { get; set; } = [];
    public Product? SelectedProduct { get; set; }
}
