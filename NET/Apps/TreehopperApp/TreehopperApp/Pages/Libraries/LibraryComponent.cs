using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;

namespace TreehopperApp.Pages.Libraries
{
    public abstract class LibraryComponent : ContentView, IDisposable
    {
        private Timer timer;

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
                    Start(); // call component-specific start
                    StartTimer(); // start the update timer
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

        protected void StartTimer()
        {
            timer = new Timer(timerCallback, null, 100, Timeout.Infinite);
        }

        private async void timerCallback(object state)
        {
            //iterate over a temp list, just in case components are added / removed.

            var tcs = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Update().ConfigureAwait(false);
                tcs.SetResult(true);
            });
            await tcs.Task.ConfigureAwait(false);

            if(IsRunning)
                timer = new Timer(timerCallback, null, 100, Timeout.Infinite);
        }

        public abstract Task Start();

        public abstract Task Stop();

        public abstract void Dispose();

        public View Configuration { get; set; }

        public ICommand RemoveCommand { protected set; get; }

        public ICommand StartCommand { protected set; get; }

        public abstract Task Update();
    }
}
