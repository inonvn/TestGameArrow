using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public CanvasGroup iconSetting;
    public CanvasGroup UISetting;
    public CanvasGroup MenuUI;
    public GameObject BackToMenuIcon;
    public GameObject ExitGameIcon;
    public TextMeshProUGUI textNhapNhay;
    public CanvasGroup  UI;
    public CanvasGroup Hint;
    public TextMeshProUGUI ShowArrowLeft;
    public CanvasGroup showArrow;
    public CanvasGroup ShowWin;
    public CanvasGroup ShowLost;
    public RectTransform[] safeAreaPanels;
    public Slider ChangeV;
    public AudioSource audioS;
    public AudioClip[] clip;
    public GameObject hpCube;
    public RectTransform spawnHp;

    public void TurnOnSetting()
    {
        RandomInon.ButtonSound(audioS, clip[0]);
        if (gameManager.instance.inMenu)
        { ExitGameIcon.SetActive(true); BackToMenuIcon.SetActive(false); }
        else { ExitGameIcon.SetActive(false); BackToMenuIcon.SetActive(true); }
        UISetting.gameObject.SetActive(true);
        RandomInon.FadeOut(UISetting);
        iconSetting.gameObject.SetActive(false);
    }
    public void TurnOffSetting()
    {
        RandomInon.ButtonSound(audioS, clip[0]);
        RandomInon.FadeIn(UISetting);
        UISetting.gameObject.SetActive(false);
        iconSetting.gameObject.SetActive(true);
    }
   
    public void clickAnyWhereToStart()
    {
        Hint.gameObject.SetActive(false);
        ShowWin.gameObject.SetActive(false);
        showArrow.gameObject.SetActive(false);
        gameManager.instance.inMenu = false;
        MenuUI.gameObject.SetActive(false);
        gameManager.instance.loadLevel(gameManager.instance.yourUnlock);
      
    }
   
    public void exit()
    {
        PlayerPrefs.SetInt("yourUnlock", gameManager.instance.yourUnlock);
        PlayerPrefs.Save();
        Application.Quit();
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("yourUnlock", gameManager.instance.yourUnlock);
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetInt("yourUnlock", gameManager.instance.yourUnlock);
            PlayerPrefs.Save();
        }
    }
    public void backToMenu ()
    {
        RandomInon.ButtonSound(audioS, clip[0]);
        foreach (Transform k in spawnHp.gameObject.transform) { GameObject.Destroy(k.gameObject); }
        MenuUI.gameObject.SetActive(true);
        RandomInon.FadeOut(MenuUI);
        UISetting.gameObject.SetActive(false);
        iconSetting.gameObject.SetActive(true);
        gameManager.instance.inMenu=true;
        Hint.gameObject.SetActive(false);
        showArrow.gameObject.SetActive(false);
    }
    public void Hint1 ()
    {
        RandomInon.ButtonSound(audioS, clip[0]);

        CubeConA hintCube = null;
        foreach (var cube in gameManager.instance.dataGrid.Values)
        {
            if (cube != null && cube.State == true)
            {
                cube.getHuong(cube.huong_enum); 
                cube.GetCanMove(cube.pos3D, cube.huongNow); 

                if (cube.canMove)
                {
                    hintCube = cube;
                    break;
                }
            }
        }

        if (hintCube != null)
        {
            hintCube.transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.yellow, 0.5f).SetLoops(2, LoopType.Yoyo);
            foreach (var part in hintCube.bodyParts)
            {
                if (part.Item1 != null)
                {
                    SpriteRenderer sr = null;
                    if (part.Item1.transform.childCount > 1)
                        sr = part.Item1.transform.GetChild(1).GetComponent<SpriteRenderer>();
                    else
                        sr = part.Item1.GetComponentInChildren<SpriteRenderer>();
                    
                    if (sr != null)
                        sr.DOColor(Color.yellow, 0.5f).SetLoops(2, LoopType.Yoyo);
                }
            }
        }
    }    
    public void getCHangePoint()
    {
        ShowArrowLeft.SetText(gameManager.instance.blockCount.ToString());
    }    
    public void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        if (safeAreaPanels != null && safeAreaPanels.Length > 0)
        {
            foreach (RectTransform panel in safeAreaPanels)
            {
                if (panel != null)
                {
                    panel.anchorMin = anchorMin;
                    panel.anchorMax = anchorMax;
                }
            }
        }
    }
    public void Update()
    {
        audioS.volume = ChangeV.value;
    }
    public void Start()
    {
        ApplySafeArea();
        Hint.gameObject.SetActive(false);
       showArrow.gameObject.SetActive(false);
        if (textNhapNhay != null)
        {
            textNhapNhay.DOFade(0f, 1f).SetLoops(-1, LoopType.Yoyo);
        }
    }
    public void ShowWinAndBack()
    {
        audioS.PlayOneShot(clip[1]);
        ShowWin.gameObject.SetActive(true);
        ShowWin.alpha = 0;
        ShowWin.DOKill();
        ShowWin.DOFade(1, 1.5f).OnComplete(() => { ShowWin.gameObject.SetActive(true);iconSetting.gameObject.SetActive(true); });
    }  
    public void ShowLostAndBack()
    {
        audioS.PlayOneShot(clip[2]);
        ShowLost.gameObject.SetActive(true);
        ShowLost.alpha = 0;
        ShowLost.DOKill();
        ShowLost.DOFade(1, 1.5f).OnComplete(() => { ShowLost.gameObject.SetActive(true); iconSetting.gameObject.SetActive(true); });
    }    
    public void ClickToPlayAgain()
    {
        ShowLost.gameObject.SetActive(false);
        gameManager.instance.loadLevel(gameManager.instance.yourUnlock);
    }    

}
