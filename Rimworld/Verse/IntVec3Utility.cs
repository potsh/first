using System;
using UnityEngine;

namespace Verse
{
	public static class IntVec3Utility
	{
		public static IntVec3 ToIntVec3(this Vector3 vect)
		{
			return new IntVec3(vect);
		}

		public static float DistanceTo(this IntVec3 a, IntVec3 b)
		{
			return (a - b).LengthHorizontal;
		}

		public static int DistanceToSquared(this IntVec3 a, IntVec3 b)
		{
			return (a - b).LengthHorizontalSquared;
		}

		public static IntVec3 RotatedBy(this IntVec3 orig, Rot4 rot)
		{
			switch (rot.AsInt)
			{
			case 0:
				return orig;
			case 1:
				return new IntVec3(orig.z, orig.y, -orig.x);
			case 2:
				return new IntVec3(-orig.x, orig.y, -orig.z);
			case 3:
				return new IntVec3(-orig.z, orig.y, orig.x);
			default:
				return orig;
			}
		}

		public static int ManhattanDistanceFlat(IntVec3 a, IntVec3 b)
		{
			return Math.Abs(a.x - b.x) + Math.Abs(a.z - b.z);
		}

		public static IntVec3 RandomHorizontalOffset(float maxDist)
		{
			return Vector3Utility.RandomHorizontalOffset(maxDist).ToIntVec3();
		}

		public static int DistanceToEdge(this IntVec3 v, Map map)
		{
			int num = Mathf.Min(v.x, v.z);
			int a = num;
			IntVec3 size = map.Size;
			num = Mathf.Min(a, size.x - v.x - 1);
			int a2 = num;
			IntVec3 size2 = map.Size;
			num = Mathf.Min(a2, size2.z - v.z - 1);
			return Mathf.Max(num, 0);
		}
	}
}
