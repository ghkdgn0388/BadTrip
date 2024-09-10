using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public Node ParentNode;

    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}
public class PathFinding : MonoBehaviour
{ 
    [SerializeField] private const float xScale = 0.5f, yScale = 0.5f;
    //public int searchSize;
    private DivideSpace divideSpace;
    public Vector2Int targetPos;
    private Vector2Int bottomLeft, topRight, startPos;
    public bool allowDiagonal, dontCrossCorner;
    public LayerMask blocked;

    public List<Node> finalNodeList;
    private int sizeX, sizeY;
    private Node[,] nodeArray;
    private Node startNode, targetNode, curNode;
    private List<Node> openList, closedList;

    private void Awake()
    {
        divideSpace = FindObjectOfType<DivideSpace>();
    }

    public void Finding(Vector3 targetPosition)
    {
        startPos = FloatToInt(transform.position.x, transform.position.y);
        targetPos = FloatToInt(targetPosition.x, targetPosition.y);
        /*bottomLeft = new Vector2Int(
            Mathf.Min(startPos.x, targetPos.x) - searchSize,
            Mathf.Min(startPos.y, targetPos.y) - searchSize
        );
        topRight = new Vector2Int(
            Mathf.Max(startPos.x, targetPos.x) + searchSize,
            Mathf.Max(startPos.y, targetPos.y) + searchSize
        );*/

        bottomLeft = FloatToInt(0, 0);
        topRight = FloatToInt(divideSpace.totalWidth + 1, divideSpace.totalHeight + 1);

        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        nodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                Vector2 posTemp = IntToFloat(i + bottomLeft.x, j + bottomLeft.y);
                foreach (Collider2D col in Physics2D.OverlapCircleAll(posTemp, 0.2f))
                    if ((blocked.value & (1 << col.gameObject.layer)) > 0) isWall = true;

                nodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }
        

        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        startNode = nodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        targetNode = nodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        openList = new List<Node>() { startNode };
        closedList = new List<Node>();
        finalNodeList = new List<Node>();

        
        while (openList.Count > 0)
        {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            curNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
                if (openList[i].F <= curNode.F && openList[i].H < curNode.H) curNode = openList[i];

            openList.Remove(curNode);
            closedList.Add(curNode);


            // 마지막
            if (curNode == targetNode)
            {
                Node targetCurNode = targetNode;
                while (targetCurNode != startNode)
                {
                    finalNodeList.Add(targetCurNode);
                    targetCurNode = targetCurNode.ParentNode;
                }
                finalNodeList.Add(startNode);
                finalNodeList.Reverse();

                //for (int i = 0; i < FinalNodeList.Count; i++) print(i + "번째는 " + FinalNodeList[i].x + ", " + FinalNodeList[i].y);
                return;
            }


            // ↗↖↙↘
            if (allowDiagonal)
            {
                OpenListAdd(curNode.x + 1, curNode.y + 1);
                OpenListAdd(curNode.x - 1, curNode.y + 1);
                OpenListAdd(curNode.x - 1, curNode.y - 1);
                OpenListAdd(curNode.x + 1, curNode.y - 1);
            }

            // ↑ → ↓ ←
            OpenListAdd(curNode.x, curNode.y + 1);
            OpenListAdd(curNode.x + 1, curNode.y);
            OpenListAdd(curNode.x, curNode.y - 1);
            OpenListAdd(curNode.x - 1, curNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !closedList.Contains(nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // 대각선 허용시, 벽 사이로 통과 안됨
            if (allowDiagonal) if (nodeArray[curNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall && nodeArray[checkX - bottomLeft.x, curNode.y - bottomLeft.y].isWall) return;

            // 코너를 가로질러 가지 않을시, 이동 중에 수직수평 장애물이 있으면 안됨
            if (dontCrossCorner) if (nodeArray[curNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || nodeArray[checkX - bottomLeft.x, curNode.y - bottomLeft.y].isWall) return;

            
            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
            Node neighborNode = nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
            int moveCost = curNode.G + (curNode.x - checkX == 0 || curNode.y - checkY == 0 ? 10 : 14);


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (moveCost < neighborNode.G || !openList.Contains(neighborNode))
            {
                neighborNode.G = moveCost;
                neighborNode.H = (Mathf.Abs(neighborNode.x - targetNode.x) + Mathf.Abs(neighborNode.y - targetNode.y)) * 10;
                neighborNode.ParentNode = curNode;

                openList.Add(neighborNode);
            }
        }
    }
    
    // 사각형 좌표를 아이소메트릭 좌표로 변환하는 함수
    public Vector2 IntToFloat(int x, int y)
    {
        // 아이소메트릭 좌표로의 변환 계산
        float floatX = x * xScale;
        float floatY = y * yScale;

        return new Vector2(floatX, floatY);
    }
    // 아이소메트릭 좌표를 사각형 좌표로 변환하는 함수
    public Vector2Int FloatToInt(float x,float y)
    {
        // 사각형 좌표로의 변환 계산
        int intX = Mathf.RoundToInt(x / xScale);
        int intY = Mathf.RoundToInt(y / yScale);

        return new Vector2Int(intX, intY);
    }
    public Vector3 GetFirstNode(){
        return IntToFloat(finalNodeList[0].x, finalNodeList[0].y);
    }

    void OnDrawGizmos()
    {
        if(finalNodeList.Count != 0){
            Gizmos.color = Color.blue;
            for (int i = 0; i < finalNodeList.Count - 1; i++)
                // Gizmos.DrawLine(new Vector2(finalNodeList[i].x, finalNodeList[i].y), new Vector2(finalNodeList[i + 1].x, finalNodeList[i + 1].y));
                Gizmos.DrawLine(IntToFloat(finalNodeList[i].x, finalNodeList[i].y), IntToFloat(finalNodeList[i + 1].x, finalNodeList[i + 1].y));
        }
    }
    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if(nodeArray[i, j].isWall){
                    Gizmos.color = Color.red;
                }
                else{
                    Gizmos.color = Color.green;
                }
                Vector2 pos = IntToFloat(nodeArray[i, j].x, nodeArray[i, j].y);
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }
    }
}