using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    private Tilemap map;

    [SerializeField]
    private string curStage;

    [SerializeField]
    private LevelManager levelManager;
    //mapVariation의 갯수가 얼마나되는지를 알려주는 변수다.
    [SerializeField]
    private int roomVar;

    [SerializeField]
    private List<TileData> tileDatas;

    //2022_02_09 - getTilemapVar 추가
    private int tilemapVar;
    public int getTilemapVar()
    {
        return tilemapVar;
    }

    //tile 그 자체의 data를 저장하는 곳
    private Dictionary<TileBase, TileData> dataFromTiles;

    //타일의 (x, y)에 무엇이 있는지를 저장하는 곳
    public Dictionary<Vector3Int, GameObject> tileLocations
        = new Dictionary<Vector3Int, GameObject>();

    [SerializeField]
    private ScriptableLocation playerLoc;

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

        //2022_02_09 - 이 부분을 통해서 load된 것을 읽어야할지, 아니면 새롭게 random을 돌려야 할지를 확인한다.
        if (levelManager.GetComponent<SaveManager>().getSameCheck())
        {
            tilemapVar = levelManager.GetComponent<SaveManager>().saving.stageVar1;
            Debug.Log("true");
        }
        else
        {
            tilemapVar = Random.Range(0, (roomVar));
            Debug.Log("false");
        }   
        //2022_02_09 - load때문에 Instantiate하는 함수 내부를 변경.
        Debug.Log(curStage + "/Tilemap/" + "Map" + tilemapVar.ToString());
        map = Tilemap.Instantiate(
                Resources.Load<Tilemap>(curStage + "/Tilemap/" + "Map" + tilemapVar.ToString()));

        //모든 map의 이름은 Map?  이며, /Tilemap/ 폴더 내부에 존재한다. 이걸 instantiate한 다음,
        //grid를 가지고 있는 Tilemanager, 즉 이 gameObject(TileManager script)의 transform을 부모로 지정해줘야지
        //제대로 출력이 된다.
        map.transform.parent = gameObject.transform;
    }

    //타일의 type을 확인(벽? 데미지타일? 일반 길?...)
    public tileType isTileType(Vector3Int gridPosition)
    {
        TileBase wantedTile = map.GetTile(gridPosition);

        return dataFromTiles[wantedTile].type;
    }


    //타일의 (x, y)로 캐릭터의 위치를 변경할 수 있는지 확인할 때 사용
    //타일이 비어있으며, 움직일 수 있는 타일이어야 true를 반환
    //기존의 isTileEmpty와 isTileMovable을 해당 함수로 통일시킴
    //isTileSafe를 사용하는 다른 함수들로 있기 때문에 isTileSafe를 없애는 것은 무리가 있음
    //또한, 해당 tile이 존재하지 않아도 false임.
    public bool isTileSafe(Vector3 gridPosition)
    {
        Vector3Int pos = map.WorldToCell(gridPosition);
        if(!map.HasTile(pos)) { return false; }

        TileBase wantedTile = map.GetTile(pos);
        return (!(tileLocations.ContainsKey(pos)) && (dataFromTiles[wantedTile].movable));
    }
    //타일의 (x, y)에 공격할 수 있는 것이 있는지를 확인
    //(x, y)에 무언가가 존재하며, (x, y)에 있는 것이 부술 수 있어야만 true를 반환
    public bool isTileDestroyable(Vector3Int gridPosition)
    {
        if(tileLocations.ContainsKey(gridPosition) && tileLocations[gridPosition].GetComponent<CharacterBase>().getIsDestroyable())
        {
            return true;
        }
        return false;
    }

    //2022_02_13 effect를 character 내부에서 적용하기 위해서 bool로 변경
    //한 번이라도 attack이 성공했다면 true를 반환
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

    public bool damagePlayer(int damage, Vector3 gridPosition)
    {
        bool temp = false;
        Vector3Int pos = map.WorldToCell(gridPosition);
        if (isTileDestroyable(pos) && tileLocations[pos].CompareTag("Player"))
        {
            temp = true;
            if (tileLocations[pos].GetComponent<CharacterBase>().hpDamage(damage))
            {
                tileLocations.Remove(pos);
            }
        }

        return temp;
    }

    //타일의 (x, y) 위치에 캐릭터의 정보를 저장하는 함수
    public void placeObject(GameObject target, Vector3 gridPosition)
    {
        Vector3Int pos = map.WorldToCell(gridPosition);
        if (target.CompareTag("Player")) { playerLoc.setLocation(pos.x, pos.y); }
        else if(target.CompareTag("Enemy"))
        {
            target.GetComponent<MonsterController>().setLocation(pos.x, pos.y);
        }
        tileLocations.Add(pos, target);
    }

    //(x, y)에 있었던 정보를 (z, w)로 옮기는 함수
    //player용
    public bool moveObject(Vector3 originalGridPosition, Vector3 gridPosition)
    {
        /*foreach (Vector3Int v in tileLocations.Keys)
            {
                if (tileLocations[v].CompareTag("Player"))
                    Debug.Log(v);
            }*/

        Vector3Int originalPos = map.WorldToCell(originalGridPosition);
        Vector3Int newPos = map.WorldToCell(gridPosition);
        /*if(!tileLocations.ContainsKey(originalPos))
        {
            return false;
        }*/
        GameObject target = tileLocations[originalPos];

        if (isTileSafe(newPos))
        {
            playerLoc.setLocation(newPos.x, newPos.y);
            tileLocations.Add(newPos, target);
            tileLocations.Remove(originalPos);
            /*foreach (Vector3Int v in tileLocations.Keys)
            {
                if (tileLocations[v].CompareTag("Player"))
                    Debug.Log(v);
            }*/

            return true;
        }
        return false;
    }
    
    public Directions moveObject(Vector3Int originalPos, Directions dir)
    {
        GameObject target = tileLocations[originalPos];
        if (isTileSafe((originalPos + DirectionChange.dirToVector(dir))))
        {
            Vector3Int newPos = (originalPos + DirectionChange.dirToVector(dir));
            target.GetComponent<MonsterController>().setLocation(newPos.x, newPos.y);
            tileLocations.Add(newPos, target);
            tileLocations.Remove(originalPos);

            return (dir);
        }
        return (Directions.O);
    }



    //monster용
    public Directions moveSequence(Vector3Int originalPos, List<Directions> Sequence)
    {
        GameObject target = tileLocations[originalPos];

        foreach(Directions d in Sequence)
        {
            if(isTileSafe((originalPos + DirectionChange.dirToVector(d))))
            {
                Vector3Int newPos = (originalPos + DirectionChange.dirToVector(d));
                target.GetComponent<MonsterController>().setLocation(newPos.x, newPos.y);
                tileLocations.Add(newPos, target);
                tileLocations.Remove(originalPos);

                return (d);
            }
        }

        return (Directions.O);
    }

    //디버깅용 - 해당 tile위에 있는 무언가의 characterName을 반환
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
