using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using RecentApp.Services;
using RecentLib;

namespace RecentApp.ViewModels
{
	public class CreateFirstStepPageViewModel : ViewModelBase
	{
		private string _phrase;
		private bool _isOk;
		private readonly RecentCore _recentCore;
		private readonly StateService _stateService;
		public DelegateCommand NextStepCommand { get; }
		public CreateFirstStepPageViewModel(INavigationService navigationService,
			RecentCore recentCore, StateService stateService)
		:base(navigationService)
		{
			_isOk = false;
			_recentCore = recentCore;
			_stateService = stateService;
			Title = "First Step";
			NextStepCommand = new DelegateCommand(GoToNext);
			Init();
		}

		public void Init()
		{
			if (string.IsNullOrEmpty(_stateService.Get12Words()))
			{
				_stateService.Set12Words(_recentCore.createAndRetrieveSeedPhrase());
			}
			Phrase = _stateService.Get12Words();
		}

		public string Phrase
		{
			get => _phrase;
			set => SetProperty(ref _phrase, value);
		}

		public bool IsOk
		{
			get => _isOk;
			set => SetProperty(ref _isOk, value);
		}

		private async void GoToNext()
		{
			await NavigationService.NavigateAsync("SecondStep");
		}

		public void PhraseChanged()
		{
			var words = Phrase.Split(' ');
			if (words.Length == 12 && !words.Any(c=> c.Length < 4 ))
			{
				IsOk = true;
			}
			else
			{
				IsOk = false;
			}
		}
	}
}
