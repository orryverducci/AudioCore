using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AudioCore.Demo
{
    public partial class DemoApp : Application
    {
        public DemoApp()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }
    }
}
