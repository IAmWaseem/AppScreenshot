using My.AppScreenshot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace My.AppScreenshot
{
	public class ScreenshotManager
	{
		private static List<FrameworkElement> Panels;
		private static List<FrameworkElement> NonPanels;
		private static DependencyObject ControlToIgnore;
		private static PointerEventHandler NonPanelScreenshotHandler = new PointerEventHandler(ScreenshotEventHandler);
		private static PointerEventHandler NonPanelOpacityEnterHandler = new PointerEventHandler(OpacityEnterEventHandler);
		private static PointerEventHandler NonPanelOpacityLeaveHandler = new PointerEventHandler(OpacityLeaveEventHandler);

		private static bool ChildScreenshotEvent;
		private static bool ChildOpacityEnterEvent;
		private static bool ChildOpacityLeaveEvent;

		private static readonly double PointerOverOpacity = 0.5f;
		private static readonly double PointerExitOpacity = 1.0f;

		public static void Start(DependencyObject depObject, DependencyObject controlToIgnore)
		{
			ControlToIgnore = controlToIgnore;
			ExtractControls(depObject, controlToIgnore);
			AttachEvents();
			SetOpacity(Panels, PointerOverOpacity);
		}

		public static void Stop()
		{
			DetachEvents();
			SetOpacity(Panels, PointerExitOpacity);
			SetOpacity(NonPanels, PointerExitOpacity);
			Clean();
		}

		private static void AttachEvents()
		{
			AttachEvent(UIElement.PointerPressedEvent, NonPanelScreenshotHandler);
			AttachEvent(UIElement.PointerEnteredEvent, NonPanelOpacityEnterHandler);
			AttachEvent(UIElement.PointerExitedEvent, NonPanelOpacityLeaveHandler);
		}

		private static void DetachEvents()
		{
			DetachEvent(UIElement.PointerPressedEvent, NonPanelScreenshotHandler);
			DetachEvent(UIElement.PointerEnteredEvent, NonPanelOpacityEnterHandler);
			DetachEvent(UIElement.PointerExitedEvent, NonPanelOpacityLeaveHandler);
		}

		private static void SetOpacity(List<FrameworkElement> elements, double amount)
		{
			foreach (var item in elements)
				item.Opacity = amount;
		}

		private static void AttachEvent(RoutedEvent e, PointerEventHandler Handler)
		{
			foreach (var item in Panels)
			{
				if (e == UIElement.PointerPressedEvent)
					item.PointerPressed += Handler;
				else if (e == UIElement.PointerEnteredEvent)
					item.PointerEntered += Handler;
				else if (e == UIElement.PointerExitedEvent)
					item.PointerExited += Handler;
			}
			foreach (var item in NonPanels)
			{
				item.AddHandler(e, Handler, true);
			}
		}

		private static void DetachEvent(RoutedEvent e, PointerEventHandler Handler)
		{
			foreach (var item in Panels)
			{
				if (e == UIElement.PointerPressedEvent)
					item.PointerPressed -= Handler;
				else if (e == UIElement.PointerEnteredEvent)
					item.PointerEntered -= Handler;
				else if (e == UIElement.PointerExitedEvent)
					item.PointerExited -= Handler;
			}
			foreach (var item in NonPanels)
			{
				item.RemoveHandler(e, Handler);
			}
		}

		private static void Clean()
		{
			Panels.Clear();
			NonPanels.Clear();
		}

		private static void ExtractControls(DependencyObject depObject, DependencyObject controlToIgnore)
		{
			Panels = XamlUtils.FindPanels(depObject).ToList();
			NonPanels = XamlUtils.FindNonPanels(depObject).ToList();
			if (controlToIgnore is Panel)
				Panels.Remove((Panel)controlToIgnore);
			else
				NonPanels.Remove((FrameworkElement)controlToIgnore);
		}

		private static async void ScreenshotEventHandler(object sender, PointerRoutedEventArgs e)
		{
			if (!(sender is Panel))
			{
				ChildScreenshotEvent = true;
				e.Handled = true;
			}
			else
			{
				ChildScreenshotEvent = false;
				await Task.Delay(TimeSpan.FromMilliseconds(1));
				if (ChildScreenshotEvent)
					return;
			}
			FileSavePicker picker = new FileSavePicker();
			picker.FileTypeChoices.Add("PNG Files", new List<string> { ".png" });
			picker.DefaultFileExtension = ".png";
			picker.SuggestedFileName = "AppScreenshot";
			var file = await picker.PickSaveFileAsync();
			if (file != null)
			{
				(sender as FrameworkElement).Opacity = PointerExitOpacity;
				SetOpacity(Panels, PointerExitOpacity);
				SetOpacity(NonPanels, PointerExitOpacity);
				await new ScreenshotService().TakeScreenshotAsync(sender as FrameworkElement, file);
			}
		}

		private static async void OpacityEnterEventHandler(object sender, PointerRoutedEventArgs e)
		{
			if (!(sender is Panel))
			{
				ChildOpacityEnterEvent = true;
				e.Handled = true;
			}
			else
			{
				ChildOpacityEnterEvent = false;
				await Task.Delay(TimeSpan.FromMilliseconds(1));
				if (ChildOpacityEnterEvent)
					return;
			}

			if (sender is Panel)
			{
				foreach (var item in (sender as Panel)?.Children)
				{
					if (item != ControlToIgnore)
						item.Opacity = PointerExitOpacity;
				}
				(ControlToIgnore as FrameworkElement).Opacity = PointerExitOpacity;
			}
			DependencyObject parent;
			if ((parent = VisualTreeHelper.GetParent(sender as DependencyObject)) != null)
			{
				(parent as FrameworkElement).Opacity = PointerExitOpacity;
				if (parent is Panel)
				{
					foreach (var item in (parent as Panel)?.Children)
					{
						if (item != ControlToIgnore)
							item.Opacity = PointerOverOpacity;
					}
				}
			}
			(sender as FrameworkElement).Opacity = PointerExitOpacity;

		}

		private static async void OpacityLeaveEventHandler(object sender, PointerRoutedEventArgs e)
		{
			if (!(sender is Panel))
			{
				ChildOpacityLeaveEvent = true;
				e.Handled = true;
			}
			else
			{
				ChildOpacityLeaveEvent = false;
				await Task.Delay(TimeSpan.FromMilliseconds(1));
				if (ChildOpacityLeaveEvent)
					return;
			}
			if (sender is Panel)
			{
				(sender as Panel).Opacity = PointerExitOpacity;
				foreach (var item in (sender as Panel)?.Children)
				{
					if (item != ControlToIgnore)
						item.Opacity = PointerExitOpacity;
				}
				(ControlToIgnore as FrameworkElement).Opacity = PointerExitOpacity;
			}
			else
			{
				(sender as FrameworkElement).Opacity = PointerOverOpacity;
			}
		}
	}
}
