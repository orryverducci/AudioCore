using System;
using AppKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;
using AudioCore.Demo.Mac;

[assembly: ExportRenderer(typeof(Button), typeof(NativeButtonRenderer))]
namespace AudioCore.Demo.Mac
{
    /// <summary>
    /// Provides a custom renderer which sets a rounded bezel style to buttons, providing them with a more native appearance.
    /// </summary>
    public class NativeButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null && Control != null)
            {
                Control.BezelStyle = NSBezelStyle.Rounded;
            }
        }
    }
}