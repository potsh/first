using UnityEngine;
using Verse;

namespace RimWorld
{
	public abstract class PawnColumnWorker_Icon : PawnColumnWorker
	{
		protected virtual int Width => 26;

		public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
		{
			Texture2D iconFor = GetIconFor(pawn);
			if (iconFor != null)
			{
				Vector2 iconSize = GetIconSize(pawn);
				int num = (int)((rect.width - iconSize.x) / 2f);
				int num2 = Mathf.Max((int)((30f - iconSize.y) / 2f), 0);
				Rect rect2 = new Rect(rect.x + (float)num, rect.y + (float)num2, iconSize.x, iconSize.y);
				GUI.color = GetIconColor(pawn);
				GUI.DrawTexture(rect2, iconFor);
				GUI.color = Color.white;
				if (Mouse.IsOver(rect2))
				{
					string iconTip = GetIconTip(pawn);
					if (!iconTip.NullOrEmpty())
					{
						TooltipHandler.TipRegion(rect2, iconTip);
					}
				}
				if (Widgets.ButtonInvisible(rect2))
				{
					ClickedIcon(pawn);
				}
				if (Mouse.IsOver(rect2) && Input.GetMouseButton(0))
				{
					PaintedIcon(pawn);
				}
			}
		}

		public override int GetMinWidth(PawnTable table)
		{
			return Mathf.Max(base.GetMinWidth(table), Width);
		}

		public override int GetMaxWidth(PawnTable table)
		{
			return Mathf.Min(base.GetMaxWidth(table), GetMinWidth(table));
		}

		public override int GetMinCellHeight(Pawn pawn)
		{
			int minCellHeight = base.GetMinCellHeight(pawn);
			Vector2 iconSize = GetIconSize(pawn);
			return Mathf.Max(minCellHeight, Mathf.CeilToInt(iconSize.y));
		}

		public override int Compare(Pawn a, Pawn b)
		{
			return GetValueToCompare(a).CompareTo(GetValueToCompare(b));
		}

		private int GetValueToCompare(Pawn pawn)
		{
			Texture2D iconFor = GetIconFor(pawn);
			return (!(iconFor != null)) ? (-2147483648) : iconFor.GetInstanceID();
		}

		protected abstract Texture2D GetIconFor(Pawn pawn);

		protected virtual string GetIconTip(Pawn pawn)
		{
			return null;
		}

		protected virtual Color GetIconColor(Pawn pawn)
		{
			return Color.white;
		}

		protected virtual void ClickedIcon(Pawn pawn)
		{
		}

		protected virtual void PaintedIcon(Pawn pawn)
		{
		}

		protected virtual Vector2 GetIconSize(Pawn pawn)
		{
			Texture2D iconFor = GetIconFor(pawn);
			if (iconFor == null)
			{
				return Vector2.zero;
			}
			return new Vector2((float)iconFor.width, (float)iconFor.height);
		}
	}
}
