using RimWorld;
using System.Linq;
using UnityEngine;

namespace Verse
{
	public class DamageWorker_AddInjury : DamageWorker
	{
		public override DamageResult Apply(DamageInfo dinfo, Thing thing)
		{
			Pawn pawn = thing as Pawn;
			if (pawn == null)
			{
				return base.Apply(dinfo, thing);
			}
			return ApplyToPawn(dinfo, pawn);
		}

		private DamageResult ApplyToPawn(DamageInfo dinfo, Pawn pawn)
		{
			DamageResult damageResult = new DamageResult();
			if (dinfo.Amount <= 0f)
			{
				return damageResult;
			}
			if (!DebugSettings.enablePlayerDamage && pawn.Faction == Faction.OfPlayer)
			{
				return damageResult;
			}
			Map mapHeld = pawn.MapHeld;
			bool spawnedOrAnyParentSpawned = pawn.SpawnedOrAnyParentSpawned;
			if (dinfo.AllowDamagePropagation && dinfo.Amount >= (float)dinfo.Def.minDamageToFragment)
			{
				int num = Rand.RangeInclusive(2, 4);
				for (int i = 0; i < num; i++)
				{
					DamageInfo dinfo2 = dinfo;
					dinfo2.SetAmount(dinfo.Amount / (float)num);
					ApplyDamageToPart(dinfo2, pawn, damageResult);
				}
			}
			else
			{
				ApplyDamageToPart(dinfo, pawn, damageResult);
				ApplySmallPawnDamagePropagation(dinfo, pawn, damageResult);
			}
			if (damageResult.wounded)
			{
				PlayWoundedVoiceSound(dinfo, pawn);
				pawn.Drawer.Notify_DamageApplied(dinfo);
			}
			if (damageResult.headshot && pawn.Spawned)
			{
				IntVec3 position = pawn.Position;
				float x = (float)position.x + 1f;
				IntVec3 position2 = pawn.Position;
				float y = (float)position2.y;
				IntVec3 position3 = pawn.Position;
				MoteMaker.ThrowText(new Vector3(x, y, (float)position3.z + 1f), pawn.Map, "Headshot".Translate(), Color.white);
				if (dinfo.Instigator != null)
				{
					(dinfo.Instigator as Pawn)?.records.Increment(RecordDefOf.Headshots);
				}
			}
			if ((damageResult.deflected || damageResult.diminished) && spawnedOrAnyParentSpawned)
			{
				EffecterDef effecterDef = damageResult.deflected ? ((damageResult.deflectedByMetalArmor && dinfo.Def.canUseDeflectMetalEffect) ? ((dinfo.Def != DamageDefOf.Bullet) ? EffecterDefOf.Deflect_Metal : EffecterDefOf.Deflect_Metal_Bullet) : ((dinfo.Def != DamageDefOf.Bullet) ? EffecterDefOf.Deflect_General : EffecterDefOf.Deflect_General_Bullet)) : ((!damageResult.diminishedByMetalArmor) ? EffecterDefOf.DamageDiminished_General : EffecterDefOf.DamageDiminished_Metal);
				if (pawn.health.deflectionEffecter == null || pawn.health.deflectionEffecter.def != effecterDef)
				{
					if (pawn.health.deflectionEffecter != null)
					{
						pawn.health.deflectionEffecter.Cleanup();
						pawn.health.deflectionEffecter = null;
					}
					pawn.health.deflectionEffecter = effecterDef.Spawn();
				}
				pawn.health.deflectionEffecter.Trigger(pawn, dinfo.Instigator ?? pawn);
				if (damageResult.deflected)
				{
					pawn.Drawer.Notify_DamageDeflected(dinfo);
				}
			}
			if (!damageResult.deflected && spawnedOrAnyParentSpawned)
			{
				ImpactSoundUtility.PlayImpactSound(pawn, dinfo.Def.impactSoundType, mapHeld);
			}
			return damageResult;
		}

		private void CheckApplySpreadDamage(DamageInfo dinfo, Thing t)
		{
			if ((dinfo.Def != DamageDefOf.Flame || t.FlammableNow) && Rand.Chance(0.5f))
			{
				dinfo.SetAmount((float)Mathf.CeilToInt(dinfo.Amount * Rand.Range(0.35f, 0.7f)));
				t.TakeDamage(dinfo);
			}
		}

