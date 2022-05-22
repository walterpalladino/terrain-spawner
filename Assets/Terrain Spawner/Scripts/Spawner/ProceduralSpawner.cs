using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;





public enum PSHit
{
    NO_HIT,
    UNKNOWN_HIT,
    TERRAIN_HIT,
    PLACEHOLDER_HIT
}


public class ProceduralSpawner : MonoBehaviour
{

    [Header("Tree Settings")]
    [SerializeField]
    private bool treesEnabled = true;
    [SerializeField]
    private GameObject[] trees;
    [SerializeField]
    private GameObject treesParent;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float treePresence = 0.5f;
    [SerializeField]
    [Range(1, 10)]
    private int treeGroupSize = 1;
    [SerializeField]
    [Range(1.0f, 15.0f)]
    private float treeGroupRadius = 1.0f;
    [SerializeField]
    [Range(0.5f, 5.0f)]
    private float treeFreeRadius = 0.5f;
    [SerializeField]
    [Range(0.0f, 90.0f)]
    private float treeMaxSlope = 20.0f;
    [SerializeField]
    private float treeMinAltitude = 0.0f;
    [SerializeField]
    private float treeMaxAltitude = 20.0f;

    [Header("Grass Settings")]
    [SerializeField]
    private bool grassEnabled = true;
    [SerializeField]
    private GameObject[] grasses;
    [SerializeField]
    private GameObject grassParent;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float grassPresence = 0.5f;
    [SerializeField]
    [Range(1, 10)]
    private int grassGroupSize = 1;
    [SerializeField]
    [Range(1.0f, 15.0f)]
    private float grassGroupRadius = 1.0f;
    [SerializeField]
    [Range(0.5f, 5.0f)]
    private float grassFreeRadius = 0.5f;
    [SerializeField]
    [Range(0.0f, 90.0f)]
    private float grassMaxSlope = 30.0f;
    [SerializeField]
    private float grassMinAltitude = 0.0f;
    [SerializeField]
    private float grassMaxAltitude = 100.0f;


    [Header("Bushes Settings")]
    [SerializeField]
    private bool bushesEnabled = true;
    [SerializeField]
    private GameObject[] bushes;
    [SerializeField]
    private GameObject bushesParent;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float bushesPresence = 0.5f;
    [SerializeField]
    [Range(1, 10)]
    private int bushesGroupSize = 1;
    [SerializeField]
    [Range(1.0f, 15.0f)]
    private float bushesGroupRadius = 1.0f;
    [SerializeField]
    [Range(0.5f, 5.0f)]
    private float bushesFreeRadius = 0.5f;
    [SerializeField]
    [Range(0.0f, 90.0f)]
    private float bushesMaxSlope = 20.0f;
    [SerializeField]
    private float bushesMinAltitude = 0.0f;
    [SerializeField]
    private float bushesMaxAltitude = 100.0f;


    [Header("Rocks Settings")]
    [SerializeField]
    private bool rocksEnabled = true;
    [SerializeField]
    private GameObject[] rocks;
    [SerializeField]
    private GameObject rocksParent;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float rocksPresence = 0.5f;
    [SerializeField]
    [Range(1, 10)]
    private int rocksGroupSize = 1;
    [SerializeField]
    [Range(1.0f, 15.0f)]
    private float rocksGroupRadius = 1.0f;
    [SerializeField]
    [Range(0.5f, 5.0f)]
    private float rocksFreeRadius = 0.5f;
    [SerializeField]
    [Range(0.0f, 90.0f)]
    private float rocksMaxSlope = 20.0f;
    [SerializeField]
    private float rocksMinAltitude = 0.0f;
    [SerializeField]
    private float rocksMaxAltitude = 100.0f;




    [Header("Random Settings")]
    [SerializeField]
    private int randomSeed = 0;

    [SerializeField]
    [Range(1, 5)]
    private int maxTriesToLocateObjects = 3;


    //public PSTerrainGenerator terrainGenerator;

    private Dictionary<string, GameObject> treesDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> rocksDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> bushesDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> grassDictionary = new Dictionary<string, GameObject>();

