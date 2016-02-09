using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace My.AppScreenshot
{
	public class ScreenshotButton : Button
	{
		public ScreenshotButton()
		{
			Click += ScreenshotButton_Click;
		}

		private bool isScreenshotModeEnabled = false;



		public Panel Root
		{
			get { return (Panel)GetValue(RootProperty); }
			set { SetValue(RootProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Root.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RootProperty =
			DependencyProperty.Register("Root", typeof(Panel), typeof(ScreenshotButton), new PropertyMetadata(null));


		private void ScreenshotButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if(!isScreenshotModeEnabled)
			{
				isScreenshotModeEnabled = true;
				ScreenshotManager.Start(VisualTreeHelper.GetParent(Root), this);
			}
			else
			{
				isScreenshotModeEnabled = false;
				ScreenshotManager.Stop();
			}
		}
	}
}
