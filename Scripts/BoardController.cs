using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController instance;

    //assigned in inspector
    public int boardColsNum, boardRowsNum;
    public int minRoomSize, maxRoomSize;
    public int corridorSize;
    public int wallPadding;
    public GameObject floorTile;
    public GameObject corridorTile;
    public GameObject wallTile;
    public GameObject portalTile;
    public int mapSeed = -1; //-1 to ignore seed

    [HideInInspector] public int level { get { return GameController.instance.level; } }
    [HideInInspector] public int seed { get; private set; }

    static int sCorridorSize;
    GameObject[,] boardPositionsFloor;
    List<Rect> roomList = new List<Rect>();

    //class for BSP implementation
    public class SubDungeon
    {
        public SubDungeon left, right;
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0);
        public int debugId;
        public List<Rect> corridors = new List<Rect>();

        private static int debugCounter = 0;

        public SubDungeon(Rect mrect)
        {
            rect = mrect;
            debugId = debugCounter;
            debugCounter++;
        }

        //if this room is an end room
        public bool IAmLeaf()
        {
            return left == null && right == null;
        }

        //function for splitting the room
        public bool Split(int minRoomSize, int maxRoomSize)
        {
            //return if branches are already occupied
            if (!IAmLeaf())
            {
                return false;
            }

            // choose a vertical or horizontal split depending on the proportions
            // too wide split vertically, too long horizontally,
            // or if nearly square split at random
            bool splitH;
            if (rect.width / rect.height >= 1.25)
            {
                splitH = false;
            }
            else if (rect.height / rect.width >= 1.25)
            {
                splitH = true;
            }
            else
            {
                splitH = Random.Range(0.0f, 1.0f) > 0.5;
            }

            if (Mathf.Min(rect.height, rect.width) / 2 < minRoomSize)
            {
                return false;
            }

            if (splitH)
            {
                int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

                left = new SubDungeon(new Rect(rect.x, rect.y, rect.width, split));
                right = new SubDungeon(
                  new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
            }
            else
            {
                int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));

                left = new SubDungeon(new Rect(rect.x, rect.y, split, rect.height));
                right = new SubDungeon(
                  new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
            }

            return true;
        }

        //function for creating a room
        public void CreateRoom()
        {
            if (left != null)
            {
                left.CreateRoom();
            }
            if (right != null)
            {
                right.CreateRoom();
            }
            if (IAmLeaf())
            {
                int roomWidth = (int)Random.Range(rect.width / 1.5f, rect.width - 2);
                int roomHeight = (int)Random.Range(rect.height / 1.5f, rect.height - 2);
                int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
                int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);

                // room position will be absolute in the world
                room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
            }

            if (!IAmLeaf()) CreateCorridorBetween(left, right);
        }

        //function for getting a room
        public Rect GetRoom()
        {
            if (IAmLeaf())
            {
                return room;
            }
            if (left != null)
            {
                Rect lroom = left.GetRoom();
                if (lroom.x != -1)
                {
                    return lroom;
                }
            }
            if (right != null)
            {
                Rect rroom = right.GetRoom();
                if (rroom.x != -1)
                {
                    return rroom;
                }
            }

            // workaround non nullable structs
            return new Rect(-1, -1, 0, 0);
        }

        //function to create a corridor between two rooms
        public void CreateCorridorBetween(SubDungeon left, SubDungeon right)
        {
            Rect lroom = left.GetRoom();
            Rect rroom = right.GetRoom();

            // attach the corridor to a random point in each room
            Vector2 lpoint = new Vector2((int)Random.Range(lroom.x + sCorridorSize, lroom.xMax - sCorridorSize), (int)Random.Range(lroom.y + sCorridorSize, lroom.yMax - sCorridorSize));
            Vector2 rpoint = new Vector2((int)Random.Range(rroom.x + sCorridorSize, rroom.xMax - sCorridorSize), (int)Random.Range(rroom.y + sCorridorSize, rroom.yMax - sCorridorSize));

            // always be sure that left point is on the left to simplify the code
            if (lpoint.x > rpoint.x)
            {
                Vector2 temp = lpoint;
                lpoint = rpoint;
                rpoint = temp;
            }

            int w = (int)(lpoint.x - rpoint.x);
            int h = (int)(lpoint.y - rpoint.y);
            int lx = (int)lpoint.x - sCorridorSize + 1;
            int ly = (int)lpoint.y - sCorridorSize + 1;
            int rx = (int)rpoint.x - sCorridorSize + 1;
            int ry = (int)rpoint.y - sCorridorSize + 1;
            int cw = sCorridorSize * 2 - 1;

            // if the points are not aligned horizontally
            if (w != 0)
            {
                // choose at random to go horizontal then vertical or the opposite
                if (Random.Range(0, 1) > 2)
                {
                    // add a corridor to the right
                    corridors.Add(new Rect(lx, ly, Mathf.Abs(w) + cw, cw));

                    // if left point is below right point go up
                    // otherwise go down
                    if (h < 0)
                    {
                        corridors.Add(new Rect(rx, ly, cw, Mathf.Abs(h)));
                    }
                    else
                    {
                        corridors.Add(new Rect(rx, ly, cw, -Mathf.Abs(h)));
                    }
                }
                else
                {
                    // go up or down
                    if (h < 0)
                    {
                        corridors.Add(new Rect(lx, ly, cw, Mathf.Abs(h)));
                    }
                    else
                    {
                        corridors.Add(new Rect(lx, ry, cw, Mathf.Abs(h)));
                    }

                    // then go right
                    corridors.Add(new Rect(lx, ry, Mathf.Abs(w) + cw, cw));
                }
            }
            else
            {
                // if the points are aligned horizontally
                // go up or down depending on the positions
                if (h < 0)
                {
                    corridors.Add(new Rect(lx, (int)ly, cw, Mathf.Abs(h)));
                }
                else
                {
                    corridors.Add(new Rect(lx, (int)ry, cw, Mathf.Abs(h)));
                }
            }
        }


    }

    //create the entire BSP recursively
    public void CreateBSP(SubDungeon subDungeon)
    {
        if (subDungeon.IAmLeaf())
        {
            // if the sub-dungeon is too large
            if (subDungeon.rect.width > maxRoomSize || subDungeon.rect.height > maxRoomSize)
            {
                if (subDungeon.Split(minRoomSize, maxRoomSize))
                {
                    CreateBSP(subDungeon.left);
                    CreateBSP(subDungeon.right);
                }
            }
        }
    }

    //Put room tiles on the play space
    public void DrawRooms(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }
        if (subDungeon.IAmLeaf())
        {
            for (int i = (int)subDungeon.room.x; i < subDungeon.room.xMax; i++)
            {
                for (int j = (int)subDungeon.room.y; j < subDungeon.room.yMax; j++)
                {
                    GameObject instance = Instantiate(floorTile, new Vector3(i, j, 0f), Quaternion.identity);
                    instance.transform.SetParent(transform);
                    boardPositionsFloor[i, j] = instance;
                }
            }
        }
        else
        {
            DrawRooms(subDungeon.left);
            DrawRooms(subDungeon.right);
        }
    }

    //Put corridor tiles on the play space
    void DrawCorridors(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }

        DrawCorridors(subDungeon.left);
        DrawCorridors(subDungeon.right);

        foreach (Rect corridor in subDungeon.corridors)
        {
            for (int i = (int)corridor.x; i < corridor.xMax; i++)
            {
                for (int j = (int)corridor.y; j < corridor.yMax; j++)
                {
                    if (boardPositionsFloor[i, j] == null)
                    {
                        GameObject instance = Instantiate(corridorTile, new Vector3(i, j, 0f), Quaternion.identity);
                        instance.transform.SetParent(transform);
                        boardPositionsFloor[i, j] = instance;
                    }
                }
            }
        }
    }

    //Put wall tiles on the play space
    void DrawWalls()
    {
        for (int i = 0; i < boardColsNum + 2 * wallPadding; i++)
        {
            for (int j = 0; j < boardRowsNum + 2 * wallPadding; j++)
            {
                if (boardPositionsFloor[i, j] == null)
                {
                    GameObject instance = Instantiate(wallTile, new Vector3(i, j, 0f), Quaternion.identity);
                    instance.transform.SetParent(transform);
                    boardPositionsFloor[i, j] = instance;
                }
            }
        }

    }

    //get all rooms recursively
    void GetRooms(SubDungeon subDungeon)
    {
        if (subDungeon == null) return;
        if (subDungeon.IAmLeaf()) 
        {
            roomList.Add(subDungeon.GetRoom());
            return;
        }

        GetRooms(subDungeon.left);
        GetRooms(subDungeon.right);
    }

    //generate the player starting position and goal position
    //nothing other than the player can spawn in the player room
    void GeneratePlayerAndGoal()
    {
        Rect portalRoom = new Rect(-1, -1, 0, 0), spawnRoom = new Rect(-1, -1, 0, 0);
        int maxX = 0, maxY = 0;
        int minX = boardColsNum + 2 * wallPadding, minY = boardRowsNum + 2 * wallPadding;
        foreach (var room in roomList)
        {
            if (room.y >= maxY)
            {
                if (room.y > minY || room.x > minX)
                {
                maxY = (int)room.y;
                maxX = (int)room.x;
                portalRoom = room;
                }
            }

            if (room.y <= minY)
            {
                if (room.y < minY || room.x < minX)
                {
                minY = (int)room.y;
                minX = (int)room.x;
                spawnRoom = room;
                }
            }
        }

        Vector2 playerSpawnPosition = new Vector2(Random.Range(spawnRoom.x + 2, spawnRoom.xMax - 2), Random.Range(spawnRoom.y + 2, spawnRoom.yMax - 2));
        GameController.instance.spawnPlayer(playerSpawnPosition);
        roomList.Remove(spawnRoom); //player spawn room should be empty

        Vector2 portalPosition = new Vector2(Random.Range(portalRoom.x + 2, portalRoom.xMax - 2), Random.Range(portalRoom.y + 2, portalRoom.yMax - 2));
        Instantiate(portalTile, portalPosition, Quaternion.identity);
    }

    //randomly place recovery packs in the rooms
    void GenerateRecoveryPacks()
    {
        int amount = Random.Range(1, Mathf.Min(roomList.Count, 3));

        for (int i = 0; i < amount; i++)
        {
            Rect target = roomList[Random.Range(0, roomList.Count)];
            Vector2 position = new Vector2(Random.Range(target.x + 2, target.xMax - 2), Random.Range(target.y + 2, target.yMax - 2));
            GameController.instance.spawnRecovery(position);
        }
    }

    //randomly generate enemies in the rooms
    void GenerateEnemies()
    {
        int amount = 10 + Mathf.RoundToInt(Random.Range(Mathf.Log(level, 2.5f), Mathf.Log(level, 1.7f)));

        for (int i = 0; i < amount; i++)
        {
            Rect target = roomList[Random.Range(0, roomList.Count)];
            Vector2 position = new Vector2(Random.Range(target.x + 2, target.xMax - 2), Random.Range(target.y + 2, target.yMax - 2));
            GameController.instance.spawnEnemy(position, Random.Range(0, (float)1));
        }
    }

    //randomly generate reroll points in the rooms
    void GenerateRerollPoints()
    {
        int amount = 1 + Mathf.RoundToInt(Random.Range(Mathf.Log(level, 10), Mathf.Log(level, 3)));

        for (int i = 0; i < amount; i++)
        {
            Rect target = roomList[Random.Range(0, roomList.Count)];
            Vector2 position = new Vector2(Random.Range(target.x + 2, target.xMax - 2), Random.Range(target.y + 2, target.yMax - 2));
            GameController.instance.spawnReroll(position);
        }
    }

    //randomly generate upgrade points in the rooms
    //only ONE upgrade point can exist in each room
    void GenerateUpgradePoints()
    {
        int amount = 2 + Mathf.RoundToInt(Random.Range(Mathf.Log(level, 10), Mathf.Log(level, 3)));

        for (int i = 0; i < amount; i++)
        {
            Rect target = roomList[Random.Range(0, roomList.Count)];
            Vector2 position = new Vector2(Random.Range(target.x + 2, target.xMax - 2), Random.Range(target.y + 2, target.yMax - 2));
            GameController.instance.spawnUpgrade(position);
            roomList.Remove(target);
            if (roomList.Count <= 0) break;
        }
    }



    private void Awake()
    {
        instance = this;
        boardPositionsFloor = new GameObject[boardColsNum + 2 * wallPadding, boardRowsNum + 2 * wallPadding];
        if (mapSeed >= 0) Random.InitState(mapSeed);
        sCorridorSize = corridorSize;
    }

    public void initializeLevel(int seed)
    {
        //use seed if its not -1
        if (seed != -1)
        {
            Random.InitState(seed);
            this.seed = seed;
        }
        else
        {
            int newSeed = Random.Range(0, int.MaxValue);
            this.seed = newSeed;
            Random.InitState(newSeed);
        }
        Debug.Log("generate using seed " + seed);

        //generate level map
        SubDungeon rootSubDungeon = new SubDungeon(new Rect(wallPadding, wallPadding, boardColsNum, boardRowsNum));
        CreateBSP(rootSubDungeon);
        rootSubDungeon.CreateRoom();
        DrawRooms(rootSubDungeon);
        DrawCorridors(rootSubDungeon);
        DrawWalls();

        //generate level objects
        GetRooms(rootSubDungeon);
        GeneratePlayerAndGoal();
        GenerateRecoveryPacks();
        GenerateEnemies();
        GenerateRerollPoints();
        GenerateUpgradePoints();
    }
}

