using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Web.Http;
using Windows.UI.Popups;
using WeatherApp.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace WeatherApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private String responseText = null;
        private HttpClient httpClient;
        private HttpResponseMessage response;

        private String cityName = null;
        private String cityId = null;
        private Boolean isTapped = false;

        private Dictionary <String, Double> cityMap;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            httpClient = new HttpClient();
            cityMap = new Dictionary<String, Double>();

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            // Just to give 2 cities' ID here as a demo,
            // to get more city ID, find most of them here: http://openweathermap.org/help/city_list.txt

            cityMap.Add("Helsinki", 658225);
            cityMap.Add("Shanghai", 1796236);

            // Default Text Content
            CityIdTextBox.Text = "City Name";

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            // Rui:
            // Use Windows.Storage.ApplicationData.Current.LocalSettings
            // to load value of CityIdTextBox when naviagte to main page,
            // If localSettings doesn't contain "value", the text is set to default: "City Name" 

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey("value"))
            {

                CityIdTextBox.Text = localSettings.Values["value"].ToString();
            }

            else
            {
                CityIdTextBox.Text = "City Name";
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            
        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {

            // Rui: 
            // Handle the Virtual Hardware Button: Back,
            // When user taps it and leaves the main page, app removes the saved localSettings "value" for CityIdTextBox
            // So that when user starts app again, the CityIdTextBox shows default: "City Name". 

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Frame frame = Window.Current.Content as Frame;
            if (!frame.CanGoBack)
            {
                localSettings.Values.Remove("value");
                return;
            }
        }

        private async void Btn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (BtnToWeatherInfoPage.Name.Equals(btn.Name)) {

                response = new HttpResponseMessage();

                Uri resourceUri;

                cityName = CityIdTextBox.Text;
                // Rui:
                // Some notifications handling, for instance not giving the city name, etc

                if (cityName == "City ID" && isTapped == false) {

                    var messageDialog = new MessageDialog("Sorry, no city name? Come on... give it to me...");    
                    await messageDialog.ShowAsync();

                }

                else if (cityName == "" && isTapped == true) {

                    var messageDialog = new MessageDialog("You have no city to search yet, please type in city name...");
                    await messageDialog.ShowAsync();

                }

                else if (cityMap.ContainsKey(cityName)) {

                    cityId = cityMap[cityName].ToString();
                    // Rui:
                    // Retrieve 7 days weather information
                    // and get the pass the weather data to next WeatherInfoPage

                    String Address = "http://api.openweathermap.org/data/2.5/forecast/daily?id=" + cityId;

                    Uri.TryCreate(Address.Trim(), UriKind.Absolute, out resourceUri);

                    try
                    {
                        response = await httpClient.GetAsync(resourceUri);

                        response.EnsureSuccessStatusCode();

                        responseText = await response.Content.ReadAsStringAsync();


                        System.Diagnostics.Debug.WriteLine("GetWeatherInfo: " + responseText);

                        // Rui:
                        // Navigate to WeatherInfoPage
                        if (responseText != null)
                            Frame.Navigate(typeof(Pages.WeatherInfoPage), responseText);

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Http request failed, " + ex.Message.ToString());

                    }
                }

                // Rui:
                //No such city supported, all cities are defined in cityMap
                else {

                    var messageDialog = new MessageDialog("Sorry, your city is not yet supported in app...");
                    await messageDialog.ShowAsync();
                }

            }
        }

        private void CityIdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {

            isTapped = true;

            if (CityIdTextBox != null)
            {
                CityIdTextBox.Text = "";
            }
        }

        private void CityIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Rui:
            // Create one instance of LocalSettings, and  
            // LocalSettings is one dictionary which can be used to save the text in assigned storage area, 
            // whenever the text is changed.

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            localSettings.Values["value"] = CityIdTextBox.Text;
            
        }

    }
}
