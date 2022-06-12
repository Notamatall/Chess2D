using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Figure : MonoBehaviour
{
    private string chessCoordinate;
    public ChessmanType chessmanType;
    public string GetFigureLocation() => chessCoordinate;
    public string SetFigureLocation(string location) => chessCoordinate = location;
    public PathData AddToPathData(string data, int type)
    {
        //if (type == 2)
        //    return null;
        PathData path = new PathData();
        if (type == 0)
            path.Empty = data;
        else if (type == 1)
            path.Ally = data;
        else if (type == -1)
            path.Enemy = data;
        return path;
    }
    public List<PathData> LoopThePath(string loc, int x, int y)
    {
        List<PathData> pathDatas = new List<PathData>();
        for (int i = 1; i < 8; i++)
        {
            string posToCheck = DirectionStringFormator(loc, i * x, i * y);
            int type = GetTypeOfFigure(posToCheck);
            if (type == 2)
                break;
            pathDatas.Add(AddToPathData(posToCheck, type));

            if (GameManager.isWhiteTurn)
            {
                if (GameManager.sharedInstance.chessLists.blackFigures.Find((x) => x.GetComponent<Figure>().chessmanType.objectName == "BlackKing")
                    .GetComponent<Figure>().GetFigureLocation() == posToCheck)
                {
                    i++;
                    posToCheck = DirectionStringFormator(loc, i * x, i * y);
                    type = GetTypeOfFigure(posToCheck);
                    if (type == 1||type == 0)
                        pathDatas.Add(AddToPathData(posToCheck, type));
                    break;
                }
            }
            else
            {
                if (GameManager.sharedInstance.chessLists.whiteFigures.Find((x) => x.GetComponent<Figure>().chessmanType.objectName == "WhiteKing")
                    .GetComponent<Figure>().GetFigureLocation() == posToCheck)
                {
                    i++;
                    posToCheck = DirectionStringFormator(loc, i * x, i * y);
                    type = GetTypeOfFigure(posToCheck);
                    if (type == 1||type ==0)
                        pathDatas.Add(AddToPathData(posToCheck, type));
                    break;
                }
            }
            if (type == 1 || type == -1)
                break;
        }
        return pathDatas;
    }
    public abstract List<PathData> GetTheRightPass(Figure searchedFigure);
    public string DirectionStringFormator(string basePosition, int x, int y)
    {
        return ((char)(basePosition[0] + x)).ToString() + ((char)(basePosition[1] + y)).ToString();
    }
    public int GetTypeOfFigure(string loc)
    {
        if (!PositionsCollector.instance.GetTileCoordinatesVector3Int().ContainsKey(loc))
            return 2;
        foreach (var figure in GameManager.sharedInstance.chessLists.whiteFigures)
        {
            if (figure.GetComponent<Figure>().GetFigureLocation() == loc)
            {
                if (GameManager.isWhiteTurn)
                    return 1;
                else
                    return -1;
            }
        }
        foreach (var figure in GameManager.sharedInstance.chessLists.blackFigures)
        {
            if (figure.GetComponent<Figure>().GetFigureLocation() == loc)
            {
                if (!GameManager.isWhiteTurn)
                    return 1;
                else
                    return -1;
            }
        }
        return 0;
    }
    public abstract List<PathData> FigurePathCreator();
}
//public enum TypeOfFigure
//{


//}
