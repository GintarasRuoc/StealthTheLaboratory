using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask miniMapMask;

    private float yPos;
    private GameObject previousTile;

    private MapGenerator map;
    private int offsetMultiplier;
    private int[,] mapLayout;

    [SerializeField] private Material currentTileMaterial;
    [SerializeField] private Material availableTileMaterial;
    [SerializeField] private Material beenTileMaterial;

    void Start()
    {
        map = GameObject.Find("GameManager").GetComponent<MapGenerator>();
        offsetMultiplier = map.getOffsetMultiplier();
        
        yPos = transform.position.y;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(target.position.x, yPos, target.position.z);

        updateMinimap();
    }

    // If player entered different room, update minimap information
    private void updateMinimap()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 220f, miniMapMask))
        {
            if (previousTile != hit.transform.gameObject)
            {
                showAdjacentTiles(hit.transform.gameObject);
                if (previousTile != null)
                    previousTile.GetComponent<Renderer>().material = beenTileMaterial;
                previousTile = hit.transform.gameObject;
            }
        }
    }

    // Check for tiles and update minimap
    private void showAdjacentTiles(GameObject currentTile)
    {
        int x = Mathf.CeilToInt(currentTile.transform.position.x / offsetMultiplier);
        int z = Mathf.CeilToInt(currentTile.transform.position.z / offsetMultiplier);
        mapLayout = map.getMapLayout();
        if (mapLayout != null)
        {
            // check hit center tile
            if(mapLayout[x, z] == 1)
                changeTile(x, z, true);
            // check hit top tile
            if (mapLayout[x + 1, z] == 1 && mapLayout.GetLength(0) > (x + 1))
                changeTile(x + 1, z, false);
            // check hit right tile
            if (mapLayout[x, z + 1] == 1 && mapLayout.GetLength(1) > (z + 1))
                changeTile(x, z + 1, false);
            // check hit left tile
            if (mapLayout[x - 1, z] == 1 && x - 1 >= 0)
                changeTile(x - 1, z, false);
            // check hit bottom tile
            if (mapLayout[x, z - 1] == 1 && z - 1 >= 0)
                changeTile(x, z - 1, false);
        }
    }

    // Get tile renderer information and change tile color
    private void changeTile(int x, int z, bool isCurrent)
    {
        Collider[] colliders = Physics.OverlapSphere(new Vector3(x * offsetMultiplier, -10, z * offsetMultiplier), 1f);
        if (colliders.Length != 0) {
            Renderer temp = colliders[0].GetComponent<Renderer>();
            if (isCurrent)
                temp.material = currentTileMaterial;
            else if (temp.sharedMaterial != beenTileMaterial)
            {
                temp.material = availableTileMaterial;
                colliders[0].GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    // Show all map ( map skill)
    public void showAllTiles()
    {
        map = GameObject.Find("GameManager").GetComponent<MapGenerator>();
        offsetMultiplier = map.getOffsetMultiplier();
        mapLayout = map.getMapLayout();
        for (int i = 0; i < mapLayout.GetLength(0); i++)
            for (int j = 0; j < mapLayout.GetLength(1); j++)
                if (mapLayout[i, j] == 1)
                    changeTile(i, j, false);
        previousTile = null;
    }

    private void OnDrawGizmos()
    {
        if (previousTile != null)
        {
            int x = Mathf.CeilToInt(previousTile.transform.position.x / offsetMultiplier);
            int z = Mathf.CeilToInt(previousTile.transform.position.z / offsetMultiplier);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(new Vector3(x * offsetMultiplier, -10, z * offsetMultiplier), 1f);
        }
    }
}
