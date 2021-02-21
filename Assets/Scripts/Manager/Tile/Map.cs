using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

//=============================================//
[System.Serializable]
public class StringInt : SerializableDictionaryBase<string, int> {}
//=============================================//
[System.Serializable]
public struct Floor
{
    //=============================================//
    // ctor
    public Floor(string name)
    {
        floor = new GameObject(name);
        tiles = new List<GameObject>();
        concreteTiles = new List<GameObject>();
    }
    //=============================================//
    // variables
    public GameObject floor;
    public List<GameObject> tiles;
    public List<GameObject> concreteTiles;
    //=============================================//
    // Func
    public void SetPos(Vector3 value)
    {
        floor.transform.localPosition = value;
    }
} // End Struct Floor
//=============================================//

[System.Serializable]
public struct Block
{
    //=============================================//
    // ctor
    public Block(string name, int floorCount)
    {
        block = new GameObject(name);
        floors = new List<Floor>();
        for (int i = 0; i < floorCount; ++i)
        {
            string floorName = i.ToString() + "_Floor";
            Floor floor = new Floor(floorName);
            floor.floor.transform.parent = block.transform;
            floors.Add(floor);
        }
    }
    //=============================================//
    // variables
    public GameObject block;
    public List<Floor> floors;
    //=============================================//
    // Funs
    public void SetPos(Vector3 value)
    {
        block.transform.localPosition = value;
    }
    //
    public Floor GetFloor(int idx)
    {
        if (idx >= floors.Count)
            return floors[0]; // 어떻게 return null??
        //
        return floors[idx];
    }
} // End Struct Block

