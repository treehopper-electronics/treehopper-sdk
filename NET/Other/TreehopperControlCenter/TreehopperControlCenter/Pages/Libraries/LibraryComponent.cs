using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace TreehopperControlCenter.Pages.Libraries
{
    public class LibraryComponent : ContentView
    {
        public LibraryComponent(string title, LibrariesPage Parent)
        {
            this.Title = title;
            RemoveCommand = new Command(() =>
            {
                Parent.Components.Remove(this);
            });
        }

        public string Title { get; set; }

        public ICommand RemoveCommand { protected set; get; }
    }
}
