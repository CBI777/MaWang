using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    private Tilemap map;

    [SerializeField]
    private string curStage;

    //2022_02_09 - inspector�� ���ٰ� �ϼż� levelManager�� inspector�� �ֵ��� �߽��ϴ�.
    [SerializeField]
    private LevelManager levelManager;
    //2022_02_09 �̸� ������ ȥ���� ���� �ʱ� ���ؼ� StageVar -> RoomVar�� �̸��� ����
    //�����, �̰� mapVariation�� ������ �󸶳��Ǵ����� �˷��ִ� ������.
    [SerializeField]
    private int roomVar;

    [SerializeField]
    private List<TileData> tileDatas;

    //2022_02_09 - getTilemapVar �߰�
    private int tilemapVar;
    public int getTilemapVar()
    {
        return tilemapVar;
    }

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

        //2022_02_09 - �� �κ��� ���ؼ� load�� ���� �о������, �ƴϸ� ���Ӱ� random�� ������ ������ Ȯ���Ѵ�.
        if (levelManager.GetComponent<PlayerSaveManager>().getSameCheck())
        {
            tilemapVar = levelManager.GetComponent<PlayerSaveManager>().saving.stageVar1;
        }
        else
        {
            tilemapVar = Random.Range(0, (roomVar));
        }

        //2022_02_09 - load������ Instantiate�ϴ� �Լ� ���θ� ����.
        map = Tilemap.Instantiate(
                Resources.Load<Tilemap>(curStage + "/Tilemap/" + "Map" + tilemapVar.ToString()));
        //��� map�� �̸��� Map?  �̸�, /Tilemap/ ���� ���ο� �����Ѵ�. �̰� instantiate�� ����,
        //grid�� ������ �ִ� Tilemanager, �� �� gameObject(TileManager script)�� transform�� �θ�� �����������
        //����� ����� �ȴ�.
        map.transform.parent = gameObject.transform;
    }

    //2022_02_09 - �Ⱦ��� update����


    //Ÿ���� type�� Ȯ��(��? ������Ÿ��? �Ϲ� ��?...)
    public tileType isTileType(Vector3Int gridPosition)
    {
        TileBase wantedTile = map.GetTile(gridPosition);

        return dataFromTiles[wantedTile].type;
    }

    /*
    private void Update()
    {
        //������ - �ʿ��� ������ �����ֱ� ���� �κ�
        
        if(Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            print("there is " + getTileObjectName(gridPosition));


            TileBase clickedTile = map.GetTile(gridPosition);

            tileType istype = dataFromTiles[clickedTile].type;
            print("Tile " + clickedTile + "'s type is " + istype.ToString());
        }
       
    }*/


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

    //2022_02_13 effect�� character ���ο��� �����ϱ� ���ؼ� bool�� ����
    //�� ���̶� attack�� �����ߴٸ� true�� ��ȯ
    public bool damageObject(int damage, Vector3 gridPosition)
    {
        bool temp = false;
        Vector3Int pos = map.WorldToCell(gridPosition);
        if (isTileDestroyable(pos))
        {
            temp = true;
            if (tileLocations[pos].GetComponent<CharacterBase>().hpDamage(damage))
            {
                tileLocations.Remove(pos);
            }
        }

        return temp;
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
        if (!(tileLocations.ContainsKey(gridPosition)))
        {
            return "Nothing";
        }
        else
        {
            return tileLocations[gridPosition].GetComponent<CharacterBase>().getName();
        }
    }
}
