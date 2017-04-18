using TreehopperShowcase.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TreehopperShowcase.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        // strongly-typed view models enable x:bind
        public SettingsPageViewModel ViewModel => DataContext as SettingsPageViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int index;
            if (int.TryParse(e.Parameter?.ToString(), out index))
                MyPivot.SelectedIndex = index;
        }
    }
}

