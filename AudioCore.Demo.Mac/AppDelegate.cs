using AppKit;
using CoreGraphics;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

namespace AudioCore.Demo.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        private NSWindow window;

        public AppDelegate()
        {
			NSWindowStyle style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled | NSWindowStyle.Miniaturizable;
			CGRect rect = new CGRect(0, 0, 800, 500);
            window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
			window.Center();
            window.Title = "AudioCore Demo";
        }

        public override NSWindow MainWindow
        {
            get => window;
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            Forms.Init();
            LoadApplication(new App());
            base.DidFinishLaunching(notification);
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender) => true;
    }
}