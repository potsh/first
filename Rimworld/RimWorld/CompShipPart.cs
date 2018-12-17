using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class CompShipPart : ThingComp
	{
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			yield return (Gizmo)new Command_Action
			{
				action = ShowReport,
				defaultLabel = "CommandShipLaunchReport".Translate(),
				defaultDesc = "CommandShipLaunchReportDesc".Translate(),
				hotKey = KeyBindingDefOf.Misc4,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchReport")
			};
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public void ShowReport()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!ShipUtility.LaunchFailReasons((Building)parent).Any())
			{
				stringBuilder.AppendLine("ShipReportCanLaunch".Translate());
			}
			else
			{
				stringBuilder.AppendLine("ShipReportCannotLaunch".Translate());
				foreach (string item in ShipUtility.LaunchFailReasons((Building)parent))
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(item);
				}
			}
			Dialog_MessageBox window = new Dialog_MessageBox(stringBuilder.ToString());
			Find.WindowStack.Add(window);
		}
	}
}
