using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinCardStackViewTest
{
	public class TouristSpot
	{
		public string Name { get; private set; }
		public string City { get; private set; }
		public string Url { get; private set; }

		public TouristSpot(string name, string city, string url)
		{
			Name = name;
			City = city;
			Url = url;
		}
	}
}