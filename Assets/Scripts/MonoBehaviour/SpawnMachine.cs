using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnMachine : MonoBehaviour
{
    private const int pawnsAmount = 8;
    public GameObject pawnPrefab;
    public GameObject kingPrefab;
    public GameObject queenPrefab;
    public GameObject knightPrefab;
    public GameObject bishopPrefab;
    public GameObject rookPrefab;
    private List<GameObject> figuresList;
    private float endPos = 7f;
    private float cellSizeX = 0;
    private float cellSizeY = 0;
    public List<GameObject> GetFigures() => figuresList;
    public void SetupSpawner()
    {
        figuresList = new List<GameObject>(16);
        if (pawnPrefab.GetComponent<Figure>().chessmanType.IsWhite)
        {
            cellSizeY = -PositionsCollector.instance.GetAttachedTileMap().cellSize.y;
            gameObject.transform.position = (PositionsCollector.instance.GetTileCoordinatesVector3Int()["A2"]  +
            PositionsCollector.instance.GetAttachedTileMap().cellSize / 2);
        }
        else
        {
            cellSizeY = PositionsCollector.instance.GetAttachedTileMap().cellSize.y;
            gameObject.transform.position = (PositionsCollector.instance.GetTileCoordinatesVector3Int()["A7"]  +
            PositionsCollector.instance.GetAttachedTileMap().cellSize / 2);
        }
        cellSizeX = PositionsCollector.instance.GetAttachedTileMap().cellSize.x;
        SpawnFigures();
    }
    public void SpawnFigures()
    {
        SpawnPawns();
        SpawnRooks();
        SpawnBishops();
        SpawnKnight();
        SpawnQueen();
        SpawnKing();
    }
    private void SpawnPawns()
    {
        if (pawnPrefab == null)
            Debug.LogWarning("PawnPrefab = null");
        for (int i = 0; i < pawnsAmount; i++)
        {
            figuresList.Add(Instantiate(pawnPrefab, transform.position +
            new Vector3(PositionsCollector.instance.GetAttachedTileMap().cellSize.x * i, 0), Quaternion.identity));
            figuresList[i].GetComponent<Figure>().SetFigureLocation($"{(char)('A' + i)}" + (cellSizeY > 0 ? "7" : "2"));
        }
    }
    private void SpawnRooks()
    {
        if (rookPrefab == null)
            Debug.LogWarning("RookPrefab = null");
        figuresList.Add(Instantiate(rookPrefab, transform.position + new Vector3(0, cellSizeY), Quaternion.identity));
        figuresList[figuresList.Count - 1].GetComponent<Figure>().SetFigureLocation($"{'A'}" + (cellSizeY > 0 ? "8" : "1"));
        figuresList.Add(Instantiate(rookPrefab, transform.position + new Vector3(cellSizeX * endPos, cellSizeY), Quaternion.identity));
        figuresList[figuresList.Count - 1].GetComponent<Figure>().SetFigureLocation($"{'H'}" + (cellSizeY > 0 ? "8" : "1"));
    }
    private void SpawnBishops()
    {
        if (bishopPrefab == null)
            Debug.LogWarning("BishopPrefab = null");
        figuresList.Add(Instantiate(bishopPrefab, transform.position + (new Vector3(2 * cellSizeX, cellSizeY)), Quaternion.identity));
        figuresList[figuresList.Count - 1].GetComponent<Figure>().SetFigureLocation($"{'C'}" + (cellSizeY > 0 ? "8" : "1"));
        figuresList.Add(Instantiate(bishopPrefab, transform.position + (new Vector3((endPos - 2) * cellSizeX, cellSizeY)), Quaternion.identity));
        figuresList[figuresList.Count - 1].GetComponent<Figure>().SetFigureLocation($"{'F'}" + (cellSizeY > 0 ? "8" : "1"));
    }
    private void SpawnKnight()
    {
        if (knightPrefab == null)
            Debug.LogWarning("KnightPrefab = null");
        figuresList.Add(Instantiate(knightPrefab, transform.position + (new Vector3(cellSizeX, cellSizeY)), Quaternion.identity));
        figuresList[figuresList.Count - 1].GetComponent<Figure>().SetFigureLocation($"{'B'}" + (cellSizeY > 0 ? "8" : "1"));
        figuresList.Add(Instantiate(knightPrefab, transform.position + (new Vector3((endPos - 1) * cellSizeX, cellSizeY)), Quaternion.identity));
        figuresList[figuresList.Count - 1].GetComponent<Figure>().SetFigureLocation($"{'G'}" + (cellSizeY > 0 ? "8" : "1"));
    }
    private void SpawnQueen()
    {
        if (queenPrefab == null)
            Debug.LogWarning("QueenPrefab = null");
        figuresList.Add(Instantiate(queenPrefab, transform.position + (new Vector3(3 * cellSizeX, cellSizeY)), Quaternion.identity));
        figuresList[figuresList.Count - 1].GetComponent<Figure>().SetFigureLocation($"{'D'}" + (cellSizeY > 0 ? "8" : "1"));


    }
    private void SpawnKing()
    {
        if (kingPrefab == null)
            Debug.LogWarning("KingPrefab = null");
        figuresList.Add(Instantiate(kingPrefab, transform.position + (new Vector3(4 * cellSizeX, cellSizeY)), Quaternion.identity));
        figuresList[figuresList.Count - 1].GetComponent<Figure>().SetFigureLocation($"{'E'}" + (cellSizeY > 0 ? "8" : "1"));
        //print(figuresList[figuresList.Count - 1].name);
    }
}


