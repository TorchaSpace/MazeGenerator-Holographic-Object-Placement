using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorWithoutConnections : MonoBehaviour
{
    public int width;
    public int height;
    public float wallHeight = 2.0f;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject ceilingPrefab;

    private int[,] maze;
    private Vector3 wallScale;

    void Start()
    {
        wallScale = new Vector3(1, wallHeight, 1);
        GenerateMaze();
        DrawMaze();
        AddFloor();
        AddCeiling();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];

        // Initialize borders
        for (int i = 0; i < width; i++)
        {
            maze[i, 0] = 1;
            maze[i, height - 1] = 1;
        }
        for (int i = 0; i < height; i++)
        {
            maze[0, i] = 1;
            maze[width - 1, i] = 1;
        }

        // Set start and end points
        int startX = Random.Range(2, width - 2);
        int startY = Random.Range(2, height - 2);
        int endX = Random.Range(2, width - 2);
        int endY = Random.Range(2, height - 2);
        maze[startX, startY] = 0;
        maze[endX, endY] = 0;

        // Generate maze with more complexity
        for (int x = 2; x < width - 2; x += 2)
        {
            for (int y = 2; y < height - 2; y += 2)
            {
                maze[x, y] = 1;

                int direction = Random.Range(0, 4);
                int turns = Random.Range(0, 3); // add more turns

                for (int i = 0; i < turns; i++)
                {
                    direction = (direction + Random.Range(1, 4)) % 4; // change direction
                }

                switch (direction)
                {
                    case 0:
                        if (x > 2) // Check if it's not the left-most column
                        {
                            maze[x - 1, y] = 1;
                        }
                        else
                        {
                            maze[x + 1, y] = 1; // Merge with the outer wall
                        }
                        break;
                    case 1:
                        if (y > 2) // Check if it's not the bottom row
                        {
                            maze[x, y - 1] = 1;
                        }
                        else
                        {
                            maze[x, y + 1] = 1; // Merge with the outer wall
                        }
                        break;
                    case 2:
                        if (x < width - 3) // Check if it's not the right-most column
                        {
                            maze[x + 1, y] = 1;
                        }
                        else
                        {
                            maze[x - 1, y] = 1; // Merge with the outer wall
                        }
                        break;
                    case 3:
                        if (y < height - 3) // Check if it's not the top row
                        {
                            maze[x, y + 1] = 1;
                        }
                        else
                        {
                            maze[x, y - 1] = 1; // Merge with the outer wall
                        }
                        break;
                }
            }
        }

        // Add more dead ends
        for (int i = 0; i < 20; i++)
        {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);

            if (maze[x, y] == 1 &&
                maze[x - 1, y] + maze[x + 1, y] + maze[x, y - 1] + maze[x, y + 1] == 3)
            {
                maze[x, y] = 0;
            }
        }
    }



    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 1)
                {
                    GameObject wall = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity);
                    wall.transform.localScale = wallScale;
                }
            }
        }
    }

    void AddFloor()
    {
        Vector3 floorScale = new Vector3(width, 1, height);
        GameObject floor = Instantiate(floorPrefab, new Vector3(width / 2.0f - 0.5f, -0.5f, height / 2.0f - 0.5f), Quaternion.identity);
        floor.transform.localScale = floorScale;
    }

    void AddCeiling()
    {
        Vector3 ceilingScale = new Vector3(width, 0.1f, height);
        GameObject ceiling = Instantiate(ceilingPrefab, new Vector3(width / 2.0f - 0.5f, wallHeight, height / 2.0f - 0.5f), Quaternion.identity);
        ceiling.transform.localScale = ceilingScale;

        // Position the ceiling so that it overlaps with the walls
        ceiling.transform.position += new Vector3(0, wallHeight - 7.51f, 0);
    }





}
