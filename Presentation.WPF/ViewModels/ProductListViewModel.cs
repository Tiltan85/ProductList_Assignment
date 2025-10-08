
using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation.WPF.ViewModels;

public partial class ProductListViewModel : ObservableObject
{

    [ObservableProperty]
    private string _title = "Product List";

}
