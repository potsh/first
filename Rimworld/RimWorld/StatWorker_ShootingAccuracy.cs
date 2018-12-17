using System.Text;
using Verse;

namespace RimWorld
{
	public class StatWorker_ShootingAccuracy : StatWorker
	{
		public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.GetExplanationFinalizePart(req, numberSense, finalVal));
			stringBuilder.AppendLine();
			for (int i = 5; i <= 45; i += 5)
			{
				float f = ShotReport.HitFactorFromShooter(finalVal, (float)i);
				stringBuilder.AppendLine("distance".Translate().CapitalizeFirst() + " " + i.ToString() + ": " + f.ToStringPercent("F1"));
			}
			return stringBuilder.ToString();
		}
	}
}
