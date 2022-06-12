using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Figure
{
    private List<PathData> pawnMovePlaces;
    private List<PathData> pawnAttackPlaces;
    private bool pawnStartAddition = true;
    public bool PawnStartAddition { get => pawnStartAddition; set => pawnStartAddition = value; }
    private string enPassantCoordinate = string.Empty;
    public string EnPassantCoordinate { get => enPassantCoordinate; set => enPassantCoordinate = value; }
    public override List<PathData> FigurePathCreator()
    {
        pawnMovePlaces = new List<PathData>();
        pawnAttackPlaces = pawnAttackPlaces = new List<PathData>();
        return PawnPathCreator();
    }
    public List<PathData> GetPawnMovePlaces()
    {
        return pawnMovePlaces;
    }
    public List<PathData> GetPawnAttackPlaces()
    {
        return pawnAttackPlaces;
    }
    private PathData PawnAttackPlaces(string posToCheck)
    {
        int type = GetTypeOfFigure(posToCheck);
        if (type == 2)
            return new PathData();
        if (type == -1)
        {
            pawnAttackPlaces.Add(AddToPathData(posToCheck, type));
            return AddToPathData(posToCheck, -1);
        }
        else
        {
            pawnAttackPlaces.Add(AddToPathData(posToCheck, 1));
            return AddToPathData(posToCheck, 1);
        }         
    }
    private List<PathData> PawnPathCreator()
    {
        List<PathData> pathDatas = new List<PathData>();
        string loc = this.GetFigureLocation();
        int y = GameManager.isWhiteTurn == true ? 1 : -1;
        string posToCheck = DirectionStringFormator(loc, 0, y);
        if (GetTypeOfFigure(posToCheck) == 0)
        {
            pathDatas.Add(AddToPathData(posToCheck, 0));
            pawnMovePlaces.Add(pathDatas[pathDatas.Count - 1]);
            if (((Pawn)this).pawnStartAddition)
            {
                posToCheck = DirectionStringFormator(loc, 0, y + y);
                if (GetTypeOfFigure(posToCheck) == 0)
                {
                    pathDatas.Add(AddToPathData(posToCheck, 0));
                    pawnMovePlaces.Add(pathDatas[pathDatas.Count - 1]);
                }
            }
        }

        posToCheck = DirectionStringFormator(loc, 1, y);
        pathDatas.Add(PawnAttackPlaces(posToCheck));
        
        posToCheck = DirectionStringFormator(loc, -1, y);
        pathDatas.Add(PawnAttackPlaces(posToCheck));
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
