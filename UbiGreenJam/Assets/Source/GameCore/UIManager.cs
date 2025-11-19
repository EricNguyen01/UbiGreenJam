using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject uiRoot; 

    public void ShowUI(bool show)
    {
        uiRoot.SetActive(show);
    }
}
