using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpreadTilemap : MonoBehaviour
{
    [SerializeField]
    private Tilemap floor;
    [SerializeField]
    //private Tilemap wall; 
    private GameObject wall;
    [SerializeField]
    private TileBase floorTile;
    [SerializeField]
    private TileBase wallTile;
    [SerializeField]
    private GameObject walls;

    public void SpreadFloorTilemap(HashSet<Vector2Int> positions) {
        SpreadTile(positions, floor, floorTile);
    }

    public void SpreadWallTilemap(HashSet<Vector2Int> positions) {
        SpreadWall(positions, wall);
    }
    
    private void SpreadWall(HashSet<Vector2Int> positions, GameObject wall)
    {
        foreach (var position in positions)
        {
            GameObject item = Instantiate(wall, walls.transform);
            item.transform.localPosition = new Vector3(position.x, position.y, 0);
        }
    }

    private void SpreadTile(HashSet<Vector2Int> positions, Tilemap tilemap, TileBase tile) { 
        foreach (var position in positions)
        {
            tilemap.SetTile((Vector3Int)position,tile);
        }
    }

    public void ClearAllTiles() { 
        floor.ClearAllTiles();
        //wall.ClearAllTiles();
        foreach(Transform wall in walls.transform)
        {
            Destroy(wall.gameObject);
        }
    }

    public void RefreshAllTiles()
    {
        //wall.RefreshAllTiles();
        //wall.gameObject.GetComponent<TilemapCollider2D>().enabled = false;
        //wall.gameObject.GetComponent<TilemapCollider2D>().enabled = true;
    }
}
