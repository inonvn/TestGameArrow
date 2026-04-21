using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

[System.Serializable]
public class CubeCon 
{
  
    public huong huong;
    public bool State;
    public Mat Face;
    public Vector3Int pos;
    public List<CubeThan> than;
    public CubeCon ( huong huong)
    {
    
        this.huong = huong;
        State = true;
    }    
}
[System.Serializable]
public class CubeThan {
    public Than than;
}
