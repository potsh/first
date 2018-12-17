using System;

namespace Verse.AI
{
	[Flags]
	public enum TargetScanFlags : byte
	{
		None = 0x0,
		NeedLOSToPawns = 0x1,
		NeedLOSToNonPawns = 0x2,
		NeedLOSToAll = 0x3,
		NeedReachable = 0x4,
		NeedReachableIfCantHitFromMyPos = 0x8,
		NeedNonBurning = 0x10,
		NeedThreat = 0x20,
		LOSBlockableByGas = 0x40
	}
}
