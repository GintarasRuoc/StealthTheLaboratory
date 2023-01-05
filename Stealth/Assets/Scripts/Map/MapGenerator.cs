using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private LayerMask doorMask;
    
    [SerializeField]
    [Tooltip("Spawn rooms further from center (number should be between 0 and 1). The bigger the number is it's more likely to spawn room further from center.")]
    private float roomFromCenter = 0;

    // Chances to get certaing size room
    [Header("Room size chances")]
    [SerializeField]
    private int oneTileSizeRoomChance = 50; // 50 Default
    [SerializeField]
    private int twoTilesSizeRoomChance = 30; // 30 Default
    [SerializeField]
    private int threeTilesSizeRoomChance = 15; // 15 Default
    [SerializeField]
    private int fourTilesSizeRoomChance = 5; // 5 Default

    // room spacing
    [Header("Room spacing")]
    [SerializeField]
    private int tileOffsetMultiplier = 50;

    // Room count generation info
    [Header("Room count")]
    [SerializeField]
    private int minTileCount = 10;
    [SerializeField]
    private int maxTileCount = 20;
    private int chosenTileCount;
    private int tilesToSpawn;

    // Full map info
    private Transform mapParent;
    private int[,] mapLayout;
    private List<Coordinates> allUsedTiles;
    private int centerTile;
    private int mapSize;
    private List<GameObject> spawnedRooms;

    // Room prefabs
    [Header("Map prefabs")]
    // One SizeRooms
    [SerializeField]
    private GameObject[] startingRooms;
    [SerializeField]
    private GameObject[] oneSizeRooms;
    // Two Size Rooms
    [SerializeField]
    private GameObject[] twoSizeRooms;
    // Three Size Rooms
    [SerializeField]
    private GameObject[] threeSizeCornerRooms;
    [SerializeField]
    private GameObject[] threeSizeLongRooms;
    // Four size Rooms
    [SerializeField]
    private GameObject[] fourSizeLongRooms;
    [SerializeField]
    private GameObject[] fourSizeLShapeRooms;
    [SerializeField]
    private GameObject[] fourSizeSquareRooms;
    [SerializeField]
    private GameObject[] fourSizeZShapeRooms;

    private void Start()
    {
        StartCoroutine(removeDoors());
    }

    // Function starts to generate map
    public int startGeneratingMap()
    {
        if (startingRooms.Length > 0 && oneSizeRooms.Length > 0 && twoSizeRooms.Length > 0 && threeSizeLongRooms.Length > 0 && threeSizeCornerRooms.Length > 0 &&
            fourSizeLongRooms.Length > 0 && fourSizeLShapeRooms.Length > 0 && fourSizeSquareRooms.Length > 0 && fourSizeZShapeRooms.Length > 0)
        {
            Debug.Log("GameManger // GameMasterController >> MapGenerator // Starting to spawn rooms");

            mapParent = Instantiate(new GameObject(), transform).transform;
            mapParent.name = "GeneratedMap";

            tilesToSpawn = chosenTileCount = Random.Range(minTileCount, maxTileCount);
            mapSize = chosenTileCount * 2 - 1;
            mapLayout = new int[mapSize, mapSize];
            allUsedTiles = new List<Coordinates>();
            spawnedRooms = new List<GameObject>();

            Debug.Log("GameManger // GameMasterController >> MapGenerator // Chosen total room count " + chosenTileCount);

            generateMap();

            GetComponent<NavMeshSurface>().BuildNavMesh();

            Debug.Log("GameManger // GameMasterController >> MapGenerator // Finished spawning map");

            //debugAvailability();
            debugBaseMapInformation();

            return centerTile * tileOffsetMultiplier;
        }
        else
        {
            Debug.Log("GameManger // GameMasterController >> MapGenerator // Room prefabs has empty list");
            return 0;
        }
                
    }

    // Function builds rooms on loop till rooms to spawn number reaches zero
    private void generateMap()
    {
        // Count what is combined chance of rooms for getting random number (Random.Range(1,3) means it can get 1 and 2, to get 3 max needs +1) 
        int maxChance = oneTileSizeRoomChance + twoTilesSizeRoomChance + threeTilesSizeRoomChance + fourTilesSizeRoomChance + 1;
        // For more readable code of ifs. Won't need to chech if the number is beetween, only needs to check if lower
        twoTilesSizeRoomChance += oneTileSizeRoomChance;
        threeTilesSizeRoomChance += twoTilesSizeRoomChance;

        spawnStartingRoom();

        while (tilesToSpawn > 0)
        {

            int randomTile = 0;
            List<string> availableTilePaths = new List<string>();
            // Get room, which has empty tiles next to it
            // If there is a room that has no space next to it, that room is removed from list and a new room is being chosen
            while (availableTilePaths.Count <= 0)
            {
                int roomMin = Mathf.FloorToInt((allUsedTiles.Count - 1) * roomFromCenter);
                randomTile = Random.Range(roomMin, allUsedTiles.Count);
                availableTilePaths = getTileAvailability("", allUsedTiles[randomTile], new Coordinates(-1, -1), 1);
                
                if (availableTilePaths.Count <= 0)
                    allUsedTiles.RemoveAt(randomTile);
            }

            // Randomly chooses, which size of room to spawn
            int randomNumber = Random.Range(1, maxChance);
            if (randomNumber <= oneTileSizeRoomChance || tilesToSpawn <= 1)
            {
                trySpawningRoom(allUsedTiles[randomTile], 1);
            }
            else if (randomNumber <= twoTilesSizeRoomChance || tilesToSpawn <= 2)
            {
                trySpawningRoom(allUsedTiles[randomTile], 2);
            }
            else if (randomNumber <= threeTilesSizeRoomChance || tilesToSpawn <= 3)
            {
                trySpawningRoom(allUsedTiles[randomTile], 3);
            }
            else
            {
                trySpawningRoom(allUsedTiles[randomTile], 4);
            }

        }
    }

    // Spawns a starting room at the center of grid
    private void spawnStartingRoom()
    {
        // Find and set center tile coordinate
        centerTile = Mathf.FloorToInt(mapSize / 2);

        // Randomly choose which room to spawn from list of starting rooms
        int randomNumber = Random.Range(0, startingRooms.Length);
        GameObject roomToSpawn = startingRooms[randomNumber];

        // Tiles used to spawn object
        List<Coordinates> tiles = new List<Coordinates>();
        tiles.Add(new Coordinates(centerTile, centerTile));

        deployRoom(tiles, roomToSpawn, new Quaternion(0, 0, 0, 0), false, false, 0, 0);
    }

    // Tries to spawn a room next to specific tile
    // baseTile - tile to which the room will be placed to
    // roomSize - size of a room that will be spawned
    private void trySpawningRoom(Coordinates baseTile, int roomSize)
    {
        List<string> availableTilePaths = getTileAvailability("", baseTile, new Coordinates(-1, -1), roomSize);

        if (availableTilePaths.Count <= 0)
        {
            if (roomSize > 1)
            {
                Debug.Log("GameManger // GameMasterController >> MapGenerator // Couldn't fit " + roomSize + " size room, will try to spawn smaller room by 1 size");
                trySpawningRoom(baseTile, roomSize - 1);
            }
            else
            {
                Debug.Log("GameManger // GameMasterController >> MapGenerator // Something went wrong, couldn't spawn even 1 size room");
            }
        }
        else
        {
            // Randomly choose tile path, which will be used to spawn room and add those tiles to list
            int randomPath = Random.Range(0, availableTilePaths.Count);
            string tilePath = availableTilePaths[randomPath];
            //Debug.Log("Tile path " + tilePath);
            List<Coordinates> tilesToUse = getTiles(baseTile, tilePath);

            // Get room that will fit the path
            GameObject roomToSpawn = getRoom(tilePath);

            // Get room rotation
            Quaternion rotation;
            rotation = getRoomRotation(tilePath);

            // Check if room need to be scaled on z axis by -1
            bool zFlip = false;
            if (tilePath.Length > 2)
                zFlip = isZFlip(tilePath);

            int xOffset = 0;
            int zOffset = 0;
            bool xFlip = false;
            // Function here checks if room is L shaped and needs to be spawned from the other end of room. Then decides offset needed to spawn room correctly
            if (tilePath.Length > 3 && checkIfBackwardsLShape(tilePath))
            {
                //Debug.Log("Room is backwards L shape");
                xFlip = true;
                switch (tilePath.Substring(1))
                {
                    case "211":
                        xOffset = -2;
                        zOffset = 1;
                        break;
                    case "411": // flip z
                        xOffset = -2;
                        zOffset = -1;
                        break;
                    case "122": // flip z
                        xOffset = -1;
                        zOffset = 2;
                        break;
                    case "322":
                        xOffset = 1;
                        zOffset = 2;
                        break;
                    case "233": // flip z
                        xOffset = 2;
                        zOffset = 1;
                        break;
                    case "433":
                        xOffset = 2;
                        zOffset = -1;
                        break;
                    case "144":
                        xOffset = -1;
                        zOffset = -2;
                        break;
                    case "344": // flip z
                        xOffset = 1;
                        zOffset = -2;
                        break;
                }

                /*Debug.Log("Starting tile: " + tilesToUse[0].getX() + " " + tilesToUse[0].getZ());
                Debug.Log("Tile path: " + tilePath);
                Debug.Log("zFlip - " + zFlip + " xFlip - " + xFlip + " xOffset: " + xOffset + " zOffset: " + zOffset);*/
            }

            deployRoom(tilesToUse, roomToSpawn, rotation, xFlip, zFlip, xOffset, zOffset);
        }
    }

    // Recursivly gets all available tile paths to spawn certain room size
    // currentTiles - currently created tile path
    // checkTile - tile to which all available paths will be found, to spwan certain size room
    // lastTile - tile which was used last in a tile path ( to negate paths going backwards into itself)
    // tileCount - room size that map is trying to generate
    private List<string> getTileAvailability(string currentTiles, Coordinates checkTile, Coordinates lastTile, int tileCount)
    {
        List<string> tilesStrings = new List<string>();

        int xCheck = checkTile.getX();
        int zCheck = checkTile.getZ();

        int xLast = lastTile.getX();
        int zLast = lastTile.getZ();

        // top tile availability check 
        if (xCheck - 1 >= 0 && mapLayout[xCheck - 1, zCheck] == 0 && !compareTiles(new Coordinates(xCheck - 1, zCheck), lastTile))
        {
            if (tileCount > 1)
                tilesStrings.AddRange(getTileAvailability(currentTiles + "1", new Coordinates(xCheck - 1, zCheck), checkTile, tileCount - 1));
            else
                tilesStrings.Add(currentTiles + "1");
        }
        // right tile availability check
        if (zCheck + 1 <= mapSize - 1 && mapLayout[xCheck, zCheck + 1] == 0 && !compareTiles(new Coordinates(xCheck, zCheck + 1), lastTile))
        {
            if (tileCount > 1)
                tilesStrings.AddRange(getTileAvailability(currentTiles + "2", new Coordinates(xCheck, zCheck + 1), checkTile, tileCount - 1));
            else
                tilesStrings.Add(currentTiles + "2");
        }
        // bottom tile availability check
        if (xCheck + 1 <= mapSize - 1 && mapLayout[xCheck + 1, zCheck] == 0 && !compareTiles(new Coordinates(xCheck + 1, zCheck), lastTile))
        {
            if (tileCount > 1)
                tilesStrings.AddRange(getTileAvailability(currentTiles + "3", new Coordinates(xCheck + 1, zCheck), checkTile, tileCount - 1));
            else
                tilesStrings.Add(currentTiles + "3");
        }
        // left tile availability check
        if (zCheck - 1 >= 0 && mapLayout[xCheck, zCheck - 1] == 0 && !compareTiles(new Coordinates(xCheck, zCheck - 1), lastTile))
        {
            if (tileCount > 1)
                tilesStrings.AddRange(getTileAvailability(currentTiles + "4", new Coordinates(xCheck, zCheck - 1), checkTile, tileCount - 1));
            else
                tilesStrings.Add(currentTiles + "4");
        }

        return tilesStrings;
    }

    // Function gets all tile from tile path in List coordinates forms
    // baseTile - tile to which the room will be placed to
    // tilePath - tile path that will be used to spawn room
    private List<Coordinates> getTiles(Coordinates baseTile, string tilePath)
    {
        List<Coordinates> result = new List<Coordinates>();

        Coordinates tempTile = getTile(baseTile, tilePath[0]);
        result.Add(tempTile);

        if (tilePath.Length > 1)
            foreach (char temp in tilePath.Substring(1))
            {
                tempTile = getTile(result[result.Count - 1], temp);
                result.Add(tempTile);
            }

        return result;
    }

    // Function gets coordinates of one tile based on given tile and which will be next
    // baseTile - tile from which next tile coordinates will be found
    // nextTile - number that shows which side next tile is ( 1 - top; 2 - right; 3 - bottoms; 4 - left)
    private Coordinates getTile(Coordinates baseTile, char nextTile)
    {
        Coordinates result = new Coordinates();

        switch (nextTile)
        {
            case '1':
                result = new Coordinates(baseTile.getX() - 1, baseTile.getZ());
                break;
            case '2':
                result = new Coordinates(baseTile.getX(), baseTile.getZ() + 1);
                break;
            case '3':
                result = new Coordinates(baseTile.getX() + 1, baseTile.getZ());
                break;
            case '4':
                result = new Coordinates(baseTile.getX(), baseTile.getZ() - 1);
                break;
        }

        return result;
    }

    // Function gets prefab that will be used to spawn
    // tilePath - tile path that is used to find a specific room to spawn
    private GameObject getRoom(string tilePath)
    {
        GameObject result;
        int randomRoom;

        switch (tilePath.Length)
        {
            case 4:
                result = getFourSizeRoom(tilePath);
                break;
            case 3:
                result = getThreeSizeRoom(tilePath);
                break;
            case 2:
                randomRoom = Random.Range(0, twoSizeRooms.Length);
                result = twoSizeRooms[randomRoom];
                break;
            default:
                randomRoom = Random.Range(0, oneSizeRooms.Length);
                result = oneSizeRooms[randomRoom];
                break;
        }

        return result;
    }

    // Function gets three size room that fits in tile path
    // tilePath - tile path that is used to spawn room
    private GameObject getThreeSizeRoom(string tilePath)
    {
        GameObject result;
        int randomRoom;
        if (tilePath[1] == tilePath[2])
        {
            randomRoom = Random.Range(0, threeSizeLongRooms.Length);
            result = threeSizeLongRooms[randomRoom];
        }
        else
        {
            randomRoom = Random.Range(0, threeSizeCornerRooms.Length);
            result = threeSizeCornerRooms[randomRoom];
        }
        return result;
    }

    // Function gets four size room that fits in tile path
    // tilePath - tile path that is used to spawn room
    private GameObject getFourSizeRoom(string tilePath)
    {
        GameObject result = new GameObject();
        int randomRoom;
        // Check if square shape room is needed
        if (checkIfSquareShape(tilePath))
        {
            randomRoom = Random.Range(0, fourSizeSquareRooms.Length);
            result = fourSizeSquareRooms[randomRoom];
        }
        // Check if L shape room is needed
        else if (checkIfLShape(tilePath) || checkIfBackwardsLShape(tilePath))
        {
            randomRoom = Random.Range(0, fourSizeLShapeRooms.Length);
            result = fourSizeLShapeRooms[randomRoom];
        }
        // Check if Z shape room is needed
        else if (checkIfZShape(tilePath))
        {
            randomRoom = Random.Range(0, fourSizeZShapeRooms.Length);
            result = fourSizeZShapeRooms[randomRoom];
        }
        // Long room are only left possible
        else
        {
            randomRoom = Random.Range(0, fourSizeLongRooms.Length);
            result = fourSizeLongRooms[randomRoom];
        }

        return result;
    }

    // Function gets room rotation based on second or third number in tile path
    // tilePath - tile path that is used to spawn room
    private Quaternion getRoomRotation(string tilePath)
    {
        int yRotation;
        int i;
        // Checks if room that is being spawn is more than two tile size
        if (tilePath.Length >= 2)
        {
            // Checks if room that is being spawned is L shaped and needs to be spawned backwards
            if (tilePath.Length > 3 && checkIfBackwardsLShape(tilePath))
                i = 2;
            else i = 1;
            // 1 - top; 2 - right; 3 - bottom; 4 - left
            switch (tilePath[i])
            {
                case '1':
                    yRotation = 180;
                    break;
                case '2':
                    yRotation = 270;
                    break;
                case '3':
                    yRotation = 0;
                    break;
                case '4':
                    yRotation = 90;
                    break;
                default:
                    yRotation = Random.Range(0, 4) * 90;
                    break;
            }
        }
        // Rooms is one tile size, so it randomly rotates to give it variety
        else yRotation = Random.Range(0, 4) * 90;

        return Quaternion.Euler(0, yRotation, 0);
    }

    // Function checks if room with given path will need to be scaled on z axis by -1
    // tilePath - path that was randomly chosen on which to spawn a room
    private bool isZFlip(string tilePath)
    {
        switch (tilePath.Substring(1))
        {
            // check for three size corner room
            case "12": case "23": case "34": case "41":
            // check for four size z room
            case "121": case "232": case "343": case "414":
            // check for four size sqaure room
            case "123": case "234": case "341": case "412":
            // check for four size l room
            case "112": case "223": case "334": case "441":
            // check for four size l room, which starts at the other end
            case "122": case "233": case "344": case "411":
                return true;
            default:
                return false;
        }
    }

    // Function deploys prefab (room) in a scene
    // tiles - tiles that were used to spawn a room
    // spawnPrefab - room that will be spawned in a scene
    // rotation - y rotation room
    // flipX - if room needs to be scaled in x axis by -1
    // flipZ - if room needs to be scaled in z axis by -1
    // xOffset - offset needed in x axis
    // zOffset - offset needed in z axis
    private void deployRoom(List<Coordinates> tiles, GameObject spawnPrefab, Quaternion rotation, bool flipX, bool flipZ, int xOffset, int zOffset)
    {
        // Get coordinates where to spawn room
        Vector3 instantiatePosition = new Vector3((tiles[0].getX() + xOffset) * tileOffsetMultiplier, 0, (tiles[0].getZ() + zOffset) * tileOffsetMultiplier);
        // Deploy prefab with given information
        GameObject temp = Instantiate(spawnPrefab, instantiatePosition, rotation, mapParent);
        // If object needs to be flipped
        Vector3 newScale = temp.transform.localScale;
        if (flipX)
            newScale.x *= -1f;
        if (flipZ)
            newScale.z *= -1f;
        temp.transform.localScale = newScale;

        unParent(temp);

        foreach (NavMeshObstacle t in temp.GetComponentsInChildren<NavMeshObstacle>())
        {
            if (!t.name.Equals("BackWall"))
            {
                if (flipX || flipZ)
                {
                    t.transform.parent = transform;
                    Destroy(t.transform.GetComponent<MeshCollider>());
                    t.gameObject.AddComponent<MeshCollider>();
                }
            }
        }

        spawnedRooms.Add(temp);
        // Fill information for map generation
        foreach (Coordinates tempTile in tiles)
        {
            mapLayout[tempTile.getX(), tempTile.getZ()] = 1;
            allUsedTiles.Add(tempTile);
            //removeDoor(tempTile);
            tilesToSpawn--;
        }
    }

    private void unParent(GameObject room)
    {
        foreach(NavMeshAgent agent in room.GetComponentsInChildren<NavMeshAgent>())
        {
            agent.transform.parent = transform;
            agent.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private IEnumerator removeDoors()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < mapSize; i++)
            for (int j = 0; j < mapSize; j++)
                if (mapLayout[i, j] == 1)
                    removeDoor(new Coordinates(i, j));
    }

    // Function checks all possible door locations
    private void removeDoor(Coordinates tile)
    {
        destroyDoor(new Vector3(tile.getX() * tileOffsetMultiplier - tileOffsetMultiplier / 2, 0, tile.getZ() * tileOffsetMultiplier));
        destroyDoor(new Vector3(tile.getX() * tileOffsetMultiplier, 0, tile.getZ() * tileOffsetMultiplier - tileOffsetMultiplier / 2));
        destroyDoor(new Vector3(tile.getX() * tileOffsetMultiplier + tileOffsetMultiplier / 2, 0, tile.getZ() * tileOffsetMultiplier));
        destroyDoor(new Vector3(tile.getX() * tileOffsetMultiplier, 0, tile.getZ() * tileOffsetMultiplier + tileOffsetMultiplier / 2));
    }

    // Function destroys door object, if there needs to be a passage
    private void destroyDoor(Vector3 pos)
    {
        
        Collider[] colliders;
        colliders = Physics.OverlapSphere(pos, 5f, doorMask);
        if (colliders.Length > 1)
            foreach (Collider door in colliders)
                Destroy(door.gameObject);
    }

    // Compares if two tiles are the same
    private bool compareTiles(Coordinates tiles1, Coordinates tiles2)
    {
        if (tiles1.getX() == tiles2.getX() && tiles1.getZ() == tiles2.getZ())
            return true;
        return false;
    }

    // Checks if four size room is long shaped
    // tilePath - path that is used to spawnn room
    private bool checkIfLongShape(string tilePath)
    {
        if (tilePath[1] == tilePath[2] && tilePath[2] == tilePath[3])
            return true;
        return false;
    }

    // Checks if four size room is long shaped
    // tilePath - path that is used to spawnn room
    private bool checkIfZShape(string tilePath)
    {
        if (tilePath[1] != tilePath[2] && tilePath[1] == tilePath[3])
            return true;
        return false;
    }

    // Checks if four size room is L shaped
    // tilePath - path that is used to spawnn room
    private bool checkIfLShape(string tilePath)
    {
        if (tilePath[1] == tilePath[2] && tilePath[1] != tilePath[3])
            return true;
        return false;
    }

    // Checks if four size room is L shape and needs to be spawned backwards
    // tilePath - path that is used to spawnn room
    private bool checkIfBackwardsLShape(string tilePath)
    {
        if (tilePath[1] != tilePath[2] && tilePath[2] == tilePath[3])
            return true;
        return false;
    }

    // Checks if four size room is square shaped
    // tilePath - path that is used to spawnn room
    private bool checkIfSquareShape(string tilePath)
    {
        if (tilePath[1] != tilePath[2] && tilePath[2] != tilePath[3] && tilePath[1] != tilePath[3])
            return true;
        return false;
    }

    public List<GameObject> getSpawnedRooms()
    {
        return spawnedRooms;
    }

    public int[,] getMapLayout()
    {
        return mapLayout;
    }

    public int getOffsetMultiplier()
    {
        return tileOffsetMultiplier;
    }

    // Debugs information about available rooms
    private void debugAvailability()
    {
        centerTile = Mathf.FloorToInt(mapSize / 2);
        mapLayout[centerTile, centerTile] = 1;
        allUsedTiles.Add(new Coordinates(centerTile, centerTile));
        List<string> availability = getTileAvailability("", allUsedTiles[0], new Coordinates(-1, -1), 4);

        foreach (string temp in availability)
        {
            Debug.Log(temp);
        }
        Debug.Log(availability.Count);
    }

    // Debugs map layout ( 4 - start point, 3 - room, 0 - empty)
    private void debugBaseMapInformation()
    {

        string info = "";
        for (int i = 0; i < mapSize; i++)
        {

            for (int j = 0; j < mapSize; j++)
            {
                if (i == centerTile && j == centerTile)
                    info += 4;
                else if (mapLayout[i, j] == 1)
                    info += 3;
                else info += 0;
            }

            info += "\n";
        }
        Debug.Log(info);
    }
}
