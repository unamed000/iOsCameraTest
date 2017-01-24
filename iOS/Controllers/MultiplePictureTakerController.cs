using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using Foundation;
using iOsCameraTest.Services;
using UIKit;

namespace iOsCameraTest.iOS
{
	[Register("MultiplePictureTakerController")]
	public class MultiplePictureTakerController : UIImagePickerController
	{
		// Resources
		protected List<UIImage> PictureData { get; private set; }
		protected List<byte[]> PictureByteData { get; private set; }

		private UIButton _retakeButton;
		private UIButton _stopButton;
		private UIButton _pictureButton;
		private UIButton _previewButton;
		private UIButton _doneButton;
		private readonly MultiplePictureTakerServiceEvents _events;

		public List<byte[]> GetPicture()
		{
			return PictureByteData;
		}

		public MultiplePictureTakerController(MultiplePictureTakerServiceEvents events, List<UIImage> pictures = null)
		{
			_events = events;
			PictureData = new List<UIImage>(10);
			PictureByteData = new List<byte[]>(10);

			if (pictures != null)
			{
				PictureData.AddRange(pictures);
				PictureByteData.AddRange(pictures.Select(ImageToByteArray));
			}
		}

		public void SetPictures(List<UIImage> pictures)
		{
			PictureData.Clear();
			PictureByteData.Clear();

			PictureData.AddRange(pictures);
			PictureByteData.AddRange(pictures.Select(ImageToByteArray));
			if (_previewButton != null && PictureData.Count > 0)
				_previewButton.SetBackgroundImage(PictureData.Last(), UIControlState.Normal);
		}


		// Methods
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Override the camera
			SourceType = UIImagePickerControllerSourceType.Camera;
			ShowsCameraControls = false;

			// Add controls
			CGSize screenSize = UIScreen.MainScreen.Bounds.Size;

			UIView topBarView;
			UIView bottomBarView;
			CGRect previewFrame;
			UIView overlayView;


			if (screenSize.Width > screenSize.Height)
			{
				// Bars
				topBarView = CreateTopBarView(screenSize.Height);
				bottomBarView = CreateBottomBarView(screenSize.Height, screenSize.Width);

				// Camera
				float cameraAspectRatio = 4.0f / 3.0f;

				float imageWidth = (float)(screenSize.Height * cameraAspectRatio);
				float horizontalAdjustment;
				if (screenSize.Width - imageWidth <= 54.0f)
				{
					horizontalAdjustment = 0;
				}
				else
				{
					horizontalAdjustment = (float)(screenSize.Width - imageWidth) / 2.0f;
					horizontalAdjustment /= 2.0f; // A little bit upper than centered
				}
				CGAffineTransform transform = CameraViewTransform;
				transform.y0 += horizontalAdjustment;
				CameraViewTransform = transform;

				previewFrame = new CGRect(0, horizontalAdjustment, screenSize.Height, imageWidth);
				overlayView = new UIView(new CGRect(0, 0, screenSize.Height, screenSize.Width));
			}
			else
			{
				// Bars
				topBarView = CreateTopBarView(screenSize.Width);
				bottomBarView = CreateBottomBarView(screenSize.Width, screenSize.Height);

				// Camera
				float cameraAspectRatio = 4.0f / 3.0f;

				float imageHeight = (float)(screenSize.Width * cameraAspectRatio);
				float verticalAdjustment;
				if (screenSize.Height - imageHeight <= 54.0f)
				{
					verticalAdjustment = 0;
				}
				else
				{
					verticalAdjustment = (float)(screenSize.Height - imageHeight) / 2.0f;
					verticalAdjustment /= 2.0f; // A little bit upper than centered
				}
				CGAffineTransform transform = CameraViewTransform;
				transform.y0 += verticalAdjustment;
				CameraViewTransform = transform;

				previewFrame = new CGRect(0, verticalAdjustment, screenSize.Width, imageHeight);
				overlayView = new UIView(new CGRect(0, 0, screenSize.Width, screenSize.Height));
			}

			// Add action
			FinishedPickingMedia += OnPictureTaken;

			overlayView.Add(bottomBarView);
			overlayView.Add(topBarView);

			// Override
			CameraOverlayView = overlayView;
		}

		// Private
		private UIView CreateTopBarView(nfloat width)
		{
			// Create layout
			UIView view = new UIView(new CGRect(0, 0, width, 40))
			{
				AutoresizingMask =
					UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin |
					UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin,
				BackgroundColor = UIColor.Black
			};

			// Add buttons
			_retakeButton = CreateButton("Retake");
			_retakeButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
			_retakeButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			_retakeButton.SizeToFit();
			_retakeButton.Center = new CGPoint((width - 15 - (_retakeButton.Frame.Width / 2)), 20);

			_stopButton = CreateButton("Close");
			_stopButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			_stopButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			_stopButton.SizeToFit();
			_stopButton.Center = new CGPoint(15 + (_stopButton.Frame.Width / 2), 20);
			// Link the events
			_retakeButton.TouchUpInside += (o, s) => OnRetakeClicked();
			_stopButton.TouchUpInside += (o, s) => OnStopClicked();

			// Add buttons
			view.Add(_retakeButton);
			view.Add(_stopButton);

			return view;
		}