    private List<GameObject> treesList = new List<GameObject>();
    private List<GameObject> rocksList = new List<GameObject>();
    private List<GameObject> bushesList = new List<GameObject>();
    private List<GameObject> grassList = new List<GameObject>();


    [Header("Terrain Settings")]
    //    [SerializeField] Vector3 minSpawn;
    //    [SerializeField] Vector3 maxSpawn;
    [SerializeField] float terrainTileSize;

    [SerializeField] Vector3 terrainMin;
    [SerializeField] Vector3 terrainMax;

    [SerializeField] float freeBorderSize;

    [SerializeField] Vector3 terrainCenter;


    [SerializeField] LayerMask terrainIgnoreLayers;

    [SerializeField] float maxDistanceFromCenter = 0.0f;


    // Start is called before the first frame update
    void Start()
    {

        CollectObjects();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Generate()
    {

        CheckRequiredLayers();

        Random.InitState(randomSeed);

        treesList = new List<GameObject>();
        rocksList = new List<GameObject>();
        bushesList = new List<GameObject>();
        grassList = new List<GameObject>();


        //terrainGenerator = FindObjectOfType<PSTerrainGenerator>();

        SetObjectsForTerrain();
        //StartCoroutine("SpawnAllObjectsInMap");
    }


    /*

Layers Required:

16 - Terrain
17 - Trees
18 - Vegetation
19 - Rocks
20 - Buildings
21 - Grass

 
*/
    private void CheckRequiredLayers()
    {
        LayerUtils.CreateLayer("Terrain", 16);
        LayerUtils.CreateLayer("Trees", 17);
        LayerUtils.CreateLayer("Vegetation", 18);
        LayerUtils.CreateLayer("Rocks", 19);
        LayerUtils.CreateLayer("Buildings", 20);
        LayerUtils.CreateLayer("Grass", 21);
    }


    /*
    IEnumerator SpawnAllObjectsInMap()
    {
        for (int z = 0; z < terrainGenerator.terrainDepth; z++)
        {
            for (int x = 0; x < terrainGenerator.terrainWidth; x++)
            {
                float xMin = x * terrainGenerator.terrainTileSize;
                float zMin = z * terrainGenerator.terrainTileSize;
                float xMax = (x + 1) * terrainGenerator.terrainTileSize;
                float zMax = (z + 1) * terrainGenerator.terrainTileSize;

                Debug.Log("Placing objects on area : " + xMin + "/" + zMin + " to " + xMax + "/" + zMax);

                //PlaceGrass(xMin, zMin, xMax, zMax);
                yield return null;
                PlaceTrees(xMin, zMin, xMax, zMax);
                yield return null;
                PlaceRocks(xMin, zMin, xMax, zMax);
                yield return null;
                PlaceBushes(xMin, zMin, xMax, zMax);
                yield return null;

            }
        }
    }
    */

    /*
    private void SetObjectsForTerrain()
    {

        for (int z = 0; z < terrainGenerator.terrainDepth; z++)
        {
            for (int x = 0; x < terrainGenerator.terrainWidth; x++)
            {
                PlaceObjects(x, z);
            }
        }

    }


    public void PlaceObjects(int x, int z)
    {
        float xMin = x * terrainGenerator.terrainTileSize;
        float zMin = z * terrainGenerator.terrainTileSize;
        float xMax = (x + 1) * terrainGenerator.terrainTileSize;
        float zMax = (z + 1) * terrainGenerator.terrainTileSize;

        Debug.Log("Placing objects on area : " + xMin + "/" + zMin + " to " + xMax + "/" + zMax);


        PlaceGrass(xMin, zMin, xMax, zMax);
        PlaceTrees(xMin, zMin, xMax, zMax);
        PlaceRocks(xMin, zMin, xMax, zMax);
        PlaceBushes(xMin, zMin, xMax, zMax);
    }
    */
    /*
        private void SetObjectsForTerrain()
        {
            PlaceObjects(minSpawn.x, maxSpawn.x, minSpawn.z, maxSpawn.z);    
        }
    */

    /*
        public void PlaceObjects(float xMin, float xMax, float zMin, float zMax)
        {

            Debug.Log("Placing objects on area : " + xMin + "/" + zMin + " to " + xMax + "/" + zMax);

            PlaceGrass(xMin, zMin, xMax, zMax);
            PlaceTrees(xMin, zMin, xMax, zMax);
            PlaceRocks(xMin, zMin, xMax, zMax);
            PlaceBushes(xMin, zMin, xMax, zMax);
        }
    */

    private void SetObjectsForTerrain()
    {

        float mapSizeX = terrainMax.x - terrainMin.x;
        float mapSizeY = terrainMax.y - terrainMin.y;
        float mapSizeZ = terrainMax.z - terrainMin.z;

        terrainCenter = new Vector3(mapSizeX / 2.0f, mapSizeY / 2.0f, mapSizeZ / 2.0f);
        terrainCenter += terrainMin; 

        int chunksX = (int)(mapSizeX) / (int)(terrainTileSize) ;
        int chunksZ = (int)(mapSizeZ) / (int)(terrainTileSize) ;

        for (int z = 0; z < chunksZ; z++)
        {
            for (int x = 0; x < chunksX; x++)
            {
                PlaceObjects(x, z, terrainTileSize);
            }
        }

//        PlaceObjects(0, 0, terrainTileSize);
    }

    public void PlaceObjects(int x, int z, float terrainTileSize)
    {

        float xMin = x * terrainTileSize + terrainMin.x;
        float zMin = z * terrainTileSize + terrainMin.z;
        float xMax = (x + 1) * terrainTileSize + terrainMin.x;
        float zMax = (z + 1) * terrainTileSize + terrainMin.z;

        Debug.Log("Placing objects on area : " + xMin + "/" + zMin + " to " + xMax + "/" + zMax);

        PlaceTrees(xMin, zMin, xMax, zMax);
        PlaceRocks(xMin, zMin, xMax, zMax);
        PlaceBushes(xMin, zMin, xMax, zMax);

        PlaceGrass(xMin, zMin, xMax, zMax);
    }



    /*
    private void SetObjectsForTerrain()
    {

        float xMin = terrainXMin;
        float zMin = terrainZMin;
        float xMax = terrainXMax;
        float zMax = terrainZMax;

        PlaceGrass(xMin, zMin, xMax, zMax);
        PlaceTrees(xMin, zMin, xMax, zMax);
        PlaceRocks(xMin, zMin, xMax, zMax);
        PlaceBushes(xMin, zMin, xMax, zMax);

    }
    */
    public static void ChangeLayersRecursively(GameObject go, string name)
    {
        go.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in go.transform)
        {
            ChangeLayersRecursively(child.gameObject, name);
        }
    }

