using DG.Tweening;
using UnityEngine;

public class CubeConA : MonoBehaviour
{
    public bool IsAnima;
    public Vector2Int huongNow;
    public bool canMove; 
    
  
    public Vector2Int pos2D;
    public bool State = true;
    public huong huong_enum;

  
    public void InitCube(Vector2Int startPos, huong huongChiDinh)
    {
        pos2D = startPos;
        State = true;
        huong_enum = huongChiDinh; 
        getHuong(huong_enum);
     
    }

    
    public void MoveArrow()
    {
        var grid = gameManager.instance.dataGrid; 

        if (!IsAnima && State == true)
        {
            getHuong(huong_enum); 
            GetCanMove(pos2D, huongNow);
            if (canMove == true)
            {
                removeAndCheckEnd();
            } 
            else
            {
                transform.gameObject.GetComponent<Renderer>().material.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);
            }
        }
    }
    public void GetCanMove(Vector2Int s,Vector2Int s1)
    {
        canMove = true; 
        var current = s + s1;
        var grid = gameManager.instance.dataGrid;
        int max_size = gameManager.instance.size;

        // Quét dài theo hướng của gạch cho tới viền bản đồ
        while (current.x >= 0 && current.x < max_size && current.y >= 0 && current.y < max_size)
        {     
            if (grid.ContainsKey(current) && grid[current].State == true)
            {
                canMove = false; 
                break;
            }  
            else
            {
                current = current + s1;
            }
        }
    }

    public void getHuong(huong huong)
    {
        switch (huong)
        {
            case huong.None:
                {
                    huongNow = Vector2Int.zero; break;
                }
            case huong.trai:
                {
                    huongNow = Vector2Int.left; break;
                }
            case huong.phai:
                {
                    huongNow = Vector2Int.right; break;
                }
            case huong.tren:
                {
                    huongNow = Vector2Int.up; break;
                }
            case huong.duoi:
                {
                    huongNow = Vector2Int.down; break;
                }
        }
    }

    public void removeAndCheckEnd()
    {
        IsAnima = true;
        State = false; 
        
        var vector = transform.position + (new Vector3(huongNow.x, huongNow.y, 0) * 10f);
        transform.DOMove(vector, 0.5f).SetEase(Ease.OutQuart).OnComplete(() => { 
            transform.DOScale(Vector3.zero, 0.2f); 
            IsAnima = false; 
            gameManager.instance.checkWin(); 
        });
    }    
}
