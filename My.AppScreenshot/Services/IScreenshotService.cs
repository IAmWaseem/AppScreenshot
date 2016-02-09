using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace My.AppScreenshot.Services
{
	public interface IScreenshotService
	{
		Task TakeScreenshotAsync(FrameworkElement element, StorageFile file);
	}
}
