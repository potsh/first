using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjurySet : MonoBehaviour
{

    public Character character;

    public List<Injury> Injuries = new List<Injury>();

    public float PainTotal
    {
        get
        {
            float totalPain = 0;
            foreach(var injury in Injuries)
            {
                totalPain += injury.Pain;
            }

            return totalPain;
        }
    }

    public float BleedRageTotal
    {
        get
        {
            float totalBleedRate = 0;
            foreach (var injury in Injuries)
            {
                totalBleedRate += injury.BleedRate;
            }

            return totalBleedRate;
        }
    }


}
