using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class InteractionWorker_Breakup : InteractionWorker
	{
		private const float BaseChance = 0.02f;

		private const float SpouseRelationChanceFactor = 0.4f;

		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			if (!LovePartnerRelationUtility.LovePartnerRelationExists(initiator, recipient))
			{
				return 0f;
			}
			float num = Mathf.InverseLerp(100f, -100f, (float)initiator.relations.OpinionOf(recipient));
			float num2 = 1f;
			if (initiator.relations.DirectRelationExists(PawnRelationDefOf.Spouse, recipient))
			{
				num2 = 0.4f;
			}
			return 0.02f * num * num2;
		}

		public Thought RandomBreakupReason(Pawn initiator, Pawn recipient)
		{
			List<Thought_Memory> list = (from m in initiator.needs.mood.thoughts.memories.Memories
			where m != null && m.otherPawn == recipient && m.CurStage != null && m.CurStage.baseOpinionOffset < 0f
			select m).ToList();
			if (list.Count == 0)
			{
				return null;
			}
			float worstMemoryOpinionOffset = list.Max((Thought_Memory m) => 0f - m.CurStage.baseOpinionOffset);
			Thought_Memory result = null;
			(from m in list
			where 0f - m.CurStage.baseOpinionOffset >= worstMemoryOpinionOffset / 2f
			select m).TryRandomElementByWeight((Thought_Memory m) => 0f - m.CurStage.baseOpinionOffset, out result);
			return result;
		}

		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef)
		{
			Thought thought = RandomBreakupReason(initiator, recipient);
			if (initiator.relations.DirectRelationExists(PawnRelationDefOf.Spouse, recipient))
			{
				initiator.relations.RemoveDirectRelation(PawnRelationDefOf.Spouse, recipient);
				initiator.relations.AddDirectRelation(PawnRelationDefOf.ExSpouse, recipient);
				recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.DivorcedMe, initiator);
				initiator.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.GotMarried);
				recipient.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.GotMarried);
				initiator.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.HoneymoonPhase, recipient);
				recipient.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.HoneymoonPhase, initiator);
			}
			else
			{
				initiator.relations.TryRemoveDirectRelation(PawnRelationDefOf.Lover, recipient);
				initiator.relations.TryRemoveDirectRelation(PawnRelationDefOf.Fiance, recipient);
				initiator.relations.AddDirectRelation(PawnRelationDefOf.ExLover, recipient);
				recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.BrokeUpWithMe, initiator);
			}
			if (initiator.ownership.OwnedBed != null && initiator.ownership.OwnedBed == recipient.ownership.OwnedBed)
			{
				Pawn pawn = (!(Rand.Value < 0.5f)) ? recipient : initiator;
				pawn.ownership.UnclaimBed();
			}
			TaleRecorder.RecordTale(TaleDefOf.Breakup, initiator, recipient);
			if (PawnUtility.ShouldSendNotificationAbout(initiator) || PawnUtility.ShouldSendNotificationAbout(recipient))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("LetterNoLongerLovers".Translate(initiator.LabelShort, recipient.LabelShort, initiator.Named("PAWN1"), recipient.Named("PAWN2")));
				if (thought != null)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine("FinalStraw".Translate(thought.CurStage.label.CapitalizeFirst()));
				}
				letterLabel = "LetterLabelBreakup".Translate();
				letterText = stringBuilder.ToString().TrimEndNewlines();
				letterDef = LetterDefOf.NegativeEvent;
			}
			else
			{
				letterLabel = null;
				letterText = null;
				letterDef = null;
			}
		}
	}
}
