using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//2022_02_14 Grass����
public enum tileType { Path, Wall }

[CreateAssetMenu]

public class TileData : ScriptableObject
{
   //unity�� tilemap���� �����ϴ� �⺻ �Լ����� ����ϱ� ���ؼ� TileBase�� �����.
    public TileBase[] tiles;

    public bool movable;
    public tileType type;
}