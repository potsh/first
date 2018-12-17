using System;
using Verse;

namespace RimWorld
{
	public static class TechLevelUtility
	{
		public static string ToStringHuman(this TechLevel tl)
		{
			switch (tl)
			{
			case TechLevel.Undefined:
				return "Undefined".Translate();
			case TechLevel.Animal:
				return "TechLevel_Animal".Translate();
			case TechLevel.Neolithic:
				return "TechLevel_Neolithic".Translate();
			case TechLevel.Medieval:
				return "TechLevel_Medieval".Translate();
			case TechLevel.Industrial:
				return "TechLevel_Industrial".Translate();
			case TechLevel.Spacer:
				return "TechLevel_Spacer".Translate();
			case TechLevel.Ultra:
				return "TechLevel_Ultra".Translate();
			case TechLevel.Archotech:
				return "TechLevel_Archotech".Translate();
			default:
				throw new NotImplementedException();
			}
		}

		public static bool CanSpawnWithEquipmentFrom(this TechLevel pawnLevel, TechLevel gearLevel)
		{
			if (gearLevel != 0)
			{
				switch (pawnLevel)
				{
				case TechLevel.Undefined:
					return false;
				case TechLevel.Neolithic:
					return (int)gearLevel <= 2;
				case TechLevel.Medieval:
					return (int)gearLevel <= 3;
				case TechLevel.Industrial:
					return gearLevel == TechLevel.Industrial;
				case TechLevel.Spacer:
					return gearLevel == TechLevel.Spacer || gearLevel == TechLevel.Industrial;
				case TechLevel.Ultra:
					return gearLevel == TechLevel.Ultra || gearLevel == TechLevel.Spacer;
				case TechLevel.Archotech:
					return gearLevel == TechLevel.Archotech;
				default:
					Log.Error("Unknown tech levels " + pawnLevel + ", " + gearLevel);
					return true;
				}
			}
			return false;
		}

		public static bool IsNeolithicOrWorse(this TechLevel techLevel)
		{
			if (techLevel == TechLevel.Undefined)
			{
				return false;
			}
			return (int)techLevel <= 2;
		}
	}
}
