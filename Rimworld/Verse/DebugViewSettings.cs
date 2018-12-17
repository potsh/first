namespace Verse
{
	public static class DebugViewSettings
	{
		public static bool drawFog = true;

		public static bool drawSnow = true;

		public static bool drawTerrain = true;

		public static bool drawTerrainWater = true;

		public static bool drawThingsDynamic = true;

		public static bool drawThingsPrinted = true;

		public static bool drawShadows = true;

		public static bool drawLightingOverlay = true;

		public static bool drawWorldOverlays = true;

		public static bool drawPaths;

		public static bool drawCastPositionSearch;

		public static bool drawDestSearch;

		public static bool drawSectionEdges;

		public static bool drawRiverDebug;

		public static bool drawPawnDebug;

		public static bool drawPawnRotatorTarget;

		public static bool drawRegions;

		public static bool drawRegionLinks;

		public static bool drawRegionDirties;

		public static bool drawRegionTraversal;

		public static bool drawRegionThings;

		public static bool drawRooms;

		public static bool drawRoomGroups;

		public static bool drawPower;

		public static bool drawPowerNetGrid;

		public static bool drawOpportunisticJobs;

		public static bool drawTooltipEdges;

		public static bool drawRecordedNoise;

		public static bool drawFoodSearchFromMouse;

		public static bool drawPreyInfo;

		public static bool drawGlow;

		public static bool drawAvoidGrid;

		public static bool drawLords;

		public static bool drawDuties;

		public static bool drawShooting;

		public static bool drawInfestationChance;

		public static bool drawStealDebug;

		public static bool drawDeepResources;

		public static bool drawAttackTargetScores;

		public static bool drawInteractionCells;

		public static bool drawDoorsDebug;

		public static bool writeGame;

		public static bool writeSteamItems;

		public static bool writeConcepts;

		public static bool writePathCosts;

		public static bool writeFertility;

		public static bool writeLinkFlags;

		public static bool writeCover;

		public static bool writeCellContents;

		public static bool writeMusicManagerPlay;

		public static bool writeStoryteller;

		public static bool writePlayingSounds;

		public static bool writeSoundEventsRecord;

		public static bool writeMoteSaturation;

		public static bool writeSnowDepth;

		public static bool writeEcosystem;

		public static bool writeRecentStrikes;

		public static bool writeBeauty;

		public static bool writeListRepairableBldgs;

		public static bool writeListFilthInHomeArea;

		public static bool writeListHaulables;

		public static bool writeListMergeables;

		public static bool writeTotalSnowDepth;

		public static bool writeCanReachColony;

		public static bool writeMentalStateCalcs;

		public static bool writeWind;

		public static bool writeTerrain;

		public static bool writeApparelScore;

		public static bool writeWorkSettings;

		public static bool writeSkyManager;

		public static bool writeMemoryUsage;

		public static bool writeMapGameConditions;

		public static bool writeAttackTargets;

		public static bool logIncapChance;

		public static bool logInput;

		public static bool logApparelGeneration;

		public static bool logLordToilTransitions;

		public static bool logGrammarResolution;

		public static bool logCombatLogMouseover;

		public static bool logMapLoad;

		public static bool logTutor;

		public static bool logSignals;

		public static bool logWorldPawnGC;

		public static bool logTaleRecording;

		public static bool logHourlyScreenshot;

		public static bool logFilthSummary;

		public static bool debugApparelOptimize;

		public static bool showAllRoomStats;

		public static bool showFloatMenuWorkGivers;

		public static void drawTerrainWaterToggled()
		{
			if (Find.CurrentMap != null)
			{
				Find.CurrentMap.mapDrawer.WholeMapChanged(MapMeshFlag.Terrain);
			}
		}

		public static void drawShadowsToggled()
		{
			if (Find.CurrentMap != null)
			{
				Find.CurrentMap.mapDrawer.WholeMapChanged((MapMeshFlag)(-1));
			}
		}
	}
}
