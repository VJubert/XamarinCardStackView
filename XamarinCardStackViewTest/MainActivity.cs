using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.DrawerLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Com.Yuyakaido.Android.CardStackView;
using Google.Android.Material.Navigation;
using Direction = Com.Yuyakaido.Android.CardStackView.Direction;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace XamarinCardStackViewTest
{
	[Activity(Label = "XamarinCardStackViewTest", MainLauncher = true)]
	public class MainActivity : AppCompatActivity, ICardStackListener
	{
		private DrawerLayout _drawerLayout;
		private CardStackView _cardStackView;
		private CardStackLayoutManager _manager;
		private CardStackAdapter _adapter;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			_drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			_cardStackView = FindViewById<CardStackView>(Resource.Id.card_stack_view);
			SetupNavigation();
			SetupCardStackView();
			SetupButton();
		}

		public override void OnBackPressed()
		{
			if (_drawerLayout.IsDrawerOpen((int)GravityFlags.Start))
			{
				_drawerLayout.CloseDrawers();
			}
			else
			{
				base.OnBackPressed();
			}
		}

		public void OnCardAppeared(View view, int position)
		{
			TextView textView = view.FindViewById<TextView>(Resource.Id.item_name);
			Log.Debug("CardStackView", $"onCardAppeared: ({position}) {textView.Text}");
		}

		public void OnCardCanceled()
		{
			Log.Debug("CardStackView", $"onCardCanceled: {_manager.TopPosition}");
		}

		public void OnCardDisappeared(View view, int position)
		{
			TextView textView = view.FindViewById<TextView>(Resource.Id.item_name);
			Log.Debug("CardStackView", $"onCardDisappeared: ({position}) ${textView.Text}");
		}

		public void OnCardDragging(Direction direction, float ratio)
		{
			Log.Debug("CardStackView", $"onCardDragging: d = {direction.Name()}, r = {ratio}");
		}

		public void OnCardRewound()
		{
			Log.Debug("CardStackView", $"onCardRewound: {_manager.TopPosition}");
		}

		public void OnCardSwiped(Direction direction)
		{
			Log.Debug("CardStackView", $"onCardSwiped: p = {_manager.TopPosition}, d = {direction}");
			if (_manager.TopPosition == _adapter.ItemCount - 5)
			{
				Paginate();
			}
		}

		private void SetupNavigation()
		{
			// Toolbar
			Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);

			// DrawerLayout
			ActionBarDrawerToggle actionBarDrawerToggle = new ActionBarDrawerToggle(this, _drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
			actionBarDrawerToggle.SyncState();
			_drawerLayout.AddDrawerListener(actionBarDrawerToggle);

			// NavigationView
			NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.navigation_view);
			navigationView.NavigationItemSelected += (_, args) =>
			{
				switch (args.MenuItem.ItemId)
				{
					case Resource.Id.reload:
						Reload();
						break;
					case Resource.Id.add_spot_to_first:
						AddFirst(1);
						break;
					case Resource.Id.add_spot_to_last:
						AddLast(1);
						break;
					case Resource.Id.remove_spot_from_first:
						RemoveFirst(1);
						break;
					case Resource.Id.remove_spot_from_last:
						RemoveLast(1);
						break;
					case Resource.Id.replace_first_spot:
						Replace();
						break;
					case Resource.Id.swap_first_for_last:
						Swap();
						break;
				}

				_drawerLayout.CloseDrawers();
				args.Handled = true;
			};
		}

		private void SetupCardStackView()
		{
			Initialize();
		}

		private void SetupButton()
		{
			View skip = FindViewById<View>(Resource.Id.skip_button);
			skip!.Click += (_, _) =>
			{
				SwipeAnimationSetting setting = new SwipeAnimationSetting.Builder()
					.SetDirection(Direction.Left)
					.SetDuration(DurationEnum.Normal.Duration)
					.SetInterpolator(new AccelerateInterpolator())
					.Build();
				_manager.SetSwipeAnimationSetting(setting);
				_cardStackView.Swipe();
			};

			View rewind = FindViewById<View>(Resource.Id.rewind_button);
			rewind!.Click += (_, _) =>
			{
				RewindAnimationSetting setting = new RewindAnimationSetting.Builder()
					.SetDirection(Direction.Bottom)
					.SetDuration(DurationEnum.Normal.Duration)
					.SetInterpolator(new DecelerateInterpolator())
					.Build();
				_manager.SetRewindAnimationSetting(setting);
				_cardStackView.Rewind();
			};

			View like = FindViewById<View>(Resource.Id.like_button);
			like.Click += (_, _) =>
			{
				SwipeAnimationSetting setting = new SwipeAnimationSetting.Builder()
					.SetDirection(Direction.Right)
					.SetDuration(DurationEnum.Normal.Duration)
					.SetInterpolator(new AccelerateInterpolator())
					.Build();
				_manager.SetSwipeAnimationSetting(setting);
				_cardStackView.Swipe();
			};
		}

		private void Initialize()
		{
			_manager = new CardStackLayoutManager(this, this);
			_manager.SetStackFrom(StackFrom.None);
			_manager.SetVisibleCount(3);
			_manager.SetTranslationInterval(8.0f);
			_manager.SetScaleInterval(0.95f);
			_manager.SetSwipeThreshold(0.3f);
			_manager.SetMaxDegree(20.0f);
			_manager.SetDirections(new List<Direction>
			{
				Direction.Left, Direction.Right
			});
			_manager.SetCanScrollHorizontal(true);
			_manager.SetCanScrollVertical(true);
			_manager.SetSwipeableMethod(SwipeableMethod.AutomaticAndManual);
			_manager.SetOverlayInterpolator(new LinearInterpolator());
			_cardStackView.SetLayoutManager(_manager);
			_adapter = new CardStackAdapter();
			_adapter.Spots = CreateSpots();
			_cardStackView.SetAdapter(_adapter);
			RecyclerView.ItemAnimator itemAnimator = _cardStackView.GetItemAnimator();
			if (itemAnimator is DefaultItemAnimator defaultItemAnimator)
			{
				defaultItemAnimator.SupportsChangeAnimations = false;
			}
		}

		private void Paginate()
		{
			List<TouristSpot> old = _adapter.Spots;
			List<TouristSpot> @new = old.Concat(CreateSpots()).ToList();
			SpotDiffCallback callback = new(old, @new);
			DiffUtil.DiffResult result = DiffUtil.CalculateDiff(callback);
			_adapter.Spots = @new;
			result.DispatchUpdatesTo(_adapter);
		}

		private void Reload()
		{
			List<TouristSpot> old = _adapter.Spots;
			List<TouristSpot> @new = CreateSpots();
			SpotDiffCallback callback = new(old, @new);
			DiffUtil.DiffResult result = DiffUtil.CalculateDiff(callback);
			_adapter.Spots = @new;
			result.DispatchUpdatesTo(_adapter);
		}

		private void AddFirst(int size)
		{
			List<TouristSpot> old = _adapter.Spots;
			List<TouristSpot> @new = old.ToList();
			@new.InsertRange(0, Enumerable.Range(0, size).Select(_ => CreateSpot()));
			SpotDiffCallback callback = new(old, @new);
			DiffUtil.DiffResult result = DiffUtil.CalculateDiff(callback);
			_adapter.Spots = @new;
			result.DispatchUpdatesTo(_adapter);
		}

		private void AddLast(int size)
		{
			List<TouristSpot> old = _adapter.Spots;
			List<TouristSpot> @new = old.ToList().Concat(Enumerable.Range(0, size).Select(_ => CreateSpot())).ToList();
			SpotDiffCallback callback = new(old, @new);
			DiffUtil.DiffResult result = DiffUtil.CalculateDiff(callback);
			_adapter.Spots = @new;
			result.DispatchUpdatesTo(_adapter);
		}

		private void RemoveFirst(int size)
		{
			if (_adapter.Spots == null || _adapter.Spots.Count == 0)
			{
				return;
			}

			List<TouristSpot> old = _adapter.Spots;
			List<TouristSpot> @new = _adapter.Spots.ToList();
			@new.RemoveRange(0, size);
			SpotDiffCallback callback = new(old, @new);
			DiffUtil.DiffResult result = DiffUtil.CalculateDiff(callback);
			_adapter.Spots = @new;
			result.DispatchUpdatesTo(_adapter);
		}

		private void RemoveLast(int size)
		{
			if (_adapter.Spots == null || _adapter.Spots.Count == 0)
			{
				return;
			}

			List<TouristSpot> old = _adapter.Spots;
			List<TouristSpot> @new = _adapter.Spots.ToList();
			_adapter.Spots.RemoveRange(@new.Count - 1 - size, size);
			SpotDiffCallback callback = new(old, @new);
			DiffUtil.DiffResult result = DiffUtil.CalculateDiff(callback);
			_adapter.Spots = @new;
			result.DispatchUpdatesTo(_adapter);
		}

		private void Replace()
		{
			List<TouristSpot> @new = _adapter.Spots.ToList();
			@new.RemoveAt(_manager.TopPosition);
			@new.Insert(_manager.TopPosition, CreateSpot());
			_adapter.Spots = @new;
			_adapter.NotifyItemChanged(_manager.TopPosition);
		}

		private void Swap()
		{
			List<TouristSpot> old = _adapter.Spots;
			List<TouristSpot> @new = _adapter.Spots.ToList();
			TouristSpot first = @new[_manager.TopPosition];
			TouristSpot last = @new[^1];
			@new.RemoveAt(_manager.TopPosition);
			@new.RemoveAt(@new.Count - 1);
			@new.Insert(_manager.TopPosition, last);
			@new.Add(first);
			SpotDiffCallback callback = new(old, @new);
			DiffUtil.DiffResult result = DiffUtil.CalculateDiff(callback);
			_adapter.Spots = @new;
			result.DispatchUpdatesTo(_adapter);
		}

		private static TouristSpot CreateSpot() => new("Yasaka Shrine", "Kyoto", "https://source.unsplash.com/Xq1ntWruZQI/600x800");

		private static List<TouristSpot> CreateSpots() => new(10)
		{
			new("Yasaka Shrine", "Kyoto", "https://source.unsplash.com/Xq1ntWruZQI/600x800"),
			new("Fushimi Inari Shrine", "Kyoto", "https://source.unsplash.com/NYyCqdBOKwc/600x800"),
			new("Bamboo Forest", "Kyoto", "https://source.unsplash.com/buF62ewDLcQ/600x800"),
			new("Brooklyn Bridge", "New York", "https://source.unsplash.com/THozNzxEP3g/600x800"),
			new("Empire State Building", "New York", "https://source.unsplash.com/USrZRcRS2Lw/600x800"),
			new("The statue of Liberty", "New York", "https://source.unsplash.com/PeFk7fzxTdk/600x800"),
			new("Louvre Museum", "Paris", "https://source.unsplash.com/LrMWHKqilUw/600x800"),
			new("Eiffel Tower", "Paris", "https://source.unsplash.com/HN-5Z6AmxrM/600x800"),
			new("Big Ben", "London", "https://source.unsplash.com/CdVAUADdqEc/600x800"),
			new("Great Wall of China", "China", "https://source.unsplash.com/AWh9C-QjhE4/600x800"),
		};
	}
}