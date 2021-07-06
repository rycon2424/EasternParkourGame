using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanoid : Actor
{
    [Header("Humanoid Stats")]
    public HorseBehaviour closestHorse;
    public int horseMountPosition = -1;
}