//=============================================//
public class Map : MonoBehaviour
{
    //=============================================//
    public static Map instance;
    //=============================================//
    public StringInt mineCounts;
    //=============================================//
    public GameObject tilePrefab;
    public GameObject concretePrefab;
    //
    public List<Block> blocks = new List<Block>();
    //public Block readyBlock;
    //public Block stage1;
    //
    public float blockStartPos;
    //=============================================//
    private void Awake()
    {
        instance = this;
    }
    //
    private void Start()
    {
        CreateStage();
    } // End Start()
    //=============================================//
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F1))
        //    SetArroundMineInfo(blocks[1]);
    }
    //=============================================//
    public Floor GetFloor(int stage, int floor)
    {
        return blocks[stage].GetFloor(floor);
    }
    //
    private void SetPrefabs()
    {
        tilePrefab = Resources.Load<GameObject>("Prefabs/Tiles/Tile");
        concretePrefab = Resources.Load<GameObject>("Prefabs/Tiles/Concrete");
    } // End SetPrefabs()

    private void CreateStage()
    {
        SetPrefabs();
        //
        //float posY = 30.0f;
        float posY = 0.0f;
        //
        CreateBlock("ReadyBlock", 2, posY);
        posY += 47 * 2;
        //
        CreateBlock("1_Stage", 10, posY);
        SetConcreteInside(blocks[1]);
        SetMine(blocks[1]);
        Invoke("SetMineInfo", 0.1f);
    } // End CreateStage()

    private void SetMineInfo()
    {
        SetArroundMineInfo(blocks[0]);
        SetArroundMineInfo(blocks[1]);
        //
        GameManager.instance.StartGame();
    }

    private void CreateBlock(string blockName,
        int floorCount, float posY)
    //private void CreateBlock(string blockName,
    //    int floorCount, float posY, out Block _block)
    {
        Block _block = new Block(blockName, floorCount);
        _block.block.transform.parent = transform;
        float offsetX = blockStartPos; // fixed / Difference By CamPos
        _block.SetPos(new Vector3(offsetX, posY, 0));
        //
        float tileWidth = 64;
        for (int y = 0; y < floorCount; ++y)
        {
            // SetFloorPos (x : 0<->32 / y : +47)
            float fPosX = ((y % 2 == 0) ? 0 : 32);
            Vector3 floorPos = new Vector3(fPosX, 47 * y, 1);
            _block.GetFloor(y).SetPos(floorPos);
            // Create Tile Into Floor
            for (int x = 0; x < 9; ++x)
            {
                Vector3 tilePos = new Vector3(tileWidth * x, 0, 0);
                if (x == 0 || x == 8)
                    CreateTile(x, y, tilePos, concretePrefab, _block);
                else
                    CreateTile(x, y, tilePos, tilePrefab, _block);
            }
        }
        //
        blocks.Add(_block);
    } // End CreateBlock()

    void CreateTile(int xOrder, int floorIdx, Vector3 pos, GameObject prefab, Block _block)
    {
        GameObject tile = Instantiate(prefab, _block.GetFloor(floorIdx).floor.transform);
        // Set Name
        string nameTemp = tile.transform.name;
        tile.transform.name = xOrder + "_" + nameTemp.Substring(0, nameTemp.Length - 7);
        //
        tile.transform.localPosition = pos;
        //
        if (prefab.CompareTag("Tile") == true)
            _block.GetFloor(floorIdx).tiles.Add(tile);
        else
        {
            tile.transform.Translate(Vector3.back);
            _block.GetFloor(floorIdx).concreteTiles.Add(tile);
        }
    } // End CreateTile()

    private void SetConcreteInside(Block stage)
    {
        for(int y = 0; y < stage.floors.Count; ++y)
        {
            int randIdx = Random.Range(0, 7);
            Vector3 pos = stage.GetFloor(y).tiles[randIdx].transform.localPosition;
            Destroy(stage.GetFloor(y).tiles[randIdx]);
            stage.GetFloor(y).tiles.RemoveAt(randIdx);
            //
            GameObject concrete = Instantiate(concretePrefab, stage.GetFloor(y).floor.transform);
            concrete.transform.localPosition = pos;
            concrete.transform.Translate(Vector3.back);
            concrete.name = (randIdx + 1) + "_Concrete";
            // Debug Color
            concrete.GetComponent<SpriteRenderer>().color = Color.green;
            //
            stage.GetFloor(y).concreteTiles.Add(concrete);
        }
    } // End SetConcreteInside()

    private void SetMine(Block stage)
    {
        int[] mineArr = new int[10];
        {
            int idx = 0;
            // Set MineArr From List
            int i = 0; // MineNumber
            foreach (var data in mineCounts)
            {
                int count = data.Value;
                for (int j = 0; j < count; ++j)
                {
                    mineArr[idx++] = i;
                }
                // next MineNumber
                ++i;
            }
            // Shuffle Arr
            MyUtility.ShuffleArray(mineArr);
        }
        //
        for (int y = 0; y < stage.floors.Count; ++y)
        {
            int randIdx = Random.Range(0, 6);
            Tile tile = stage.GetFloor(y).tiles[randIdx].GetComponent<Tile>();
            //
            //int mineType = Random.Range(0, 2); // Random
            //int mineType = 0; // Pull
            //int mineType = 1; // Push
            //int mineType = 2; // Narrow
            //int mineType = 3; // Crash
            //tile.inner.SetDanger(mineType);
            tile.inner.SetDanger(mineArr[y]);
            // Debug
            //
            tile.closet.GetComponent<SpriteRenderer>().color = Color.red;
        }
    } // End SetMine()

    private void SetArroundMineInfo(Block stage)
    {
        int tileCount = stage.GetFloor(0).tiles.Count;
        //
        for (int y = 0; y < stage.floors.Count; ++y)
        {
            for (int x = 0; x < tileCount; ++x)
            {
                Inner inner = stage.GetFloor(y).tiles[x].GetComponent<Tile>().inner;
                //if (inner.isDanger == true)
                //    continue;
                inner.CheckArround();
            }
        }

    } // End SetArroundMineInfo()
    //=============================================//
}