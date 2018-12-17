using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public static class ThingDefGenerator_Buildings
	{
		public static readonly string BlueprintDefNamePrefix = "Blueprint_";

		public static readonly string InstallBlueprintDefNamePrefix = "Install_";

		public static readonly string BuildingFrameDefNamePrefix = "Frame_";

		private static readonly string TerrainBlueprintGraphicPath = "Things/Special/TerrainBlueprint";

		private static Color BlueprintColor = new Color(0.8235294f, 0.921568632f, 1f, 0.6f);

		public static IEnumerable<ThingDef> ImpliedBlueprintAndFrameDefs()
		{
			foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs.ToList())
			{
				ThingDef blueprint2 = null;
				if (item.BuildableByPlayer)
				{
					blueprint2 = NewBlueprintDef_Thing(item, isInstallBlueprint: false);
					yield return blueprint2;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				if (item.Minifiable)
				{
					yield return NewBlueprintDef_Thing(item, isInstallBlueprint: true, blueprint2);
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			foreach (TerrainDef allDef in DefDatabase<TerrainDef>.AllDefs)
			{
				if (allDef.BuildableByPlayer)
				{
					yield return NewBlueprintDef_Terrain(allDef);
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			yield break;
			IL_0226:
			/*Error near IL_0227: Unexpected return in MoveNext()*/;
		}

		private static ThingDef BaseBlueprintDef()
		{
			ThingDef thingDef = new ThingDef();
			thingDef.category = ThingCategory.Ethereal;
			thingDef.label = "Unspecified blueprint";
			thingDef.altitudeLayer = AltitudeLayer.Blueprint;
			thingDef.useHitPoints = false;
			thingDef.selectable = true;
			thingDef.seeThroughFog = true;
			thingDef.comps.Add(new CompProperties_Forbiddable());
			thingDef.drawerType = DrawerType.MapMeshAndRealTime;
			return thingDef;
		}

		private static ThingDef BaseFrameDef()
		{
			ThingDef thingDef = new ThingDef();
			thingDef.isFrameInt = true;
			thingDef.category = ThingCategory.Building;
			thingDef.label = "Unspecified building frame";
			thingDef.thingClass = typeof(Frame);
			thingDef.altitudeLayer = AltitudeLayer.Building;
			thingDef.useHitPoints = true;
			thingDef.selectable = true;
			thingDef.building = new BuildingProperties();
			thingDef.comps.Add(new CompProperties_Forbiddable());
			thingDef.scatterableOnMapGen = false;
			thingDef.leaveResourcesWhenKilled = true;
			return thingDef;
		}

		private static ThingDef NewBlueprintDef_Thing(ThingDef def, bool isInstallBlueprint, ThingDef normalBlueprint = null)
		{
			ThingDef thingDef = BaseBlueprintDef();
			thingDef.defName = BlueprintDefNamePrefix + def.defName;
			thingDef.label = def.label + "BlueprintLabelExtra".Translate();
			thingDef.size = def.size;
			thingDef.clearBuildingArea = def.clearBuildingArea;
			thingDef.modContentPack = def.modContentPack;
			if (!isInstallBlueprint)
			{
				thingDef.constructionSkillPrerequisite = def.constructionSkillPrerequisite;
			}
			thingDef.drawPlaceWorkersWhileSelected = def.drawPlaceWorkersWhileSelected;
			if (def.placeWorkers != null)
			{
				thingDef.placeWorkers = new List<Type>(def.placeWorkers);
			}
			if (isInstallBlueprint)
			{
				thingDef.defName = BlueprintDefNamePrefix + InstallBlueprintDefNamePrefix + def.defName;
			}
			if (isInstallBlueprint && normalBlueprint != null)
			{
				thingDef.graphicData = normalBlueprint.graphicData;
			}
			else
			{
				thingDef.graphicData = new GraphicData();
				if (def.building.blueprintGraphicData != null)
				{
					thingDef.graphicData.CopyFrom(def.building.blueprintGraphicData);
					if (thingDef.graphicData.graphicClass == null)
					{
						thingDef.graphicData.graphicClass = typeof(Graphic_Single);
					}
					if (thingDef.graphicData.shaderType == null)
					{
						thingDef.graphicData.shaderType = ShaderTypeDefOf.Transparent;
					}
					thingDef.graphicData.drawSize = def.graphicData.drawSize;
					thingDef.graphicData.linkFlags = def.graphicData.linkFlags;
					thingDef.graphicData.linkType = def.graphicData.linkType;
					thingDef.graphicData.color = BlueprintColor;
				}
				else
				{
					thingDef.graphicData.CopyFrom(def.graphicData);
					thingDef.graphicData.shaderType = ShaderTypeDefOf.EdgeDetect;
					thingDef.graphicData.color = BlueprintColor;
					thingDef.graphicData.colorTwo = Color.white;
					thingDef.graphicData.shadowData = null;
				}
			}
			if (thingDef.graphicData.shadowData != null)
			{
				Log.Error("Blueprint has shadow: " + def);
			}
			if (isInstallBlueprint)
			{
				thingDef.thingClass = typeof(Blueprint_Install);
			}
			else
			{
				thingDef.thingClass = def.building.blueprintClass;
			}
			if (def.thingClass == typeof(Building_Door))
			{
				thingDef.drawerType = DrawerType.RealtimeOnly;
			}
			else
			{
				thingDef.drawerType = DrawerType.MapMeshAndRealTime;
			}
			thingDef.entityDefToBuild = def;
			if (isInstallBlueprint)
			{
				def.installBlueprintDef = thingDef;
			}
			else
			{
				def.blueprintDef = thingDef;
			}
			return thingDef;
		}

		private static ThingDef NewFrameDef_Thing(ThingDef def)
		{
			ThingDef thingDef = BaseFrameDef();
			thingDef.defName = BuildingFrameDefNamePrefix + def.defName;
			thingDef.label = def.label + "FrameLabelExtra".Translate();
			thingDef.size = def.size;
			thingDef.SetStatBaseValue(StatDefOf.MaxHitPoints, (float)def.BaseMaxHitPoints * 0.25f);
			thingDef.SetStatBaseValue(StatDefOf.Beauty, -8f);
			thingDef.SetStatBaseValue(StatDefOf.Flammability, def.BaseFlammability);
			thingDef.fillPercent = 0.2f;
			thingDef.pathCost = 10;
			thingDef.description = def.description;
			thingDef.passability = def.passability;
			if ((int)thingDef.passability > 1)
			{
				thingDef.passability = Traversability.PassThroughOnly;
			}
			thingDef.selectable = def.selectable;
			thingDef.constructEffect = def.constructEffect;
			thingDef.building.isEdifice = def.building.isEdifice;
			thingDef.constructionSkillPrerequisite = def.constructionSkillPrerequisite;
			thingDef.clearBuildingArea = def.clearBuildingArea;
			thingDef.modContentPack = def.modContentPack;
			thingDef.drawPlaceWorkersWhileSelected = def.drawPlaceWorkersWhileSelected;
			if (def.placeWorkers != null)
			{
				thingDef.placeWorkers = new List<Type>(def.placeWorkers);
			}
			if (def.BuildableByPlayer)
			{
				thingDef.stuffCategories = def.stuffCategories;
			}
			thingDef.entityDefToBuild = def;
			def.frameDef = thingDef;
			return thingDef;
		}

		private static ThingDef NewBlueprintDef_Terrain(TerrainDef terrDef)
		{
			ThingDef thingDef = BaseBlueprintDef();
			thingDef.thingClass = typeof(Blueprint_Build);
			thingDef.defName = BlueprintDefNamePrefix + terrDef.defName;
			thingDef.label = terrDef.label + "BlueprintLabelExtra".Translate();
			thingDef.entityDefToBuild = terrDef;
			thingDef.graphicData = new GraphicData();
			thingDef.graphicData.shaderType = ShaderTypeDefOf.MetaOverlay;
			thingDef.graphicData.texPath = TerrainBlueprintGraphicPath;
			thingDef.graphicData.graphicClass = typeof(Graphic_Single);
			thingDef.constructionSkillPrerequisite = terrDef.constructionSkillPrerequisite;
			thingDef.clearBuildingArea = false;
			thingDef.modContentPack = terrDef.modContentPack;
			thingDef.entityDefToBuild = terrDef;
			terrDef.blueprintDef = thingDef;
			return thingDef;
		}

		private static ThingDef NewFrameDef_Terrain(TerrainDef terrDef)
		{
			ThingDef thingDef = BaseFrameDef();
			thingDef.defName = BuildingFrameDefNamePrefix + terrDef.defName;
			thingDef.label = terrDef.label + "FrameLabelExtra".Translate();
			thingDef.entityDefToBuild = terrDef;
			thingDef.useHitPoints = false;
			thingDef.fillPercent = 0f;
			thingDef.description = "Terrain building in progress.";
			thingDef.passability = Traversability.Standable;
			thingDef.selectable = true;
			thingDef.constructEffect = terrDef.constructEffect;
			thingDef.building.isEdifice = false;
			thingDef.constructionSkillPrerequisite = terrDef.constructionSkillPrerequisite;
			thingDef.clearBuildingArea = false;
			thingDef.modContentPack = terrDef.modContentPack;
			thingDef.category = ThingCategory.Ethereal;
			thingDef.entityDefToBuild = terrDef;
			terrDef.frameDef = thingDef;
			if (!thingDef.IsFrame)
			{
				Log.Error("Framedef is not frame: " + thingDef);
			}
			return thingDef;
		}
	}
}
