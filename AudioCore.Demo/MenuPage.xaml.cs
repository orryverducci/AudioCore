using System;
using Xamarin.Forms;

namespace AudioCore.Demo
{
    public partial class MenuPage : ContentPage
    {
        public ListView MenuList
        {
            get {
                return menuList;
            }
        }

        public MenuPage()
        {
            InitializeComponent();
        }
    }
}