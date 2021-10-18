using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shot
{
    public float upForce;
    public float hitForce;
}

// F TopSpin
// C Rapida

public class ShotManager : MonoBehaviour
{
    public Shot topSpin;
    public Shot flat;
    public Shot flatServe;
    public Shot kickServe;
}