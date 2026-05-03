using System.Linq;
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
    public List<(GameObject,Vector3Int)> bodyParts = new List<(GameObject,Vector3Int)>();
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
        if (gameManager.instance.settingManager != null && gameManager.instance.settingManager.audioS != null)
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

                if (sprite != null)
                {
                    sprite.DOKill();
                    sprite.color = Color.black;
                    sprite.DOColor(Color.red, 0.5f).SetLoops(2, LoopType.Yoyo);
                }
                gameManager.instance.LostHP(1);
                foreach (var part in bodyParts)
                {
                    if (part.Item1 != null)
                    {
                        SpriteRenderer sr = null;
                        if (part.Item1.transform.childCount > 1)
                            sr = part.Item1.transform.GetChild(1).GetComponent<SpriteRenderer>();
                        else
                            sr = part.Item1.GetComponentInChildren<SpriteRenderer>();
                        
                        if (sr != null)
                        {
                            sr.DOKill();
                            sr.color = Color.black;
                            sr.DOColor(Color.red, 0.5f).SetLoops(2, LoopType.Yoyo);
                        }
                    }
                }
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
            var isHittingArrow = grid.ContainsKey(current) && grid[current].State == true;
            var isHittingStatic = gameManager.instance.staticBlocks.ContainsKey(current);
            var isHittingBody = grid.Values.Any(cube => cube.State == true && cube.bodyParts.Any(part => part.Item1 != null && part.Item2 == current));

            if (isHittingArrow || isHittingStatic || isHittingBody)
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
        Vector3 localOffset = new Vector3(vitri[0], vitri[1], 0);
        Vector3 worldOffset = GetRotationForMat(mat) * localOffset;
        Pos += new Vector3Int(Mathf.RoundToInt(worldOffset.x), Mathf.RoundToInt(worldOffset.y), Mathf.RoundToInt(worldOffset.z));
        return Pos;
    }
    private Quaternion GetRotationForMat(Mat mat)
    {
        switch (mat)
        {
            case Mat.mat1: return Quaternion.LookRotation(Vector3.forward);
            case Mat.mat2: return Quaternion.LookRotation(Vector3.back);
            case Mat.mat3: return Quaternion.LookRotation(Vector3.right);
            case Mat.mat4: return Quaternion.LookRotation(Vector3.left);
            case Mat.mat5: return Quaternion.LookRotation(Vector3.down, Vector3.forward);
            case Mat.mat6: return Quaternion.LookRotation(Vector3.up, Vector3.forward);
        }
        return Quaternion.identity;
    }
    public void UpdateBodyPath(List<Than> steps, Mat startMat, Vector3Int startPos, int size)
    {
    bool DauTien=true;
        if (transform.childCount > 1)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }

        foreach (var part in bodyParts) if (part.Item1 != null) Destroy(part.Item1);
        bodyParts.Clear();

        int maxSize = Mathf.FloorToInt(size / 2);
        Mat currentMat = startMat;
        Vector3Int currentPos = startPos;

        foreach (var than in steps)
        {
            int[] step = getThan(than);
            Vector3Int nextPos = GetThanPos(currentMat, step, currentPos);
            var (newMat, wrappedPos) = CheckMat(currentMat, nextPos, maxSize);

            Quaternion faceRot = GetRotationForMat(newMat);

            if (DauTien == false) { 
                GameObject bodyPart = Instantiate(gameObject, wrappedPos, faceRot, transform.parent);
                
                if (than == Than.sangphai || than == Than.sangtrai)
                {
                    bodyPart.transform.Rotate(0, 0, 90f, Space.Self);
                }

                CubeConA comp = bodyPart.GetComponent<CubeConA>();
                if (comp != null) Destroy(comp);

                if (bodyPart.transform.childCount > 1)
                {
                    bodyPart.transform.GetChild(0).gameObject.SetActive(false);
                    bodyPart.transform.GetChild(1).gameObject.SetActive(true);
                }

                bodyParts.Add((bodyPart,wrappedPos));

                currentPos = wrappedPos;
                currentMat = newMat;
            }
            DauTien = false;
        }
            
   
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
        float rotationZ = 0f;
        Vector3 localDir = Vector3.zero;

        switch (huong)
        {
            case huong.trai:
                localDir = Vector3.left; 
                rotationZ = 90f; break; 
            case huong.phai:
                localDir = Vector3.right; 
                rotationZ = -90f; break;
            case huong.tren:
                localDir = Vector3.up; 
                rotationZ = 0f; break;
            case huong.duoi:
                localDir = Vector3.down; 
                rotationZ = 180f; break;
            case huong.truoc:
                localDir = Vector3.up; 
                rotationZ = 0f; break; 
            case huong.sau:
                localDir = Vector3.down; 
                rotationZ = 180f; break;
        }

        Vector3 gridDir = GetRotationForMat(matNow) * localDir;
        huongNow = new Vector3Int(Mathf.RoundToInt(gridDir.x), Mathf.RoundToInt(gridDir.y), Mathf.RoundToInt(gridDir.z));
        UpdateArrowVisual(rotationZ);
    }
    private void UpdateArrowVisual(float rotationZ)
    {
        if (gameManager.instance.ArrowImg != null && gameManager.instance.ArrowImg.Count > 0)
        {
            sprite.sprite = gameManager.instance.ArrowImg[0];
        }
        
        sprite.transform.localRotation = Quaternion.Euler(0, 0, rotationZ);
    }
    public void removeAndCheckEnd()
    {
       gameManager.instance. settingManager.getCHangePoint();
        IsAnima = true;
        State = false; 
        Vector3 localDir = new Vector3(huongNow.x, huongNow.y, huongNow.z);
        Vector3 worldDir = transform.parent != null ? transform.parent.TransformDirection(localDir) : localDir;
        var vector = transform.position + (worldDir * 10f);
        if (bodyParts.Count > 0)
        {
            float partDuration = 0.8f / bodyParts.Count;
            for (int i = bodyParts.Count - 1; i >= 0; i--)
            {
                int index = i;
                GameObject part = bodyParts[index].Item1;
                if (part != null)
                {
                    float delay = (bodyParts.Count - 1 - index) * partDuration;
                    part.transform.DOScale(Vector3.zero, partDuration).SetDelay(delay).OnComplete(() => {
                        Destroy(part);
                    });
                }
            }
        }
        transform.DOMove(vector, 0.8f).SetEase(Ease.OutQuart).OnComplete(() => { 
            transform.DOScale(Vector3.zero, 0.5f); 
            IsAnima = false; 
            gameManager.instance.checkWin(); 
        });
    }    
}
