using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TileSpawner : MonoBehaviour {

    [Header("Spawning")] ////////////////////////////////

    /// <summary>
    /// Number of tiles loaded in at a time. Can be adjusted to suit performance and screen size needs.
    /// </summary>
    [SerializeField] private int m_loadedTiles = 8;
    public int LoadedTiles { get { return m_loadedTiles; } set { m_loadedTiles = value; } }

    /// <summary>
    /// Queue of the currenly moving tiles.
    /// </summary>
    public LinkedList<GameObject> ActiveTiles;

    /// <summary>
    /// Queue of the old, passed tiles that are still in existence.
    /// </summary>
    LinkedList<GameObject> ExpiredTiles;

    /// <summary>
    /// List of the available tiles that can be spawned by this spawner. 0 is the spawn tile, and 1 is a blank tile.
    /// </summary>
    public List<GameObject> AvailableTiles;

    [Header("Expiring")]

    public float XCutoffValue = -1.0f;
    public float YCutoffValue = -1.0f;
    public float ZCutoffValue = -1.0f;

    public float XDeleteValue = -16.0f;
    public float YDeleteValue = -16.0f;
    public float ZDeleteValue = -16.0f;

    [Header("Movement and Animation")] ////////////////////////////////

    /// <summary>
    /// Whether or not the spawner is enabled; default value is true.
    /// </summary>
    public bool Enabled = true;

    /// <summary>
    /// Whether or not the tiles should drop off the end; default value is true. 
    /// If false, tiles will enter a secondary queue and scroll off the screen.
    /// </summary>
    public bool DropOffEnd {
        get
        {
            return m_dropOffEnd;
        }
        set
        { 
            m_dropOffEnd = value;
        }
    }
    [SerializeField] private bool m_dropOffEnd = true;

    /// <summary>
    /// The constant speed the tiles move at; default value is 5.
    /// </summary>
    public float MovementSpeed = 5.0f;

    [Header("Debug")]

    public Material CurrentTileMaterial;
    public Material ExpiredTileMaterial;

    //
    void Awake ()
    {
        ActiveTiles = new LinkedList<GameObject>();
        ExpiredTiles = new LinkedList<GameObject>();
    }

    // 
    void Start ()
    {
        // generate the spawn tile
        GenerateTiles(1, 0, 0);
        // then, generate the rest of the beginning tiles
        GenerateTiles(m_loadedTiles - 1, 1, -1);
    }

    /// <summary>
    /// 
    /// </summary>
    void Update ()
    {
        // make sure we have enabled the spawner to function
        // make sure we have tiles to update before we attempt to update them
        if (Enabled == true && ActiveTiles.Count != 0)
        {
            // the current tile is the one the player is on
            GameObject currentTile = ActiveTiles.First.Value;

            // DEBUG ONLY: Show the current tile in a different material
            foreach (MeshRenderer mr in currentTile.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material = CurrentTileMaterial;
            }

            // first, we get the direction the tiles should be moving in; we assume this is the vector from attach_next to attach_prev
            Vector3 direction = (currentTile.transform.Find("attach_prev").localPosition - currentTile.transform.Find("attach_next").localPosition);

            // then, we normalize the direction
            direction.Normalize();

            // move all the tiles in the direction of the current tile
            foreach (GameObject tile in ActiveTiles)
            {
                MoveTile(tile, direction);
            }

            // list of objects to delete
            List<GameObject> deleteList = new List<GameObject>(8);

            // if the animation is not supposed to drop the tiles off the end, they will continue to scroll as expired tiles
            foreach (GameObject tile in ExpiredTiles)
            {
                // we set the drop direction to our custom gravity formula if we have drop animations enabled, otherwise we have it move back
                // the custom gravity animation is predictably reversible and we can control it
                Vector3 dropDirection;
                if (DropOffEnd)
                {
                    // apply gravity to the falling tile
                    dropDirection = (Vector3.down * 9.812f) + (direction * MovementSpeed);
                    dropDirection.y += tile.transform.position.y * 100f * Time.deltaTime;

                    // adjust the direction so it doesn't take into account the movement speed
                    dropDirection /= MovementSpeed;
                }
                else
                {
                    dropDirection = direction;
                }
                
                // we move the tile according to the direction we chose
                MoveTile(tile, dropDirection);

                // we check if any tiles need to be destroyed, and add them to the delete list
                if (tile.transform.Find("attach_next").position.x < XDeleteValue
                    ||
                    tile.transform.Find("attach_next").position.y < YDeleteValue
                    ||
                    tile.transform.Find("attach_next").position.z < ZDeleteValue)
                {
                    deleteList.Add(tile);
                }
            }

            // WARNING: we may need to change this to a iterator-based delete if this proves inefficient

            // we cannot remove C# LinkedList items while we are traversing the list, so we must iterate over the deleted array we created and remove them this time
            foreach (GameObject tile in deleteList)
            {
                ExpiredTiles.Remove(tile);
                GameObject.Destroy(tile);
            }

            // if the entire tile falls behind the cutoffValue for x or z it must be expired
            if (currentTile.transform.Find("attach_next").position.x < XCutoffValue
                ||
                currentTile.transform.Find("attach_next").position.y < YCutoffValue
                ||
                currentTile.transform.Find("attach_next").position.z < ZCutoffValue)
            {
                // add the current tile to the list of expired tiles
                ExpiredTiles.AddLast(currentTile);

                // DEBUG ONLY: Show the current tile in a different material
                foreach (MeshRenderer mr in currentTile.GetComponentsInChildren<MeshRenderer>())
                {
                    mr.material = ExpiredTileMaterial;
                }

                // remove the current tile from the queue so we don't care about it anymore
                ActiveTiles.RemoveFirst();

                // generate another tile at the end of the queue
                GenerateTiles(1,1,-1);
            }
        }
	}

    /// <summary>
    /// Moves the tile according to the Tile Spawner's Movement and Animation parameters.
    /// </summary>
    /// <param name="tile">The tile to be moved.</param>
    /// <param name="direction">the direction to move the tile in.</param>
    public void MoveTile(GameObject tile, Vector3 direction)
    {
        // next, we get the tile's rigidbody component so we can move it properly
        //Rigidbody body = tile.GetComponent<Rigidbody>();

        // finally, we translate the tile in the direction by our speed * deltaTime
        tile.transform.position += (direction * MovementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Generates tiles randomly from a given range to be placed on the queue, using the Available Tiles list.
    /// </summary>
    /// <param name="count">The number of tiles to be generated</param>
    /// <param name="min">The lower end of the random range of tiles to choose from; default value is 0.</param>
    /// <param name="max">The upper end of the random range of tiles to choose from; default value is -1 (maximum value).</param>
    public void GenerateTiles(int count, int min = 0, int max = -1)
    {
        // if max is set to -1, set it to the maximum indexible value in available tiles
        if (max == -1) max = AvailableTiles.Count;

        // only run the spawning procedure if the AvailableTiles list is populated
        if (AvailableTiles.Count > 0)
        {
            // create a temporary variable to store the previous tile once they have begun to be spawned
            GameObject previousTile = null;
            int previousId = -1;
            // run the spawning procedure count times
            for (int i = 0; i < count; i++)
            {
                // get a random id from the range
                int id = Random.Range(min, max);

                // if there are tiles already on the queue, we will attach the next tiles to the end of the tiles in the queue
                if (ActiveTiles.Count != 0)
                {
                    previousTile = ActiveTiles.Last.Value;
                }

                // get the tile prefab from the available tiles corresponding to the random id
                GameObject tilePrefab = AvailableTiles[id];

                // if there was no previous tile spawned, default spawn location is origin
                Vector3 spawnLocation = Vector3.zero;

                // if there are tiles already on the queue, we will attach the next tiles to the end of the tiles in the queue
                if(ActiveTiles.Count != 0) previousTile = ActiveTiles.Last.Value;

                // NOTE: Might have to change how spawn location is calculated //

                // otherwise, the default spawn location is shifted by the displacement vector from attach_prev on the new tile to attach_next on the previous tile
                if (previousTile != null)
                {
                    spawnLocation = previousTile.transform.FindChild("attach_next").position - tilePrefab.transform.FindChild("attach_prev").position;
                }
                else
                {
                    spawnLocation = tilePrefab.transform.FindChild("attach_next").position;
                }
                // spawn the tile
                GameObject tile = Instantiate(tilePrefab, spawnLocation, Quaternion.identity) as GameObject;

                // finally, add the tile to the queue and set previous tile to the new tile for the next iteration of the loop
                ActiveTiles.AddLast(tile);
                previousTile = tile;
                previousId = id;
            }
        }
    }
}