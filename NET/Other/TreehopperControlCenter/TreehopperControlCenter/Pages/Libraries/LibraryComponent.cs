using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TreehopperControlCenter.Pages.Libraries
{
    public abstract class LibraryComponent : ContentView, IDisposable
    {
        public LibraryComponent(string title, LibrariesPage Parent)
        {
            this.Title = title;
            StartButtonText = "Start";
            RemoveCommand = new Command(() =>
            {
                if(Parent.Components.Contains(this))
                    Parent.Components.Remove(this);
                
                this.Dispose();
            });

            StartCommand = new Command(() =>
            {
                if(!IsRunning)
                {
                    // start
                    StartButtonText = "Stop";
                    IsRunning = true;
                    Start();
                } else
                {
                    StartButtonText = "Start";
                    IsRunning = false;
                    Stop();
                }
            });
        }

        private bool isRunning = false;

        public bool IsRunning
        {
            get { return isRunning; }
            set {
                isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
                OnPropertyChanged(nameof(IsStopped));
            }
        }

        public bool IsStopped => !IsRunning;


        public string Title { get; set; }

        private string startButtonText;

        public string StartButtonText
        {
            get { return startButtonText; }
            set {
                startButtonText = value;
                OnPropertyChanged(nameof(StartButtonText));
            }
        }

        protected abstract Task Start();

        protected abstract Task Stop();

        public abstract void Dispose();

        public View Configuration { get; set; }

        public ICommand RemoveCommand { protected set; get; }

        public ICommand StartCommand { protected set; get; }

        public abstract Task Update();
    }
}
