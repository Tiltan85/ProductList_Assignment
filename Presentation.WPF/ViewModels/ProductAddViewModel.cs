
using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation.WPF.ViewModels;

public partial class ProductAddViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "New Product";


}
