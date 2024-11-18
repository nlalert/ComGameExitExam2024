using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Candy[,] candies;
    public GameObject[] candyPrefabs; // Assign prefabs for each candy type
    public int width = 15;
    public int height = 20;
    private float candySpacing = 1.2f; // Adjust this to increase/decrease spacing

    private void Start() {
        InitializeBoard();
    }

    private void InitializeBoard() {
        candies = new Candy[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                SpawnCandy(x, y);
            }
        }
    }

    private void SpawnCandy(int x, int y) {
        int randomType;

        do {
            randomType = Random.Range(0, candyPrefabs.Length);
        } while (WillMatchIfAdd(x, y, randomType)); // Retry until no match is formed

        // Apply spacing to position
        Vector3 position = new Vector3(
            x * candySpacing - (width * candySpacing) / 2,
            y * candySpacing - (height * candySpacing) / 2,
            0
        );

        GameObject newCandy = Instantiate(candyPrefabs[randomType], position, Quaternion.identity);
        Candy candy = newCandy.GetComponent<Candy>();
        candy.Initialize(x, y, randomType);
        candies[x, y] = candy;
    }

    private bool WillMatchIfAdd(int x, int y, int type) {
        // Check horizontally
        if (x >= 2) { // Ensure we are at least 2 columns away from the left edge
            if (candies[x - 1, y] != null && candies[x - 2, y] != null) {
                if (candies[x - 1, y].type == type && candies[x - 2, y].type == type) {
                    return true;
                }
            }
        }

        // Check vertically
        if (y >= 2) { // Ensure we are at least 2 rows away from the bottom edge
            if (candies[x, y - 1] != null && candies[x, y - 2] != null) {
                if (candies[x, y - 1].type == type && candies[x, y - 2].type == type) {
                    return true;
                }
            }
        }

        return false; // No match found
    }
}
