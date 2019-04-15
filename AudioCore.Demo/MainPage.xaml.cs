using System;
using Xamarin.Forms;

namespace AudioCore.Demo
{
	public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
			menuPage.MenuList.ItemSelected += MenuItemSelected;
        }

		private void MenuItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			MenuItem selectedPage = e.SelectedItem as MenuItem;
			if (selectedPage != null)
			{
                ((DemoPage)Detail).Dispose();
                Detail = (Page)Activator.CreateInstance(selectedPage.TargetType);
				menuPage.MenuList.SelectedItem = null;
			}
		}
    }
}