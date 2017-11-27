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
		private ProgressBar progressBar;
		private CardStackView cardStackView;
		private TouristSpotCardAdapter adapter;

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
			List<TouristSpot> spots = new List<TouristSpot>
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
			TouristSpotCardAdapter adapter = new TouristSpotCardAdapter(this);
			adapter.AddAll(CreateTouristSpots());
			return adapter;
		}

		private void Setup()
		{
			progressBar = FindViewById<ProgressBar>(Resource.Id.activity_main_progress_bar);

			cardStackView = FindViewById<CardStackView>(Resource.Id.CardStackView);
			cardStackView.CardReversed += (sender, e) => Log.Debug("Debug", "OnCardReversed");
			cardStackView.CardDragging += (sender, e) => Log.Debug("Debug", "OnCardDragging");
			cardStackView.CardMovedToOrigin += (sender, e) => Log.Debug("Debug", "OnCardMovedToOrigin");
			cardStackView.CardClicked += (sender, e) => Log.Debug("CardStackView", $"onCardClicked: {e.Index}");
			cardStackView.CardSwiped += (sender, e) =>
			{
				Log.Debug("CardStackView", $"onCardSwiped: {e.Direction.ToString()}");
				Log.Debug("CardStackView", $"topIndex: {cardStackView.TopIndex}");
				if (cardStackView.TopIndex == adapter.Count - 5)
				{
					Log.Debug("CardStackView", "Paginate: " + cardStackView.TopIndex);
					Paginate();
				}
			};
		}

		private void Reload()
		{
			cardStackView.Visibility = ViewStates.Gone;
			progressBar.Visibility = ViewStates.Visible;
			new Handler().PostDelayed(() =>
			{
				adapter = CreateTouristSpotCardAdapter();
				cardStackView.SetAdapter(adapter);
				cardStackView.Visibility = ViewStates.Visible;
				progressBar.Visibility = ViewStates.Gone;
			}, 1000);
		}

		private List<TouristSpot> ExtractRemainingTouristSpots()
		{
			List<TouristSpot> spots = new List<TouristSpot>();
			for (int i = cardStackView.TopIndex; i < adapter.Count; i++)
			{
				spots.Add(adapter.GetItem(i));
			}
			return spots;
		}

		private void AddFirst()
		{
			List<TouristSpot> spots = ExtractRemainingTouristSpots();
			spots.Insert(0, CreateTouristSpot());
			adapter.Clear();
			adapter.AddAll(spots);
			adapter.NotifyDataSetChanged();
		}

		private void AddLast()
		{
			List<TouristSpot> spots = ExtractRemainingTouristSpots();
			spots.Insert(spots.Count - 1, CreateTouristSpot());
			adapter.Clear();
			adapter.AddAll(spots);
			adapter.NotifyDataSetChanged();
		}

		private void RemoveFirst()
		{
			List<TouristSpot> spots = ExtractRemainingTouristSpots();
			if (spots.Count == 0)
			{
				return;
			}
			spots.RemoveAt(0);
			adapter.Clear();
			adapter.AddAll(spots);
			adapter.NotifyDataSetChanged();
		}

		private void RemoveLast()
		{
			List<TouristSpot> spots = ExtractRemainingTouristSpots();
			if (spots.Count == 0)
			{
				return;
			}
			spots.RemoveAt(spots.Count - 1);
			adapter.Clear();
			adapter.AddAll(spots);
			adapter.NotifyDataSetChanged();
		}

		private void Paginate()
		{
			cardStackView.SetPaginationReserved();
			adapter.AddAll(CreateTouristSpots());
			adapter.NotifyDataSetChanged();
		}

		public void SwipeLeft()
		{
			List<TouristSpot> spots = ExtractRemainingTouristSpots();
			if (spots.Count == 0)
			{
				return;
			}

			View target = cardStackView.TopView;

			ValueAnimator rotation = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("rotation", -10f));
			rotation.SetDuration(200);
			ValueAnimator translateX = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("translationX", 0f, -2000f));
			ValueAnimator translateY = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("translationY", 0f, 500f));
			translateX.StartDelay = 100;
			translateY.StartDelay = 100;
			translateX.SetDuration(500);
			translateY.SetDuration(500);
			AnimatorSet set = new AnimatorSet();
			set.PlayTogether(rotation, translateX, translateY);

			cardStackView.Swipe(SwipeDirection.Left, set);
		}

		public void SwipeRight()
		{
			List<TouristSpot> spots = ExtractRemainingTouristSpots();
			if (spots.Count == 0)
			{
				return;
			}

			View target = cardStackView.TopView;

			ValueAnimator rotation = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("rotation", 10f));
			rotation.SetDuration(200);
			ValueAnimator translateX = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("translationX", 0f, 2000f));
			ValueAnimator translateY = ObjectAnimator.OfPropertyValuesHolder(target, PropertyValuesHolder.OfFloat("translationY", 0f, 500f));
			translateX.StartDelay = 100;
			translateY.StartDelay = 100;
			translateX.SetDuration(500);
			translateY.SetDuration(500);
			AnimatorSet set = new AnimatorSet();
			set.PlayTogether(rotation, translateX, translateY);

			cardStackView.Swipe(SwipeDirection.Right, set);
		}

		private void Reverse()
		{
			cardStackView.Reverse();
		}
	}
}

