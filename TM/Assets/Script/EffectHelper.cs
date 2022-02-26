using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectHelper
{
    public static void printEffect(string effectName, Vector3 loc, Vector3 rotation)
    {
        Vector3 temp = loc;
        temp.z -= 0.1f;
        GameObject.Instantiate(
                Resources.Load("Effect/" + effectName, typeof(GameObject)) as GameObject,
                temp, Quaternion.Euler(rotation));
    }
}