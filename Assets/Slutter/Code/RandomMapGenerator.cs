using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomMapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase[] tiles; // 타일 배열 (여기에 여러 타일을 넣을 수 있습니다)
    public int mapWidth = 10;
    public int mapHeight = 10;

    void Start()
    {
        GenerateRandomMap();
    }

    void GenerateRandomMap()
    {
        tilemap.ClearAllTiles();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                TileBase randomTile = tiles[Random.Range(0, tiles.Length)];
                tilemap.SetTile(new Vector3Int(x, y, 0), randomTile);
            }
        }
    }
}
