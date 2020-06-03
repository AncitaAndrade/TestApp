using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceBasedApp;
using Xamarin.Essentials;
using Xamarin.Forms;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace TestApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private ISpeechToTextService _speechRecongnitionService;
        private bool isPermissionGranted;

        public MainPage()
        {
            InitializeComponent();
            try
            {
                _speechRecongnitionService = DependencyService.Get<ISpeechToTextService>();
                MyButton.ImageSource = ImageSource.FromResource("TestApp.Images.mic.png");
               
                CheckPermissionStatus();
            }
            catch (Exception ex)
            {
                recon.Text = ex.Message;
            }


            MessagingCenter.Subscribe<ISpeechToTextService, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultRecieved(args);
            });

            MessagingCenter.Subscribe<ISpeechToTextService>(this, "Final", (sender) =>
            {
                //used for iOS specific code. Enabling button is done here

            });
        }

        private async void CheckPermissionStatus()
        {
           var status=await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone);
            if (status == PermissionStatus.Granted)
            {
                isPermissionGranted = true;
                await SpeakInitialInstructionAsync();
            }
            else
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Microphone))
                {
                    await DisplayAlert("Need Mic", "Need Microphone Access", "OK");
                }

                var permissionStatus = await CrossPermissions.Current.RequestPermissionAsync<MicrophonePermission>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    isPermissionGranted = true;
                    await SpeakInitialInstructionAsync();
                }
            }
                

        }

        private async Task SpeakInitialInstructionAsync()
        {
            await TextToSpeech.SpeakAsync("To Speak Press and Hold the microphone Image. Release when done!");
        }

        private void SpeechToTextFinalResultRecieved(string args)
        {
            recon.Text = args;
        }

        private async void MyButton_Pressed(object sender, EventArgs e)
        {
            if (isPermissionGranted)
            {
                MyButton.ImageSource = ImageSource.FromResource("TestApp.Images.MicrophoneOnMute.png");
                _speechRecongnitionService.StartSpeechToText();

            }
        }

        private void MyButton_Released(object sender, EventArgs e)
        {
            MyButton.ImageSource = ImageSource.FromResource("TestApp.Images.mic.png");
            _speechRecongnitionService.StopSpeechToText();
        }
    }
}
