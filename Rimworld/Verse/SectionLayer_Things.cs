using System.Collections.Generic;

namespace Verse
{
	public abstract class SectionLayer_Things : SectionLayer
	{
		protected bool requireAddToMapMesh;

		public SectionLayer_Things(Section section)
			: base(section)
		{
		}

		public override void DrawLayer()
		{
			if (DebugViewSettings.drawThingsPrinted)
			{
				base.DrawLayer();
			}
		}

		public override void Regenerate()
		{
			ClearSubMeshes(MeshParts.All);
			foreach (IntVec3 item in section.CellRect)
			{
				IntVec3 current = item;
				List<Thing> list = base.Map.thingGrid.ThingsListAt(current);
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					Thing thing = list[i];
					if (thing.def.drawerType != 0 && (thing.def.drawerType != DrawerType.RealtimeOnly || !requireAddToMapMesh) && (!(thing.def.hideAtSnowDepth < 1f) || !(base.Map.snowGrid.GetDepth(thing.Position) > thing.def.hideAtSnowDepth)))
					{
						IntVec3 position = thing.Position;
						if (position.x == current.x)
						{
							IntVec3 position2 = thing.Position;
							if (position2.z == current.z)
							{
								TakePrintFrom(thing);
							}
						}
					}
				}
			}
			FinalizeMesh(MeshParts.All);
		}

		protected abstract void TakePrintFrom(Thing t);
	}
}
