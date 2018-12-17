using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public BodyPartDef bodyPartDef;

    public float hp;

    public float enegy;
    public float maxEnegy;

    public float coverage;

    public BodyPart parent;

    public bool IsDestroyed => hp == 0;







    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
