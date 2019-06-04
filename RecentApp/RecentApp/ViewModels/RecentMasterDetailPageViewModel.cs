using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;

namespace RecentApp.ViewModels
{
	public class RecentMasterDetailPageViewModel : ViewModelBase
	{
		public RecentMasterDetailPageViewModel(INavigationService navigationService)
		:base(navigationService)
		{

		}
	}
}
