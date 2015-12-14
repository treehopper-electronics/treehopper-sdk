using TreehopperShowcase.ViewModels;
using Windows.UI.Xaml.Controls;

namespace TreehopperShowcase.Views
{
    public sealed partial class Blink : Page
    {
        public Blink()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
        }

        // strongly-typed view models enable x:bind
        public MainPageViewModel ViewModel => this.DataContext as MainPageViewModel;
    }
}
