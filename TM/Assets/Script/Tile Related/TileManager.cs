using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    /*22-1-22 변동사항
    isTileEmpty와 isTileMovable을 isTileSafe로 합침
    이제 두 함수의 일을 모두 isTileSafe가 처리하게 되었음.
    또한, isTileSafe에서 정말 그 타일이 존재하는지도 확인을 하도록 되어있음.

    placeObject는 이제 object를 그 자리에 해당하는 tileLocations에 저장하는
    일만 하도록 되어있음. (오류처리 X)
    이는 spawnManager에서 보다 쉽게 오류처리를 하기 위해서임.
    */

    private Tilemap map;

    [SerializeField]
    private string curStage;

    [SerializeField]
    private int stageVar;

    [SerializeField]
    private List<TileData> tileDatas;

    //디버그용
    private string tilemapName;

    //tile 그 자체의 data를 저장하는 곳
    private Dictionary<TileBase, TileData> dataFromTiles;

    //타일의 (x, y)에 무엇이 있는지를 저장하는 곳
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

        //모든 map의 이름은 Map?  이며, /Tilemap/ 폴더 내부에 존재한다. 이걸 instantiate한 다음,
        map = Tilemap.Instantiate(
                Resources.Load<Tilemap>(curStage + "/Tilemap/" + "Map" + ((Random.Range(0, (stageVar))).ToString())));
        //grid를 가지고 있는 Tilemanager, 즉 이 gameObject(TileManager script)의 transform을 부모로 지정해줘야지
        //제대로 출력이 된다.
        map.transform.parent = gameObject.transform;
    }

    /*
        private void Update()
        {
            //디버깅용 - 필요한 정보를 보여주기 위한 부분

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

    //타일의 (x, y)에 존재하는 무언가에게 데미지를 주는 함수
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

    //타일의 (x, y) 위치에 캐릭터의 정보를 저장하는 함수
    public void placeObject(GameObject target, Vector3 gridPosition)
    {
        Vector3Int pos = map.WorldToCell(gridPosition);
        tileLocations.Add(pos, target);
    }

    //(x, y)에 있었던 정보를 (z, w)로 옮기는 함수
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

    //디버깅용 - 해당 tile위에 있는 무언가의 characterName을 반환
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
