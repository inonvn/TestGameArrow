using DG.Tweening;
using UnityEngine;

public class CubeConA : MonoBehaviour
{
    public bool IsAnima;
    public Vector3Int huongNow;
    public bool canMove;

    public SpriteRenderer sprite;
    public Vector3Int pos3D;
    public bool State = true;
    public huong huong_enum;
    public void InitCube(Vector3Int startPos, huong huongChiDinh)
    {
        pos3D = startPos;
        State = true;
        huong_enum = huongChiDinh; 
        getHuong(huong_enum);
     
    }
    public void MoveArrow()
    {
        var grid = gameManager.instance.dataGrid;
        gameManager.instance.settingManager.audioS.PlayOneShot(gameManager.instance.settingManager.clip[0]);
        if (!IsAnima && State == true)
        {
            getHuong(huong_enum); 
            GetCanMove(pos3D, huongNow);
            if (canMove == true)
            {
                removeAndCheckEnd();
            } 
            else
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.red, 0.5f).SetLoops(2, LoopType.Yoyo);
            }
        }
    }
    public void GetCanMove(Vector3Int s, Vector3Int s1)
    {
        canMove = true; 
        var current = s + s1;
        var grid = gameManager.instance.dataGrid;
        for (int i = 0; i < 50; i++)
        {     
            bool isHittingArrow = grid.ContainsKey(current) && grid[current].State == true;
            bool isHittingStatic = gameManager.instance.staticBlocks.ContainsKey(current);

            if (isHittingArrow || isHittingStatic)
            {
                canMove = false; 
                break;
            }  
            current = current + s1;
        }
    }
    public void getHuong(huong huong)
    {
        switch (huong)
        {
            case huong.None:
                {
                    huongNow = Vector3Int.zero; break;
                }
            case huong.trai:
                {
                    if(gameManager.instance.ArrowImg.Count > 0) sprite.sprite = gameManager.instance.ArrowImg[0];
                    huongNow = new Vector3Int(-1, 0, 0); break;
                }
            case huong.phai:
                {
                    if(gameManager.instance.ArrowImg.Count > 1) sprite.sprite = gameManager.instance.ArrowImg[1];
                    huongNow = new Vector3Int(1, 0, 0); break;
                }
            case huong.tren:
                {
                    if(gameManager.instance.ArrowImg.Count > 2) sprite.sprite = gameManager.instance.ArrowImg[2];
                    huongNow = new Vector3Int(0, 1, 0); break;
                }
            case huong.duoi:
                {
                    if(gameManager.instance.ArrowImg.Count > 3) sprite.sprite = gameManager.instance.ArrowImg[3];
                    huongNow = new Vector3Int(0, -1, 0); break;
                }
            case huong.truoc:
                {
                    if (gameManager.instance.ArrowImg.Count > 4) sprite.sprite = gameManager.instance.ArrowImg[4]; // Ảnh mũi tên trục Z
                    huongNow = new Vector3Int(0, 0, -1); break; 
                }
            case huong.sau:
                {
                    if (gameManager.instance.ArrowImg.Count > 5) sprite.sprite = gameManager.instance.ArrowImg[5];
                    huongNow = new Vector3Int(0, 0, 1); break;  
                }
        }
    }

    public void removeAndCheckEnd()
    {
       gameManager.instance. settingManager.getCHangePoint();
        IsAnima = true;
        State = false; 
        Vector3 localDir = new Vector3(huongNow.x, huongNow.y, huongNow.z);
        Vector3 worldDir = transform.parent != null ? transform.parent.TransformDirection(localDir) : localDir;
        
        var vector = transform.position + (worldDir * 10f);
        transform.DOMove(vector, 0.8f).SetEase(Ease.OutQuart).OnComplete(() => { 
            transform.DOScale(Vector3.zero, 0.5f); 
            IsAnima = false; 
            gameManager.instance.checkWin(); 
        });
    }    
}
