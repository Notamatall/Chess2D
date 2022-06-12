using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptLists")]
public class ChessLists : ScriptableObject
{
    public List<GameObject> whiteFigures;
    public List<GameObject> blackFigures;
}
