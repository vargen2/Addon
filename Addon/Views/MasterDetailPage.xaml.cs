using Addon.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Addon.Views
{
    public sealed partial class MasterDetailPage : Page
    {

        public static MasterDetailPage MyMasterDetailPage;

        public MasterDetailViewModel ViewModel { get; } = new MasterDetailViewModel();

        public MasterDetailPage()
        {
            InitializeComponent();
            Loaded += MasterDetailPage_Loaded;

        }

        private async void MasterDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync(MasterDetailsViewControl.ViewState);
            MyMasterDetailPage = this;
            Window.Current.Content.PointerPressed +=ForegroundElement_PointerPressed;
        }

        public void MyResize(double xDelta)
        {
            //Debug.WriteLine(ContentArea.ActualWidth);
            var elements = ContentArea.Children;
            foreach (var item in elements)
            {
                var mast = item as MasterDetailsView;
                mast.MasterPaneWidth = mast.MasterPaneWidth + xDelta;
                //Debug.WriteLine(mast.ActualWidth + ", " + mast.MasterPaneWidth);
                //Debug.WriteLine(mast.Parent.ToString());
            }
        }

         private void ForegroundElement_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            
            Debug.WriteLine("pressed");
            //isPressed = true;
            //xPrev = e.GetCurrentPoint(sender as UIElement).Position.X;
            ////var grid = this.Parent as Grid;
            //var gridParent = grid.Parent;

            //Debug.WriteLine(gridParent.ToString());
            //var elements = grid.Children;
            //foreach (var item in elements)
            //{
            //    Debug.WriteLine(item.ToString());
            //}

        }
    }
}
