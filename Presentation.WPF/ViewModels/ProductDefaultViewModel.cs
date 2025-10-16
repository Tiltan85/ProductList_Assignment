
using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation.WPF.ViewModels;

public partial class ProductDefaultViewModel : ObservableObject
{
    [ObservableProperty]
    public string _message = "Select a product to view";
}
