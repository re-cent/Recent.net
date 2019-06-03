using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using RecentApp.Services;
using RecentLib;
using Xamarin.Forms;

namespace RecentApp.ViewModels
{
	public class CreateSecondStepPageViewModel : ViewModelBase
	{
		private StateService _stateService;
		private RecentCore _recentCore;
		private string _phrase;
		private string _inputPhrase;
		private string[] selected = new string[12];
		private string[] wordsArray;

		public DelegateCommand<object> AddWordCommand { get; }
		public DelegateCommand CancelCommand { get; }
		public DelegateCommand CheckCommand { get; }
		public CreateSecondStepPageViewModel(INavigationService navigationService,
			StateService stateService, RecentCore recentCore)
		:base(navigationService)
		{
			_stateService = stateService;
			Title = "Second Step";
			AddWordCommand = new DelegateCommand<object>(addWord);
			CancelCommand = new DelegateCommand(Clear);
			CheckCommand = new DelegateCommand(CheckMatch);
			InputPhrase = "";
			Init();
		}

		private void CheckMatch()
		{
			ShowMessage(InputPhrase == Phrase ? "Phrases match!" : "Phrases don't match!");
		}

		private void Clear()
		{
			InputPhrase = "";
		}

		private void addWord(object obj)
		{
			if (!(obj is string word) ||
			    InputPhrase.Split(' ').Length > 12) return;
			InputPhrase = string.IsNullOrWhiteSpace(InputPhrase) ?
				word : $"{InputPhrase} {word}";
			InputPhrase = InputPhrase.Trim();
		}

		private void Init()
		{
			if (string.IsNullOrEmpty(_stateService.Get12Words()))
			{
				NavigationService.GoBackAsync();
			}
			Phrase = _stateService.Get12Words();
			WordsArr = Phrase.Split(' ')
				.OrderBy(c => Guid.NewGuid()).ToArray();
		}

		public string[] WordsArr
		{
			get => wordsArray;
			set => SetProperty(ref wordsArray, value);
		}

		public string Phrase
		{
			get => _phrase;
			set => SetProperty(ref _phrase, value);
		}

		public string InputPhrase
		{
			get => _inputPhrase;
			set => SetProperty(ref _inputPhrase, value);
		}
	}
}
