using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Figure
{
    private bool isItMoved = false;
    public bool IsItMoved { get => isItMoved; set => isItMoved = value; }
    private const int possibleDirections = 4;
    public override List<PathData> FigurePathCreator()
    {
        return RookPathCreator();
    }
    private List<PathData> CheckRookPath(int direction, string loc)
    {
        switch (direction)
        {
            case 0:
                return LoopThePath(loc, 0, 1);
            case 1:
                return LoopThePath(loc, -1, 0);
            case 2:
                return LoopThePath(loc, 0, -1);
            case 3:
                return LoopThePath(loc, 1, 0);
        }
        return new List<PathData>();
    }

    private List<PathData> RookPathCreator()
    {
        List<PathData> pathDatas = new List<PathData>();
        for (int direction = 0; direction < possibleDirections; direction++)
        {
            pathDatas.AddRange(CheckRookPath(direction, this.GetFigureLocation()));
        }
        return pathDatas;
    }
    public override List<PathData> GetTheRightPass(Figure searchedFigure )
    {
        bool trigger = false;
        List<PathData> pathDatas = new List<PathData>()
         {
             new PathData(this.GetFigureLocation(),PathType.EnemyPath)
         };
        for (int direction = 0; direction < possibleDirections; direction++)
        {
            pathDatas.AddRange(CheckRookPath(direction, this.GetFigureLocation()));
            foreach (var path in pathDatas)
            {
                if (path.GetData() == searchedFigure.GetFigureLocation())
                {
                    trigger = true;
                    break;
                }
            }
            if (trigger) 
                break;
            else
                pathDatas.RemoveRange(1, pathDatas.Count - 1);
        }
        if(pathDatas[pathDatas.Count-1].Ally!=string.Empty|| pathDatas[pathDatas.Count - 1].Empty != string.Empty)
        pathDatas.RemoveRange(pathDatas.Count - 2, 2);
        else
            pathDatas.RemoveAt(pathDatas.Count - 1);
        return pathDatas;
    }
}
