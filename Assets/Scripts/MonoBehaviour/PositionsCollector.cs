using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PositionsCollector : MonoBehaviour
{
    private Tilemap attachedTilemap;
    private Dictionary<string, Vector3Int> chessBoardTilesCoordinate;
    private Dictionary<Vector3Int, string> chessBoardTilesValues;
    public static PositionsCollector instance;
  
    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        else
            instance = this;
     
    }
 
    public void SetupTileMapData()
    {
        attachedTilemap = GetComponent<Tilemap>();
        attachedTilemap.CompressBounds();
        chessBoardTilesCoordinate = new Dictionary<string, Vector3Int>();
        chessBoardTilesValues = new Dictionary<Vector3Int, string>();
        char letChanger = 'A';
        int numChanger = 1;
        string chessSquareLoc = string.Empty;
        foreach (var cellLocalOrigin in attachedTilemap.cellBounds.allPositionsWithin)
        {
            if (letChanger == 'I')
            {
                letChanger = 'A';
                numChanger++;
            }
            chessSquareLoc = $"{letChanger++}{numChanger}";
            chessBoardTilesCoordinate.Add(chessSquareLoc, cellLocalOrigin*(int)attachedTilemap.cellSize.x);
            chessBoardTilesValues.Add(cellLocalOrigin* (int)attachedTilemap.cellSize.x, chessSquareLoc);
        }
    }
    public Dictionary<string, Vector3Int> GetTileCoordinatesVector3Int() => chessBoardTilesCoordinate;
    public Dictionary<Vector3Int, string> GetChessCoordinatesString() => chessBoardTilesValues;
    public Tilemap GetAttachedTileMap() => attachedTilemap;
}
