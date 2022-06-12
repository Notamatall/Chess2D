using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementController : MonoBehaviour
{
    private List<PathData> localPathDataList;// список который хранит данные куда может ходить одна фигура, на которую только что нажали
    private List<PathData> globalPathDataList;// список который хранит данные куда бьют все вражеские фигуры
    private List<PathData> linePathDataList;//список который хранит данные по линии от одной фигуры, до другой
    private List<GameObject> allowedToMoveFigures;//список который хранит объекты, которые могут ходить, в случае, если королю объявлен шаг
    private string[] placesTosign;
    List<List<PathData>> squaresWhereFigureMayGo;//список, который хранит списки мест, куда может пойти сооветсвенная по индексу списка фигура, что бы защитить короля или убить нападающего.
    private List<GameObject> attacker;//хранит ссылку на фигуру, которая объявляет шаг в данный момент, облегчает доступ
    private bool check = false;
    private bool calculated = false;
    private string pressedFigurePos = string.Empty;
    public enum CollectionType
    {
        global,
        local,
        line,
    }
    private void Start()
    {
        localPathDataList = new List<PathData>();
        globalPathDataList = new List<PathData>();
        linePathDataList = new List<PathData>();
        allowedToMoveFigures = new List<GameObject>();
        squaresWhereFigureMayGo = new List<List<PathData>>();
        attacker = new List<GameObject>();
    }
    private void CollectGlobalAttacks()
    {
        GameManager.isWhiteTurn = !GameManager.isWhiteTurn;
        foreach (var figure in (GameManager.isWhiteTurn == false ? GameManager.sharedInstance.chessLists.blackFigures : GameManager.sharedInstance.chessLists.whiteFigures))
            CollectPathData(figure.GetComponent<Figure>(), CollectionType.global);
        GameManager.isWhiteTurn = !GameManager.isWhiteTurn;
        calculated = true;
    }
    private void CollectLineOfAttack()
    {
        GameManager.isWhiteTurn = !GameManager.isWhiteTurn;
        List<PathData> tmp = new List<PathData>(25);
        foreach (var figure in (GameManager.isWhiteTurn == false ? GameManager.sharedInstance.chessLists.blackFigures : GameManager.sharedInstance.chessLists.whiteFigures))
        {
            CollectPathData(figure.GetComponent<Figure>(), CollectionType.line);
            if (linePathDataList.Count > 0)
            {
                tmp.AddRange(linePathDataList);
                attacker.Add(figure);
            }
        }
        linePathDataList = tmp;
        GameManager.isWhiteTurn = !GameManager.isWhiteTurn;
    }
    private void ClearGlobalList()
    {
        globalPathDataList.Clear();
    }
    private void CheckMate(bool isCheckMate)
    {
        if (isCheckMate)
        {
            ChangeTileColor(GetKingGameObject().GetComponent<Figure>().GetFigureLocation(), Color.HSVToRGB(0.7f, 0.7f, 1.6f));
            gameObject.SetActive(false);
        }
    }
    private bool CollectAllAllowedToMove()
    {
        bool isAllowed = false;
        foreach (var gameObject in (GameManager.isWhiteTurn == false ? GameManager.sharedInstance.chessLists.blackFigures : GameManager.sharedInstance.chessLists.whiteFigures))
        {
            Figure figure = gameObject.GetComponent<Figure>();
            CollectPathData(figure, CollectionType.local);
            if (figure.chessmanType.type == ChessmanType.Type.King)// этот иф нужен для того что бы добавить квадраты куда король может убежать от шага.
            {
                if (attacker.Count == 2)
                {
                    squaresWhereFigureMayGo.Clear();
                    allowedToMoveFigures.Clear();
                }

                List<PathData> tmpList = new List<PathData>();
                for (int i = 0; i < localPathDataList.Count; i++)
                    if (localPathDataList[i].Empty != string.Empty || localPathDataList[i].Enemy != string.Empty)
                        tmpList.Add(localPathDataList[i]);

                localPathDataList = tmpList;
                if (localPathDataList.Count > 0)//если король не может никуда пойти вообще и сьесть фигуру которая на него нападает то никуда его не добавляем
                {
                    allowedToMoveFigures.Add(gameObject);
                    squaresWhereFigureMayGo.Add(new List<PathData>());
                    squaresWhereFigureMayGo[squaresWhereFigureMayGo.Count - 1].AddRange(localPathDataList);
                    ClearLocalPathData();
                    continue;
                }
                if (allowedToMoveFigures.Count == 0 && attacker.Count == 2)
                {
                    return true;
                }
            }
            else
            if (figure.chessmanType.type == ChessmanType.Type.Pawn)
            {
                ClearLocalPathData();
                foreach (var path in ((Pawn)figure).GetPawnMovePlaces())
                {
                    foreach (var path2 in linePathDataList.GetRange(1, linePathDataList.Count - 1))// начинаю с 1 элемента списка линии атаки фигуры, потому что пешка в данном случае может только закрыть собой, но не сьесть фигуру
                    {
                        if (path.GetData() == path2.GetData())
                            localPathDataList.Add(path);
                    }
                }
                foreach (var path in ((Pawn)figure).GetPawnAttackPlaces())
                {
                    if (attacker[0].GetComponent<Figure>().GetFigureLocation() == path.GetData())
                    {
                        localPathDataList.Add(path);
                    }
                }
            }

            foreach (var localSquare in localPathDataList)
            {
                foreach (var lineSqaure in linePathDataList)
                {
                    if (localSquare.GetData() == lineSqaure.GetData())
                    {
                        if (!isAllowed)
                        {
                            isAllowed = true;
                            squaresWhereFigureMayGo.Add(new List<PathData>());
                        }
                        squaresWhereFigureMayGo[squaresWhereFigureMayGo.Count - 1].Add(localSquare);
                    }
                }
            }
            if (isAllowed)
            {
                //ChangeTileColor(GetKingGameObject().GetComponent<Figure>().GetFigureLocation(),)
                allowedToMoveFigures.Add(gameObject);
                isAllowed = false;
            }
            ClearLocalPathData();
        }
        // i will get all figures which may go to the line of attack and those places if more then one;

        if (allowedToMoveFigures.Count == 0)
        {
            return true;
        }

        //right now i need to put all allowed to move figures to places where they will close king 
        //or kill the attacker and then if attacked team will be still underAttack - remove figure from list 
        //можно убрать список со списком возможных ходов и просто ставить фигуры на любые атакуемые места и проверять будет ли шаг после этого.
        string prevPos = string.Empty;
        bool trigger = false;

        string attackerPrevPos = attacker[0].GetComponent<Figure>().GetFigureLocation();
        Figure currFigure = null;
        GameObject gO = null;
        for (int i = 0; i < allowedToMoveFigures.Count; i++)
        {
            currFigure = allowedToMoveFigures[i].GetComponent<Figure>();
            prevPos = allowedToMoveFigures[i].GetComponent<Figure>().GetFigureLocation();
            if (currFigure == GetKingGameObject().GetComponent<Figure>())
                continue;
            for (int j = 0; j < squaresWhereFigureMayGo[i].Count; j++)
            {
                allowedToMoveFigures[i].GetComponent<Figure>().SetFigureLocation(squaresWhereFigureMayGo[i][j].GetData());
                gO = DestroyFigure2(attackerPrevPos);
                if (CheckIfKingCheck())
                {
                    trigger = true;
                    break;
                }
                RestoreFigure(gO, attackerPrevPos);
            }
            allowedToMoveFigures[i].GetComponent<Figure>().SetFigureLocation(prevPos);
            if (trigger)
            {
                allowedToMoveFigures.RemoveAt(i);
                squaresWhereFigureMayGo.RemoveAt(i);
                i--;
                trigger = false;
            }
        }
        if (allowedToMoveFigures.Count == 0)
        {
            return true;
        }
        return false;
    }
    private GameObject GetFigureOnCell()
    {
        string pressedCellValue = GetChessBoardCoordinateIfExist();
        if (pressedCellValue == null)
            return null;
        GameObject gameObject = GetFigureIfExist(pressedCellValue);
        return gameObject;
    }
    private void CastlingWithAllIfs()
    {
        GameObject kingObject = GetKingGameObject();
        Figure kingFigure = kingObject?.GetComponent<Figure>();
        if (pressedFigurePos == kingFigure.GetFigureLocation())
        {
            if (!((King)kingFigure).IsItMoved)
            {
                GameObject rookObject = GetFigureOnCell();
                Figure rookFigure = rookObject?.GetComponent<Figure>();
                if (rookFigure != null && rookFigure.chessmanType.type == ChessmanType.Type.Rook)
                {
                    if (!((Rook)rookFigure).IsItMoved)
                    {
                        Castling(kingObject, rookObject);
                    }
                }
            }
        }
    }
    private void Castling(GameObject kingObject, GameObject rookObject)
    {
        Figure kingFigure = kingObject.GetComponent<Figure>();
        Figure rookFigure = rookObject.GetComponent<Figure>();
        List<PathData> pathData = GetZoneBetweenTwoHorizontal(kingFigure, rookFigure);
        int startingSize = pathData.Count;
        ((King)kingFigure).RemoveDangerousPlaces(pathData, globalPathDataList);
        if (pathData.Count != startingSize)
            return;
        foreach (var path in pathData)
        {
            if (path.Empty == string.Empty)
                return;
        }
        rookFigure.SetFigureLocation(pathData[0].GetData());
        kingFigure.SetFigureLocation(pathData[1].GetData());
        MoveFigureTo(kingObject, pathData[1].GetData());
        MoveFigureTo(rookObject, pathData[0].GetData());
        ((King)kingFigure).IsItMoved = true;
        ((Rook)rookFigure).IsItMoved = true;
        NextTurn();
    }
    private void CheckForEnPassant(Figure chessman)
    {
        if (chessman.chessmanType.type == ChessmanType.Type.Pawn)
        {
            string chessmanLocation = chessman.GetFigureLocation();
            if (GameManager.isWhiteTurn)
            {
                if (chessman.DirectionStringFormator(chessmanLocation, 0, -2) != pressedFigurePos)
                    return;
            }
            else
            {
                if (chessman.DirectionStringFormator(chessmanLocation, 0, 2) != pressedFigurePos)
                    return;
            }
            GameObject gameObjectOne = GetFigureIfExist(chessman.DirectionStringFormator(chessmanLocation, -1, 0), false);
            GameObject gameObjectTwo = GetFigureIfExist(chessman.DirectionStringFormator(chessmanLocation, 1, 0), false);
            Figure figureOne = gameObjectOne?.GetComponent<Figure>();
            Figure figureTwo = gameObjectTwo?.GetComponent<Figure>();
            if (figureOne?.chessmanType.type == ChessmanType.Type.Pawn)
                ((Pawn)figureOne).EnPassantCoordinate = chessmanLocation;
            if (figureTwo?.chessmanType.type == ChessmanType.Type.Pawn)
                ((Pawn)figureTwo).EnPassantCoordinate = chessmanLocation;
        }
    }
    private List<PathData> GetZoneBetweenTwoHorizontal(Figure king, Figure rook)
    {
        List<PathData> pathDatas = new List<PathData>();
        int xDirection = rook.GetFigureLocation().CompareTo(king.GetFigureLocation());
        int maxLengthFromKingToRook = xDirection > 0 ? 3 : 4;
        for (int i = 1; i < maxLengthFromKingToRook; i++)
        {
            string posToCheck = rook.DirectionStringFormator(king.GetFigureLocation(), i * xDirection, 0);
            int type = rook.GetTypeOfFigure(posToCheck);
            pathDatas.Add(rook.AddToPathData(posToCheck, type));
        }
        return pathDatas;
    }
    private void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            CastlingWithAllIfs();
        }
        if (Input.GetMouseButtonDown(0))
        {
            MoveFiguresFunction();
        }
    }
    private void MoveFiguresFunction()
    {
        string pressedCellValue = GetChessBoardCoordinateIfExist();
        if (pressedCellValue == null)
            return;
        if (!calculated)
        {
            CollectGlobalAttacks();
        }

        if (MoveFigure(pressedCellValue))
        {
            NextTurn();
            if (check)
                check = false;
            if (CheckIfKingCheck())
            {
                check = true;
                CollectLineOfAttack();
                CheckMate(CollectAllAllowedToMove());
                ShowAllowedToMoveColor();
            }
            else
            {
                CheckStalemate(CollectIfThereAnyPossibleMoves());
            }

            calculated = false;
        }
        else
            ChooseFigure(GetFigureIfExist(pressedCellValue));
    }
    private void ClearEnPassant()
    {
        foreach (var gO in GameManager.isWhiteTurn == true ? GameManager.sharedInstance.chessLists.whiteFigures : GameManager.sharedInstance.chessLists.blackFigures)
        {
            Figure figure = gO.GetComponent<Figure>();
            if (figure.chessmanType.type == ChessmanType.Type.Pawn)
            {
                ((Pawn)figure).EnPassantCoordinate = string.Empty;
            }
        }
    }
    private void NextTurn()
    {
        ClearEnPassant();
        GameManager.isWhiteTurn = !GameManager.isWhiteTurn;
        ClearAllAllowedTilesWithChangedColor();
        ClearTilesWithChangedColor();
        ClearLocalPathData();
        ClearGlobalList();
        ClearIineData();
        allowedToMoveFigures.Clear();
        squaresWhereFigureMayGo.Clear();
        attacker.Clear();
    }
    private bool CollectIfThereAnyPossibleMoves()
    {
        foreach (var figure in (GameManager.isWhiteTurn == false ? GameManager.sharedInstance.chessLists.blackFigures : GameManager.sharedInstance.chessLists.whiteFigures))
        {
            CollectPathData(figure.GetComponent<Figure>(), CollectionType.local);
            for (int i = 0; i < localPathDataList.Count; i++)
                if (localPathDataList[i].Ally != string.Empty)
                {
                    localPathDataList.RemoveAt(i);
                    i--;
                }
            if (localPathDataList.Count > 0)
            {
                ClearLocalPathData();
                return false;
            }
        }
        return true;
    }
    private void CheckStalemate(bool isStaleMate)
    {
        if (isStaleMate)
        {
            ChangeTileColor(GetKingGameObject().GetComponent<Figure>().GetFigureLocation(), Color.HSVToRGB(0.3f, 0.8f, 1.6f));//Green
            gameObject.SetActive(false);
        }
    }
    private GameObject DestroyFigure2(string chessCoordinate)
    {
        GameObject figureToDestroy = GetFigureIfExist(chessCoordinate, false);
        if (GameManager.isWhiteTurn == true)
            GameManager.sharedInstance.chessLists.blackFigures.Remove(figureToDestroy);
        else
            GameManager.sharedInstance.chessLists.whiteFigures.Remove(figureToDestroy);
        figureToDestroy.GetComponent<Figure>().SetFigureLocation("");
        return figureToDestroy;
    }
    private GameObject DestroyFigure(string chessCoordinate)
    {
        GameObject figureToDestroy = GetFigureIfExist(chessCoordinate, false);
        if (GameManager.isWhiteTurn == true)
            GameManager.sharedInstance.chessLists.blackFigures.Remove(figureToDestroy);
        else
            GameManager.sharedInstance.chessLists.whiteFigures.Remove(figureToDestroy);
        figureToDestroy.GetComponent<Figure>().SetFigureLocation("");
        figureToDestroy.SetActive(false);
        return figureToDestroy;
    }
    private void RestoreFigure(GameObject figureToRestore, string prevLocation)
    {

        if (GameManager.isWhiteTurn == true)
            GameManager.sharedInstance.chessLists.blackFigures.Add(figureToRestore);
        else
            GameManager.sharedInstance.chessLists.whiteFigures.Add(figureToRestore);
        figureToRestore.GetComponent<Figure>().SetFigureLocation(prevLocation);
        figureToRestore.SetActive(true);
    }
    private string CheckIfEnPassant(Figure chessman, string locToMove)
    {
        if (chessman.chessmanType.type == ChessmanType.Type.Pawn)
        {
            Pawn pawn = chessman as Pawn;
            if (pawn.EnPassantCoordinate == string.Empty)
                return locToMove;
            if (GameManager.isWhiteTurn)
            {
                if (locToMove == pawn.DirectionStringFormator(pawn.EnPassantCoordinate, 0, 1))
                    return pawn.EnPassantCoordinate;
            }
            else
            {
                if (locToMove == pawn.DirectionStringFormator(pawn.EnPassantCoordinate, 0, -1))
                    return pawn.EnPassantCoordinate;
            }
        }
        return locToMove;
    }
    private bool MoveFigure(string chessSquare)
    {
        if (localPathDataList.Count == 0)
            return false;
        GameObject currentlyChosenFigure = GetFigureIfExist(pressedFigurePos);
        Figure chessman = currentlyChosenFigure.GetComponent<Figure>();
        foreach (var pathData in localPathDataList)
        {
            if (pathData.Empty == chessSquare)
            {
                chessman.SetFigureLocation(chessSquare);
                if (CheckIfKingCheck())
                {
                    chessman.SetFigureLocation(pressedFigurePos);
                    return false;
                }
                else
                    calculated = false;
                CheckForEnPassant(chessman);
                PersonalDataChanger(chessman);
                MoveFigureTo(currentlyChosenFigure, chessSquare);
                return true;
            }
            else if (pathData.Enemy == chessSquare)
            {
                chessman.SetFigureLocation(chessSquare);
                GameObject tmp = DestroyFigure(CheckIfEnPassant(chessman, chessSquare));
                if (CheckIfKingCheck())
                {
                    chessman.SetFigureLocation(pressedFigurePos);
                    RestoreFigure(tmp, CheckIfEnPassant(chessman, chessSquare));
                    return false;
                }
                else
                    calculated = false;
                PersonalDataChanger(chessman);
                MoveFigureTo(currentlyChosenFigure, chessSquare);
                return true;
            }
        }
        return false;
    }
    private static void PersonalDataChanger(Figure chessman)
    {
        if (chessman.chessmanType.type == ChessmanType.Type.Pawn)
            ((Pawn)chessman).PawnStartAddition = false;
        else if (chessman.chessmanType.type == ChessmanType.Type.Rook)
            ((Rook)chessman).IsItMoved = true;
        else if (chessman.chessmanType.type == ChessmanType.Type.Rook)
            ((King)chessman).IsItMoved = true;
    }
    private void MoveFigureTo(GameObject currentlyChosenFigure, string place)
    {
        currentlyChosenFigure.transform.position =
                    PositionsCollector.instance.GetTileCoordinatesVector3Int()[place] +
                    new Vector3(PositionsCollector.instance.GetAttachedTileMap().cellSize.x / 2,
                    PositionsCollector.instance.GetAttachedTileMap().cellSize.y / 2, 0);
    }
    private GameObject GetKingGameObject()
    {
        return GameManager.isWhiteTurn == true ?
            GameManager.sharedInstance.chessLists.whiteFigures.Find((name) => { return name.GetComponent<Figure>().chessmanType.objectName == "WhiteKing"; }) :
         GameManager.sharedInstance.chessLists.blackFigures.Find((name) => { return name.GetComponent<Figure>().chessmanType.objectName == "BlackKing"; });
    }
    public bool CheckIfKingCheck()
    {
        ClearGlobalList();
        CollectGlobalAttacks();
        GameObject king = GetKingGameObject();
        foreach (var pos in globalPathDataList)
        {
            if (king.GetComponent<Figure>().GetFigureLocation() == pos.GetData())
            {
                print("King under Check");
                return true;
            }
        }
        return false;
    }
    private void ClearTilesWithChangedColor()
    {
        foreach (var squareLocation in localPathDataList)
        {
            ChangeTileColor(squareLocation.GetData(), Color.white);
        }
    }
    private void ClearAllAllowedTilesWithChangedColor()
    {
        if (allowedToMoveFigures.Count == 0)
            return;
        foreach (var data in placesTosign)
        {
            ChangeTileColor(data, Color.white);
        }
    }
    private Vector3Int GetMousePositionInCell()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return PositionsCollector.instance.GetAttachedTileMap().WorldToCell(mousePos);
    }
    public string GetChessBoardCoordinateIfExist()
    {
        Vector3Int cellLocalPos = GetMousePositionInCell() * (int)PositionsCollector.instance.GetAttachedTileMap().cellSize.x;
        if (PositionsCollector.instance.GetChessCoordinatesString().ContainsKey(cellLocalPos))
            return PositionsCollector.instance.GetChessCoordinatesString()[cellLocalPos];
        return null;
    }
    public void CollectPathData(Figure chessman, CollectionType collectionType)
    {
        if (collectionType == CollectionType.global)
        {
            foreach (var g in chessman.FigurePathCreator())
            {
                if (!globalPathDataList.Contains(g))
                    globalPathDataList.Add(g);
            }
            globalPathDataList.RemoveAll((x) => x.GetData() == "");
            if (chessman.chessmanType.type == ChessmanType.Type.Pawn)
            {
                foreach (var movePlace in ((Pawn)chessman).GetPawnMovePlaces())
                    globalPathDataList.Remove(movePlace);
            }

        }
        else if (collectionType == CollectionType.local)
        {
            localPathDataList.AddRange(chessman.FigurePathCreator());
            localPathDataList.RemoveAll((x) => x.GetData() == "");
            if (chessman.chessmanType.type == ChessmanType.Type.Pawn)
            {
                AddEnPassant(chessman);
            }
            else
            if (chessman.chessmanType.type == ChessmanType.Type.King)
            {
                ((King)chessman).RemoveDangerousPlaces(localPathDataList, globalPathDataList);
            }
        }
        else if (collectionType == CollectionType.line)
        {
            linePathDataList.AddRange(chessman.FigurePathCreator());
            linePathDataList.RemoveAll((x) => x.GetData() == "");
            if (chessman.chessmanType.type == ChessmanType.Type.Pawn)
            {
                foreach (var movePlace in ((Pawn)chessman).GetPawnMovePlaces())
                    globalPathDataList.Remove(movePlace);
            }
            GameManager.isWhiteTurn = !GameManager.isWhiteTurn;
            Figure king = GetKingGameObject().GetComponent<Figure>();
            GameManager.isWhiteTurn = !GameManager.isWhiteTurn;
            foreach (var loc in linePathDataList)
                if (king.GetFigureLocation() == loc.GetData())
                {
                    linePathDataList = chessman.GetTheRightPass(king);
                    return;
                }
            ClearIineData();
        }
    }
    private void AddEnPassant(Figure chessman)
    {
        if (((Pawn)chessman).EnPassantCoordinate != string.Empty)
        {
            Pawn pawn = chessman as Pawn;
            if (GameManager.isWhiteTurn)
            {
                localPathDataList.Add(new PathData(pawn.DirectionStringFormator(pawn.EnPassantCoordinate, 0, 1), PathType.EnemyPath));
            }
            else
            {
                localPathDataList.Add(new PathData(pawn.DirectionStringFormator(pawn.EnPassantCoordinate, 0, -1), PathType.EnemyPath));
            }
        }
    }
    private void ClearIineData()
    {
        linePathDataList.Clear();
    }
    private void ClearLocalPathData()
    {
        localPathDataList.Clear();
    }
    public void ChooseFigure(GameObject pressedFigure)
    {
        if (pressedFigure == null)
            return;
        ClearTilesWithChangedColor();
        ClearLocalPathData();
        Figure chessman = pressedFigure.GetComponent<Figure>();
        if (chessman.GetFigureLocation() == pressedFigurePos)
            print("same figure pressed");
        pressedFigurePos = chessman.GetFigureLocation();
        if (check)
        {
            if (!allowedToMoveFigures.Contains(pressedFigure))
                return;
            FillLocalWithDirectPlaces(pressedFigure);
        }
        else
            CollectPathData(chessman, CollectionType.local);
        ShowPathColor();
    }
    private void FillLocalWithDirectPlaces(GameObject pressedFigure)
    {
        localPathDataList.AddRange(squaresWhereFigureMayGo[allowedToMoveFigures.IndexOf(pressedFigure)]);
    }
    public void ShowPathColor()
    {
        foreach (var data in localPathDataList)
        {
            if (data.Empty != string.Empty)
                ChangeTileColor(data.Empty, Color.HSVToRGB(0.6f, 0.8f, 1.6f));// LightBlue
            else if (data.Enemy != string.Empty)
                ChangeTileColor(data.Enemy, Color.HSVToRGB(1f, 0.75f, 1.6f));//Red
        }
    }
    public void ShowAllowedToMoveColor()
    {
        if (allowedToMoveFigures.Count == 0)
            return;
        placesTosign = new string[allowedToMoveFigures.Count];
        for (int i = 0; i < allowedToMoveFigures.Count; i++)
        {
            placesTosign[i] = allowedToMoveFigures[i].GetComponent<Figure>().GetFigureLocation();
            ChangeTileColor(placesTosign[i], Color.HSVToRGB(0.2f, 0.8f, 1.6f));
        }
    }
    private void ChangeTileColor(string squareLocation, Color color)
    {
        Vector3Int cellOrigin = PositionsCollector.instance.GetTileCoordinatesVector3Int()[squareLocation] / (int)PositionsCollector.instance.GetAttachedTileMap().cellSize.x;
        PositionsCollector.instance.GetAttachedTileMap().SetTileFlags(cellOrigin, TileFlags.None);
        PositionsCollector.instance.GetAttachedTileMap().SetColor(cellOrigin, color);
    }
    private GameObject GetFigureIfExist(string chessCoordinate, bool ifOnlyFriendly = true)
    {
        if (GameManager.isWhiteTurn == ifOnlyFriendly)
        {
            foreach (var figure in GameManager.sharedInstance.chessLists.whiteFigures)
            {
                string sadasd = figure.GetComponent<Figure>().GetFigureLocation();
                if (sadasd == chessCoordinate)
                {
                    return figure;
                }
            }
        }
        else if (!GameManager.isWhiteTurn == ifOnlyFriendly)
        {
            foreach (var figure in GameManager.sharedInstance.chessLists.blackFigures)
            {
                if (figure.GetComponent<Figure>().GetFigureLocation() == chessCoordinate)
                {
                    return figure;
                }
            }
        }
        return null;
    }
}