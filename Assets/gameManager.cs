using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public int size;
    public bool inMenu;
    public List<saveLevel> saveLevel;
    public saveLevel LevelNow;
    public CubeConA CubeConA;
    
  
    public Dictionary<Vector2Int, CubeConA> dataGrid = new Dictionary<Vector2Int, CubeConA>();
    public int blockCount = 0; 
    public void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        
    }
    
    public void loadLevel (int i)
    {
        LevelNow = saveLevel[i];
        size = LevelNow.CubeSize;

      
        foreach(var block in dataGrid.Values)
        {
            if (block != null) Destroy(block.gameObject);
        }
        dataGrid.Clear();
        blockCount = 0;

      
        foreach (var data in LevelNow.CubeCon)
        {
            Vector2Int pos2D = new Vector2Int(Mathf.RoundToInt(data.pos.x), Mathf.RoundToInt(data.pos.y));
            Vector3 worldPos = new Vector3(pos2D.x, pos2D.y, 0); 
            
            var f = Instantiate(CubeConA, worldPos, Quaternion.identity);
            f.InitCube(pos2D, data.huong); 
            if (!dataGrid.ContainsKey(pos2D))
            {
                dataGrid.Add(pos2D, f);
                blockCount++;
            }
            else 
            {
                Debug.LogWarning("Tọa độ " + pos2D + " bị trùng trong file SaveLevel!");
            }
        }
    }    
    
    void Update()
    {
        
    }
    
    public void checkWin ()
    {
        blockCount--;
        if (blockCount <= 0)
        {
            Debug.Log("YOU WIN! TẤT CẢ KHỐI ĐÃ BAY CẢ!");
           
        }
    }    
}
public enum huong
{
    None,
    tren,
    duoi,
    trai,
    phai,
}
