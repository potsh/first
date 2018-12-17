using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobGiver_Work : ThinkNode
	{
		public bool emergency;

		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_Work jobGiver_Work = (JobGiver_Work)base.DeepCopy(resolve);
			jobGiver_Work.emergency = emergency;
			return jobGiver_Work;
		}

		public override float GetPriority(Pawn pawn)
		{
			if (pawn.workSettings == null || !pawn.workSettings.EverWork)
			{
				return 0f;
			}
			TimeAssignmentDef timeAssignmentDef = (pawn.timetable != null) ? pawn.timetable.CurrentAssignment : TimeAssignmentDefOf.Anything;
			if (timeAssignmentDef == TimeAssignmentDefOf.Anything)
			{
				return 5.5f;
			}
			if (timeAssignmentDef == TimeAssignmentDefOf.Work)
			{
				return 9f;
			}
			if (timeAssignmentDef == TimeAssignmentDefOf.Sleep)
			{
				return 3f;
			}
			if (timeAssignmentDef == TimeAssignmentDefOf.Joy)
			{
				return 2f;
			}
			throw new NotImplementedException();
		}

		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			if (emergency && pawn.mindState.priorityWork.IsPrioritized)
			{
				List<WorkGiverDef> workGiversByPriority = pawn.mindState.priorityWork.WorkType.workGiversByPriority;
				for (int i = 0; i < workGiversByPriority.Count; i++)
				{
					WorkGiver worker = workGiversByPriority[i].Worker;
					Job job = GiverTryGiveJobPrioritized(pawn, worker, pawn.mindState.priorityWork.Cell);
					if (job != null)
					{
						job.playerForced = true;
						return new ThinkResult(job, this, workGiversByPriority[i].tagToGive);
					}
				}
				pawn.mindState.priorityWork.Clear();
			}
			List<WorkGiver> list = emergency ? pawn.workSettings.WorkGiversInOrderEmergency : pawn.workSettings.WorkGiversInOrderNormal;
			int num = -999;
			TargetInfo targetInfo = TargetInfo.Invalid;
			WorkGiver_Scanner workGiver_Scanner = null;
			WorkGiver_Scanner scanner;
			for (int j = 0; j < list.Count; j++)
			{
				WorkGiver workGiver = list[j];
				if (workGiver.def.priorityInType != num && targetInfo.IsValid)
				{
					break;
				}
				if (PawnCanUseWorkGiver(pawn, workGiver))
				{
					try
					{
						Job job2 = workGiver.NonScanJob(pawn);
						if (job2 != null)
						{
							return new ThinkResult(job2, this, list[j].def.tagToGive);
						}
						scanner = (workGiver as WorkGiver_Scanner);
						if (scanner != null)
						{
							if (scanner.def.scanThings)
							{
								Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
								IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
								Thing thing;
								if (scanner.Prioritized)
								{
									IEnumerable<Thing> enumerable2 = enumerable;
									if (enumerable2 == null)
									{
										enumerable2 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
									}
									if (scanner.AllowUnreachable)
									{
										IntVec3 position = pawn.Position;
										IEnumerable<Thing> searchSet = enumerable2;
										Predicate<Thing> validator = predicate;
										thing = GenClosest.ClosestThing_Global(position, searchSet, 99999f, validator, (Thing x) => scanner.GetPriority(pawn, x));
									}
									else
									{
										IntVec3 position = pawn.Position;
										Map map = pawn.Map;
										IEnumerable<Thing> searchSet = enumerable2;
										PathEndMode pathEndMode = scanner.PathEndMode;
										TraverseParms traverseParams = TraverseParms.For(pawn, scanner.MaxPathDanger(pawn));
										Predicate<Thing> validator = predicate;
										thing = GenClosest.ClosestThing_Global_Reachable(position, map, searchSet, pathEndMode, traverseParams, 9999f, validator, (Thing x) => scanner.GetPriority(pawn, x));
									}
								}
								else if (scanner.AllowUnreachable)
								{
									IEnumerable<Thing> enumerable3 = enumerable;
									if (enumerable3 == null)
									{
										enumerable3 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
									}
									IntVec3 position = pawn.Position;
									IEnumerable<Thing> searchSet = enumerable3;
									Predicate<Thing> validator = predicate;
									thing = GenClosest.ClosestThing_Global(position, searchSet, 99999f, validator);
								}
								else
								{
									IntVec3 position = pawn.Position;
									Map map = pawn.Map;
									ThingRequest potentialWorkThingRequest = scanner.PotentialWorkThingRequest;
									PathEndMode pathEndMode = scanner.PathEndMode;
									TraverseParms traverseParams = TraverseParms.For(pawn, scanner.MaxPathDanger(pawn));
									Predicate<Thing> validator = predicate;
									bool forceGlobalSearch = enumerable != null;
									thing = GenClosest.ClosestThingReachable(position, map, potentialWorkThingRequest, pathEndMode, traverseParams, 9999f, validator, enumerable, 0, scanner.LocalRegionsToScanFirst, forceGlobalSearch);
								}
								if (thing != null)
								{
									targetInfo = thing;
									workGiver_Scanner = scanner;
								}
							}
							if (scanner.def.scanCells)
							{
								IntVec3 position2 = pawn.Position;
								float num2 = 99999f;
								float num3 = -3.40282347E+38f;
								bool prioritized = scanner.Prioritized;
								bool allowUnreachable = scanner.AllowUnreachable;
								Danger maxDanger = scanner.MaxPathDanger(pawn);
								foreach (IntVec3 item in scanner.PotentialWorkCellsGlobal(pawn))
								{
									bool flag = false;
									float num4 = (float)(item - position2).LengthHorizontalSquared;
									float num5 = 0f;
									if (prioritized)
									{
										if (!item.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, item))
										{
											if (!allowUnreachable && !pawn.CanReach(item, scanner.PathEndMode, maxDanger))
											{
												continue;
											}
											num5 = scanner.GetPriority(pawn, item);
											if (num5 > num3 || (num5 == num3 && num4 < num2))
											{
												flag = true;
											}
										}
									}
									else if (num4 < num2 && !item.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, item))
									{
										if (!allowUnreachable && !pawn.CanReach(item, scanner.PathEndMode, maxDanger))
										{
											continue;
										}
										flag = true;
									}
									if (flag)
									{
										targetInfo = new TargetInfo(item, pawn.Map);
										workGiver_Scanner = scanner;
										num2 = num4;
										num3 = num5;
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						Log.Error(pawn + " threw exception in WorkGiver " + workGiver.def.defName + ": " + ex.ToString());
					}
					finally
					{
					}
					if (targetInfo.IsValid)
					{
						pawn.mindState.lastGivenWorkType = workGiver.def.workType;
						Job job3 = (!targetInfo.HasThing) ? workGiver_Scanner.JobOnCell(pawn, targetInfo.Cell) : workGiver_Scanner.JobOnThing(pawn, targetInfo.Thing);
						if (job3 != null)
						{
							return new ThinkResult(job3, this, list[j].def.tagToGive);
						}
						Log.ErrorOnce(workGiver_Scanner + " provided target " + targetInfo + " but yielded no actual job for pawn " + pawn + ". The CanGiveJob and JobOnX methods may not be synchronized.", 6112651);
					}
					num = workGiver.def.priorityInType;
				}
			}
			return ThinkResult.NoJob;
		}

		private bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
		{
			if (!giver.def.nonColonistsCanDo && !pawn.IsColonist)
			{
				return false;
			}
			if (pawn.story != null && pawn.story.WorkTagIsDisabled(giver.def.workTags))
			{
				return false;
			}
			if (giver.ShouldSkip(pawn))
			{
				return false;
			}
			if (giver.MissingRequiredCapacity(pawn) != null)
			{
				return false;
			}
			return true;
		}

		private Job GiverTryGiveJobPrioritized(Pawn pawn, WorkGiver giver, IntVec3 cell)
		{
			if (!PawnCanUseWorkGiver(pawn, giver))
			{
				return null;
			}
			try
			{
				Job job = giver.NonScanJob(pawn);
				if (job != null)
				{
					return job;
				}
				WorkGiver_Scanner scanner = giver as WorkGiver_Scanner;
				if (scanner != null)
				{
					if (giver.def.scanThings)
					{
						Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
						List<Thing> thingList = cell.GetThingList(pawn.Map);
						for (int i = 0; i < thingList.Count; i++)
						{
							Thing thing = thingList[i];
							if (scanner.PotentialWorkThingRequest.Accepts(thing) && predicate(thing))
							{
								pawn.mindState.lastGivenWorkType = giver.def.workType;
								return scanner.JobOnThing(pawn, thing);
							}
						}
					}
					if (giver.def.scanCells && !cell.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, cell))
					{
						pawn.mindState.lastGivenWorkType = giver.def.workType;
						return scanner.JobOnCell(pawn, cell);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(pawn + " threw exception in GiverTryGiveJobTargeted on WorkGiver " + giver.def.defName + ": " + ex.ToString());
			}
			return null;
		}
	}
}
