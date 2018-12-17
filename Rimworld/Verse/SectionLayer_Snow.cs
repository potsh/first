using UnityEngine;

namespace Verse
{
	internal class SectionLayer_Snow : SectionLayer
	{
		private float[] vertDepth = new float[9];

		private static readonly Color32 ColorClear = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

		private static readonly Color32 ColorWhite = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		public override bool Visible => DebugViewSettings.drawSnow;

		public SectionLayer_Snow(Section section)
			: base(section)
		{
			relevantChangeTypes = MapMeshFlag.Snow;
		}

		private bool Filled(int index)
		{
			Building building = base.Map.edificeGrid[index];
			return building != null && building.def.Fillage == FillCategory.Full;
		}

		public override void Regenerate()
		{
			LayerSubMesh subMesh = GetSubMesh(MatBases.Snow);
			if (subMesh.mesh.vertexCount == 0)
			{
				SectionLayerGeometryMaker_Solid.MakeBaseGeometry(section, subMesh, AltitudeLayer.Terrain);
			}
			subMesh.Clear(MeshParts.Colors);
			float[] depthGridDirect_Unsafe = base.Map.snowGrid.DepthGridDirect_Unsafe;
			CellRect cellRect = section.CellRect;
			IntVec3 size = base.Map.Size;
			int num = size.z - 1;
			IntVec3 size2 = base.Map.Size;
			int num2 = size2.x - 1;
			bool flag = false;
			CellIndices cellIndices = base.Map.cellIndices;
			for (int i = cellRect.minX; i <= cellRect.maxX; i++)
			{
				for (int j = cellRect.minZ; j <= cellRect.maxZ; j++)
				{
					float num3 = depthGridDirect_Unsafe[cellIndices.CellToIndex(i, j)];
					int num4 = cellIndices.CellToIndex(i, j - 1);
					float num5 = (j <= 0) ? num3 : depthGridDirect_Unsafe[num4];
					num4 = cellIndices.CellToIndex(i - 1, j - 1);
					float num6 = (j <= 0 || i <= 0) ? num3 : depthGridDirect_Unsafe[num4];
					num4 = cellIndices.CellToIndex(i - 1, j);
					float num7 = (i <= 0) ? num3 : depthGridDirect_Unsafe[num4];
					num4 = cellIndices.CellToIndex(i - 1, j + 1);
					float num8 = (j >= num || i <= 0) ? num3 : depthGridDirect_Unsafe[num4];
					num4 = cellIndices.CellToIndex(i, j + 1);
					float num9 = (j >= num) ? num3 : depthGridDirect_Unsafe[num4];
					num4 = cellIndices.CellToIndex(i + 1, j + 1);
					float num10 = (j >= num || i >= num2) ? num3 : depthGridDirect_Unsafe[num4];
					num4 = cellIndices.CellToIndex(i + 1, j);
					float num11 = (i >= num2) ? num3 : depthGridDirect_Unsafe[num4];
					num4 = cellIndices.CellToIndex(i + 1, j - 1);
					float num12 = (j <= 0 || i >= num2) ? num3 : depthGridDirect_Unsafe[num4];
					vertDepth[0] = (num5 + num6 + num7 + num3) / 4f;
					vertDepth[1] = (num7 + num3) / 2f;
					vertDepth[2] = (num7 + num8 + num9 + num3) / 4f;
					vertDepth[3] = (num9 + num3) / 2f;
					vertDepth[4] = (num9 + num10 + num11 + num3) / 4f;
					vertDepth[5] = (num11 + num3) / 2f;
					vertDepth[6] = (num11 + num12 + num5 + num3) / 4f;
					vertDepth[7] = (num5 + num3) / 2f;
					vertDepth[8] = num3;
					for (int k = 0; k < 9; k++)
					{
						if (vertDepth[k] > 0.01f)
						{
							flag = true;
						}
						subMesh.colors.Add(SnowDepthColor(vertDepth[k]));
					}
				}
			}
			if (flag)
			{
				subMesh.disabled = false;
				subMesh.FinalizeMesh(MeshParts.Colors);
			}
			else
			{
				subMesh.disabled = true;
			}
		}

		private static Color32 SnowDepthColor(float snowDepth)
		{
			return Color32.Lerp(ColorClear, ColorWhite, snowDepth);
		}
	}
}
