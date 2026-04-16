using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public int size;
    public void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void checkWin ()
    {

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
