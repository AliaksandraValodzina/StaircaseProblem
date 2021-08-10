using System;
using StaircaseProblem.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StaircaseProblem
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new StairPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
