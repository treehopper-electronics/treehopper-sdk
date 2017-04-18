using TreehopperShowcase.ViewModels;
using Windows.UI.Xaml.Controls;

namespace TreehopperShowcase.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
        }

        // strongly-typed view models enable x:bind
        public MainPageViewModel ViewModel => DataContext as MainPageViewModel;
    }
}
