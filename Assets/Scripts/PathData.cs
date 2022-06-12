using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathData
{
    public PathData() { }
    public PathData(string path,PathType pathType)
    {
        if (pathType == PathType.EnemyPath)
            Enemy = path;
        if (pathType == PathType.AllyPath)
            Ally = path;
        if (pathType == PathType.EmptyPath)
            Empty = path;
    }
    public string Empty { get; set; } = string.Empty;
    public string Enemy { get; set; } = string.Empty;
    public string Ally { get; set; } = string.Empty;
    public string GetData()
    {
        if (Empty != string.Empty)
            return Empty;
        if (Enemy != string.Empty)
            return Enemy;
        if (Ally != string.Empty)
            return Ally;
        return string.Empty;
    }
}

public enum PathType
{
    EnemyPath = -1,
    EmptyPath,
    AllyPath,
    OutOfTilemapPath
}
