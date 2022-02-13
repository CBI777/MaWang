using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionChange
{
    /// <summary>
    /// 방향인 Direction을 Vector3Int로 바꿔준다.
    /// </summary>
    /// <param name="directions"></param>
    /// <returns></returns>
    public static Vector3Int dirToVector(Directions directions)
    {
        Vector3Int temp = new Vector3Int(0, 0, 0);
        switch(directions)
        {
            case Directions.E:
                temp.x = 1;
                break;
            case Directions.N:
                temp.y = 1;
                break;
            case Directions.W:
                temp.x = -1;
                break;
            case Directions.S:
                temp.y = -1;
                break;
        }

        return temp;
    }

    /// <summary>
    /// 방향인 Directions를 그에 맞는 회전으로 바꿔준다.
    /// 기준은 동쪽 = Vector(1,0) = Directions.E
    /// 사용할때는 Quaternion.Euler()의 매개변수로 사용할 것.
    /// </summary>
    /// <param name="directions"></param>
    /// <returns></returns>
    public static Vector3 dirToRotation(Directions directions)
    {
        Vector3 temp = new Vector3(0f, 0f, 0f);
        switch (directions)
        {
            case Directions.E:
                break;
            case Directions.N:
                temp.z = 90f;
                break;
            case Directions.W:
                temp.z = 180f;
                break;
            case Directions.S:
                temp.z = -90f;
                break;
        }

        return temp;
    }

    /// <summary>
    /// 방향인 Directions를 그에 맞는 회전으로 바꿔준다.
    /// 기준은 Vector3Int(1, 0, 0) = 동쪽.
    /// 사용할때는 Quaternion.Euler()의 매개변수로 사용할 것.
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector3 dirToRotation(Vector3Int vec)
    {
        Vector3 temp = new Vector3(0f, 0f, 0f);
        if((vec.x + vec.y) == 1)//그냥 나가면 E
        {
            if(vec.y == 1)//N
            {
                temp.z = 90f;
            }
        }
        else
        {
            if (vec.y == -1)//S
            {
                temp.z = -90f;
            }
            else//W
            {
                temp.z = 180f;
            }
        }

        return temp;
    }
}
