using Prism;
using Prism.Ioc;
using RecentApp.Services;
using RecentApp.ViewModels;
using RecentApp.Views;
using RecentLib;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace RecentApp
{
	public partial class App
	{
		/* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
		public App() : this(null) { }

		public App(IPlatformInitializer initializer) : base(initializer) { }

		protected override async void OnInitialized()
		{
			InitializeComponent();

			await NavigationService.NavigateAsync("Master/Navigation/FirstStep");
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterSingleton<RecentCore>();
			containerRegistry.RegisterSingleton<StateService>();

			containerRegistry.RegisterForNavigation<NavigationPage>("Navigation");
			containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>("Main");
			containerRegistry.RegisterForNavigation<CreateFirstStepPage, CreateFirstStepPageViewModel>("FirstStep");
			containerRegistry.RegisterForNavigation<RecentMasterDetailPage, RecentMasterDetailPageViewModel>("Master");
			containerRegistry.RegisterForNavigation<CreateSecondStepPage, CreateSecondStepPageViewModel>("SecondStep");
		}
	}
}
