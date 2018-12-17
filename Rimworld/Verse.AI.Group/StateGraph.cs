using System.Collections.Generic;
using System.Linq;

namespace Verse.AI.Group
{
	public class StateGraph
	{
		public List<LordToil> lordToils = new List<LordToil>();

		public List<Transition> transitions = new List<Transition>();

		private static HashSet<LordToil> checkedToils;

		public LordToil StartingToil
		{
			get
			{
				return lordToils[0];
			}
			set
			{
				if (lordToils.Contains(value))
				{
					lordToils.Remove(value);
				}
				lordToils.Insert(0, value);
			}
		}

		public void AddToil(LordToil toil)
		{
			lordToils.Add(toil);
		}

		public void AddTransition(Transition transition, bool highPriority = false)
		{
			if (highPriority)
			{
				transitions.Insert(0, transition);
			}
			else
			{
				transitions.Add(transition);
			}
		}

		public StateGraph AttachSubgraph(StateGraph subGraph)
		{
			for (int i = 0; i < subGraph.lordToils.Count; i++)
			{
				lordToils.Add(subGraph.lordToils[i]);
			}
			for (int j = 0; j < subGraph.transitions.Count; j++)
			{
				transitions.Add(subGraph.transitions[j]);
			}
			return subGraph;
		}

		public void ErrorCheck()
		{
			if (lordToils.Count == 0)
			{
				Log.Error("Graph has 0 lord toils.");
			}
			foreach (LordToil item in lordToils.Distinct())
			{
				int num = (from s in lordToils
				where s == item
				select s).Count();
				if (num != 1)
				{
					Log.Error("Graph has lord toil " + item + " registered " + num + " times.");
				}
			}
			foreach (Transition transition in transitions)
			{
				int num2 = (from t in transitions
				where t == transition
				select t).Count();
				if (num2 != 1)
				{
					Log.Error("Graph has transition " + transition + " registered " + num2 + " times.");
				}
			}
			checkedToils = new HashSet<LordToil>();
			CheckForUnregisteredLinkedToilsRecursive(StartingToil);
			checkedToils = null;
		}

		private void CheckForUnregisteredLinkedToilsRecursive(LordToil toil)
		{
			if (!lordToils.Contains(toil))
			{
				Log.Error("Unregistered linked lord toil: " + toil);
			}
			checkedToils.Add(toil);
			for (int i = 0; i < transitions.Count; i++)
			{
				Transition transition = transitions[i];
				if (transition.sources.Contains(toil) && !checkedToils.Contains(toil))
				{
					CheckForUnregisteredLinkedToilsRecursive(transition.target);
				}
			}
		}
	}
}
