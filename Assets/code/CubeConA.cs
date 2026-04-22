using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CubeConA : MonoBehaviour
{
    public bool IsAnima;
    public Vector3Int huongNow;
    public bool canMove;
    public List<Vector3Int> ThanAllPos;
    public SpriteRenderer sprite;
    public LineRenderer lineRenderer;
    public Vector3Int pos3D;
    public bool State = true;
    public huong huong_enum;
    public Mat matNow; 
    public void InitCube(Vector3Int startPos, huong huongChiDinh, Mat matKhoiDau)
    {
        pos3D = startPos;
        matNow = matKhoiDau;
        State = true;
        huong_enum = huongChiDinh; 
        
       
        if (sprite == null && transform.childCount > 0) sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
        
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

    public int[] getThan(Than than)
    {
        int[] vitri = {0,0};
        switch (than) {
            case Than.luilai: { vitri[1]-=1; break; }
            case Than.tienlai: { vitri[1] +=1; break; }
            case Than.sangphai: { vitri[0] +=1; break; }
            case Than.sangtrai: { vitri[0] -= 1; break; }
                
        }
        return vitri;
        }
    public Vector3Int GetThanPos (Mat mat,int[] vitri,Vector3Int Pos)
    {
        
        switch (mat) {
            case Mat.mat1: { Pos += new Vector3Int(vitri[0], vitri[1], 0); break;  } // -Z face
            case Mat.mat2: { Pos += new Vector3Int(vitri[0], vitri[1], 0); break; } // +Z face
            case Mat.mat3: { Pos += new Vector3Int(0, vitri[1], vitri[0]); break; } // -X face
            case Mat.mat4: { Pos += new Vector3Int(0, vitri[1], vitri[0]); break; } // +X face
            case Mat.mat5: { Pos += new Vector3Int(vitri[0], 0, vitri[1]); break; } // +Y face
            case Mat.mat6: { Pos += new Vector3Int(vitri[0], 0, vitri[1]); break; } // -Y face
        }
        return Pos;
    }
    public void UpdateBodyPath(List<int[]> steps, Mat startMat, Vector3Int startPos, int size)
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null) return;
        lineRenderer.useWorldSpace = false;

        int maxSize = Mathf.FloorToInt(size / 2);
        List<Vector3> pathPoints = new List<Vector3>();
        pathPoints.Add(Vector3.zero);

        Mat currentMat = startMat;
        Vector3Int currentPos = startPos;

        foreach (var step in steps)
        {
            Vector3Int prevPos = currentPos;
            
            // Tính toán vị trí tiếp theo
            Vector3Int nextPos = GetThanPos(currentMat, step, currentPos);
            var (newMat, wrappedPos) = CheckMat(currentMat, nextPos, maxSize);

            if (newMat != currentMat)
            {
                Vector3 worldEdge = CalculateEdgePoint(prevPos, wrappedPos, currentMat, newMat, maxSize);
                pathPoints.Add(transform.InverseTransformPoint(worldEdge));
            }

            pathPoints.Add(transform.InverseTransformPoint((Vector3)wrappedPos));
            currentPos = wrappedPos;
            currentMat = newMat;
        }

        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());
    }

    private Vector3 CalculateEdgePoint(Vector3 p1, Vector3 p2, Mat m1, Mat m2, int maxSize)
    {
        Vector3 edge = (p1 + p2) * 0.5f;
        float limit = maxSize + 1.0f;

    
        if (Mathf.Abs(edge.x) > maxSize) edge.x = Mathf.Sign(edge.x) * limit;
        if (Mathf.Abs(edge.y) > maxSize) edge.y = Mathf.Sign(edge.y) * limit;
        if (Mathf.Abs(edge.z) > maxSize) edge.z = Mathf.Sign(edge.z) * limit;

        return edge;
    }

    public (Mat, Vector3Int) CheckMat(Mat matHienTai, Vector3Int Pos, int maxSize)
    {
        int limit = maxSize + 1;

        switch (matHienTai)
        {
            case Mat.mat1: // Front (-Z face)
                if (Pos.y > maxSize) return (Mat.mat5, new Vector3Int(Pos.x, limit, -maxSize)); // Up -> Top
                if (Pos.y < -maxSize) return (Mat.mat6, new Vector3Int(Pos.x, -limit, -maxSize)); // Down -> Bottom
                if (Pos.x > maxSize) return (Mat.mat4, new Vector3Int(limit, Pos.y, -maxSize)); // Right -> Right
                if (Pos.x < -maxSize) return (Mat.mat3, new Vector3Int(-limit, Pos.y, -maxSize)); // Left -> Left
                break;
            case Mat.mat2: // Back (+Z face)
                if (Pos.y > maxSize) return (Mat.mat5, new Vector3Int(Pos.x, limit, maxSize)); // Up -> Top
                if (Pos.y < -maxSize) return (Mat.mat6, new Vector3Int(Pos.x, -limit, maxSize)); // Down -> Bottom
                if (Pos.x > maxSize) return (Mat.mat4, new Vector3Int(limit, Pos.y, maxSize)); // Right -> Right
                if (Pos.x < -maxSize) return (Mat.mat3, new Vector3Int(-limit, Pos.y, maxSize)); // Left -> Left
                break;
            case Mat.mat3: // Left (-X face)
                if (Pos.y > maxSize) return (Mat.mat5, new Vector3Int(-maxSize, limit, Pos.z)); // Up -> Top
                if (Pos.y < -maxSize) return (Mat.mat6, new Vector3Int(-maxSize, -limit, Pos.z)); // Down -> Bottom
                if (Pos.z > maxSize) return (Mat.mat2, new Vector3Int(-maxSize, Pos.y, limit)); // Back -> Back
                if (Pos.z < -maxSize) return (Mat.mat1, new Vector3Int(-maxSize, Pos.y, -limit)); // Front -> Front
                break;
            case Mat.mat4: // Right (+X face)
                if (Pos.y > maxSize) return (Mat.mat5, new Vector3Int(maxSize, limit, Pos.z)); // Up -> Top
                if (Pos.y < -maxSize) return (Mat.mat6, new Vector3Int(maxSize, -limit, Pos.z)); // Down -> Bottom
                if (Pos.z > maxSize) return (Mat.mat2, new Vector3Int(maxSize, Pos.y, limit)); // Back -> Back
                if (Pos.z < -maxSize) return (Mat.mat1, new Vector3Int(maxSize, Pos.y, -limit)); // Front -> Front
                break;
            case Mat.mat5: // Top (+Y face)
                if (Pos.x > maxSize) return (Mat.mat4, new Vector3Int(limit, maxSize, Pos.z)); // Right -> Right
                if (Pos.x < -maxSize) return (Mat.mat3, new Vector3Int(-limit, maxSize, Pos.z)); // Left -> Left
                if (Pos.z > maxSize) return (Mat.mat2, new Vector3Int(Pos.x, maxSize, limit)); // Back -> Back
                if (Pos.z < -maxSize) return (Mat.mat1, new Vector3Int(Pos.x, maxSize, -limit)); // Front -> Front
                break;
            case Mat.mat6: // Bottom (-Y face)
                if (Pos.x > maxSize) return (Mat.mat4, new Vector3Int(limit, -maxSize, Pos.z)); // Right -> Right
                if (Pos.x < -maxSize) return (Mat.mat3, new Vector3Int(-limit, -maxSize, Pos.z)); // Left -> Left
                if (Pos.z > maxSize) return (Mat.mat2, new Vector3Int(Pos.x, -maxSize, limit)); // Back -> Back
                if (Pos.z < -maxSize) return (Mat.mat1, new Vector3Int(Pos.x, -maxSize, -limit)); // Front -> Front
                break;
        }
        return (matHienTai, Pos);
    }
    public void getHuong(huong huong)
    {
        if (sprite == null && transform.childCount > 0) sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite == null) return;

        
        sprite.transform.localRotation = Quaternion.identity;

        switch (huong)
        {
            case huong.trai:
                huongNow = Vector3Int.left; 
                UpdateArrowVisual(0, 90f); break; 
            case huong.phai:
                huongNow = Vector3Int.right; 
                UpdateArrowVisual(1, -90f); break;
            case huong.tren:
                huongNow = Vector3Int.up; 
                UpdateArrowVisual(2, 0f); break;
            case huong.duoi:
                huongNow = Vector3Int.down; 
                UpdateArrowVisual(3, 180f); break;
            case huong.truoc:
                huongNow = new Vector3Int(0, 0, -1); 
                UpdateArrowVisual(4, 0f); break; 
            case huong.sau:
                huongNow = new Vector3Int(0, 0, 1); 
                UpdateArrowVisual(5, 0f); break;
        }
    }

    private void UpdateArrowVisual(int spriteIndex, float rotationZ)
    {
        
        if (gameManager.instance.ArrowImg != null && gameManager.instance.ArrowImg.Count > spriteIndex)
        {
            sprite.sprite = gameManager.instance.ArrowImg[spriteIndex];
        }
        else { 
       
        sprite.transform.localRotation = Quaternion.Euler(0, 0, rotationZ);
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
