using UnityEngine;

public class TileDataContainer : MonoBehaviour
{
    /*
     * bool isCorrupted : true if the tile is corrupted, false otherwise
     * */
    public bool isCorrupted = false;

    /*
     * bool isOccupied : true if the tile is occupied by a building, false otherwise
     * */
    public bool isOccupied = false;

    /*
     * int type : the type of the tile (0 = grass, 1 = dry_ground, 2 = neutral)
     * */
    public int type;
}