		private void ApplySmallPawnDamagePropagation(DamageInfo dinfo, Pawn pawn, DamageResult result)
		{
			if (dinfo.AllowDamagePropagation && result.LastHitPart != null && dinfo.Def.harmsHealth && result.LastHitPart != pawn.RaceProps.body.corePart && result.LastHitPart.parent != null && pawn.health.hediffSet.GetPartHealth(result.LastHitPart.parent) > 0f && result.LastHitPart.parent.coverageAbs > 0f && dinfo.Amount >= 10f && pawn.HealthScale <= 0.5001f)
			{
				DamageInfo dinfo2 = dinfo;
				dinfo2.SetHitPart(result.LastHitPart.parent);
				ApplyDamageToPart(dinfo2, pawn, result);
			}
		}

		private void ApplyDamageToPart(DamageInfo dinfo, Pawn pawn, DamageResult result)
		{
			BodyPartRecord exactPartFromDamageInfo = GetExactPartFromDamageInfo(dinfo, pawn);
			if (exactPartFromDamageInfo != null)
			{
				dinfo.SetHitPart(exactPartFromDamageInfo);
				float num = dinfo.Amount;
				bool flag = !dinfo.InstantPermanentInjury;
				bool deflectedByMetalArmor = false;
				if (flag)
				{
					DamageDef damageDef = dinfo.Def;
					num = ArmorUtility.GetPostArmorDamage(pawn, num, dinfo.ArmorPenetrationInt, dinfo.HitPart, ref damageDef, out deflectedByMetalArmor, out bool diminishedByMetalArmor);
					dinfo.Def = damageDef;
					if (num < dinfo.Amount)
					{
						result.diminished = true;
						result.diminishedByMetalArmor = diminishedByMetalArmor;
					}
				}
				if (num <= 0f)
				{
					result.AddPart(pawn, dinfo.HitPart);
					result.deflected = true;
					result.deflectedByMetalArmor = deflectedByMetalArmor;
				}
				else
				{
					if (IsHeadshot(dinfo, pawn))
					{
						result.headshot = true;
					}
					if (dinfo.InstantPermanentInjury)
					{
						HediffDef hediffDefFromDamage = HealthUtility.GetHediffDefFromDamage(dinfo.Def, pawn, dinfo.HitPart);
						if (hediffDefFromDamage.CompPropsFor(typeof(HediffComp_GetsPermanent)) == null || dinfo.HitPart.def.permanentInjuryChanceFactor == 0f || pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(dinfo.HitPart))
						{
							return;
						}
					}
					if (!dinfo.AllowDamagePropagation)
					{
						FinalizeAndAddInjury(pawn, num, dinfo, result);
					}
					else
					{
						ApplySpecialEffectsToPart(pawn, num, dinfo, result);
					}
				}
			}
		}

		protected virtual void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageResult result)
		{
			totalDamage = ReduceDamageToPreserveOutsideParts(totalDamage, dinfo, pawn);
			FinalizeAndAddInjury(pawn, totalDamage, dinfo, result);
			CheckDuplicateDamageToOuterParts(dinfo, pawn, totalDamage, result);
		}

		protected float FinalizeAndAddInjury(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageResult result)
		{
			if (pawn.health.hediffSet.PartIsMissing(dinfo.HitPart))
			{
				return 0f;
			}
			HediffDef hediffDefFromDamage = HealthUtility.GetHediffDefFromDamage(dinfo.Def, pawn, dinfo.HitPart);
			Hediff_Injury hediff_Injury = (Hediff_Injury)HediffMaker.MakeHediff(hediffDefFromDamage, pawn);
			hediff_Injury.Part = dinfo.HitPart;
			hediff_Injury.source = dinfo.Weapon;
			hediff_Injury.sourceBodyPartGroup = dinfo.WeaponBodyPartGroup;
			hediff_Injury.sourceHediffDef = dinfo.WeaponLinkedHediff;
			hediff_Injury.Severity = totalDamage;
			if (dinfo.InstantPermanentInjury)
			{
				HediffComp_GetsPermanent hediffComp_GetsPermanent = hediff_Injury.TryGetComp<HediffComp_GetsPermanent>();
				if (hediffComp_GetsPermanent != null)
				{
					hediffComp_GetsPermanent.IsPermanent = true;
				}
				else
				{
					Log.Error("Tried to create instant permanent injury on Hediff without a GetsPermanent comp: " + hediffDefFromDamage + " on " + pawn);
				}
			}
			return FinalizeAndAddInjury(pawn, hediff_Injury, dinfo, result);
		}

