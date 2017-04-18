using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Utilities;
using Treehopper.Uwp;
using Windows.UI.Xaml.Media.Imaging;

namespace TreehopperShowcase.ViewModels
{
    public class FlirViewModel : Mvvm.ViewModelBase
    {
        public FlirViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                SelectedBoard = Boards[1];
            }
        }

        public ObservableCollection<TreehopperUsb> Boards => ConnectionService.Instance.Boards;

        WriteableBitmap currentFrame;
        public WriteableBitmap CurrentFrame { get { return currentFrame; } set { Set(ref currentFrame, value); } }

        TreehopperUsb selectedBoard;
        public TreehopperUsb SelectedBoard
        {
            get
            {
                return selectedBoard;
            }
            set
            {
                if (selectedBoard != null)
                    selectedBoard.Disconnect();
                Set(ref selectedBoard, value);
                if (selectedBoard != null)
                {
                    Start().Forget();
                }

            }
        }
        public async Task Start()
        {
            await selectedBoard.ConnectAsync();

            //SelectedBoard.SPI.ChipSelect = SelectedBoard.Pin6;

            //FlirLepton flir = new FlirLepton(SelectedBoard);

            while (true)
            {
                //CurrentFrame = await flir.GetFrame();
                await Task.Delay(10);
            }
        }
    }
}
