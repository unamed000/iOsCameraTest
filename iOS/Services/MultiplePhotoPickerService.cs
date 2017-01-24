using System.Collections.Generic;
using System.Linq;
using UIKit;
using iOsCameraTest.Services;

namespace iOsCameraTest.iOS
{
	public class MultiplePhotoPickerService : IMultiplePhotoPickerService
	{
		private MultiplePictureTakerServiceEvents Events { get; set; }

		private MultiplePictureTakerController CameraPicker { get; set; }

		private UIViewController GetRootController()
		{
			// Find the root element
			UIWindow window = UIApplication.SharedApplication.KeyWindow;
			UIViewController rootController = window.RootViewController;

			if (rootController == null || (rootController.PresentedViewController != null && rootController.PresentedViewController.GetType() == typeof(UIAlertController)))
			{
				window = UIApplication.SharedApplication.Windows.OrderByDescending(w => w.WindowLevel)
						.FirstOrDefault(w => w.RootViewController != null);

				rootController = window.RootViewController;
			}

			while (rootController.PresentedViewController != null)
			{
				rootController = rootController.PresentedViewController;
			}
			return rootController;
		}

		public MultiplePictureTakerServiceEvents TakePhotos(List<byte[]> imageBytesList = null, MultiplePictureTakerServiceEvents events = null)
		{
			var rootController = GetRootController();

			// Start the new controller
			Events = events ?? new MultiplePictureTakerServiceEvents();
			CameraPicker = new MultiplePictureTakerController(Events, imageBytesList?.Select(MultiplePictureTakerController.ImageFromByteArray).ToList());
			rootController.PresentViewController(CameraPicker, true, null);

			return Events;
		}

		public void SetPhotosAndDisplayAgain(List<byte[]> pictures)
		{
			CameraPicker.SetPictures(pictures?.Select(MultiplePictureTakerController.ImageFromByteArray).ToList());
			var rootController = GetRootController();

			rootController.PresentViewController(CameraPicker, true, null);
		}
	}
}
