using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TreehopperControlCenter.Pages.Libraries
{
    [ContentProperty("Content")]
    public class CardView : Frame
    {
        public string Title { get; set; }

        public CardView()
        {
            Padding = 10;
            Margin = 5;
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    HasShadow = false;
                    OutlineColor = Color.Transparent;
                    BackgroundColor = Color.Transparent;
                    break;

                case Device.Windows:
                    BackgroundColor = Color.FromRgb(0xF0, 0xF0, 0xF0);
                    break;
            }

            Content = new Label();
        }
    }
}
