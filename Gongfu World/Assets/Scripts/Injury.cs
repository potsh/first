using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Injury : MonoBehaviour
{

    public float hp;

    private float painPerHp;

    private float bleedPerHp;

    public bool canMerge;

    public BodyPart part;

    public int source; //todo

    public float Pain => hp * painPerHp;

    public float BleedRate => hp * bleedPerHp;

}
