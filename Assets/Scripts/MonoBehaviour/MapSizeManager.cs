using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSizeManager : MonoBehaviour
{
    public static MapSizeManager instance;
    [SerializeField]
    private Grid chessBoardGrid;
    [SerializeField]
    private Tilemap chessBoardTilemap;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public Grid GetGrid() => chessBoardGrid;
    public float GetXMultiplier() => chessBoardGrid.transform.localScale.x * chessBoardGrid.cellSize.x * chessBoardTilemap.transform.localScale.x;
    public float GetYMultiplier() =>  chessBoardGrid.transform.localScale.y * chessBoardGrid.cellSize.y * chessBoardTilemap.transform.localScale.y;
}
