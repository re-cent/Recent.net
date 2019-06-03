using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Acr.UserDialogs;
using Xamarin.Essentials;

namespace RecentApp.ViewModels
{
	public class ViewModelBase : BindableBase, INavigationAware, IDestructible
	{
		protected INavigationService NavigationService { get; private set; }

		private string _title;
		private bool _isBusy;
		public string Title
		{
			get => _title;
			set => SetProperty(ref _title, value);
		}

		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value, ShowOrHideLoading);
		}

		public ViewModelBase(INavigationService navigationService)
		{
			NavigationService = navigationService;
		}

		public virtual void OnNavigatedFrom(INavigationParameters parameters)
		{

		}

		public virtual void OnNavigatedTo(INavigationParameters parameters)
		{

		}

		public virtual void OnNavigatingTo(INavigationParameters parameters)
		{

		}

		public virtual void Destroy()
		{

		}

		public void ShowMessage(string msg)
		{
			if (MainThread.IsMainThread)
			{
				UserDialogs.Instance.Alert(message: msg);
			}
			else
			{
				MainThread.BeginInvokeOnMainThread(() =>
				{
					UserDialogs.Instance.Alert(message: msg);
				});
			}
		}

		public void ShowOrHideLoading()
		{
			if (MainThread.IsMainThread)
			{
				if (_isBusy)
					UserDialogs.Instance.ShowLoading();
				else
					UserDialogs.Instance.HideLoading();
			}
			else
			{
				MainThread.BeginInvokeOnMainThread(() =>
				{
					if (_isBusy)
						UserDialogs.Instance.ShowLoading();
					else
						UserDialogs.Instance.HideLoading();
				});
			}
		}
	}
}
