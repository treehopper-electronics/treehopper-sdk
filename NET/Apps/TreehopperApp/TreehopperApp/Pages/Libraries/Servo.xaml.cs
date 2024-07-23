﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Treehopper.Libraries.Sensors.Temperature;
using Treehopper.Libraries.Sensors.Optical;
using Treehopper.Libraries.Sensors;
using Treehopper.Libraries.Sensors.Pressure;
using Treehopper.Libraries.Motors;

namespace TreehopperApp.Pages.Libraries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Servo : LibraryComponent
    {
        public Pwm SelectedPin { get; set; }
        public TreehopperUsb Board { get; }

        public HobbyServo HobbyServo { get; set; }

        public Servo(LibrariesPage page, TreehopperUsb Board = null) : base("Servo", page)
        {
            this.Board = Board;
            InitializeComponent();
            BindingContext = this;
        }

        public Servo() : base("Servo", new LibrariesPage())
        {
            InitializeComponent();
        }

        public override async Task Start()
        {
            HobbyServo = new HobbyServo(SelectedPin);
            HobbyServo.Angle = 90;
            OnPropertyChanged("HobbyServo");
        }

        public override async Task Stop()
        {
            HobbyServo?.Dispose();
            HobbyServo = null;
        }

        public override void Dispose()
        {

        }

        public override async Task Update()
        {

        }
    }
}