		protected float FinalizeAndAddInjury(Pawn pawn, Hediff_Injury injury, DamageInfo dinfo, DamageResult result)
		{
			injury.TryGetComp<HediffComp_GetsPermanent>()?.PreFinalizeInjury();
			pawn.health.AddHediff(injury, null, dinfo, result);
			float num = Mathf.Min(injury.Severity, pawn.health.hediffSet.GetPartHealth(injury.Part));
			result.totalDamageDealt += num;
			result.wounded = true;
			result.AddPart(pawn, injury.Part);
			result.AddHediff(injury);
			return num;
		}

		private void CheckDuplicateDamageToOuterParts(DamageInfo dinfo, Pawn pawn, float totalDamage, DamageResult result)
		{
			if (dinfo.AllowDamagePropagation && dinfo.Def.harmAllLayersUntilOutside && dinfo.HitPart.depth == BodyPartDepth.Inside)
			{
				BodyPartRecord parent = dinfo.HitPart.parent;
				do
				{
					if (pawn.health.hediffSet.GetPartHealth(parent) != 0f && parent.coverageAbs > 0f)
					{
						HediffDef hediffDefFromDamage = HealthUtility.GetHediffDefFromDamage(dinfo.Def, pawn, parent);
						Hediff_Injury hediff_Injury = (Hediff_Injury)HediffMaker.MakeHediff(hediffDefFromDamage, pawn);
						hediff_Injury.Part = parent;
						hediff_Injury.source = dinfo.Weapon;
						hediff_Injury.sourceBodyPartGroup = dinfo.WeaponBodyPartGroup;
						hediff_Injury.Severity = totalDamage;
						if (hediff_Injury.Severity <= 0f)
						{
							hediff_Injury.Severity = 1f;
						}
						FinalizeAndAddInjury(pawn, hediff_Injury, dinfo, result);
					}
					if (parent.depth == BodyPartDepth.Outside)
					{
						break;
					}
					parent = parent.parent;
				}
				while (parent != null);
			}
		}

		private static bool IsHeadshot(DamageInfo dinfo, Pawn pawn)
		{
			if (dinfo.InstantPermanentInjury)
			{
				return false;
			}
			return dinfo.HitPart.groups.Contains(BodyPartGroupDefOf.FullHead) && dinfo.Def == DamageDefOf.Bullet;
		}

		private BodyPartRecord GetExactPartFromDamageInfo(DamageInfo dinfo, Pawn pawn)
		{
			if (dinfo.HitPart != null)
			{
				return (!pawn.health.hediffSet.GetNotMissingParts().Any((BodyPartRecord x) => x == dinfo.HitPart)) ? null : dinfo.HitPart;
			}
			BodyPartRecord bodyPartRecord = ChooseHitPart(dinfo, pawn);
			if (bodyPartRecord == null)
			{
				Log.Warning("ChooseHitPart returned null (any part).");
			}
			return bodyPartRecord;
		}

		protected virtual BodyPartRecord ChooseHitPart(DamageInfo dinfo, Pawn pawn)
		{
			return pawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, dinfo.Depth);
		}

		private static void PlayWoundedVoiceSound(DamageInfo dinfo, Pawn pawn)
		{
			if (!pawn.Dead && !dinfo.InstantPermanentInjury && pawn.SpawnedOrAnyParentSpawned && dinfo.Def.ExternalViolenceFor(pawn))
			{
				LifeStageUtility.PlayNearestLifestageSound(pawn, (LifeStageAge ls) => ls.soundWounded);
			}
		}

		protected float ReduceDamageToPreserveOutsideParts(float postArmorDamage, DamageInfo dinfo, Pawn pawn)
		{
			if (!ShouldReduceDamageToPreservePart(dinfo.HitPart))
			{
				return postArmorDamage;
			}
			float partHealth = pawn.health.hediffSet.GetPartHealth(dinfo.HitPart);
			if (postArmorDamage < partHealth)
			{
				return postArmorDamage;
			}
			float maxHealth = dinfo.HitPart.def.GetMaxHealth(pawn);
			float num = postArmorDamage - partHealth;
			float f = num / maxHealth;
			float chance = def.overkillPctToDestroyPart.InverseLerpThroughRange(f);
			if (!Rand.Chance(chance))
			{
				return postArmorDamage = partHealth - 1f;
			}
			return postArmorDamage;
		}

		public static bool ShouldReduceDamageToPreservePart(BodyPartRecord bodyPart)
		{
			return bodyPart.depth == BodyPartDepth.Outside && !bodyPart.IsCorePart;
		}
	}
}
