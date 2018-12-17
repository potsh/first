using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class SkillNeed_BaseBonus : SkillNeed
	{
		private float baseValue = 0.5f;

		private float bonusPerLevel = 0.05f;

		public override float ValueFor(Pawn pawn)
		{
			if (pawn.skills == null)
			{
				return 1f;
			}
			int level = pawn.skills.GetSkill(skill).Level;
			return ValueAtLevel(level);
		}

		private float ValueAtLevel(int level)
		{
			return baseValue + bonusPerLevel * (float)level;
		}

		public override IEnumerable<string> ConfigErrors()
		{
			using (IEnumerator<string> enumerator = base.ConfigErrors().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string error = enumerator.Current;
					yield return error;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			int i = 1;
			while (true)
			{
				if (i > 20)
				{
					yield break;
				}
				float factor = ValueAtLevel(i);
				if (factor <= 0f)
				{
					break;
				}
				i++;
			}
			yield return "SkillNeed yields factor < 0 at skill level " + i;
			/*Error: Unable to find new state assignment for yield return*/;
			IL_013a:
			/*Error near IL_013b: Unexpected return in MoveNext()*/;
		}
	}
}
