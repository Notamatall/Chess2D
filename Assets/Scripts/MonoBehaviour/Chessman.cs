using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    private string figureCurrLocation;
    public bool pawnStartAddition = true;
    public ChessmanType chessmanType;
    public string GetFigureLocation() => figureCurrLocation;
    public string SetFigureLocation(string location) => figureCurrLocation = location;
}
