using UnityEngine;
using UnityEngine.EventSystems;

public class buttonChoose : MonoBehaviour
{
    public CanvasGroup ChooseLevel;
    public void chooseLevel()
    {
        GameObject click = EventSystem.current.currentSelectedGameObject;
        if (click != null)
        {
            int levelIndex = click.transform.GetSiblingIndex();
            gameManager.instance.loadLevel(levelIndex);
            ChooseLevel.gameObject.SetActive(false);
        }
    }
}
