using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace FabWiz.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadApplication(new FabWiz.App());
        }
    }
}
