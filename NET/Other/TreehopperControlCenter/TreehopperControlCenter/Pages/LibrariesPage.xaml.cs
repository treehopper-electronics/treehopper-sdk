using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TreehopperControlCenter.Pages.Libraries;
using System.Reflection;
using System.Threading;

namespace TreehopperControlCenter.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LibrariesPage : ContentPage
	{
        public ObservableCollection<LibraryComponent> Components { get; set; } = new ObservableCollection<LibraryComponent>();

        public Dictionary<string, Type> ComponentList = new Dictionary<string, Type>();

        TreehopperUsb Board;

        public LibrariesPage (TreehopperUsb Board)
		{
            InitializeComponent ();

            this.Board = Board;

            components.ItemsSource = Components;

            foreach (Type type in typeof(LibraryComponent).GetTypeInfo().Assembly.GetTypes().Where(type => typeof(LibraryComponent).IsAssignableFrom(type) && type != typeof(LibraryComponent)))
            {
                LibraryComponent item = (LibraryComponent)Activator.CreateInstance(type, new object[] { this, Board });

                ComponentList.Add(item.Title, type);
            }
        }

        public LibrariesPage()
        {
            InitializeComponent();
            components.ItemsSource = Components;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Select Driver", "Cancel", "", ComponentList.Keys.ToArray());
            if(ComponentList.Keys.Contains(action))
            {
                Components.Add((LibraryComponent)Activator.CreateInstance(ComponentList[action], new object[] { this, Board }));
            }
        }
    }

    public static class ReflectiveEnumerator
    {
        static ReflectiveEnumerator() { }

        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            List<Type> derivedClassList = typeof(T).GetTypeInfo().Assembly.GetTypes().Where(type => typeof(T).IsAssignableFrom(type) && type != typeof(T)).ToList();

            foreach (Type type in derivedClassList)
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }

            return objects;
        }
    }
}