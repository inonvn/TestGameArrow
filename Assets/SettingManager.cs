using TMPro;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public CanvasGroup iconSetting;
    public CanvasGroup UISetting;
    public CanvasGroup MenuUI;
    public GameObject BackToMenuIcon;
    public GameObject ExitGameIcon;
    public CanvasGroup ChooseLevel;
    public GameObject ChooseLevelCon;
    public Transform ChooseLevelSpawn;
   
    public void TurnOnSetting ()
    {
        if (gameManager.instance.inMenu)
        { ExitGameIcon.SetActive(true); BackToMenuIcon.SetActive(false); }
        else { ExitGameIcon.SetActive(false); BackToMenuIcon.SetActive(true); }
        UISetting.gameObject.SetActive (true);
        RandomInon.FadeOut(UISetting);
        iconSetting.gameObject.SetActive (false);
    }    
    public void TurnOffSetting ()
    {
        RandomInon.FadeIn(UISetting);
        UISetting.gameObject.SetActive(false);
        iconSetting.gameObject.SetActive(true);
    }
    int n = 1;
    public void clickAnyWhereToStart ()
    {
        gameManager.instance.inMenu= false;
        MenuUI.gameObject.SetActive(false);
        ChooseLevel.gameObject.SetActive(true);
        RandomInon.FadeOut(ChooseLevel);
        for (; n <= gameManager.instance.saveLevel.Count; n++)
        {
            var e = Instantiate(ChooseLevelCon, ChooseLevelSpawn);
            e.GetComponent<TextMeshProUGUI>().SetText("level " + n);
        }
    }   

    public void chooseLevel ()
    {
       
    }    

}
