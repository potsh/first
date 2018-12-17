using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class Frame : Building, IThingHolder, IConstructible
	{
		public ThingOwner resourceContainer;

		public float workDone;

		private Material cachedCornerMat;

		private Material cachedTileMat;

		protected const float UnderfieldOverdrawFactor = 1.15f;

		protected const float CenterOverdrawFactor = 0.5f;

		private const int LongConstructionProjectThreshold = 9500;

		private static readonly Material UnderfieldMat = MaterialPool.MatFrom("Things/Building/BuildingFrame/Underfield", ShaderDatabase.Transparent);

		private static readonly Texture2D CornerTex = ContentFinder<Texture2D>.Get("Things/Building/BuildingFrame/Corner");

		private static readonly Texture2D TileTex = ContentFinder<Texture2D>.Get("Things/Building/BuildingFrame/Tile");

		[TweakValue("Pathfinding", 0f, 1000f)]
		public static ushort AvoidUnderConstructionPathFindCost = 800;

		private List<ThingDefCountClass> cachedMaterialsNeeded = new List<ThingDefCountClass>();

		public float WorkToBuild => def.entityDefToBuild.GetStatValueAbstract(StatDefOf.WorkToBuild, base.Stuff);

		public float WorkLeft => WorkToBuild - workDone;

		public float PercentComplete => workDone / WorkToBuild;

		public override string Label => LabelEntityToBuild + "FrameLabelExtra".Translate();

		public string LabelEntityToBuild
		{
			get
			{
				string label = def.entityDefToBuild.label;
				if (base.Stuff != null)
				{
					return base.Stuff.label + " " + label;
				}
				return label;
			}
		}

		public override Color DrawColor
		{
			get
			{
				if (!def.MadeFromStuff)
				{
					List<ThingDefCountClass> costList = def.entityDefToBuild.costList;
					if (costList != null)
					{
						for (int i = 0; i < costList.Count; i++)
						{
							ThingDef thingDef = costList[i].thingDef;
							if (thingDef.IsStuff && thingDef.stuffProps.color != Color.white)
							{
								return thingDef.stuffProps.color;
							}
						}
					}
					return new Color(0.6f, 0.6f, 0.6f);
				}
				return base.DrawColor;
			}
		}

		public EffecterDef ConstructionEffect
		{
			get
			{
				if (base.Stuff != null && base.Stuff.stuffProps.constructEffect != null)
				{
					return base.Stuff.stuffProps.constructEffect;
				}
				if (def.entityDefToBuild.constructEffect != null)
				{
					return def.entityDefToBuild.constructEffect;
				}
				return EffecterDefOf.ConstructMetal;
			}
		}

		private Material CornerMat
		{
			get
			{
				if (cachedCornerMat == null)
				{
					cachedCornerMat = MaterialPool.MatFrom(CornerTex, ShaderDatabase.Cutout, DrawColor);
				}
				return cachedCornerMat;
			}
		}

		private Material TileMat
		{
			get
			{
				if (cachedTileMat == null)
				{
					cachedTileMat = MaterialPool.MatFrom(TileTex, ShaderDatabase.Cutout, DrawColor);
				}
				return cachedTileMat;
			}
		}

		public Frame()
		{
			resourceContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return resourceContainer;
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref workDone, "workDone", 0f);
			Scribe_Deep.Look(ref resourceContainer, "resourceContainer", this);
		}

		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			bool spawned = base.Spawned;
			Map map = base.Map;
			base.Destroy(mode);
			if (spawned)
			{
				ThingUtility.CheckAutoRebuildOnDestroyed(this, mode, map, def.entityDefToBuild);
			}
		}

		public ThingDef UIStuff()
		{
			return base.Stuff;
		}

		public List<ThingDefCountClass> MaterialsNeeded()
		{
			cachedMaterialsNeeded.Clear();
			List<ThingDefCountClass> list = def.entityDefToBuild.CostListAdjusted(base.Stuff);
			for (int i = 0; i < list.Count; i++)
			{
				ThingDefCountClass thingDefCountClass = list[i];
				int num = resourceContainer.TotalStackCountOfDef(thingDefCountClass.thingDef);
				int num2 = thingDefCountClass.count - num;
				if (num2 > 0)
				{
					cachedMaterialsNeeded.Add(new ThingDefCountClass(thingDefCountClass.thingDef, num2));
				}
			}
			return cachedMaterialsNeeded;
		}

		public void CompleteConstruction(Pawn worker)
		{
			resourceContainer.ClearAndDestroyContents();
			Map map = base.Map;
			Destroy();
			if (this.GetStatValue(StatDefOf.WorkToBuild) > 150f && def.entityDefToBuild is ThingDef && ((ThingDef)def.entityDefToBuild).category == ThingCategory.Building)
			{
				SoundDefOf.Building_Complete.PlayOneShot(new TargetInfo(base.Position, map));
			}
			ThingDef thingDef = def.entityDefToBuild as ThingDef;
			Thing thing = null;
			if (thingDef != null)
			{
				thing = ThingMaker.MakeThing(thingDef, base.Stuff);
				thing.SetFactionDirect(base.Faction);
				CompQuality compQuality = thing.TryGetComp<CompQuality>();
				if (compQuality != null)
				{
					QualityCategory q = QualityUtility.GenerateQualityCreatedByPawn(worker, SkillDefOf.Construction);
					compQuality.SetQuality(q, ArtGenerationContext.Colony);
					QualityUtility.SendCraftNotification(thing, worker);
				}
				CompArt compArt = thing.TryGetComp<CompArt>();
				if (compArt != null)
				{
					if (compQuality == null)
					{
						compArt.InitializeArt(ArtGenerationContext.Colony);
					}
					compArt.JustCreatedBy(worker);
				}
				thing.HitPoints = Mathf.CeilToInt((float)HitPoints / (float)base.MaxHitPoints * (float)thing.MaxHitPoints);
				GenSpawn.Spawn(thing, base.Position, map, base.Rotation, WipeMode.FullRefund);
			}
			else
			{
				map.terrainGrid.SetTerrain(base.Position, (TerrainDef)def.entityDefToBuild);
				FilthMaker.RemoveAllFilth(base.Position, map);
			}
			worker.records.Increment(RecordDefOf.ThingsConstructed);
			if (thing != null && thing.GetStatValue(StatDefOf.WorkToBuild) >= 9500f)
			{
				TaleRecorder.RecordTale(TaleDefOf.CompletedLongConstructionProject, worker, thing.def);
			}
		}

		public void FailConstruction(Pawn worker)
		{
			Map map = base.Map;
			Destroy(DestroyMode.FailConstruction);
			Blueprint_Build blueprint_Build = null;
			if (def.entityDefToBuild.blueprintDef != null)
			{
				blueprint_Build = (Blueprint_Build)ThingMaker.MakeThing(def.entityDefToBuild.blueprintDef);
				blueprint_Build.stuffToUse = base.Stuff;
				blueprint_Build.SetFactionDirect(base.Faction);
				GenSpawn.Spawn(blueprint_Build, base.Position, map, base.Rotation, WipeMode.FullRefund);
			}
			worker.GetLord()?.Notify_ConstructionFailed(worker, this, blueprint_Build);
			MoteMaker.ThrowText(DrawPos, map, "TextMote_ConstructionFail".Translate(), 6f);
			if (base.Faction == Faction.OfPlayer && WorkToBuild > 1400f)
			{
				Messages.Message("MessageConstructionFailed".Translate(LabelEntityToBuild, worker.LabelShort, worker.Named("WORKER")), new TargetInfo(base.Position, map), MessageTypeDefOf.NegativeEvent);
			}
		}

		public override void Draw()
		{
			Vector2 vector = new Vector2((float)def.size.x, (float)def.size.z);
			vector.x *= 1.15f;
			vector.y *= 1.15f;
			Vector3 s = new Vector3(vector.x, 1f, vector.y);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(DrawPos, base.Rotation.AsQuat, s);
			Graphics.DrawMesh(MeshPool.plane10, matrix, UnderfieldMat, 0);
			int num = 4;
			for (int i = 0; i < num; i++)
			{
				IntVec2 rotatedSize = base.RotatedSize;
				int x = rotatedSize.x;
				IntVec2 rotatedSize2 = base.RotatedSize;
				float num2 = (float)Mathf.Min(x, rotatedSize2.z);
				float num3 = num2 * 0.38f;
				IntVec3 intVec = default(IntVec3);
				switch (i)
				{
				case 0:
					intVec = new IntVec3(-1, 0, -1);
					break;
				case 1:
					intVec = new IntVec3(-1, 0, 1);
					break;
				case 2:
					intVec = new IntVec3(1, 0, 1);
					break;
				case 3:
					intVec = new IntVec3(1, 0, -1);
					break;
				}
				Vector3 b = default(Vector3);
				float num4 = (float)intVec.x;
				IntVec2 rotatedSize3 = base.RotatedSize;
				b.x = num4 * ((float)rotatedSize3.x / 2f - num3 / 2f);
				float num5 = (float)intVec.z;
				IntVec2 rotatedSize4 = base.RotatedSize;
				b.z = num5 * ((float)rotatedSize4.z / 2f - num3 / 2f);
				Vector3 s2 = new Vector3(num3, 1f, num3);
				Matrix4x4 matrix2 = default(Matrix4x4);
				matrix2.SetTRS(DrawPos + Vector3.up * 0.03f + b, new Rot4(i).AsQuat, s2);
				Graphics.DrawMesh(MeshPool.plane10, matrix2, CornerMat, 0);
			}
			float num6 = PercentComplete / 1f;
			float num7 = num6;
			IntVec2 rotatedSize5 = base.RotatedSize;
			float num8 = num7 * (float)rotatedSize5.x;
			IntVec2 rotatedSize6 = base.RotatedSize;
			int num9 = Mathf.CeilToInt(num8 * (float)rotatedSize6.z * 4f);
			IntVec2 intVec2 = base.RotatedSize * 2;
			for (int j = 0; j < num9; j++)
			{
				IntVec2 intVec3 = default(IntVec2);
				intVec3.z = j / intVec2.x;
				intVec3.x = j - intVec3.z * intVec2.x;
				Vector3 a = new Vector3((float)intVec3.x * 0.5f, 0f, (float)intVec3.z * 0.5f) + DrawPos;
				float x2 = a.x;
				IntVec2 rotatedSize7 = base.RotatedSize;
				a.x = x2 - ((float)rotatedSize7.x * 0.5f - 0.25f);
				float z = a.z;
				IntVec2 rotatedSize8 = base.RotatedSize;
				a.z = z - ((float)rotatedSize8.z * 0.5f - 0.25f);
				Vector3 s3 = new Vector3(0.5f, 1f, 0.5f);
				Matrix4x4 matrix3 = default(Matrix4x4);
				matrix3.SetTRS(a + Vector3.up * 0.02f, Quaternion.identity, s3);
				Graphics.DrawMesh(MeshPool.plane10, matrix3, TileMat, 0);
			}
			Comps_PostDraw();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			using (IEnumerator<Gizmo> enumerator = base.GetGizmos().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Gizmo c = enumerator.Current;
					yield return c;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			Command buildCopy = BuildCopyCommandUtility.BuildCopyCommand(def.entityDefToBuild, base.Stuff);
			if (buildCopy != null)
			{
				yield return (Gizmo)buildCopy;
				/*Error: Unable to find new state assignment for yield return*/;
			}
			if (base.Faction == Faction.OfPlayer)
			{
				using (IEnumerator<Command> enumerator2 = BuildFacilityCommandUtility.BuildFacilityCommands(def.entityDefToBuild).GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						Command facility = enumerator2.Current;
						yield return (Gizmo)facility;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			yield break;
			IL_01be:
			/*Error near IL_01bf: Unexpected return in MoveNext()*/;
		}

		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			stringBuilder.AppendLine("ContainedResources".Translate() + ":");
			List<ThingDefCountClass> list = def.entityDefToBuild.CostListAdjusted(base.Stuff);
			for (int i = 0; i < list.Count; i++)
			{
				ThingDefCountClass need = list[i];
				int num = need.count;
				foreach (ThingDefCountClass item in from needed in MaterialsNeeded()
				where needed.thingDef == need.thingDef
				select needed)
				{
					num -= item.count;
				}
				stringBuilder.AppendLine(need.thingDef.LabelCap + ": " + num + " / " + need.count);
			}
			stringBuilder.Append("WorkLeft".Translate() + ": " + WorkLeft.ToStringWorkAmount());
			return stringBuilder.ToString();
		}

		public override ushort PathFindCostFor(Pawn p)
		{
			if (base.Faction == null)
			{
				return 0;
			}
			if (def.entityDefToBuild is TerrainDef)
			{
				return 0;
			}
			if (p.Faction == base.Faction || p.HostFaction == base.Faction)
			{
				return AvoidUnderConstructionPathFindCost;
			}
			return 0;
		}
	}
}
