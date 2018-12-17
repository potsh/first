using UnityEngine;
using Verse;

namespace RimWorld
{
	public class Graphic_LinkedTransmitterOverlay : Graphic_Linked
	{
		public Graphic_LinkedTransmitterOverlay()
		{
		}

		public Graphic_LinkedTransmitterOverlay(Graphic subGraphic)
			: base(subGraphic)
		{
		}

		public override bool ShouldLinkWith(IntVec3 c, Thing parent)
		{
			if (!c.InBounds(parent.Map))
			{
				return false;
			}
			if (parent.Map.powerNetGrid.TransmittedPowerNetAt(c) != null)
			{
				return true;
			}
			return false;
		}

		public override void Print(SectionLayer layer, Thing parent)
		{
			CellRect.CellRectIterator iterator = parent.OccupiedRect().GetIterator();
			while (!iterator.Done())
			{
				IntVec3 current = iterator.Current;
				Vector3 center = current.ToVector3ShiftedWithAltitude(AltitudeLayer.MapDataOverlay);
				Printer_Plane.PrintPlane(layer, center, new Vector2(1f, 1f), LinkedDrawMatFrom(parent, current));
				iterator.MoveNext();
			}
		}
	}
}
