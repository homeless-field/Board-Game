using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBoard : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private RoomManager roomManager;
    private GameManager gameManager;
    private Mesh mesh;

    // CREATES THE CORRECT NUMBER OF VERTICES IN THE CORRECT PLACES
    private Vector3[] CreateVertices(Vector2Int roomSize)
    {
        int vertexCount = (roomSize.x + 1) * (roomSize.y + 1); // I have no idea where this number comes from, but it works
        Vector3[] vertices = new Vector3[vertexCount];

        for (int currentVertex = 0; currentVertex < vertexCount; currentVertex++)
        {
            Vector2 basicVertexPosition = new Vector2(currentVertex % (roomSize.x + 1), currentVertex / (roomSize.x + 1));
            vertices[currentVertex] = new Vector3(basicVertexPosition.x * gameManager.tileSize, 0, basicVertexPosition.y * gameManager.tileSize);
        }

        return vertices;
    }

    // CREATES TRIANGLES TO CONNECT THE VERTICES IN THE RIGHT ORDER
    private int[] CreateTriangles(Vector2Int roomSize)
    {
        int tileCount = roomSize.x * roomSize.y;
        int[] triangles = new int[tileCount * 6]; // 2 triangles with 3 vertices each

        for (int currentTile = 0; currentTile < tileCount; currentTile++)
        {
            int currentIndex = currentTile + (currentTile / roomSize.x);
            int indexBelow = currentIndex + (roomSize.x + 1);

            // UPPER-LEFT TRIANGLE
            triangles[(currentTile * 6)] = currentIndex;
            triangles[(currentTile * 6) + 1] = currentIndex + 1;
            triangles[(currentTile * 6) + 2] = indexBelow;

            // LOWER-RIGHT TRIANGLE
            triangles[(currentTile * 6) + 3] = currentIndex + 1;
            triangles[(currentTile * 6) + 4] = indexBelow + 1;
            triangles[(currentTile * 6) + 5] = indexBelow;
        }

        System.Array.Reverse(triangles);
        return triangles;
    }

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        
        Vector3[] vertices = CreateVertices(roomManager.roomSize - Vector2Int.one);
        int[] triangles = CreateTriangles(roomManager.roomSize - Vector2Int.one);

        mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        Vector3 actualBoardSize = new Vector3((roomManager.roomSize.x - 1) * gameManager.tileSize, 0, (roomManager.roomSize.y - 1) * gameManager.tileSize);
        transform.parent.localPosition += actualBoardSize / 2;
        transform.localPosition -= actualBoardSize / 2;
    }
}
