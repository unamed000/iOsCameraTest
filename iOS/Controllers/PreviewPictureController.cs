﻿using System;
using UIKit;
using CoreGraphics;
using Carousels;
using System.Collections.Generic;

namespace iOsCameraTest.iOS
{
	public class PreviewPictureController : UIViewController
	{
		UIImageView background;
		iCarousel carousel;
		public IList<UIImage> Pictures;

		public PreviewPictureController(IList<UIImage> pictures)
		{
			Pictures = pictures;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			bool wrap = false;

			// create a nice background
			background = new UIImageView(View.Bounds);
			background.Image = UIImage.FromBundle("background.png");
			background.ContentMode = UIViewContentMode.ScaleToFill;
			background.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			View.AddSubview(background);

			// create the carousel
			carousel = new iCarousel(View.Bounds);
			carousel.Type = iCarouselType.CoverFlow2;
			carousel.DataSource = new CarouselDataSource(Pictures);
			carousel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			View.AddSubview(carousel);

			// customize the appearance of the carousel
			carousel.GetValue = (sender, option, value) =>
			{
				// set a nice spacing between items
				if (option == iCarouselOption.Spacing)
				{
					return value * 1.1F;
				}
				else if (option == iCarouselOption.Wrap)
				{
					return wrap ? 1 : 0;
				}

				// use the defaults for everything else
				return value;
			};

			// handle item selections
			carousel.ItemSelected += (sender, args) =>
			{
				using (var alert = new UIAlertView("Item Selected", string.Format("You selected item '{0}'.", args.Index), null, "OK"))
					alert.Show();
			};
		}

		// a data source that displays 100 items
		private class CarouselDataSource : iCarouselDataSource
		{
			IList<UIImage> Pictures;

			public CarouselDataSource(IList<UIImage> pictures)
			{
				// create our amazing data source
				Pictures = pictures;
			}

			// let the carousel know how many items to render
			public override nint GetNumberOfItems(iCarousel carousel)
			{
				// return the number of items in the data
				return Pictures.Count;
			}

			// create the view each item in the carousel
			public override UIView GetViewForItem(iCarousel carousel, nint index, UIView view)
			{
				UILabel label = null;
				UIImageView imageView = null;

				if (view == null)
				{
					// create new view if no view is available for recycling
					imageView = new UIImageView(new CGRect(0, 0, 200.0f, 200.0f));
					imageView.Image = Pictures[(int)index];
					imageView.ContentMode = UIViewContentMode.Center;

					label = new UILabel(imageView.Bounds);
					label.BackgroundColor = UIColor.Clear;
					label.TextAlignment = UITextAlignment.Center;
					label.Font = label.Font.WithSize(50);
					label.Tag = 1;
					imageView.AddSubview(label);
				}
				else
				{
					// get a reference to the label in the recycled view
					imageView = (UIImageView)view;
					label = (UILabel)view.ViewWithTag(1);
				}

				// set the values of the view
				label.Text = Pictures[(int)index].ToString();

				return imageView;
			}
		}


	}
}
