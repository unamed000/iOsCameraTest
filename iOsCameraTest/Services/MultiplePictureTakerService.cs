using System;
using System.Collections.Generic;

namespace iOsCameraTest.Services
{
	public interface IMultiplePhotoPickerService
	{
		/// <summary>
		/// When called, will show a camera view letting the user to take multiple pictures
		/// </summary>
		/// <param name="imageBytesList">List of default pictures those already taken</param>
		/// <param name="events"></param>
		MultiplePictureTakerServiceEvents TakePhotos(List<byte[]> imageBytesList = null, MultiplePictureTakerServiceEvents events = null);

		void SetPhotosAndDisplayAgain(List<byte[]> pictures);
	}

	public class MultiplePictureTakerServiceEvents
	{
		public virtual Func<List<byte[]>, bool> OnDoneClicked { get; private set; }
		public virtual Action<List<byte[]>> OnDoneDismissed { get; private set; }
		public virtual Func<List<byte[]>, bool> OnPreviewClicked { get; private set; }
		public virtual Action<List<byte[]>> OnPreviewDismissed { get; private set; }
		public virtual Func<List<byte[]>, bool> OnStopClicked { get; private set; }

		/// <summary>
		/// Sets the on done clicked.
		/// </summary>
		/// <returns></returns>
		/// <param name="onDoneClicked">The event that will be triggered when user click on Done button. If result is true, the camera view will be dismissed</param>
		public MultiplePictureTakerServiceEvents SetOnDoneClicked(Func<List<byte[]>, bool> onDoneClicked)
		{
			OnDoneClicked += onDoneClicked;
			return this;
		}

		/// <summary>
		/// Sets the on done dismissed.
		/// </summary>
		/// <returns></returns>
		/// <param name="onDoneDismissed">The event that will be triggered when user click on Done button and the view has been dismissed</param>
		public MultiplePictureTakerServiceEvents SetOnDoneDismissed(Action<List<byte[]>> onDoneDismissed)
		{
			OnDoneDismissed += onDoneDismissed;
			return this;
		}

		/// <summary>
		/// Sets the on preview clicked.
		/// </summary>
		/// <returns></returns>
		/// <param name="onPreviewClicked">The event that will be triggered when use click on the preview button. If the result is true, the camera view will be dismissed</param>
		public MultiplePictureTakerServiceEvents SetOnPreviewClicked(Func<List<byte[]>, bool> onPreviewClicked)
		{
			OnPreviewClicked += onPreviewClicked;
			return this;
		}

		/// <summary>
		/// Sets the on preview clicked.
		/// </summary>
		/// <returns></returns>
		/// <param name="onPreviewDismissed">The event that will be triggered when user click on the preview button and the camera has been dismissed</param>
		public MultiplePictureTakerServiceEvents SetOnPreviewDismissed(Action<List<byte[]>> onPreviewDismissed)
		{
			OnPreviewDismissed += onPreviewDismissed;
			return this;
		}

		/// <summary>
		/// Set the event when user want to close the camera picker
		/// </summary>
		/// <returns></returns>
		/// <param name="onStopClicked">The event that will be triggered when user press on done button, if result is true, the camera view will be dismissed.</param>
		public MultiplePictureTakerServiceEvents SetOnStopClicked(Func<List<byte[]>, bool> onStopClicked)
		{
			OnStopClicked += onStopClicked;
			return this;
		}

	}
}
