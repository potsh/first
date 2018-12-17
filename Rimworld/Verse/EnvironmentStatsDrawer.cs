using RimWorld;
using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
	public static class EnvironmentStatsDrawer
	{
		private const float StatLabelColumnWidth = 100f;

		private const float ScoreColumnWidth = 50f;

		private const float ScoreStageLabelColumnWidth = 160f;

		private static readonly Color RelatedStatColor = new Color(0.85f, 0.85f, 0.85f);

		private const float DistFromMouse = 26f;

		private const float WindowPadding = 18f;

		private const float LineHeight = 23f;

		private const float SpaceBetweenLines = 2f;

		private const float SpaceBetweenColumns = 35f;

		private static int DisplayedRoomStatsCount
		{
			get
			{
				int num = 0;
				List<RoomStatDef> allDefsListForReading = DefDatabase<RoomStatDef>.AllDefsListForReading;
				for (int i = 0; i < allDefsListForReading.Count; i++)
				{
					if (!allDefsListForReading[i].isHidden || DebugViewSettings.showAllRoomStats)
					{
						num++;
					}
				}
				return num;
			}
		}

		private static bool ShouldShowWindowNow()
		{
			if (!ShouldShowRoomStats() && !ShouldShowBeauty())
			{
				return false;
			}
			if (Mouse.IsInputBlockedNow)
			{
				return false;
			}
			return true;
		}

		private static bool ShouldShowRoomStats()
		{
			if (!Find.PlaySettings.showRoomStats)
			{
				return false;
			}
			if (!UI.MouseCell().InBounds(Find.CurrentMap) || UI.MouseCell().Fogged(Find.CurrentMap))
			{
				return false;
			}
			Room room = UI.MouseCell().GetRoom(Find.CurrentMap, RegionType.Set_All);
			return room != null && room.Role != RoomRoleDefOf.None;
		}

		private static bool ShouldShowBeauty()
		{
			if (!Find.PlaySettings.showBeauty)
			{
				return false;
			}
			if (!UI.MouseCell().InBounds(Find.CurrentMap) || UI.MouseCell().Fogged(Find.CurrentMap))
			{
				return false;
			}
			return UI.MouseCell().GetRoom(Find.CurrentMap) != null;
		}

		public static void EnvironmentStatsOnGUI()
		{
			if (Event.current.type == EventType.Repaint && ShouldShowWindowNow())
			{
				DrawInfoWindow();
			}
		}

		private static void DrawInfoWindow()
		{
			Text.Font = GameFont.Small;
			Vector2 mousePosition = Event.current.mousePosition;
			float x = mousePosition.x;
			Vector2 mousePosition2 = Event.current.mousePosition;
			Rect windowRect = new Rect(x, mousePosition2.y, 416f, 36f);
			bool flag = ShouldShowBeauty();
			if (flag)
			{
				windowRect.height += 25f;
			}
			if (ShouldShowRoomStats())
			{
				if (flag)
				{
					windowRect.height += 13f;
				}
				windowRect.height += 23f;
				windowRect.height += (float)DisplayedRoomStatsCount * 25f;
			}
			windowRect.x += 26f;
			windowRect.y += 26f;
			if (windowRect.xMax > (float)UI.screenWidth)
			{
				windowRect.x -= windowRect.width + 52f;
			}
			if (windowRect.yMax > (float)UI.screenHeight)
			{
				windowRect.y -= windowRect.height + 52f;
			}
			Find.WindowStack.ImmediateWindow(74975, windowRect, WindowLayer.Super, delegate
			{
				FillWindow(windowRect);
			});
		}

		private static void FillWindow(Rect windowRect)
		{
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.InspectRoomStats, KnowledgeAmount.FrameDisplayed);
			Text.Font = GameFont.Small;
			float num = 18f;
			bool flag = ShouldShowBeauty();
			if (flag)
			{
				float beauty = BeautyUtility.AverageBeautyPerceptible(UI.MouseCell(), Find.CurrentMap);
				Rect rect = new Rect(18f, num, windowRect.width - 36f, 100f);
				GUI.color = BeautyDrawer.BeautyColor(beauty, 40f);
				Widgets.Label(rect, "BeautyHere".Translate() + ": " + beauty.ToString("F1"));
				num += 25f;
			}
			if (ShouldShowRoomStats())
			{
				if (flag)
				{
					num += 5f;
					GUI.color = new Color(1f, 1f, 1f, 0.4f);
					Widgets.DrawLineHorizontal(18f, num, windowRect.width - 36f);
					GUI.color = Color.white;
					num += 8f;
				}
				Room room = UI.MouseCell().GetRoom(Find.CurrentMap, RegionType.Set_All);
				Rect rect2 = new Rect(18f, num, windowRect.width - 36f, 100f);
				GUI.color = Color.white;
				Widgets.Label(rect2, GetRoomRoleLabel(room));
				num += 25f;
				Text.WordWrap = false;
				for (int i = 0; i < DefDatabase<RoomStatDef>.AllDefsListForReading.Count; i++)
				{
					RoomStatDef roomStatDef = DefDatabase<RoomStatDef>.AllDefsListForReading[i];
					if (!roomStatDef.isHidden || DebugViewSettings.showAllRoomStats)
					{
						float stat = room.GetStat(roomStatDef);
						RoomStatScoreStage scoreStage = roomStatDef.GetScoreStage(stat);
						if (room.Role.IsStatRelated(roomStatDef))
						{
							GUI.color = RelatedStatColor;
						}
						else
						{
							GUI.color = Color.gray;
						}
						Rect rect3 = new Rect(rect2.x, num, 100f, 23f);
						Widgets.Label(rect3, roomStatDef.LabelCap);
						Rect rect4 = new Rect(rect3.xMax + 35f, num, 50f, 23f);
						string label = roomStatDef.ScoreToString(stat);
						Widgets.Label(rect4, label);
						Rect rect5 = new Rect(rect4.xMax + 35f, num, 160f, 23f);
						Widgets.Label(rect5, (scoreStage != null) ? scoreStage.label : string.Empty);
						num += 25f;
					}
				}
				Text.WordWrap = true;
			}
			GUI.color = Color.white;
		}

		public static void DrawRoomOverlays()
		{
			if (Find.PlaySettings.showBeauty && UI.MouseCell().InBounds(Find.CurrentMap))
			{
				GenUI.RenderMouseoverBracket();
			}
			if (ShouldShowWindowNow() && ShouldShowRoomStats())
			{
				Room room = UI.MouseCell().GetRoom(Find.CurrentMap, RegionType.Set_All);
				if (room != null && room.Role != RoomRoleDefOf.None)
				{
					room.DrawFieldEdges();
				}
			}
		}

		private static string GetRoomRoleLabel(Room room)
		{
			Pawn pawn = null;
			Pawn pawn2 = null;
			foreach (Pawn owner in room.Owners)
			{
				if (pawn == null)
				{
					pawn = owner;
				}
				else
				{
					pawn2 = owner;
				}
			}
			if (pawn == null)
			{
				return room.Role.LabelCap;
			}
			if (pawn2 == null)
			{
				return "SomeonesRoom".Translate(pawn.LabelShort, room.Role.label, pawn.Named("PAWN"));
			}
			return "CouplesRoom".Translate(pawn.LabelShort, pawn2.LabelShort, room.Role.label, pawn.Named("PAWN1"), pawn2.Named("PAWN2"));
		}
	}
}
