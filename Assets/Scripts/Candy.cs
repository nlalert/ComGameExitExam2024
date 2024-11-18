using UnityEngine;

public class Candy : MonoBehaviour {
    public int x; // Grid position
    public int y;
    public int type; // Candy type (e.g., 0 for red, 1 for blue)

    public void Initialize(int x, int y, int type) {
        this.x = x;
        this.y = y;
        this.type = type;
    }
}
