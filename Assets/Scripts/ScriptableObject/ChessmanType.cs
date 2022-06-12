using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="ChessmanType")]
public class ChessmanType : ScriptableObject
{
    public string objectName;
    public Type type;
    public bool IsWhite;
    public enum Type
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

}
