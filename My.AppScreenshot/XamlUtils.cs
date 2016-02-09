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
	public class XamlUtils
	{
		public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObject) where T : DependencyObject
		{
			if(depObject != null)
			{
				for(int i = 0; i < VisualTreeHelper.GetChildrenCount(depObject); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObject, i);
					if(child != null && child is T)
					{
						yield return (T)child;
					}
					foreach (T childOfChild in FindVisualChildren<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}

		public static IEnumerable<FrameworkElement> FindPanels(DependencyObject depObject)
		{
			if (depObject != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObject); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObject, i);
					if (child != null && (child is Panel))
					{
						yield return (Panel)child;
					}
					else if (child != null && child is Panel)
					{
						foreach (var childOfChild in FindPanels(child))
						{
							yield return childOfChild;
						}
					}
				}
			}
		}

		public static IEnumerable<FrameworkElement> FindNonPanels(DependencyObject depObject)
		{
			if(depObject != null)
			{
				for(int i = 0; i < VisualTreeHelper.GetChildrenCount(depObject); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObject, i);
					if(child != null && !(child is Panel))
					{
						yield return (FrameworkElement)child;
					}
					else if(child != null && child is Panel)
					{
						foreach (FrameworkElement childOfChild in FindNonPanels(child))
						{
							yield return childOfChild;
						}
					}
				}
			}
		}
	}
}
