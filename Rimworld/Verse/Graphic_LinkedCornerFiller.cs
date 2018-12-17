using UnityEngine;

namespace Verse
{
	public class Graphic_LinkedCornerFiller : Graphic_Linked
	{
		private const float ShiftUp = 0.09f;

		private const float CoverSize = 0.5f;

		private static readonly float CoverSizeCornerCorner = new Vector2(0.5f, 0.5f).magnitude;

		private static readonly float DistCenterCorner = new Vector2(0.5f, 0.5f).magnitude;

		private static readonly float CoverOffsetDist = DistCenterCorner - CoverSizeCornerCorner * 0.5f;

		private static readonly Vector2[] CornerFillUVs = new Vector2[4]
		{
			new Vector2(0.5f, 0.6f),
			new Vector2(0.5f, 0.6f),
			new Vector2(0.5f, 0.6f),
			new Vector2(0.5f, 0.6f)
		};

		public override LinkDrawerType LinkerType => LinkDrawerType.CornerFiller;

		public Graphic_LinkedCornerFiller(Graphic subGraphic)
			: base(subGraphic)
		{
		}

		public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
		{
			Graphic_LinkedCornerFiller graphic_LinkedCornerFiller = new Graphic_LinkedCornerFiller(subGraphic.GetColoredVersion(newShader, newColor, newColorTwo));
			graphic_LinkedCornerFiller.data = data;
			return graphic_LinkedCornerFiller;
		}

		public override void Print(SectionLayer layer, Thing thing)
		{
			base.Print(layer, thing);
			IntVec3 position = thing.Position;
			for (int i = 0; i < 4; i++)
			{
				IntVec3 c = thing.Position + GenAdj.DiagonalDirectionsAround[i];
				if (ShouldLinkWith(c, thing) && (i != 0 || (ShouldLinkWith(position + IntVec3.West, thing) && ShouldLinkWith(position + IntVec3.South, thing))) && (i != 1 || (ShouldLinkWith(position + IntVec3.West, thing) && ShouldLinkWith(position + IntVec3.North, thing))) && (i != 2 || (ShouldLinkWith(position + IntVec3.East, thing) && ShouldLinkWith(position + IntVec3.North, thing))) && (i != 3 || (ShouldLinkWith(position + IntVec3.East, thing) && ShouldLinkWith(position + IntVec3.South, thing))))
				{
					Vector3 vector = thing.DrawPos + GenAdj.DiagonalDirectionsAround[i].ToVector3().normalized * CoverOffsetDist + Altitudes.AltIncVect + new Vector3(0f, 0f, 0.09f);
					Vector2 vector2 = new Vector2(0.5f, 0.5f);
					if (!c.InBounds(thing.Map))
					{
						if (c.x == -1)
						{
							vector.x -= 1f;
							vector2.x *= 5f;
						}
						if (c.z == -1)
						{
							vector.z -= 1f;
							vector2.y *= 5f;
						}
						int x = c.x;
						IntVec3 size = thing.Map.Size;
						if (x == size.x)
						{
							vector.x += 1f;
							vector2.x *= 5f;
						}
						int z = c.z;
						IntVec3 size2 = thing.Map.Size;
						if (z == size2.z)
						{
							vector.z += 1f;
							vector2.y *= 5f;
						}
					}
					Vector3 center = vector;
					Vector2 size3 = vector2;
					Material mat = LinkedDrawMatFrom(thing, thing.Position);
					float rot = 0f;
					Vector2[] cornerFillUVs = CornerFillUVs;
					Printer_Plane.PrintPlane(layer, center, size3, mat, rot, flipUv: false, cornerFillUVs);
				}
			}
		}
	}
}
