using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Figure
{
    private bool isItMoved = false;
    public bool IsItMoved { get => isItMoved; set => isItMoved = value; }
    public override List<PathData> FigurePathCreator()
    {
        return KingPathCreator();
    }
    public override List<PathData> GetTheRightPass(Figure searchedFigure )
    {
        return new List<PathData>();
    }
    public void RemoveDangerousPlaces(List<PathData> toChange, List<PathData> toCompareWith)
    {
        List<PathData> indexesList = new List<PathData>();
        for (int way = 0; way < toChange.Count; way++)
        {
            foreach (var waysUnderAttack in toCompareWith)
            {
                if (toChange[way].GetData() == waysUnderAttack.GetData())
                {
                    if (!indexesList.Contains(toChange[way]))
                        indexesList.Add(toChange[way]);
                }
            }
        }
        for (int index = 0; index < indexesList.Count; index++)
            toChange.Remove(indexesList[index]);
    }
    private List<PathData> KingPathCreator()
    {
        List<PathData> pathDatas = new List<PathData>();
        string loc = this.GetFigureLocation();
        string posToCheck = string.Empty;
        List<string> kingWaysToGo = new List<string>
        {
        DirectionStringFormator(loc, -1, 1),
        DirectionStringFormator(loc, 0, 1),
        DirectionStringFormator(loc, 1, 1),
        DirectionStringFormator(loc, 1, 0),
        DirectionStringFormator(loc, 1, -1),
        DirectionStringFormator(loc, 0, -1),
        DirectionStringFormator(loc, -1, -1),
        DirectionStringFormator(loc, -1, 0),
        };
        foreach (var way in kingWaysToGo)
        {
            pathDatas.Add(AddToPathData(way, GetTypeOfFigure(way)));
        }
        return pathDatas;
    }
}
