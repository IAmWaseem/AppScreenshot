using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace My.AppScreenshot.Services
{
	public class ScreenshotService : IScreenshotService
	{
		public async Task TakeScreenshotAsync(FrameworkElement element, StorageFile file)
		{
			CachedFileManager.DeferUpdates(file);
			using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
			{
				await CaptureToStreamAsync(element, stream, BitmapEncoder.PngEncoderId);
			}
			await CachedFileManager.CompleteUpdatesAsync(file);
		}

		private async Task<RenderTargetBitmap> CaptureToStreamAsync(FrameworkElement element, IRandomAccessStream stream, Guid encoderId)
		{
			var renderTargetBitmap = new RenderTargetBitmap();
			await renderTargetBitmap.RenderAsync(element);
			IBuffer pixels = await renderTargetBitmap.GetPixelsAsync();
			double logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
			var encoder = await BitmapEncoder.CreateAsync(encoderId, stream);
			encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (UInt32)(renderTargetBitmap.PixelWidth), (UInt32)(renderTargetBitmap.PixelHeight), logicalDpi, logicalDpi, pixels.ToArray());
			await encoder.FlushAsync();
			return renderTargetBitmap;
		}
	}
}
