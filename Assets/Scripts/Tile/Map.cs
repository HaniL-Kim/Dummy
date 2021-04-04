using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart;
using System.Linq;
using MyUtilityNS;

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
    public static float FloorHeight = 47.0f;
    public static int FloorCount = 10;
    public static int TileCount = 6;
    public static float BlockHeight = FloorHeight * (float)FloorCount;
    public static float TileWidth = 64.0f;
    //=============================================//
    public static Map instance;
    //=============================================//
    public bool isInfinite = false;
    //public StringInt mineCounts;
    //=============================================//
    public GameObject tilePrefab;
    public GameObject concretePrefab;
    //
    public List<Block> blocks = new List<Block>();
    //
    public float blockStartPos;
    //=============================================//
    public DicStageData stageData;
    public StageData currentStageData = null;
    //=============================================//
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        SetMap();
    }
    //
    private void Update()
    {
        DebugMap();
    }
    ///////////////////////////////////////
    public void DebugMap()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetMineColor(Color.red);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            SetMineColor(Color.white);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            FlipAllMine();
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            FlipAllTile();
        }
    }
    //=============================================//
    public void SetMap()
    {
        stageData = SceneControl.instance.stageData;
        //
        Difficulty difficulty = SceneControl.instance.currentDifficulty;
        int stage = SceneControl.instance.currentStage;
        //
        CreateStage(MyUtility.DiffToStr(difficulty), stage);
        // CreateStage(NORMAL, 1);
    }
    //=============================================//
    // For Debug
    public void FlipAllTile()
    {
        for (int i = 0; i < blocks.Count; ++i)
        {
            Block b = blocks[i];
            for (int j = 0; j < b.floors.Count; ++j)
            {
                Floor f = b.floors[j];
                for (int k = 0; k < f.tiles.Count; ++k)
                {
                    Inner inner = f.tiles[k].GetComponent<Tile>().inner;
                    inner.closet.Flip();
                }
            }
        }
    }
    //
    public void FlipAllMine()
    {
        for (int i = 0; i < blocks.Count; ++i)
        {
            Block b = blocks[i];
            for (int j = 0; j < b.floors.Count; ++j)
            {
                Floor f = b.floors[j];
                for (int k = 0; k < f.tiles.Count; ++k)
                {
                    Inner inner = f.tiles[k].GetComponent<Tile>().inner;
                    if (inner.isDanger == true)
                        inner.closet.Flip();
                }
            }
        }
    }
    //
    public void SetMineColor(Color c)
    {
        for (int i = 0; i < blocks.Count; ++i)
        {
            Block b = blocks[i];
            for (int j = 0; j < b.floors.Count; ++j)
            {
                Floor f = b.floors[j];
                for (int k = 0; k < f.tiles.Count; ++k)
                {
                    Inner inner = f.tiles[k].GetComponent<Tile>().inner;
                    if(inner.isDanger == true)
                        inner.closet.GetComponent<SpriteRenderer>().color = c;
                }
            }
        }
    }
    //=============================================//
    public Floor GetFloor(int stage, int floor)
    {
        return blocks[stage].GetFloor(floor);
    }
    ///////////////////////////////////////
    private void LoadTilePrefabs()
    {
        tilePrefab = Resources.Load<GameObject>("Prefabs/Tiles/Tile");
        concretePrefab = Resources.Load<GameObject>("Prefabs/Tiles/Concrete");
    }
    ///////////////////////////////////////
    private void IsInfiniteMode(int lv)
    {
        if (lv == 0)
            isInfinite = true;
    }
    ///////////////////////////////////////
    private void CreateStage(string diff, int lv)
    {
        LoadTilePrefabs();
        //
        StageData sd = stageData[diff][lv];
        currentStageData = sd;
        //
        MineController.instance.SetDifficulty(diff);
        // Infinite Mode
        IsInfiniteMode(lv);
        //
        GameManager.instance.elecShooter.superViser.SetUnit(sd.unit);
        //GameManager.instance.elecShooter.superViser.SetUnit(3);
        //
        float blockPosY = 0.0f;
        //
        int readyBlockfloorCount = 2;
            CreateBlock("ReadyBlock", readyBlockfloorCount, blockPosY);
        blockPosY += Map.FloorHeight * 2.0f;
        //
        int blockfloorCount = 10;
        for (int i = 0; i < sd.blocks; ++i)
        {
            string blockName = (i + 1).ToString() + "_block";
            CreateBlock(blockName, blockfloorCount, blockPosY);
            //
            SetConcreteInside(blocks[i + 1]);
            SetMine(blocks[i + 1]);
            blockPosY += Map.FloorHeight * blockfloorCount;
        }
        //
        //SetMineInfo();
        Invoke("SetMineInfo", 0.1f);
    }
    ///////////////////////////////////////
    private void SetMineInfo()
    {
        // ready block
        for (int i = 0; i < 2; ++i)
            SetArroundMineInfo(blocks[0].floors[i]);
        // stage blocks
        for (int i = 1; i < blocks.Count; ++i)
            SetArroundMineInfo(blocks[i]);
        //
        // Active only ready_block & 1_block
        for (int i = 2; i < blocks.Count; ++i)
            blocks[i].block.SetActive(false);
        //
        GameManager.instance.StartGame();
    }
    ///////////////////////////////////////
    private void CreateBlock(string blockName, int floorCount, float posY)
    {
        Block _block = new Block(blockName, floorCount);
        _block.block.transform.parent = transform;
        float offsetX = blockStartPos; // fixed / Difference By CamPos
        _block.SetPos(new Vector3(offsetX, posY, 0));
        //
        float tileWidth = 64;
        for (int y = 0; y < floorCount; ++y)
        {
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
        BlockControl bc = _block.block.AddComponent<BlockControl>();
        bc.block = _block; 
        bc.tf_bottom = _block.GetFloor(0).floor.transform;
        bc.tf_top = _block.GetFloor(floorCount - 1).floor.transform;
        //
        blocks.Add(_block);
    }
    ///////////////////////////////////////
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
    }
    ///////////////////////////////////////
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
            // For Debug
            //concrete.GetComponent<SpriteRenderer>().color = Color.green;
            //
            stage.GetFloor(y).concreteTiles.Add(concrete);
        }
    }
    ///////////////////////////////////////
    public void SetMine(Block block)//, StageData sd = null)
    {
        StageData sd = currentStageData;
        if (MyUtility.IsNull(sd, "StageData"))
            return;
        //
        int[] mineArr;
        {
            //if (sd == null)
            //{
            //    mineArr = new int[10];
            //    { // For Debug
            //        int idx = 0;
            //        // Set MineArr From List
            //        int i = 0; // MineNumber
            //        foreach (var data in mineCounts)
            //        {
            //            int count = data.Value;
            //            for (int j = 0; j < count; ++j)
            //            {
            //                mineArr[idx++] = i;
            //            }
            //            // next MineNumber
            //            ++i;
            //        }
            //    }
            //}
            //else
        }
        { // Crate Mine Array
            int[] mines = { sd.pull, sd.push, sd.ghost, sd.thunder, sd.narrow };
            //
            int totalCount = 0;
            for(int i = 0; i < mines.Length; ++i)
                totalCount += mines[i];
            //
            mineArr = new int[totalCount];
            int idx = 0;
            for(int i = 0; i < mines.Length; ++i)
            {
                for (int j = 0; j < mines[i]; ++j)
                    mineArr[idx++] = i;
            }
        }
        int floorCount = block.floors.Count, tileCount = block.GetFloor(0).tiles.Count;
        // Set Crash Mine First
        if (sd.crash > 0)
        {
            Tile t;
            int randTileIdx = 0;
            //
            if (sd.crash == 1)
            {
                int randFloor = Random.Range(0, floorCount); // 0 ~ 9
                randTileIdx = Random.Range(0, tileCount);
                t = block.GetFloor(randFloor).tiles[randTileIdx].GetComponent<Tile>();
                t.inner.SetDanger((int)MineTypes.CRASH);
            }
            if (sd.crash == 2)
            {
                int oddFloor = (Random.Range(0, floorCount / 2) * 2) + 0; // 0, 2, 4, 6, 8
                int eveFloor = (Random.Range(0, floorCount / 2) * 2) + 1; // 1, 3, 5, 7, 9
                                                                          //
                randTileIdx = Random.Range(0, 6);
                t = block.GetFloor(oddFloor).tiles[randTileIdx].GetComponent<Tile>();
                t.inner.SetDanger(5);
                //
                randTileIdx = Random.Range(0, 6);
                t = block.GetFloor(eveFloor).tiles[randTileIdx].GetComponent<Tile>();
                t.inner.SetDanger(5);
            }
        }
        //

        // Get All Tiles
        List<Tile> allTiles = new List<Tile>();
        for (int i = 0; i < floorCount; ++i)
        {
            for (int j = 0; j < tileCount; ++j)
            {
                Tile t = block.GetFloor(i).tiles[j].GetComponent<Tile>();
                // pass crash mine
                if (t.inner.isDanger == true)
                    continue;
                //
                allTiles.Add(t);
            }
        }
        // Shuffle All Tiles Be Set
        MyUtility.ShuffleList(allTiles);
        // Shuffle Mines To Set
        MyUtility.ShuffleArray(mineArr);
        // Set Mine
        for (int i = 0; i < mineArr.Length; ++i)
            allTiles[i].inner.SetDanger(mineArr[i]);
        //
        {
        /* one mine in one floor
        for (int y = 0; y < block.floors.Count; ++y)
        {
            int randIdx = Random.Range(0, 6);
            Tile tile = block.GetFloor(y).tiles[randIdx].GetComponent<Tile>();
            //
            tile.inner.SetDanger(mineArr[y]);
        }
        */
        }
    }
    ///////////////////////////////////////
    public void SetArroundMineInfo(Floor floor)
    {
        for (int x = 0; x < TileCount; ++x)
        {
            Inner inner = floor.tiles[x].GetComponent<Tile>().inner;
            //if (inner.isDanger == true)
            //    continue;
            inner.CheckArround();
        }
    }
    //
    public void SetArroundMineInfo(Block block)
    {
        for (int y = 0; y < FloorCount; ++y)
        {
            for (int x = 0; x < TileCount; ++x)
            {
                Inner inner = block.GetFloor(y).tiles[x].GetComponent<Tile>().inner;
                //if (inner.isDanger == true)
                //    continue;
                inner.CheckArround();
            }
        }
    }
    //=============================================//
}