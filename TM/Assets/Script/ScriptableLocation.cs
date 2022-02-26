using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableLocation : ScriptableObject
{
    [SerializeField]
    private Vector3Int location;

    public int getX()
    {
        return this.location.x;
    }
    public int getY()
    {
        return this.location.y;
    }

    public void setLocation(int x, int y)
    {
        this.location.x = x;
        this.location.y = y;
    }
}