		private UIView CreateBottomBarView(nfloat width, nfloat height)
		{
			// Create layout
			UIView view = new UIView(new CGRect(0, height - 100, width, 100))
			{
				AutoresizingMask =
					UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin |
					UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin,
				BackgroundColor = UIColor.Black
			};

			// Add buttons
			_doneButton = CreateButton("Done");
			_doneButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
			_doneButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			_doneButton.SizeToFit();
			_doneButton.Center = new CGPoint((width - 15 - (_doneButton.Frame.Width / 2)), 50);

			_previewButton = CreateButton(string.Empty);
			_previewButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Fill;
			_previewButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			_previewButton.Frame = new RectangleF(0, 20, 70, 70);
			if (PictureData.Count > 0)
				_previewButton.SetBackgroundImage(PictureData.Last(), UIControlState.Normal);

			_pictureButton = new UIButton
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin |
								   UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin |
								   UIViewAutoresizing.FlexibleBottomMargin,
				BackgroundColor = UIColor.Clear
			};

			UIImage normalImage = UIImage.FromBundle("PictureButton");
			_pictureButton.SetBackgroundImage(normalImage, UIControlState.Normal);

			UIImage focusedImage = UIImage.FromBundle("PictureButtonFocused");
			_pictureButton.SetBackgroundImage(focusedImage, UIControlState.Highlighted);

			_pictureButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Fill;
			_pictureButton.VerticalAlignment = UIControlContentVerticalAlignment.Fill;
			_pictureButton.Frame = new RectangleF((float)(width / 2) - 35, 20, 70, 70);

			// Link the events
			_doneButton.TouchUpInside += (o, s) => OnDoneClicked();
			_pictureButton.TouchUpInside += (o, s) => TakePicture();
			_previewButton.TouchUpInside += (o, s) => OnPreviewClicked();

			// Add button
			view.Add(_doneButton);
			view.Add(_previewButton);
			view.Add(_pictureButton);

			return view;
		}

		private UIButton CreateButton(string text)
		{
			UIButton button = new UIButton
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin |
								   UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin |
								   UIViewAutoresizing.FlexibleBottomMargin,
				BackgroundColor = UIColor.Clear
			};

			button.SetTitle(text, UIControlState.Normal);
			button.Font = UIFont.SystemFontOfSize(18);

			return button;
		}

		public static UIImage ImageFromByteArray(byte[] data)
		{
			if (data == null)
			{
				return null;
			}

			UIImage image;
			try
			{
				image = new UIImage(NSData.FromArray(data));
			}
			catch (Exception)
			{
				return null;
			}

			return image;
		}

		private static byte[] ImageToByteArray(UIImage image)
		{
			NSData data = image.AsJPEG();
			var result = new byte[data.Length];
			System.Runtime.InteropServices.Marshal.Copy(data.Bytes, result, 0, Convert.ToInt32(data.Length));

			return result;
		}

		// Override
		public override bool ShouldAutorotate()
		{
			return false;
		}

		// Events
		public void OnPictureTaken(object picker, UIImagePickerMediaPickedEventArgs info)
		{
			UIImage image = info.OriginalImage;

			if (PictureData.Count >= 10)
			{
				var alertView = new UIAlertView("Out of limit", "You can't take more than 10 pictures at once", null, "Ok");
				alertView.Show();
			}
			PictureData.Add(image);
			PictureByteData.Add(ImageToByteArray(image));
			_previewButton.SetBackgroundImage(image, UIControlState.Normal);

		}

		public void OnDoneClicked()
		{
			if (_events.OnDoneClicked?.Invoke(PictureByteData) ?? true)
			{
				DismissViewController(false, () => _events.OnDoneDismissed?.Invoke(PictureByteData));
			}

		}

		public void OnStopClicked()
		{
			if (_events.OnStopClicked?.Invoke(PictureByteData) ?? true)
			{
				DismissViewController(false, null);
			}
		}

		public void OnRetakeClicked()
		{
			PictureData.Clear();
			PictureByteData.Clear();
			_previewButton.SetBackgroundImage(null, UIControlState.Normal);
		}

		public void OnPreviewClicked()
		{
			//if (_events.OnPreviewClicked?.Invoke(PictureByteData) ?? true)
			//{
			//	DismissViewController(false, () => _events.OnPreviewDismissed?.Invoke(PictureByteData));
			//}

			this.NavigationController.PresentViewController(new PreviewPictureController(this.PictureData), true, null);
		}
	}
}
