using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace TreehopperShowcase.ViewModels
{
    public class DetailPageViewModel : Mvvm.ViewModelBase
    {
        public DetailPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                Value = "Designtime value";
        }

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (state.ContainsKey(nameof(Value)))
            {
                Value = state[nameof(Value)]?.ToString();
                state.Clear();
            }
            else
            {
                Value = parameter?.ToString();
            }

            return base.OnNavigatedToAsync(parameter, mode, state);
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (suspending)
                state[nameof(Value)] = Value;
            await Task.Yield();
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            return base.OnNavigatingFromAsync(args);

        }
    }
}

