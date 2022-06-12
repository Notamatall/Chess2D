using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public SpawnMachine whiteFiguresSpawn;
    public SpawnMachine blackFiguresSpawn;
    public static GameManager sharedInstance = null;
    public static bool isWhiteTurn = true;
    public ChessLists chessLists;
    void Awake()
    {
        if (sharedInstance != null && sharedInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            sharedInstance = this;
        }

    }
    public void StartNewGame()
    {
        PositionsCollector.instance.SetupTileMapData();

        if (whiteFiguresSpawn != null)
        {
            whiteFiguresSpawn.SetupSpawner();
            chessLists.whiteFigures = new List<GameObject>(16);
            chessLists.whiteFigures.AddRange(whiteFiguresSpawn.GetFigures());
        }
        if (blackFiguresSpawn != null)
        {
            blackFiguresSpawn.SetupSpawner();
            chessLists.blackFigures = new List<GameObject>(16);
            chessLists.blackFigures.AddRange(blackFiguresSpawn.GetFigures());
        }
    }
    private void Start()
    {
        PositionsCollector.instance.SetupTileMapData();

        if (whiteFiguresSpawn != null)
        {
            whiteFiguresSpawn.SetupSpawner();
            chessLists.whiteFigures = new List<GameObject>(16);
            chessLists.whiteFigures.AddRange(whiteFiguresSpawn.GetFigures());
        }
        if (blackFiguresSpawn != null)
        {
            blackFiguresSpawn.SetupSpawner();
            chessLists.blackFigures = new List<GameObject>(16);
            chessLists.blackFigures.AddRange(blackFiguresSpawn.GetFigures());
        }
    }
 
    private void OnDestroy()
    {
        chessLists.blackFigures = null;
        chessLists.whiteFigures = null;
    }
}