    private void PlaceBushes(float xMin, float zMin, float xMax, float zMax)
    {
        if (bushesEnabled)
        {

            int bushesGroupsQty = (int)(bushesPresence * (xMax - xMin) * (zMax - zMin) / (2 * bushesGroupRadius) / (2 * bushesGroupRadius));
            int bushesPlaced = 0;

            GameObject go = new GameObject();
            go.transform.parent = bushesParent.transform;
            go.name = "" + xMin + "/" + zMin;


            for (int n = 0; n < bushesGroupsQty; n++)
            {

                //                Vector3 centerGroup = GetRandomPosition();
                Vector3 centerGroup = GetGridRandomPosition(xMin, zMin, xMax, zMax, treePresence);

                for (int t = 0; t < bushesGroupSize; t++)
                {

                    Vector3 bushPosition;
                    if (GetItemPosition(centerGroup, xMin, zMin, xMax, zMax, bushesGroupRadius, bushesMaxSlope, bushesMinAltitude, bushesMaxAltitude, bushesFreeRadius, maxTriesToLocateObjects, out bushPosition))
                    {
                        Quaternion orientation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up);

                        GameObject bushPrefab = GetBushPrefab();
                        GameObject instance = Instantiate(bushPrefab, bushPosition, orientation);
                        instance.transform.parent = go.transform;

                        ChangeLayersRecursively(instance, "Vegetation");

                        bushesPlaced++;
                    }

                }

            }
            //Debug.Log("Placed " + bushesPlaced + " bushes of " + bushesGroupsQty + " groups of " + bushesGroupSize + " bush.");
        }

    }

    private void PlaceRocks(float xMin, float zMin, float xMax, float zMax)
    {
        if (rocksEnabled)
        {

            int rocksGroupsQty = (int)(rocksPresence * (xMax - xMin) * (zMax - zMin) / (2 * rocksGroupRadius) / (2 * rocksGroupRadius));
            int rocksPlaced = 0;

            GameObject go = new GameObject();
            go.transform.parent = rocksParent.transform;
            go.name = "" + xMin + "/" + zMin;

            for (int n = 0; n < rocksGroupsQty; n++)
            {

                //                Vector3 centerGroup = GetRandomPosition();
                Vector3 centerGroup = GetGridRandomPosition(xMin, zMin, xMax, zMax, treePresence);

                for (int t = 0; t < rocksGroupSize; t++)
                {

                    Vector3 rockPosition;
                    if (GetItemPosition(centerGroup, xMin, zMin, xMax, zMax, rocksGroupRadius, rocksMaxSlope, rocksMinAltitude, rocksMaxAltitude, rocksFreeRadius, maxTriesToLocateObjects, out rockPosition))
                    {
                        Quaternion orientation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up);

                        GameObject rockPrefab = GetRockPrefab();
                        GameObject instance = Instantiate(rockPrefab, rockPosition, orientation);
                        instance.transform.parent = go.transform;

                        ChangeLayersRecursively(instance, "Rocks");

                        rocksPlaced++;
                    }

                }

            }
            //Debug.Log("Placed " + rocksPlaced + " rocks of " + rocksGroupsQty + " groups of " + rocksGroupSize + " rocks.");
        }

    }

    private void PlaceTrees(float xMin, float zMin, float xMax, float zMax)
    {
        if (treesEnabled)
        {

            int treesGroupsQty = (int)(treePresence * (xMax - xMin) * (zMax - zMin) / (2 * treeGroupRadius) / (2 * treeGroupRadius));
            int treesPlaced = 0;

            GameObject go = new GameObject();
            go.transform.parent = treesParent.transform;
            go.name = "" + xMin + "/" + zMin;

            //  For every group
            for (int n = 0; n < treesGroupsQty; n++)
            {
                //  Get the group center position
                //                Vector3 centerGroup = GetRandomPosition();
                Vector3 centerGroup = GetGridRandomPosition(xMin, zMin, xMax, zMax, treePresence);

                //Debug.Log("New Center Group at: " + centerGroup);

                //  Place the elements of the group
                for (int t = 0; t < treeGroupSize; t++)
                {

                    Vector3 treePosition;
                    if (GetItemPosition(centerGroup, xMin, zMin, xMax, zMax, treeGroupRadius, treeMaxSlope, treeMinAltitude, treeMaxAltitude, treeFreeRadius, maxTriesToLocateObjects, out treePosition))
                    {
                        //  Randomize the orientation
                        Quaternion orientation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up);

                        GameObject treePrefab = GetTreePrefab();

                        GameObject instance = Instantiate(treePrefab, treePosition, orientation);
                        instance.transform.parent = go.transform;

                        ChangeLayersRecursively(instance, "Trees");

                        treesPlaced++;
                        treesList.Add(instance);
                    }

                }

            }
            //Debug.Log("Placed " + treesPlaced + " trees of " + treesGroupsQty + " groups of " + treeGroupSize + " tress.");
        }

    }

    private void PlaceGrass(float xMin, float zMin, float xMax, float zMax)
    {
        if (grassEnabled)
        {

            int grassGroupsQty = (int)(grassPresence * (xMax - xMin) * (zMax - zMin) / (2 * grassGroupRadius) / (2 * grassGroupRadius));
            int grassPlaced = 0;

            GameObject go = new GameObject();
            go.transform.parent = grassParent.transform;
            go.name = "" + xMin + "/" + zMin;

            for (int n = 0; n < grassGroupsQty; n++)
            {

                //                Vector3 centerGroup = GetRandomPosition();
                Vector3 centerGroup = GetGridRandomPosition(xMin, zMin, xMax, zMax, treePresence);

                for (int t = 0; t < grassGroupSize; t++)
                {

                    Vector3 grassPosition;
                    //if (GetGrassPosition(centerGroup, grassGroupRadius, grassMaxSlope, out grassPosition))
                    if (GetItemPosition(centerGroup, xMin, zMin, xMax, zMax, grassGroupRadius, grassMaxSlope, grassMinAltitude, grassMaxAltitude, grassFreeRadius, maxTriesToLocateObjects, out grassPosition))
                    {
                        Quaternion orientation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up);

                        GameObject grassPrefab = GetGrassPrefab();
                        GameObject instance = Instantiate(grassPrefab, grassPosition, orientation);

                        instance.transform.parent = go.transform;

                        ChangeLayersRecursively(instance, "Grass");

                        grassPlaced++;
                    }

                }

            }
            //Debug.Log("Placed " + grassPlaced + " grass of " + grassGroupsQty + " groups of " + grassGroupSize + " grass.");
        }

    }

    private GameObject GetGrassPrefab()
    {
        int idx = Random.Range(0, grasses.Length);
        return grasses[idx];
    }
    /*
    private bool GetGrassPosition(Vector3 centerGroup, float groupRadius, float maxSlope, out Vector3 position)
    {

        //  Obtain a position
        position = new Vector3(Random.Range(terrainMin.x - groupRadius, terrainMax.x + groupRadius), 0, Random.Range(terrainMin.z - groupRadius, terrainMax.z + groupRadius));

        //  Check values are in terrain boundaries
        if ((position.x < terrainMin.x) || (position.x > terrainMax.x) || (position.z < terrainMin.z) || (position.z > terrainMax.z))
        {
            return false;
        }

        float height;
        Vector3 normal;
        if (!GetTerrainHeight(position, out height, out normal))
        {
            return false;
        }
        position.y = height;

        //  Check Min and Max Altitude
        if (height < grassMinAltitude || height > grassMaxAltitude)
        {
            return false;
        }

        if (CheckPlaceholderAt(position))
        {
            return false;
        }

        //  Check max slope
        if (Vector3.Angle(Vector3.up, normal) > maxSlope)
        {
            return false;
        }

        //  Return the position
        return true;
    }
    */
    private GameObject GetTreePrefab()
    {
        int idx = Random.Range(0, trees.Length);
        return trees[idx];
    }

    private GameObject GetBushPrefab()
    {
        int idx = Random.Range(0, bushes.Length);
        return bushes[idx];
    }

    private GameObject GetRockPrefab()
    {
        int idx = Random.Range(0, rocks.Length);
        return rocks[idx];
    }


    private bool OverlapTree(Vector3 position)
    {
        foreach (Transform child in treesParent.transform)
        {
            if (Vector3.Distance(position, child.transform.position) <= treeFreeRadius)
            {
                return true;
            }
        }
        //Debug.Log("Overlapping");
        return false;
    }

    private bool CheckOverlap(Vector3 position, float freeRadius)
    {

        foreach (GameObject go in treesList)
        {
            if (Vector3.Distance(position, go.transform.position) <= freeRadius)
            {
                return true;
            }
        }

        foreach (GameObject go in bushesList)
        {
            if (Vector3.Distance(position, go.transform.position) <= freeRadius)
            {
                return true;
            }
        }

        foreach (GameObject go in rocksList)
        {
            if (Vector3.Distance(position, go.transform.position) <= freeRadius)
            {
                return true;
            }
        }

        foreach (GameObject go in grassList)
        {
            if (Vector3.Distance(position, go.transform.position) <= freeRadius)
            {
                return true;
            }
        }

        return false;
    }

    private void CleanGrassInArea(Vector3 position, float distance)
    {
        foreach (Transform child in grassParent.transform)
        {
            if (Vector3.Distance(position, child.transform.position) <= distance)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }

    /*
    private bool GetTerrainHeight(Vector3 position, out float height, out Vector3 normal)
    {

        height = -1;
        normal = Vector3.zero;

        position.y = 10000f;

        Ray ray = new Ray(position, Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~terrainIgnoreLayers))
        //            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.NameToLayer("Terrain")))
        {
            if (hit.collider.gameObject.GetComponent<PSTerrain>() != null)
            {
                //Debug.Log(hit.collider.gameObject.name);
                height = hit.point.y;
                normal = hit.normal;

                return true;
            }
        }

        return false;
    }
    */
    /*
    private Vector3 GetRandomPosition()
    {
        Vector3 position = new Vector3(Random.Range(terrainMin.x + freeBorderSize, terrainMax.x - freeBorderSize), 0, Random.Range(terrainMin.z + freeBorderSize, terrainMax.z - freeBorderSize));

        return position;
    }
    */
    private Vector3 GetGridRandomPosition(float minX, float minZ, float maxX, float maxZ, float presence)
    {

        float xSize = (maxX - minX) * presence;
        float zSize = (maxZ - minZ) * presence;

        int xGridSize = (int)xSize;
        int zGridSize = (int)zSize;
        //Debug.Log("Grid Size x:" + xGridSize + " z:" + zGridSize);

        float xGridCellSize = (maxX - minX) / xGridSize;
        float zGridCellSize = (maxZ - minZ) / zGridSize;

        //  Select a grid position
        Vector3 position = new Vector3(Random.Range(0, xGridSize), 0, Random.Range(0, zGridSize));

        //  Adjust the position to the center of the grid cell
        position.x = ((int)position.x) + 0.5f;
        position.z = ((int)position.z) + 0.5f;

        //  Scale it based on the cell dimensions
        position.x *= xGridCellSize;
        position.z *= zGridCellSize;

        position.x += minX;
        position.z += minZ;

        return position;
    }
/*
    private Vector3 GetGridRandomPosition(float presence)
    {

        float xSize = (terrainMax.x - terrainMin.x) * presence;
        float zSize = (terrainMax.z - terrainMin.z) * presence;

        int xGridSize = (int)xSize;
        int zGridSize = (int)zSize;
        //Debug.Log("Grid Size x:" + xGridSize + " z:" + zGridSize);

        float xGridCellSize = (terrainMax.x - terrainMin.x) / xGridSize;
        float zGridCellSize = (terrainMax.z - terrainMin.z) / zGridSize;

        //  Select a grid position
        Vector3 position = new Vector3(Random.Range(0, xGridSize), 0, Random.Range(0, zGridSize));

        //  Adjust the position to the center of the grid cell
        position.x = ((int)position.x) + 0.5f;
        position.z = ((int)position.z) + 0.5f;

        //  Scale it based on the cell dimensions
        position.x *= xGridCellSize;
        position.z *= zGridCellSize;

        return position;
    }
*/

    private bool GetItemPosition(Vector3 centerGroup, float xMin, float zMin, float xMax, float zMax, float groupRadius, float maxSlope, float minAltitude, float maxAltitude, float freeRadius, int maxTries, out Vector3 position)
    {

        position = Vector3.zero;
        float height = 0;

        int tryCount = 0;
        bool validPosition = false;

        while (!validPosition && tryCount++ < maxTries)
        {
            //  Obtain a position
            position = GetPositionInArea(centerGroup, groupRadius);

            validPosition = CheckValidPosition(position, maxSlope, minAltitude, maxAltitude, out height);
            position.y = height;
        }

        return validPosition;
    }

    private bool CheckValidPosition(Vector3 position, float maxSlope, float minAltitude, float maxAltitude, out float height)
    {
        height = 0;

        //  Check values are in terrain boundaries
        if ((position.x < terrainMin.x + freeBorderSize) || (position.x > terrainMax.x - freeBorderSize) || (position.z < terrainMin.z + freeBorderSize) || (position.z > terrainMax.z - freeBorderSize))
        {
            //Debug.Log(position);
            return false;
        }

        //  Check if there is a maximum distance set and the distance to the center if bigger than that
        float distance = (position - terrainCenter).magnitude;
        if (maxDistanceFromCenter > 0.0f && maxDistanceFromCenter < distance)
        {
            return false;
        }


        Vector3 normal;

        PSHit psHit = CheckAt(position, out height, out normal);

        //        if (!GetTerrainHeight(position, out height, out normal))
        if (psHit != PSHit.TERRAIN_HIT)
        {
            return false;
        }
        position.y = height;

        //  Check Min and Max Altitude
        if (height < minAltitude || height > maxAltitude)
        {
            //Debug.Log(height);
            //Debug.Log(position);
            return false;
        }
        /*
                if (CheckPlaceholderAt(position))
                {
                    return false;
                }
        */
        //  Check max slope
        if (Vector3.Angle(Vector3.up, normal) > maxSlope)
        {
            return false;
        }

        //if (OverlapTree(position))
        if (CheckOverlap(position, treeFreeRadius))
        {
            return false;
        }
        /*
                if (clearGrass)
                {
                    //  If overlaps any grass remove the grass
                    CleanGrassInArea(position, freeRadius);
                }
        */
        //  Return the position
        return true;

    }

    private Vector3 GetPositionInArea(Vector3 centerGroup, float groupRadius)
    {
        Vector3 position;

        //position = new Vector3(Random.Range(xMin - groupRadius, xMax + groupRadius), 0, Random.Range(zMin - groupRadius, zMax + groupRadius));
        position = new Vector3(Random.Range(centerGroup.x - groupRadius, centerGroup.x + groupRadius), 0, Random.Range(centerGroup.z - groupRadius, centerGroup.z + groupRadius));


        float angle = Random.Range(0, 2 * Mathf.PI);
        float distance = Random.Range(0, groupRadius);
        position = centerGroup + groupRadius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

        return position;
    }

    //private bool GetItemPosition(Vector3 centerGroup, float groupRadius, float maxSlope, float minAltitude, float maxAltitude, float freeRadius, bool clearGrass, out Vector3 position)
    //{

    //    //  Obtain a position
    //    position = new Vector3(Random.Range(terrainXMin - groupRadius, terrainXMax + groupRadius), 0, Random.Range(terrainZMin - groupRadius, terrainZMax + groupRadius));

    //    //  Check values are in terrain boundaries
    //    if ((position.x < terrainXMin) || (position.x > terrainXMax) || (position.z < terrainZMin) || (position.z > terrainZMax))
    //    {
    //        return false;
    //    }

    //    float height;
    //    Vector3 normal;

    //    PSHit psHit = CheckAt(position, out height, out normal);

    //    //        if (!GetTerrainHeight(position, out height, out normal))
    //    if (psHit != PSHit.TERRAIN_HIT)
    //    {
    //        return false;
    //    }
    //    position.y = height;

    //    //  Check Min and Max Altitude
    //    if (height < minAltitude || height > maxAltitude)
    //    {
    //        return false;
    //    }
    //    /*
    //            if (CheckPlaceholderAt(position))
    //            {
    //                return false;
    //            }
    //    */
    //    //  Check max slope
    //    if (Vector3.Angle(Vector3.up, normal) > maxSlope)
    //    {
    //        return false;
    //    }

    //    if (OverlapTree(position))
    //    {
    //        return false;
    //    }

    //    if (clearGrass)
    //    {
    //        //  If overlaps any grass remove the grass
    //        CleanGrassInArea(position, freeRadius);
    //    }

    //    //  Return the position
    //    return true;
    //}

    /*
    private bool CheckPlaceholderAt(Vector3 position)
    {

        position.y = 10000f;

        Ray ray = new Ray(position, Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        //            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.NameToLayer("Terrain")))
        {
            if (hit.collider.gameObject.GetComponent<PSPlaceholder>() != null)
            {
                return true;
            }
        }

        return false;
    }
    */

    private PSHit CheckAt(Vector3 position, out float height, out Vector3 normal)
    {

        height = -1;
        normal = Vector3.zero;

        position.y = 10000f;

        Ray ray = new Ray(position, Vector3.down);

        RaycastHit hit;

//        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~terrainIgnoreLayers))

        {
            //Debug.Log("Hit object : " + hit.collider.gameObject.name + " with layer : " + LayerMask.LayerToName(hit.collider.gameObject.layer));
            if (hit.collider.gameObject.GetComponent<PSPlaceholder>() != null)
            {
                height = hit.point.y;
                normal = hit.normal;

                return PSHit.PLACEHOLDER_HIT;
            }
            else
            //            if (hit.collider.gameObject.GetComponent<PSTerrain>() != null)
            if (LayerMask.LayerToName(hit.collider.gameObject.layer).Equals("Terrain"))
            {
                height = hit.point.y;
                normal = hit.normal;

                return PSHit.TERRAIN_HIT;
            }
            //  Check terrain meshes using LOD
            //if (hit.collider.gameObject.name.Contains("LOD0") && hit.collider.transform.parent.gameObject.GetComponent<PSTerrain>() != null)
            if (hit.collider.gameObject.name.Contains("LOD0") && LayerMask.LayerToName(hit.collider.gameObject.layer).Equals("Terrain"))
            {
                height = hit.point.y;
                normal = hit.normal;

                return PSHit.TERRAIN_HIT;
            }
            else
            {
                return PSHit.UNKNOWN_HIT;
            }
        }

        return PSHit.NO_HIT;
    }


    public GameObject[] GetPois()
    {
        PSTown[] towns = FindObjectsOfType<PSTown>();

        GameObject[] pois = new GameObject[towns.Length];

        for (int n = 0; n < towns.Length; n++)
        {
            pois[n] = towns[n].gameObject;
        }

        return pois;
    }

    public PSRoad[] GetRoads()
    {
        PSRoad[] roads = GetComponent<PSRoadNetwork>().Roads;
        return roads;
    }

    public void Clear()
    {

        if (Application.isPlaying)
        {

            foreach (Transform child in grassParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            foreach (Transform child in treesParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            foreach (Transform child in bushesParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            foreach (Transform child in rocksParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

        }
        else
        {

            while (grassParent.transform.childCount != 0)
            {
                DestroyImmediate(grassParent.transform.GetChild(0).gameObject);
            }

            while (treesParent.transform.childCount != 0)
            {
                DestroyImmediate(treesParent.transform.GetChild(0).gameObject);
            }

            while (bushesParent.transform.childCount != 0)
            {
                DestroyImmediate(bushesParent.transform.GetChild(0).gameObject);
            }

            while (rocksParent.transform.childCount != 0)
            {
                DestroyImmediate(rocksParent.transform.GetChild(0).gameObject);
            }
        }
    }


    private void CollectObjects()
    {
        foreach (Transform child in treesParent.transform)
        {
            treesDictionary.Add(child.gameObject.name, child.gameObject);
        }
        foreach (Transform child in rocksParent.transform)
        {
            rocksDictionary.Add(child.gameObject.name, child.gameObject);
        }
        foreach (Transform child in bushesParent.transform)
        {
            bushesDictionary.Add(child.gameObject.name, child.gameObject);
        }
        foreach (Transform child in grassParent.transform)
        {
            grassDictionary.Add(child.gameObject.name, child.gameObject);
        }
    }
    /*
    public void HideObjects(int x, int z)
    {
        if (x < 0 || z < 0 || x >= terrainGenerator.terrainWidth || z >= terrainGenerator.terrainDepth)
        {
            return;
        }

        string key = "" + x * terrainGenerator.terrainTileSize + "/" + z * terrainGenerator.terrainTileSize;

        treesDictionary[key].SetActive(false);
        rocksDictionary[key].SetActive(false);
        bushesDictionary[key].SetActive(false);
        grassDictionary[key].SetActive(false);
    }

    public void ShowObjects(int x, int z)
    {
        if (x < 0 || z < 0 || x >= terrainGenerator.terrainWidth || z >= terrainGenerator.terrainDepth)
        {
            return;
        }

        string key = "" + x * terrainGenerator.terrainTileSize + "/" + z * terrainGenerator.terrainTileSize;

        treesDictionary[key].SetActive(true);
        rocksDictionary[key].SetActive(true);
        bushesDictionary[key].SetActive(true);
        grassDictionary[key].SetActive(true);
    }
    */
}