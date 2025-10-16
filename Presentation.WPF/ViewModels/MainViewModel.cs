
using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject _rightViewModel = null!;

    [ObservableProperty]
    private ObservableObject _leftViewModel = null!;

}
