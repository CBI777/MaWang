using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    /*22-1-22 ��������
    isTileEmpty�� isTileMovable�� isTileSafe�� ��ħ
    ���� �� �Լ��� ���� ��� isTileSafe�� ó���ϰ� �Ǿ���.
    ����, isTileSafe���� ���� �� Ÿ���� �����ϴ����� Ȯ���� �ϵ��� �Ǿ�����.

    placeObject�� ���� object�� �� �ڸ��� �ش��ϴ� tileLocations�� �����ϴ�
    �ϸ� �ϵ��� �Ǿ�����. (����ó�� X)
    �̴� spawnManager���� ���� ���� ����ó���� �ϱ� ���ؼ���.
    */

    private Tilemap map;

    [SerializeField]
    private string curStage;

    [SerializeField]
    private int stageVar;

    [SerializeField]
    private List<TileData> tileDatas;

    //����׿�
    private string tilemapName;

    //tile �� ��ü�� data�� �����ϴ� ��
    private Dictionary<TileBase, TileData> dataFromTiles;

    //Ÿ���� (x, y)�� ������ �ִ����� �����ϴ� ��
    public Dictionary<Vector3Int, GameObject> tileLocations
        = new Dictionary<Vector3Int, GameObject>();




    private void Awake()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

        //��� map�� �̸��� Map?  �̸�, /Tilemap/ ���� ���ο� �����Ѵ�. �̰� instantiate�� ����,
        map = Tilemap.Instantiate(
                Resources.Load<Tilemap>(curStage + "/Tilemap/" + "Map" + ((Random.Range(0, (stageVar))).ToString())));
        //grid�� ������ �ִ� Tilemanager, �� �� gameObject(TileManager script)�� transform�� �θ�� �����������
        //����� ����� �ȴ�.
        map.transform.parent = gameObject.transform;
    }

    /*
        private void Update()
        {
            //������ - �ʿ��� ������ �����ֱ� ���� �κ�

            if(Input.GetMouseButton(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPosition = map.WorldToCell(mousePosition);

                print("IsEmpty : " + isTileEmpty(gridPosition) + ", there is " + getTileObjectName(gridPosition));


                TileBase clickedTile = map.GetTile(gridPosition);

                tileType istype = dataFromTiles[clickedTile].type;
                print("Tile " + clickedTile + "'s type is " + istype.ToString());
            }

        }
    */



    //Ÿ���� type�� Ȯ��(��? ������Ÿ��? �Ϲ� ��?...)
    public tileType isTileType(Vector3Int gridPosition)
    {
        TileBase wantedTile = map.GetTile(gridPosition);

        return dataFromTiles[wantedTile].type;
    }


    //Ÿ���� (x, y)�� ĳ������ ��ġ�� ������ �� �ִ��� Ȯ���� �� ���
    //Ÿ���� ���������, ������ �� �ִ� Ÿ���̾�� true�� ��ȯ
    //������ isTileEmpty�� isTileMovable�� �ش� �Լ��� ���Ͻ�Ŵ
    //isTileSafe�� ����ϴ� �ٸ� �Լ���� �ֱ� ������ isTileSafe�� ���ִ� ���� ������ ����
    //����, �ش� tile�� �������� �ʾƵ� false��.
    public bool isTileSafe(Vector3 gridPosition)
    {
        Vector3Int pos = map.WorldToCell(gridPosition);
        if(!map.HasTile(pos)) { return false; }

        TileBase wantedTile = map.GetTile(pos);
        return (!(tileLocations.ContainsKey(pos)) && (dataFromTiles[wantedTile].movable));
    }
    //Ÿ���� (x, y)�� ������ �� �ִ� ���� �ִ����� Ȯ��
    //(x, y)�� ���𰡰� �����ϸ�, (x, y)�� �ִ� ���� �μ� �� �־�߸� true�� ��ȯ
    public bool isTileDestroyable(Vector3Int gridPosition)
    {
        if(tileLocations.ContainsKey(gridPosition) && tileLocations[gridPosition].GetComponent<CharacterBase>().getIsDestroyable())
        {
            return true;
        }
        return false;
    }

    //Ÿ���� (x, y)�� �����ϴ� ���𰡿��� �������� �ִ� �Լ�
    public void damageObject(int damage, Vector3 gridPosition)
    {
        Vector3Int pos = map.WorldToCell(gridPosition);
        if (isTileDestroyable(pos))
        {
            if (tileLocations[pos].GetComponent<CharacterBase>().hpDamage(damage))
            {
                tileLocations.Remove(pos);
            }
        }
    }

    //Ÿ���� (x, y) ��ġ�� ĳ������ ������ �����ϴ� �Լ�
    public void placeObject(GameObject target, Vector3 gridPosition)
    {
        Vector3Int pos = map.WorldToCell(gridPosition);
        tileLocations.Add(pos, target);
    }

    //(x, y)�� �־��� ������ (z, w)�� �ű�� �Լ�
    public bool moveObject(Vector3 originalGridPosition, Vector3 gridPosition)
    {
        foreach (Vector3Int v in tileLocations.Keys) {
            if (tileLocations[v].CompareTag("Player"))
            Debug.Log(v);
        }



        Vector3Int originalPos = map.WorldToCell(originalGridPosition);
        Vector3Int newPos = map.WorldToCell(gridPosition);
        /*if(!tileLocations.ContainsKey(originalPos))
        {
            return false;
        }*/
        GameObject target = tileLocations[originalPos];

        if (isTileSafe(newPos))
        {
            tileLocations.Add(newPos, target);
            tileLocations.Remove(originalPos);
            foreach (Vector3Int v in tileLocations.Keys)
            {
                if (tileLocations[v].CompareTag("Player"))
                    Debug.Log(v);
            }
            return true;
        }
        return false;
    }

    //������ - �ش� tile���� �ִ� ������ characterName�� ��ȯ
    public string getTileObjectName(Vector3Int gridPosition)
    {
        if(!(tileLocations.ContainsKey(gridPosition)))
        {
            return "Nothing";
        }
        else
        {
            return tileLocations[gridPosition].GetComponent<CharacterBase>().getName();
        }
    }
}
