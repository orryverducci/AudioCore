using System;
using Xamarin.Forms;

namespace AudioCore.Demo
{
    public abstract class DemoPage : ContentPage, IDisposable
    {
        /// <summary>
        /// Disposes all resources held by the page.
        /// </summary>
        public abstract void Dispose();
    }
}