using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;

public class CubeConA : MonoBehaviour
{
    public Dictionary<Vector2Int, CubeCon> dataGrid = new Dictionary<Vector2Int, CubeCon>();
    public bool IsAnima;
    public Vector2Int huongNow;
    public bool cantMove;

    public void MoveArrow(Vector2Int x)
    {
        if (IsAnima || dataGrid.ContainsKey(x) || !dataGrid[x].State == false)
        {
            var currCube = dataGrid[x];
            getHuong(dataGrid[x].huong);
            GetCanMove(x, huongNow);
            if (cantMove==true)
            {
                removeAndCheckEnd(x);
            } 
            else
            {
                dataGrid[x].transform.gameObject.GetComponent<Renderer>().material.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);
            }
        }
    }
    public void GetCanMove(Vector2Int s,Vector2Int s1)
    {
        var current = s + s1;
        while (current.x >= 0 && current.x <gameManager.instance. size && current.y >= 0 && current.y < gameManager.instance. size)
        {     
        if (dataGrid.ContainsKey(current) == true && dataGrid[current].State == true)
            {
                cantMove = false;
                break;
            }  
        else
            {
                current = current + s1;
            }
        }
        cantMove = true;
    }
    public void getHuong (huong huong)
    {
        switch(huong) 
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
                    huongNow=Vector2Int.right; break;
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
    public void removeAndCheckEnd (Vector2Int x)
    {
        IsAnima = true;
        dataGrid[x].State = false;
        getHuong (dataGrid[x].huong);
        var vector = dataGrid[x].transform.position + (new Vector3(huongNow.x, huongNow.y,0)*10f);
        dataGrid[x].transform.DOMove(vector, 0.5f).SetEase(Ease.OutQuart).OnComplete(() => { dataGrid[x].transform.DOScale(Vector3.zero, 0.2f); IsAnima = false; gameManager.instance.checkWin(); });
    }    
}
