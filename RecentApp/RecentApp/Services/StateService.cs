using System;
using System.Collections.Generic;
using System.Text;

namespace RecentApp.Services
{
	public class StateService
	{
		private string _12words;

		public string Get12Words()
		{
			return _12words;
		}

		public void Set12Words(string val)
		{
			_12words = val;
		}
	}
}
