using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Figure
{
    public override List<PathData> FigurePathCreator()
    {
        return KnightPathCreator();
    }
    private List<PathData> KnightPathCreator()
    {
        List<PathData> pathDatas = new List<PathData>();
        string loc = this.GetFigureLocation();
        string posToCheck = string.Empty;
        posToCheck = DirectionStringFormator(loc, -1, 2);
        pathDatas.Add(AddToPathData(posToCheck, GetTypeOfFigure(posToCheck)));
        posToCheck = DirectionStringFormator(loc, -2, 1);
        pathDatas.Add(AddToPathData(posToCheck, GetTypeOfFigure(posToCheck)));
        posToCheck = DirectionStringFormator(loc, 1, 2);
        pathDatas.Add(AddToPathData(posToCheck, GetTypeOfFigure(posToCheck)));
        posToCheck = DirectionStringFormator(loc, 2, 1);
        pathDatas.Add(AddToPathData(posToCheck, GetTypeOfFigure(posToCheck)));
        posToCheck = DirectionStringFormator(loc, -1, -2);
        pathDatas.Add(AddToPathData(posToCheck, GetTypeOfFigure(posToCheck)));
        posToCheck = DirectionStringFormator(loc, -2, -1);
        pathDatas.Add(AddToPathData(posToCheck, GetTypeOfFigure(posToCheck)));
        posToCheck = DirectionStringFormator(loc, 1, -2);
        pathDatas.Add(AddToPathData(posToCheck, GetTypeOfFigure(posToCheck)));
        posToCheck = DirectionStringFormator(loc, 2, -1);
        pathDatas.Add(AddToPathData(posToCheck, GetTypeOfFigure(posToCheck)));
        return pathDatas;
    }
    public override List<PathData> GetTheRightPass(Figure searchedFigure )
    {
        List<PathData> pathDatas = new List<PathData>()
         {
             new PathData(this.GetFigureLocation(),PathType.EnemyPath)
         };
        return pathDatas;
    }

}
