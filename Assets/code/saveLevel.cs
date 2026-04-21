using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "saveLevel", menuName = "Scriptable Objects/saveLevel")]
public class saveLevel : ScriptableObject
{
    public int CubeSize;
    public List<CubeCon> CubeCon  ;
}


