using GameCore;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject uiRoot; 

    public void OnStartBtnClicks()
    {
        GameManager.Instance.StartGame();
    }
    public void ShowUI(bool show)
    {
        uiRoot.SetActive(show);
    }
}
