/*using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Vector2Int gridSize; // �׸����� ũ��
    public float cellSize; // �� ���� ũ��
    public LayerMask obstacleLayer; // ��ֹ� ���̾�

    private Node[,] grid;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new Node[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2 worldPoint = new Vector2(x, y) * cellSize;
                bool walkable = !Physics2D.OverlapCircle(worldPoint, cellSize / 2, obstacleLayer);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node GetNodeFromWorldPosition(Vector2 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.y / cellSize);
        return grid[x, y];
    }
}
*/