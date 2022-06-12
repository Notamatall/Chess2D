using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Figure
{
    const int possibleDirections = 4;
    public override List<PathData> FigurePathCreator()
    {
        return QueenPathCreator();
    }
    private List<PathData> QueenPathCreator()
    {
        List<PathData> pathDatas = new List<PathData>();
        pathDatas.AddRange(RookPathCreator());
        pathDatas.AddRange(BishopPathCreator());
        return pathDatas;
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
    private List<PathData> BishopPathCreator()
    {
        List<PathData> pathDatas = new List<PathData>();

        for (int direction = 0; direction < possibleDirections; direction++)
        {
            pathDatas.AddRange(CheckBishopPath(direction, this.GetFigureLocation()));
        }
        return pathDatas;
    }
    private List<PathData> CheckBishopPath(int direction, string loc)
    {
        switch (direction)
        {
            case 0:
                return LoopThePath(loc, -1, 1);
            case 1:
                return LoopThePath(loc, -1, -1);
            case 2:
                return LoopThePath(loc, 1, -1);
            case 3:
                return LoopThePath(loc, 1, 1);
        }
        return new List<PathData>();
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

    public override List<PathData> GetTheRightPass(Figure searchedFigure )
    {
        bool trigger = false;
        List<PathData> pathDatas = new List<PathData>()
         {
             new PathData(this.GetFigureLocation(),PathType.EnemyPath)
         };
        for (int direction = 0; direction < possibleDirections; direction++)
        {
            pathDatas.AddRange(CheckBishopPath(direction, this.GetFigureLocation()));
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
        if (trigger)
        {
            if (pathDatas[pathDatas.Count - 1].Ally != string.Empty || pathDatas[pathDatas.Count - 1].Empty != string.Empty)
                pathDatas.RemoveRange(pathDatas.Count - 2, 2);
            else
                pathDatas.RemoveAt(pathDatas.Count - 1);
            return pathDatas;
        }
        
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
        if (pathDatas[pathDatas.Count - 1].Ally != string.Empty || pathDatas[pathDatas.Count - 1].Empty != string.Empty)
            pathDatas.RemoveRange(pathDatas.Count - 2, 2);
        else
            pathDatas.RemoveAt(pathDatas.Count - 1);
        return pathDatas;
    }
}
