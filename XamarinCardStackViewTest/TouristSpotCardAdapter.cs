using System;
using System.Collections.Generic;
using Android.App;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using JetBrains.Annotations;

namespace XamarinCardStackViewTest
{
	public class SpotViewHolder : RecyclerView.ViewHolder
	{
		public TextView Name;
		public TextView City;
		public ImageView Image;

		public SpotViewHolder([NotNull] View itemView) : base(itemView)
		{
			Name = itemView.FindViewById<TextView>(Resource.Id.item_name);
			City = itemView.FindViewById<TextView>(Resource.Id.item_city);
			Image = itemView.FindViewById<ImageView>(Resource.Id.item_image);
		}

		protected SpotViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
	}

	public class CardStackAdapter : RecyclerView.Adapter
	{
		public List<TouristSpot> Spots { get; set; }

		public CardStackAdapter() { }
		protected CardStackAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			TouristSpot spot = Spots[position];
			var cell = (SpotViewHolder)holder;
			cell.Name.Text = $"{spot.Id}. ${spot.Name}";
			cell.City.Text = spot.City;
			Glide.With(cell.Image)
				.Load(spot.Url)
				.Into(cell.Image);
			cell.ItemView.Click += (_, _) =>
			{
				Toast.MakeText(Application.Context, spot.Name, ToastLength.Short)?.Show();
			};
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			LayoutInflater inflater = LayoutInflater.From(parent.Context);
			return new SpotViewHolder(inflater!.Inflate(Resource.Layout.item_spot, parent, false)!);
		}

		public override int ItemCount => Spots?.Count ?? 0;
	}
}