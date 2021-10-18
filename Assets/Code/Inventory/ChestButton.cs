using UnityEngine;

public class ChestButton : MonoBehaviour
{
    [SerializeField] private GameObject chest;
    private static GameObject activeChest;
    public void ToggleChest()
    {
        if (activeChest == null)
        {
            activeChest = chest;
            activeChest.SetActive(true);
        }
        else if(activeChest == chest)
        {
            activeChest.SetActive(false);
            activeChest = null;
        }
        else
        {
            activeChest.SetActive(false);
            activeChest = chest;
            chest.SetActive(true);
        }

    }

    
}

