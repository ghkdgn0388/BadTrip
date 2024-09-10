using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MakeRandomMap : MonoBehaviour
{
    [SerializeField]
    private int distance;
    [SerializeField]
    private int minRoomWidth;
    [SerializeField]
    private int minRoomHeigth;
    [SerializeField]
    private DivideSpace divideSpace;
    [SerializeField]
    private SpreadTilemap spreadTilemap;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject entrance;
    [SerializeField]
    private GameObject[] enemyPrefabs;  // �� ������ �迭
    [SerializeField]
    private int enemiesPerRoom = 2;  // �� �濡 ������ ���� ��

    private int startRandomMapCallCount = 0; // StartRandomMap ȣ�� Ƚ�� ����

    private HashSet<Vector2Int> floor;
    private HashSet<Vector2Int> wall;
    private List<GameObject> spawnedEnemies = new List<GameObject>(); // ������ ���� ����Ʈ

    void Start()
    {
        //StartRandomMap();
    }

    public void StartRandomMap()
    {
        startRandomMapCallCount++;  // ȣ�� Ƚ�� ����

        // 5�� ȣ��� ������ enemiesPerRoom ����
        if (startRandomMapCallCount % 5 == 0)
        {
            enemiesPerRoom++;
            divideSpace.totalWidth += 5;
            divideSpace.totalHeight += 5;
        }

        // Interaction ���̾ �ִ� ������Ʈ�� ����
        ClearInteractionLayerObjects();

        ClearEtcLayerObjects();

        // ������ ������ ���� ����
        ClearEnemies();

        spreadTilemap.ClearAllTiles();
        divideSpace.totalSpace = new RectangleSpace(new Vector2Int(0, 0), divideSpace.totalWidth, divideSpace.totalHeight);
        divideSpace.spaceList = new List<RectangleSpace>();
        floor = new HashSet<Vector2Int>();
        wall = new HashSet<Vector2Int>();
        divideSpace.DivideRoom(divideSpace.totalSpace);
        MakeRandomRooms();

        MakeCorridors();

        MakeWall();

        spreadTilemap.SpreadFloorTilemap(floor);
        spreadTilemap.SpreadWallTilemap(wall);
        spreadTilemap.RefreshAllTiles();
        

        player.transform.position = (Vector2)divideSpace.spaceList[0].Center();
        entrance.transform.position = (Vector2)divideSpace.spaceList[divideSpace.spaceList.Count - 1].Center();

        SpawnEnemiesInRooms();  // �� ���� �Լ� ȣ��
    }

    private void MakeRandomRooms()
    {
        foreach (var space in divideSpace.spaceList)
        {
            HashSet<Vector2Int> positions = MakeRandomRectangleRoom(space);
            floor.UnionWith(positions);
        }
    }

    private HashSet<Vector2Int> MakeRandomRectangleRoom(RectangleSpace space)
    {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();
        int width = Random.Range(minRoomWidth, space.width + 1 - distance * 2);
        int height = Random.Range(minRoomHeigth, space.height + 1 - distance * 2);
        for (int i = space.Center().x - width / 2; i <= space.Center().x + width / 2; i++)
        {
            for (int j = space.Center().y - height / 2; j <= space.Center().y + height / 2; j++)
            {
                positions.Add(new Vector2Int(i, j));
            }
        }
        return positions;
    }

    private void MakeCorridors()
    {
        List<Vector2Int> tempCenters = new List<Vector2Int>();
        foreach (var space in divideSpace.spaceList)
        {
            tempCenters.Add(new Vector2Int(space.Center().x, space.Center().y));
        }
        Vector2Int nextCenter;
        Vector2Int currentCenter = tempCenters[0];
        tempCenters.Remove(currentCenter);
        while (tempCenters.Count != 0)
        {
            nextCenter = ChooseShortestNextCorridor(tempCenters, currentCenter);

            // ���� �����ڸ����� ���� ����
            Vector2Int currentEdge = FindRoomEdge(currentCenter, nextCenter);
            Vector2Int nextEdge = FindRoomEdge(nextCenter, currentCenter);

            MakeOneCorridor(currentEdge, nextEdge);

            currentCenter = nextCenter;
            tempCenters.Remove(currentCenter);
        }
    }

    private Vector2Int ChooseShortestNextCorridor(List<Vector2Int> tempCenters, Vector2Int previousCenter)
    {
        int n = 0;
        float minLength = float.MaxValue;
        for (int i = 0; i < tempCenters.Count; i++)
        {
            if (Vector2.Distance(previousCenter, tempCenters[i]) < minLength)
            {
                minLength = Vector2.Distance(previousCenter, tempCenters[i]);
                n = i;
            }
        }
        return tempCenters[n];
    }

    private void MakeOneCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = new Vector2Int(start.x, start.y);
        Vector2Int next = new Vector2Int(end.x, end.y);
        floor.Add(current);
        while (current.x != next.x)
        {
            if (current.x < next.x)
            {
                current.x += 1;
            }
            else
            {
                current.x -= 1;
            }
            floor.Add(current);
        }
        while (current.y != next.y)
        {
            if (current.y < next.y)
            {
                current.y += 1;
            }
            else
            {
                current.y -= 1;
            }
            floor.Add(current);
        }
    }

    private Vector2Int FindRoomEdge(Vector2Int roomCenter, Vector2Int direction)
    {
        Vector2Int edge = new Vector2Int(roomCenter.x, roomCenter.y);

        if (direction.x > roomCenter.x)
        {
            edge.x = roomCenter.x + minRoomWidth / 2;
        }
        else if (direction.x < roomCenter.x)
        {
            edge.x = roomCenter.x - minRoomWidth / 2;
        }

        if (direction.y > roomCenter.y)
        {
            edge.y = roomCenter.y + minRoomHeigth / 2;
        }
        else if (direction.y < roomCenter.y)
        {
            edge.y = roomCenter.y - minRoomHeigth / 2;
        }

        return edge;
    }

    private void MakeWall()
    {
        foreach (Vector2Int tile in floor)
        {
            HashSet<Vector2Int> boundary = Make3X3Square(tile);
            boundary.ExceptWith(floor);
            if (boundary.Count != 0)
            {
                wall.UnionWith(boundary);
            }
        }
    }

    private HashSet<Vector2Int> Make3X3Square(Vector2Int tile)
    {
        HashSet<Vector2Int> boundary = new HashSet<Vector2Int>();
        for (int i = tile.x - 1; i <= tile.x + 1; i++)
        {
            for (int j = tile.y - 1; j <= tile.y + 1; j++)
            {
                boundary.Add(new Vector2Int(i, j));
            }
        }
        return boundary;
    }

    private void SpawnEnemiesInRooms()
    {
        // �÷��̾ �����Ǵ� ���� �ε��� (ù ��° ��)
        int playerRoomIndex = 0;

        for (int i = 0; i < divideSpace.spaceList.Count; i++)
        {
            // �÷��̾ �����Ǵ� ���� �ǳʶٱ�
            if (i == playerRoomIndex) continue;

            var space = divideSpace.spaceList[i];
            for (int j = 0; j < enemiesPerRoom; j++)
            {
                Vector2Int spawnPosition = GetValidSpawnPosition(space);

                if (spawnPosition != Vector2Int.zero)  // ��ȿ�� ��ġ�� �ִ� ��쿡�� �� ����
                {
                    GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];  // ������ �� ������ ����
                    GameObject enemy = Instantiate(enemyPrefab, (Vector2)spawnPosition, Quaternion.identity);
                    spawnedEnemies.Add(enemy);
                }
                else
                {
                    Debug.LogWarning("��ȿ�� �� ���� ��ġ�� ã�� �� �����ϴ�.");
                }
            }
        }
    }

    private Vector2Int GetValidSpawnPosition(RectangleSpace space)
    {
        for (int attempts = 0; attempts < 10; attempts++)  // �ִ� 10�� �õ�
        {
            Vector2Int spawnPosition = GetRandomPositionInRoom(space);

            // Wall ���̾ �������� �ʵ��� üũ (Wall ���̾�� �浹 �˻�)
            Collider2D[] collidersAtPosition = Physics2D.OverlapCircleAll((Vector2)spawnPosition, 0.5f, LayerMask.GetMask("Wall"));
            if (collidersAtPosition.Length > 0)
            {
                continue;  // Wall�� ��ġ�� ��� ���� ��ġ ��õ�
            }

            // �� ��� ���� �ִ��� Ȯ��
            if (!IsInsideRoom(space, spawnPosition))
            {
                continue;  // �� ��踦 ��� ��� ���� ��ġ ��õ�
            }

            // �ٸ� ���� ��ġ�� �ʵ��� üũ
            bool overlapWithOtherEnemies = false;
            foreach (var enemy in spawnedEnemies)
            {
                if (enemy != null && Vector2.Distance(enemy.transform.position, spawnPosition) < 1f)
                {
                    overlapWithOtherEnemies = true;
                    break;
                }
            }

            if (!overlapWithOtherEnemies)
            {
                return spawnPosition;  // ��ȿ�� ��ġ�� Ȯ�εǸ� ��ȯ
            }
        }

        return Vector2Int.zero;  // ��ȿ�� ��ġ�� ã�� ���� ���
    }

    private bool IsInsideRoom(RectangleSpace space, Vector2Int position)
    {
        int minX = space.Center().x - space.width / 2 + 1;
        int maxX = space.Center().x + space.width / 2 - 1;
        int minY = space.Center().y - space.height / 2 + 1;
        int maxY = space.Center().y + space.height / 2 - 1;

        return position.x >= minX && position.x <= maxX && position.y >= minY && position.y <= maxY;
    }


    private Vector2Int GetRandomPositionInRoom(RectangleSpace space)
    {
        int x = Random.Range(space.Center().x - minRoomWidth / 2, space.Center().x + minRoomWidth / 2);
        int y = Random.Range(space.Center().y - minRoomHeigth / 2, space.Center().y + minRoomHeigth / 2);
        return new Vector2Int(x, y);
    }


    private void ClearEnemies()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }

    private void ClearInteractionLayerObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("Interaction"))
            {
                Destroy(obj);
            }
        }
    }

    private void ClearEtcLayerObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("etc"))
            {
                Destroy(obj);
            }
        }
    }
}
