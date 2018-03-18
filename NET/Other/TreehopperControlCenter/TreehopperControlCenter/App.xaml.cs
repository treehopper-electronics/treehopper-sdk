using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TreehopperControlCenter
{
	public partial class App : Application
	{
        MainPage mainPage = new MainPage();

        public App ()
		{
			InitializeComponent();
            
            MainPage = new NavigationPage(mainPage);
		}

		protected override async void OnStart ()
		{
            await Task.Delay(500);
            await mainPage.StartApp();
            // Handle when your app starts
        }

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
