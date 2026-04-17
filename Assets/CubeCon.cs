using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "CubeCon", menuName = "Scriptable Objects/CubeCon")]
public class CubeCon : ScriptableObject
{
    public Transform transform;
    public huong huong;
    public bool State;
    public Vector2Int pos;
    public CubeCon (Transform transform, huong huong)
    {
        this.transform = transform;
        this.huong = huong;
        State = true;
    }    
}
