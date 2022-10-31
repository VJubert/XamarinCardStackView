namespace XamarinCardStackViewTest
{
	public record TouristSpot
	{
		private static int _id = 0;
		public int Id { get; }
		public string Name { get; }
		public string City { get; }
		public string Url { get; }

		public TouristSpot(string name, string city, string url)
		{
			Id = _id++;
			Name = name;
			City = city;
			Url = url;
		}
	}
}