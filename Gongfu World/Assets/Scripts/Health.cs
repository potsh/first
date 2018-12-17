using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health
{
    private Character character;

    private InjurySet injurySet = new InjurySet();

    private Capacities capacities = new Capacities();

    public bool Downed;

    public bool Dead;

    public bool Awake;

    public bool InPainShock => injurySet.PainTotal >= 0.8;


}
