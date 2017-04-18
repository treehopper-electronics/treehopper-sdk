using System.Collections.Specialized;

namespace Treehopper.Mvvm.ViewModel
{
    /// <summary>
    ///     A view model used to power a XAML Selector view
    /// </summary>
    public class SelectorDesignTimeViewModel : SelectorViewModelBase
    {
        /// <summary>
        ///     Construct a view model used to power a XAML Selector view
        /// </summary>
        public SelectorDesignTimeViewModel(bool selectBoard = true,
            DesignTimeTestData testData = DesignTimeTestData.None) : base(new DesignTimeConnectionService())
        {
            if (selectBoard)
            {
                SelectedBoard = Boards[0];
                ConnectCommand.Execute(this);
            }

            //switch(testData)
            //{
            //    case DesignTimeTestData.Analog:
            //        SelectedBoard.GenerateAnalogDemoData();
            //        break;
            //    case DesignTimeTestData.Digital:
            //        SelectedBoard.GenerateDigitalTestData();
            //        break;

            //}
        }

        /// <summary>
        ///     Occurs when the board collection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }
    }

    /// <summary>
    ///     Generate design time data
    /// </summary>
    public enum DesignTimeTestData
    {
        /// <summary>
        ///     No data
        /// </summary>
        None,

        /// <summary>
        ///     Analog data
        /// </summary>
        Analog,

        /// <summary>
        ///     Digital data
        /// </summary>
        Digital
    }
}