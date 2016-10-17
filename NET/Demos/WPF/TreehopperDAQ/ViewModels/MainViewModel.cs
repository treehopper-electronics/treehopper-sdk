﻿using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using Treehopper.Mvvm.ViewModel;
using Treehopper;
using Treehopper.Mvvm;
using TreehopperDAQ.Models;
using System.Diagnostics;
using System.Windows;
using System.Timers;
using System.Collections.Generic;

namespace TreehopperDAQ.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly SelectorViewModel selector = new SelectorViewModel();
        public SelectorViewModel Selector { get { return selector; } }

        private Stopwatch sw = new Stopwatch(); // accurate timer
        Timer timer = new Timer(); // fire every second to update the GUI

        public MainViewModel()
        {
            Selector.OnBoardConnected += Selector_OnBoardConnected;
            Selector.OnBoardDisconnected += Selector_OnBoardDisconnected;
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1000 / refreshRate;
            msFreq = Stopwatch.Frequency / 1000.0;
        }

        Queue<DataPoint> buffer = new Queue<DataPoint>(); // temp buffer to avoid tying down the GUI
        public ObservableCollection<DataPoint> Data { get; set; } = new ObservableCollection<DataPoint>(); // GUI-accessible buffer

        private TreehopperUsb board;

        private int sampleRate = 10000;
        public string SampleRate { get { return sampleRate.ToString(); } set { sampleRate = int.Parse(value); if(board != null) board.Connection.UpdateRate = sampleRate; } }

        public double refreshRate = 2;
        public string RefreshRate { get { return refreshRate.ToString(); } set { refreshRate = double.Parse(value); timer.Interval = 1000 / refreshRate; } }

        private void Selector_OnBoardConnected(object sender, BoardConnectedEventArgs e)
        {
            Data.Clear();
            board = e.Board;
            board.Connection.UpdateRate = sampleRate;
            for (int i = 0; i < 20; i++)
                board.Pins[i].Mode = PinMode.AnalogInput;


            board.OnPinValuesUpdated += Board_OnPinValuesUpdated;
            sw.Restart();
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                while (buffer.Count > 0)
                    Data.Add(buffer.Dequeue());
            });
        }

        double msFreq = 0;
        private void Board_OnPinValuesUpdated(object sender, System.EventArgs e)
        {
            var data = new DataPoint();
            var newValues = new double[board.Pins.Count];
            for (int i = 0; i < board.Pins.Count; i++)
            {
                newValues[i] = board.Pins[i].AnalogVoltage;
            }
            data.Values = newValues;
            data.TimestampOffset = sw.ElapsedTicks / msFreq;

            buffer.Enqueue(data);
        }

        private void Selector_OnBoardDisconnected(object sender, BoardDisconnectedEventArgs e)
        {
            timer.Stop();
        }

    }
}