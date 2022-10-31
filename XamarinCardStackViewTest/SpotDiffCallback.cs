using System.Collections.Generic;
using AndroidX.RecyclerView.Widget;

namespace XamarinCardStackViewTest
{
	public class SpotDiffCallback : DiffUtil.Callback
	{
		private readonly List<TouristSpot> _old;
		private readonly List<TouristSpot> _new;

		public SpotDiffCallback(List<TouristSpot> old, List<TouristSpot> @new)
		{
			_old = old;
			_new = @new;
		}

		public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
		{
			return _old[oldItemPosition] == _new[newItemPosition];
		}

		public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
		{
			return _old[oldItemPosition].Id == _new[newItemPosition].Id;
		}

		public override int NewListSize => _new.Count;
		public override int OldListSize => _old.Count;
	}
}