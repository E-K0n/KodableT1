using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridPathfinder : MonoBehaviour
{

    public TileBase pathTile;
    public TileBase stairTile;
    public Tilemap mazeMap;
    public Vector3Int[,] nodes;
    Astar astar;
    BoundsInt mapBounds;
    List<Node> pathFound = new List<Node>();

    private GameObject player;

    public void GeneratePath()
    {
        mapBounds = mazeMap.cellBounds;
        Vector3Int entrance = gameObject.GetComponent<MazeParser>().entranceTile;
        Vector3Int exit = gameObject.GetComponent<MazeParser>().exitTile;
        player = gameObject.GetComponent<MazeParser>().playerSprite;

        CreateGrid();

        /*
        for (int k = 0; k < nodes.GetLength(0); k++)
            for (int l = 0; l < nodes.GetLength(1); l++)
                Debug.Log(nodes[k, l]);
        */
        astar = new Astar(nodes, mapBounds.size.x, mapBounds.size.y);

        pathFound = astar.CreatePath(nodes, entrance, exit, 9999);
        NavigateSprite();
    }

    //generate a grid of nodes for the Astar algorithm
    private void CreateGrid()
    {
        nodes = new Vector3Int[mapBounds.size.x, mapBounds.size.y];
        for (int x = mapBounds.xMin, i = 0; i < (mapBounds.size.x); x++, i++)
        {
            for (int y = mapBounds.yMin, j = 0; j < (mapBounds.size.y); y++, j++)
            {

                // check if tile at this point is valid. if so, add it to the grid

                if (mazeMap.GetTile(new Vector3Int(x, y, 0)) == pathTile || mazeMap.GetTile(new Vector3Int(x, y, 0)) == stairTile)
                {
                    nodes[i, j] = new Vector3Int(x, y, 0);
                }
                else
                {
                    nodes[i, j] = new Vector3Int(x, y, 1);
                }
            }
        }
    }

    private void NavigateSprite()
    {
        StartCoroutine(Translateplayer(0.2f, 10, pathFound.Count-1));
    }

    IEnumerator Translateplayer(float delay, float speed, int moveindex)
    {
        //Debug.Log("Moving to");
        Node currentTarget = pathFound[moveindex];
        yield return new WaitForSeconds(delay);
        float step = speed * delay;
        Vector3 targetposition = new Vector3(currentTarget.X, currentTarget.Y, 0);
        player.transform.position = Vector3.MoveTowards(player.transform.position, targetposition, step);

        if (player.transform.position == targetposition)
            moveindex -= 1;


        if (moveindex >= 0)
            StartCoroutine(Translateplayer(delay, speed, moveindex));
    }
}
