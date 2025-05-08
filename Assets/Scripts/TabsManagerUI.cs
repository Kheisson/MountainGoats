using UnityEngine;
using UnityEngine.UI;

public class TabsManagerUI : MonoBehaviour
{
    public GameObject[] Pages;
    public Button[] TabButtons;

    public void SwitchToTab(int TabID)
    {
        foreach (GameObject go in Pages)
        {
            go.SetActive(false);
        }
        Pages[TabID].SetActive(true);

    }
}

