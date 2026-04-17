using System.IO;
using UnityEngine;

[System.Serializable]
public class CubeCon 
{
  
    public huong huong;
    public bool State;
    public Mat Face;
    public Vector3Int pos;
    
    public CubeCon ( huong huong)
    {
    
        this.huong = huong;
        State = true;
    }    
}
