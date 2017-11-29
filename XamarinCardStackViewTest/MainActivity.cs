using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Com.Yuyakaido.Android.CardStackView;
using System.Collections.Generic;
using Android.Animation;
using Android.Util;

namespace XamarinCardStackViewTest
{
	[Activity(Label = "XamarinCardStackViewTest", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		private ProgressBar _progressBar;
		private CardStackView _cardStackView;
		private TouristSpotCardAdapter _adapter;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);
			Setup();
			Reload();
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.activity_main, menu);
			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.menu_activity_main_reload:
					Reload();
					break;
				case Resource.Id.menu_activity_main_add_first:
					AddFirst();
					break;
				case Resource.Id.menu_activity_main_add_last:
					AddLast();
					break;
				case Resource.Id.menu_activity_main_remove_first:
					RemoveFirst();
					break;
				case Resource.Id.menu_activity_main_remove_last:
					RemoveLast();
					break;
				case Resource.Id.menu_activity_main_swipe_left:
					SwipeLeft();
					break;
				case Resource.Id.menu_activity_main_swipe_right:
					SwipeRight();
					break;
				case Resource.Id.menu_activity_main_reverse:
					Reverse();
					break;
			}
			return base.OnOptionsItemSelected(item);
		}

		private TouristSpot CreateTouristSpot()
		{
			return new TouristSpot("Yasaka Shrine", "Kyoto", "https://source.unsplash.com/Xq1ntWruZQI/600x800");
		}

		private List<TouristSpot> CreateTouristSpots()
		{
			var spots = new List<TouristSpot>
			{
				new TouristSpot("Yasaka Shrine", "Kyoto", "https://source.unsplash.com/Xq1ntWruZQI/600x800"),
				new TouristSpot("Fushimi Inari Shrine", "Kyoto", "https://source.unsplash.com/NYyCqdBOKwc/600x800"),
				new TouristSpot("Bamboo Forest", "Kyoto", "https://source.unsplash.com/buF62ewDLcQ/600x800"),
				new TouristSpot("Brooklyn Bridge", "New York", "https://source.unsplash.com/THozNzxEP3g/600x800"),
				new TouristSpot("Empire State Building", "New York", "https://source.unsplash.com/USrZRcRS2Lw/600x800"),
				new TouristSpot("The statue of Liberty", "New York", "https://source.unsplash.com/PeFk7fzxTdk/600x800"),
				new TouristSpot("Louvre Museum", "Paris", "https://source.unsplash.com/LrMWHKqilUw/600x800"),
				new TouristSpot("Eiffel Tower", "Paris", "https://source.unsplash.com/HN-5Z6AmxrM/600x800"),
				new TouristSpot("Big Ben", "London", "https://source.unsplash.com/CdVAUADdqEc/600x800"),
				new TouristSpot("Great Wall of China", "China", "https://source.unsplash.com/AWh9C-QjhE4/600x800")
			};
			return spots;
		}

		private TouristSpotCardAdapter CreateTouristSpotCardAdapter()
		{
			var adapter = new TouristSpotCardAdapter(this);
			adapter.AddAll(CreateTouristSpots());
			return adapter;
		}

		private void Setup()
		{
			_progressBar = FindViewById<ProgressBar>(Resource.Id.activity_main_progress_bar);

			_cardStackView = FindViewById<CardStackView>(Resource.Id.CardStackView);
			_cardStackView.CardReversed += (sender, e) => Log.Debug("Debug", "OnCardReversed");
			_cardStackView.CardDragging += (sender, e) => Log.Debug("Debug", "OnCardDragging");
			_cardStackView.CardMovedToOrigin += (sender, e) => Log.Debug("Debug", "OnCardMovedToOrigin");
			_cardStackView.CardClicked += (sender, e) => Log.Debug("CardStackView", $"onCardClicked: {e.Index}");
			_cardStackView.CardSwiped += (sender, e) =>
			{
				Log.Debug("CardStackView", $"onCardSwiped: {e.Direction.ToString()}");
				Log.Debug("CardStackView", $"topIndex: {_cardStackView.TopIndex}");
				if (_cardStackView.TopIndex == _adapter.Count - 5)
				{
					Log.Debug("CardStackView", "Paginate: " + _cardStackView.TopIndex);
					Paginate();
				}
			};
		}

		private void Reload()
		{
			_cardStackView.Visibility = ViewStates.Gone;
			_progressBar.Visibility = ViewStates.Visible;
			new Handler().PostDelayed(() =>
			{
				_adapter = CreateTouristSpotCardAdapter();
				_cardStackView.SetAdapter(_adapter);
				_cardStackView.Visibility = ViewStates.Visible;
				_progressBar.Visibility = ViewStates.Gone;
			}, 1000);
		}

		private List<TouristSpot> ExtractRemainingTouristSpots()
		{
			var spots = new List<TouristSpot>();
			for (var i = _cardStackView.TopIndex; i < _adapter.Count; i++)
			{
				spots.Add(_adapter.GetItem(i));
			}
			return spots;
		}

		private void AddFirst()
		{
			var spots = ExtractRemainingTouristSpots();
			spots.Insert(0, CreateTouristSpot());
			_adapter.Clear();
			_adapter.AddAll(spots);
			_adapter.NotifyDataSetChanged();
		}

		private void AddLast()
		{
			var spots = ExtractRemainingTouristSpots();
			spots.Insert(spots.Count - 1, CreateTouristSpot());
			_adapter.Clear();
			_adapter.AddAll(spots);
			_adapter.NotifyDataSetChanged();
		}

		private void RemoveFirst()
		{
			var spots = ExtractRemainingTouristSpots();
			if (spots.Count == 0)
			{
				return;
			}
			spots.RemoveAt(0);
			_adapter.Clear();
			_adapter.AddAll(spots);
			_adapter.NotifyDataSetChanged();
		}

		private void RemoveLast()
		{
			var spots = ExtractRemainingTouristSpots();
			if (spots.Count == 0)
			{
				return;
			}
			spots.RemoveAt(spots.Count - 1);
			_adapter.Clear();
			_adapter.AddAll(spots);
			_adapter.NotifyDataSetChanged();
		}

		private void Paginate()
		{
			_cardStackView.SetPaginationReserved();
			_adapter.AddAll(CreateTouristSpots());
			_adapter.NotifyDataSetChanged();
		}

		public void SwipeLeft()
		{
			var spots = ExtractRemainingTouristSpots();
			if (spots.Count == 0)
			{
				return;
			}

			View target = _cardStackView.TopView;

			ValueAnimator rotation = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("rotation", -10f));
			rotation.SetDuration(200);
			ValueAnimator translateX = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("translationX", 0f, -2000f));
			ValueAnimator translateY = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("translationY", 0f, 500f));
			translateX.StartDelay = 100;
			translateY.StartDelay = 100;
			translateX.SetDuration(500);
			translateY.SetDuration(500);
			var set = new AnimatorSet();
			set.PlayTogether(rotation, translateX, translateY);

			_cardStackView.Swipe(SwipeDirection.Left, set);
		}

		public void SwipeRight()
		{
			var spots = ExtractRemainingTouristSpots();
			if (spots.Count == 0)
			{
				return;
			}

			View target = _cardStackView.TopView;

			ValueAnimator rotation = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("rotation", 10f));
			rotation.SetDuration(200);
			ValueAnimator translateX = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("translationX", 0f, 2000f));
			ValueAnimator translateY = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("translationY", 0f, 500f));
			translateX.StartDelay = 100;
			translateY.StartDelay = 100;
			translateX.SetDuration(500);
			translateY.SetDuration(500);
			var set = new AnimatorSet();
			set.PlayTogether(rotation, translateX, translateY);

			_cardStackView.Swipe(SwipeDirection.Right, set);
		}

		private void Reverse()
		{
			_cardStackView.Reverse();
		}
	}
}

