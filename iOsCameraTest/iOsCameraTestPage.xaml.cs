using Xamarin.Forms;
using iOsCameraTest.Services;

namespace iOsCameraTest
{
	public partial class iOsCameraTestPage : ContentPage
	{
		void Handle_Clicked(object sender, System.EventArgs e)
		{
			var service = DependencyService.Get<IMultiplePhotoPickerService>();

			service.TakePhotos().SetOnDoneClicked(arg => {
				DisplayAlert("Message", arg.Count + " were taken", "Ok");
				return true;
			});
		}

		public iOsCameraTestPage()
		{
			InitializeComponent();
		}
	}
}
