#define PFPROFILE
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Verse.AI
{
	public class PathFinder
	{
		internal struct CostNode
		{
			public int index;

			public int cost;

			public CostNode(int index, int cost)
			{
				this.index = index;
				this.cost = cost;
			}
		}

		private struct PathFinderNodeFast
		{
			public int knownCost;

			public int heuristicCost;

			public int parentIndex;

			public int costNodeCost;

			public ushort status;
		}

		internal class CostNodeComparer : IComparer<CostNode>
		{
			public int Compare(CostNode a, CostNode b)
			{
				return a.cost.CompareTo(b.cost);
			}
		}

		private Map map;

		private FastPriorityQueue<CostNode> openList;

		private PathFinderNodeFast[] calcGrid;

		private ushort statusOpenValue = 1;

		private ushort statusClosedValue = 2;

		private RegionCostCalculatorWrapper regionCostCalculator;

		private int mapSizeX;

		private int mapSizeZ;

		private PathGrid pathGrid;

		private Building[] edificeGrid;

		private List<Blueprint>[] blueprintGrid;

		private CellIndices cellIndices;

		private List<int> disallowedCornerIndices = new List<int>(4);

		public const int DefaultMoveTicksCardinal = 13;

		private const int DefaultMoveTicksDiagonal = 18;

		private const int SearchLimit = 160000;

		private static readonly int[] Directions = new int[16]
		{
			0,
			1,
			0,
			-1,
			1,
			1,
			-1,
			-1,
			-1,
			0,
			1,
			0,
			-1,
			1,
			1,
			-1
		};

		private const int Cost_DoorToBash = 300;

		private const int Cost_BlockedWallBase = 70;

		private const float Cost_BlockedWallExtraPerHitPoint = 0.2f;

		private const int Cost_BlockedDoor = 50;

		private const float Cost_BlockedDoorPerHitPoint = 0.2f;

		public const int Cost_OutsideAllowedArea = 600;

		private const int Cost_PawnCollision = 175;

		private const int NodesToOpenBeforeRegionBasedPathing_NonColonist = 2000;

		private const int NodesToOpenBeforeRegionBasedPathing_Colonist = 100000;

		private const float NonRegionBasedHeuristicStrengthAnimal = 1.75f;

		private static readonly SimpleCurve NonRegionBasedHeuristicStrengthHuman_DistanceCurve = new SimpleCurve
		{
			new CurvePoint(40f, 1f),
			new CurvePoint(120f, 2.8f)
		};

		private static readonly SimpleCurve RegionHeuristicWeightByNodesOpened = new SimpleCurve
		{
			new CurvePoint(0f, 1f),
			new CurvePoint(3500f, 1f),
			new CurvePoint(4500f, 5f),
			new CurvePoint(30000f, 50f),
			new CurvePoint(100000f, 500f)
		};

		public PathFinder(Map map)
		{
			this.map = map;
			IntVec3 size = map.Size;
			mapSizeX = size.x;
			IntVec3 size2 = map.Size;
			mapSizeZ = size2.z;
			calcGrid = new PathFinderNodeFast[mapSizeX * mapSizeZ];
			openList = new FastPriorityQueue<CostNode>(new CostNodeComparer());
			regionCostCalculator = new RegionCostCalculatorWrapper(map);
		}

		public PawnPath FindPath(IntVec3 start, LocalTargetInfo dest, Pawn pawn, PathEndMode peMode = PathEndMode.OnCell)
		{
			bool flag = false;
			if (pawn != null && pawn.CurJob != null && pawn.CurJob.canBash)
			{
				flag = true;
			}
			Danger maxDanger = Danger.Deadly;
			bool canBash = flag;
			return FindPath(start, dest, TraverseParms.For(pawn, maxDanger, TraverseMode.ByPawn, canBash), peMode);
		}

		public PawnPath FindPath(IntVec3 start, LocalTargetInfo dest, TraverseParms traverseParms, PathEndMode peMode = PathEndMode.OnCell)
		{
			if (DebugSettings.pathThroughWalls)
			{
				traverseParms.mode = TraverseMode.PassAllDestroyableThings;
			}
			Pawn pawn = traverseParms.pawn;
			if (pawn != null && pawn.Map != map)
			{
				Log.Error("Tried to FindPath for pawn which is spawned in another map. His map PathFinder should have been used, not this one. pawn=" + pawn + " pawn.Map=" + pawn.Map + " map=" + map);
				return PawnPath.NotFound;
			}
			if (!start.IsValid)
			{
				Log.Error("Tried to FindPath with invalid start " + start + ", pawn= " + pawn);
				return PawnPath.NotFound;
			}
			if (!dest.IsValid)
			{
				Log.Error("Tried to FindPath with invalid dest " + dest + ", pawn= " + pawn);
				return PawnPath.NotFound;
			}
			if (traverseParms.mode == TraverseMode.ByPawn)
			{
				if (!pawn.CanReach(dest, peMode, Danger.Deadly, traverseParms.canBash, traverseParms.mode))
				{
					return PawnPath.NotFound;
				}
			}
			else if (!map.reachability.CanReach(start, dest, peMode, traverseParms))
			{
				return PawnPath.NotFound;
			}
			PfProfilerBeginSample("FindPath for " + pawn + " from " + start + " to " + dest + ((!dest.HasThing) ? string.Empty : (" at " + dest.Cell)));
			cellIndices = map.cellIndices;
			pathGrid = map.pathGrid;
			this.edificeGrid = map.edificeGrid.InnerArray;
			blueprintGrid = map.blueprintGrid.InnerArray;
			IntVec3 cell = dest.Cell;
			int x = cell.x;
			IntVec3 cell2 = dest.Cell;
			int z = cell2.z;
			int curIndex = cellIndices.CellToIndex(start);
			int num = cellIndices.CellToIndex(dest.Cell);
			ByteGrid byteGrid = pawn?.GetAvoidGrid();
			bool flag = traverseParms.mode == TraverseMode.PassAllDestroyableThings || traverseParms.mode == TraverseMode.PassAllDestroyableThingsNotWater;
			bool flag2 = traverseParms.mode != TraverseMode.NoPassClosedDoorsOrWater && traverseParms.mode != TraverseMode.PassAllDestroyableThingsNotWater;
			bool flag3 = !flag;
			CellRect cellRect = CalculateDestinationRect(dest, peMode);
			bool flag4 = cellRect.Width == 1 && cellRect.Height == 1;
			int[] array = map.pathGrid.pathGrid;
			TerrainDef[] topGrid = map.terrainGrid.topGrid;
			EdificeGrid edificeGrid = map.edificeGrid;
			int num2 = 0;
			int num3 = 0;
			Area allowedArea = GetAllowedArea(pawn);
			bool flag5 = pawn != null && PawnUtility.ShouldCollideWithPawns(pawn);
			bool flag6 = true && DebugViewSettings.drawPaths;
			bool flag7 = !flag && start.GetRegion(map) != null && flag2;
			bool flag8 = !flag || !flag3;
			bool flag9 = false;
			bool flag10 = pawn?.Drafted ?? false;
			int num4 = (!(pawn?.IsColonist ?? false)) ? 2000 : 100000;
			int num5 = 0;
			int num6 = 0;
			float num7 = DetermineHeuristicStrength(pawn, start, dest);
			int num8;
			int num9;
			if (pawn != null)
			{
				num8 = pawn.TicksPerMoveCardinal;
				num9 = pawn.TicksPerMoveDiagonal;
			}
			else
			{
				num8 = 13;
				num9 = 18;
			}
			CalculateAndAddDisallowedCorners(traverseParms, peMode, cellRect);
			InitStatusesAndPushStartNode(ref curIndex, start);
			while (true)
			{
				PfProfilerBeginSample("Open cell");
				if (openList.Count <= 0)
				{
					string text = (pawn == null || pawn.CurJob == null) ? "null" : pawn.CurJob.ToString();
					string text2 = (pawn == null || pawn.Faction == null) ? "null" : pawn.Faction.ToString();
					Log.Warning(pawn + " pathing from " + start + " to " + dest + " ran out of cells to process.\nJob:" + text + "\nFaction: " + text2);
					DebugDrawRichData();
					PfProfilerEndSample();
					return PawnPath.NotFound;
				}
				num5 += openList.Count;
				num6++;
				CostNode costNode = openList.Pop();
				curIndex = costNode.index;
				if (costNode.cost != calcGrid[curIndex].costNodeCost)
				{
					PfProfilerEndSample();
				}
				else if (calcGrid[curIndex].status == statusClosedValue)
				{
					PfProfilerEndSample();
				}
				else
				{
					IntVec3 c = cellIndices.IndexToCell(curIndex);
					int x2 = c.x;
					int z2 = c.z;
					if (flag6)
					{
						DebugFlash(c, (float)calcGrid[curIndex].knownCost / 1500f, calcGrid[curIndex].knownCost.ToString());
					}
					if (flag4)
					{
						if (curIndex == num)
						{
							PfProfilerEndSample();
							PawnPath result = FinalizedPath(curIndex, flag9);
							PfProfilerEndSample();
							return result;
						}
					}
					else if (cellRect.Contains(c) && !disallowedCornerIndices.Contains(curIndex))
					{
						PfProfilerEndSample();
						PawnPath result2 = FinalizedPath(curIndex, flag9);
						PfProfilerEndSample();
						return result2;
					}
					if (num2 > 160000)
					{
						break;
					}
					PfProfilerEndSample();
					PfProfilerBeginSample("Neighbor consideration");
					for (int i = 0; i < 8; i++)
					{
						uint num10 = (uint)(x2 + Directions[i]);
						uint num11 = (uint)(z2 + Directions[i + 8]);
						if (num10 < mapSizeX && num11 < mapSizeZ)
						{
							int num12 = (int)num10;
							int num13 = (int)num11;
							int num14 = cellIndices.CellToIndex(num12, num13);
							if (calcGrid[num14].status != statusClosedValue || flag9)
							{
								int num15 = 0;
								bool flag11 = false;
								if (flag2 || !new IntVec3(num12, 0, num13).GetTerrain(map).HasTag("Water"))
								{
									if (!pathGrid.WalkableFast(num14))
									{
										if (!flag)
										{
											if (flag6)
											{
												DebugFlash(new IntVec3(num12, 0, num13), 0.22f, "walk");
											}
											continue;
										}
										flag11 = true;
										num15 += 70;
										Building building = edificeGrid[num14];
										if (building == null || !IsDestroyable(building))
										{
											continue;
										}
										num15 += (int)((float)building.HitPoints * 0.2f);
									}
									switch (i)
									{
									case 4:
										if (BlocksDiagonalMovement(curIndex - mapSizeX))
										{
											if (flag8)
											{
												if (flag6)
												{
													DebugFlash(new IntVec3(x2, 0, z2 - 1), 0.9f, "corn");
												}
												break;
											}
											num15 += 70;
										}
										if (BlocksDiagonalMovement(curIndex + 1))
										{
											if (flag8)
											{
												if (flag6)
												{
													DebugFlash(new IntVec3(x2 + 1, 0, z2), 0.9f, "corn");
												}
												break;
											}
											num15 += 70;
										}
										goto default;
									case 5:
										if (BlocksDiagonalMovement(curIndex + mapSizeX))
										{
											if (flag8)
											{
												if (flag6)
												{
													DebugFlash(new IntVec3(x2, 0, z2 + 1), 0.9f, "corn");
												}
												break;
											}
											num15 += 70;
										}
										if (BlocksDiagonalMovement(curIndex + 1))
										{
											if (flag8)
											{
												if (flag6)
												{
													DebugFlash(new IntVec3(x2 + 1, 0, z2), 0.9f, "corn");
												}
												break;
											}
											num15 += 70;
										}
										goto default;
									case 6:
										if (BlocksDiagonalMovement(curIndex + mapSizeX))
										{
											if (flag8)
											{
												if (flag6)
												{
													DebugFlash(new IntVec3(x2, 0, z2 + 1), 0.9f, "corn");
												}
												break;
											}
											num15 += 70;
										}
										if (BlocksDiagonalMovement(curIndex - 1))
										{
											if (flag8)
											{
												if (flag6)
												{
													DebugFlash(new IntVec3(x2 - 1, 0, z2), 0.9f, "corn");
												}
												break;
											}
											num15 += 70;
										}
										goto default;
									case 7:
										if (BlocksDiagonalMovement(curIndex - mapSizeX))
										{
											if (flag8)
											{
												if (flag6)
												{
													DebugFlash(new IntVec3(x2, 0, z2 - 1), 0.9f, "corn");
												}
												break;
											}
											num15 += 70;
										}
										if (BlocksDiagonalMovement(curIndex - 1))
										{
											if (flag8)
											{
												if (flag6)
												{
													DebugFlash(new IntVec3(x2 - 1, 0, z2), 0.9f, "corn");
												}
												break;
											}
											num15 += 70;
										}
										goto default;
									default:
									{
										int num16 = (i <= 3) ? num8 : num9;
										num16 += num15;
										if (!flag11)
										{
											num16 += array[num14];
											num16 = ((!flag10) ? (num16 + topGrid[num14].extraNonDraftedPerceivedPathCost) : (num16 + topGrid[num14].extraDraftedPerceivedPathCost));
										}
										if (byteGrid != null)
										{
											num16 += byteGrid[num14] * 8;
										}
										if (allowedArea != null && !allowedArea[num14])
										{
											num16 += 600;
										}
										if (flag5 && PawnUtility.AnyPawnBlockingPathAt(new IntVec3(num12, 0, num13), pawn, actAsIfHadCollideWithPawnsJob: false, collideOnlyWithStandingPawns: false, forPathFinder: true))
										{
											num16 += 175;
										}
										Building building2 = this.edificeGrid[num14];
										if (building2 != null)
										{
											PfProfilerBeginSample("Edifices");
											int buildingCost = GetBuildingCost(building2, traverseParms, pawn);
											if (buildingCost == 2147483647)
											{
												PfProfilerEndSample();
												break;
											}
											num16 += buildingCost;
											PfProfilerEndSample();
										}
										List<Blueprint> list = blueprintGrid[num14];
										if (list != null)
										{
											PfProfilerBeginSample("Blueprints");
											int num17 = 0;
											for (int j = 0; j < list.Count; j++)
											{
												num17 = Mathf.Max(num17, GetBlueprintCost(list[j], pawn));
											}
											if (num17 == 2147483647)
											{
												PfProfilerEndSample();
												break;
											}
											num16 += num17;
											PfProfilerEndSample();
										}
										int num18 = num16 + calcGrid[curIndex].knownCost;
										ushort status = calcGrid[num14].status;
										if (status == statusClosedValue || status == statusOpenValue)
										{
											int num19 = 0;
											if (status == statusClosedValue)
											{
												num19 = num8;
											}
											if (calcGrid[num14].knownCost <= num18 + num19)
											{
												break;
											}
										}
										if (flag9)
										{
											calcGrid[num14].heuristicCost = Mathf.RoundToInt((float)regionCostCalculator.GetPathCostFromDestToRegion(num14) * RegionHeuristicWeightByNodesOpened.Evaluate((float)num3));
											if (calcGrid[num14].heuristicCost < 0)
											{
												Log.ErrorOnce("Heuristic cost overflow for " + pawn.ToStringSafe() + " pathing from " + start + " to " + dest + ".", pawn.GetHashCode() ^ 0xB8DC389);
												calcGrid[num14].heuristicCost = 0;
											}
										}
										else if (status != statusClosedValue && status != statusOpenValue)
										{
											int dx = Math.Abs(num12 - x);
											int dz = Math.Abs(num13 - z);
											int num20 = GenMath.OctileDistance(dx, dz, num8, num9);
											calcGrid[num14].heuristicCost = Mathf.RoundToInt((float)num20 * num7);
										}
										int num21 = num18 + calcGrid[num14].heuristicCost;
										if (num21 < 0)
										{
											Log.ErrorOnce("Node cost overflow for " + pawn.ToStringSafe() + " pathing from " + start + " to " + dest + ".", pawn.GetHashCode() ^ 0x53CB9DE);
											num21 = 0;
										}
										calcGrid[num14].parentIndex = curIndex;
										calcGrid[num14].knownCost = num18;
										calcGrid[num14].status = statusOpenValue;
										calcGrid[num14].costNodeCost = num21;
										num3++;
										openList.Push(new CostNode(num14, num21));
										break;
									}
									}
								}
							}
						}
					}
					PfProfilerEndSample();
					num2++;
					calcGrid[curIndex].status = statusClosedValue;
					if (num3 >= num4 && flag7 && !flag9)
					{
						flag9 = true;
						regionCostCalculator.Init(cellRect, traverseParms, num8, num9, byteGrid, allowedArea, flag10, disallowedCornerIndices);
						InitStatusesAndPushStartNode(ref curIndex, start);
						num3 = 0;
						num2 = 0;
					}
				}
			}
			Log.Warning(pawn + " pathing from " + start + " to " + dest + " hit search limit of " + 160000 + " cells.");
			DebugDrawRichData();
			PfProfilerEndSample();
			return PawnPath.NotFound;
		}

		public static int GetBuildingCost(Building b, TraverseParms traverseParms, Pawn pawn)
		{
			Building_Door building_Door = b as Building_Door;
			if (building_Door != null)
			{
				switch (traverseParms.mode)
				{
				case TraverseMode.NoPassClosedDoors:
				case TraverseMode.NoPassClosedDoorsOrWater:
					if (building_Door.FreePassage)
					{
						return 0;
					}
					return 2147483647;
				case TraverseMode.PassAllDestroyableThings:
				case TraverseMode.PassAllDestroyableThingsNotWater:
					if (pawn != null && building_Door.PawnCanOpen(pawn) && !building_Door.IsForbiddenToPass(pawn) && !building_Door.FreePassage)
					{
						return building_Door.TicksToOpenNow;
					}
					if ((pawn != null && building_Door.CanPhysicallyPass(pawn)) || building_Door.FreePassage)
					{
						return 0;
					}
					return 50 + (int)((float)building_Door.HitPoints * 0.2f);
				case TraverseMode.PassDoors:
					if (pawn != null && building_Door.PawnCanOpen(pawn) && !building_Door.IsForbiddenToPass(pawn) && !building_Door.FreePassage)
					{
						return building_Door.TicksToOpenNow;
					}
					if ((pawn != null && building_Door.CanPhysicallyPass(pawn)) || building_Door.FreePassage)
					{
						return 0;
					}
					return 150;
				case TraverseMode.ByPawn:
					if (!traverseParms.canBash && building_Door.IsForbiddenToPass(pawn))
					{
						if (DebugViewSettings.drawPaths)
						{
							DebugFlash(b.Position, b.Map, 0.77f, "forbid");
						}
						return 2147483647;
					}
					if (building_Door.PawnCanOpen(pawn) && !building_Door.FreePassage)
					{
						return building_Door.TicksToOpenNow;
					}
					if (building_Door.CanPhysicallyPass(pawn))
					{
						return 0;
					}
					if (traverseParms.canBash)
					{
						return 300;
					}
					if (DebugViewSettings.drawPaths)
					{
						DebugFlash(b.Position, b.Map, 0.34f, "cant pass");
					}
					return 2147483647;
				}
			}
			else if (pawn != null)
			{
				return b.PathFindCostFor(pawn);
			}
			return 0;
		}

		public static int GetBlueprintCost(Blueprint b, Pawn pawn)
		{
			if (pawn != null)
			{
				return b.PathFindCostFor(pawn);
			}
			return 0;
		}

		public static bool IsDestroyable(Thing th)
		{
			return th.def.useHitPoints && th.def.destroyable;
		}

		private bool BlocksDiagonalMovement(int x, int z)
		{
			return BlocksDiagonalMovement(x, z, map);
		}

		private bool BlocksDiagonalMovement(int index)
		{
			return BlocksDiagonalMovement(index, map);
		}

		public static bool BlocksDiagonalMovement(int x, int z, Map map)
		{
			return BlocksDiagonalMovement(map.cellIndices.CellToIndex(x, z), map);
		}

		public static bool BlocksDiagonalMovement(int index, Map map)
		{
			if (!map.pathGrid.WalkableFast(index))
			{
				return true;
			}
			if (map.edificeGrid[index] is Building_Door)
			{
				return true;
			}
			return false;
		}

		private void DebugFlash(IntVec3 c, float colorPct, string str)
		{
			DebugFlash(c, map, colorPct, str);
		}

		private static void DebugFlash(IntVec3 c, Map map, float colorPct, string str)
		{
			map.debugDrawer.FlashCell(c, colorPct, str);
		}

		private PawnPath FinalizedPath(int finalIndex, bool usedRegionHeuristics)
		{
			PawnPath emptyPawnPath = map.pawnPathPool.GetEmptyPawnPath();
			int num = finalIndex;
			while (true)
			{
				PathFinderNodeFast pathFinderNodeFast = calcGrid[num];
				int parentIndex = pathFinderNodeFast.parentIndex;
				emptyPawnPath.AddNode(map.cellIndices.IndexToCell(num));
				if (num == parentIndex)
				{
					break;
				}
				num = parentIndex;
			}
			emptyPawnPath.SetupFound((float)calcGrid[finalIndex].knownCost, usedRegionHeuristics);
			return emptyPawnPath;
		}

		private void InitStatusesAndPushStartNode(ref int curIndex, IntVec3 start)
		{
			statusOpenValue += 2;
			statusClosedValue += 2;
			if (statusClosedValue >= 65435)
			{
				ResetStatuses();
			}
			curIndex = cellIndices.CellToIndex(start);
			calcGrid[curIndex].knownCost = 0;
			calcGrid[curIndex].heuristicCost = 0;
			calcGrid[curIndex].costNodeCost = 0;
			calcGrid[curIndex].parentIndex = curIndex;
			calcGrid[curIndex].status = statusOpenValue;
			openList.Clear();
			openList.Push(new CostNode(curIndex, 0));
		}

		private void ResetStatuses()
		{
			int num = calcGrid.Length;
			for (int i = 0; i < num; i++)
			{
				calcGrid[i].status = 0;
			}
			statusOpenValue = 1;
			statusClosedValue = 2;
		}

		[Conditional("PFPROFILE")]
		private void PfProfilerBeginSample(string s)
		{
		}

		[Conditional("PFPROFILE")]
		private void PfProfilerEndSample()
		{
		}

		private void DebugDrawRichData()
		{
			if (DebugViewSettings.drawPaths)
			{
				while (openList.Count > 0)
				{
					CostNode costNode = openList.Pop();
					int index = costNode.index;
					IntVec3 c = new IntVec3(index % mapSizeX, 0, index / mapSizeX);
					map.debugDrawer.FlashCell(c, 0f, "open");
				}
			}
		}

		private float DetermineHeuristicStrength(Pawn pawn, IntVec3 start, LocalTargetInfo dest)
		{
			if (pawn != null && pawn.RaceProps.Animal)
			{
				return 1.75f;
			}
			float lengthHorizontal = (start - dest.Cell).LengthHorizontal;
			return (float)Mathf.RoundToInt(NonRegionBasedHeuristicStrengthHuman_DistanceCurve.Evaluate(lengthHorizontal));
		}

		private CellRect CalculateDestinationRect(LocalTargetInfo dest, PathEndMode peMode)
		{
			CellRect result = (dest.HasThing && peMode != PathEndMode.OnCell) ? dest.Thing.OccupiedRect() : CellRect.SingleCell(dest.Cell);
			if (peMode == PathEndMode.Touch)
			{
				result = result.ExpandedBy(1);
			}
			return result;
		}

		private Area GetAllowedArea(Pawn pawn)
		{
			if (pawn != null && pawn.playerSettings != null && !pawn.Drafted && ForbidUtility.CaresAboutForbidden(pawn, cellTarget: true))
			{
				Area area = pawn.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap;
				if (area != null && area.TrueCount <= 0)
				{
					area = null;
				}
				return area;
			}
			return null;
		}

		private void CalculateAndAddDisallowedCorners(TraverseParms traverseParms, PathEndMode peMode, CellRect destinationRect)
		{
			disallowedCornerIndices.Clear();
			if (peMode == PathEndMode.Touch)
			{
				int minX = destinationRect.minX;
				int minZ = destinationRect.minZ;
				int maxX = destinationRect.maxX;
				int maxZ = destinationRect.maxZ;
				if (!IsCornerTouchAllowed(minX + 1, minZ + 1, minX + 1, minZ, minX, minZ + 1))
				{
					disallowedCornerIndices.Add(map.cellIndices.CellToIndex(minX, minZ));
				}
				if (!IsCornerTouchAllowed(minX + 1, maxZ - 1, minX + 1, maxZ, minX, maxZ - 1))
				{
					disallowedCornerIndices.Add(map.cellIndices.CellToIndex(minX, maxZ));
				}
				if (!IsCornerTouchAllowed(maxX - 1, maxZ - 1, maxX - 1, maxZ, maxX, maxZ - 1))
				{
					disallowedCornerIndices.Add(map.cellIndices.CellToIndex(maxX, maxZ));
				}
				if (!IsCornerTouchAllowed(maxX - 1, minZ + 1, maxX - 1, minZ, maxX, minZ + 1))
				{
					disallowedCornerIndices.Add(map.cellIndices.CellToIndex(maxX, minZ));
				}
			}
		}

		private bool IsCornerTouchAllowed(int cornerX, int cornerZ, int adjCardinal1X, int adjCardinal1Z, int adjCardinal2X, int adjCardinal2Z)
		{
			return TouchPathEndModeUtility.IsCornerTouchAllowed(cornerX, cornerZ, adjCardinal1X, adjCardinal1Z, adjCardinal2X, adjCardinal2Z, map);
		}
	}
}
