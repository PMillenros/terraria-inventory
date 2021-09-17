
using UnityEngine;

public class ChestButton : MonoBehaviour
{
    [SerializeField] private GameObject chest;

    public void ToggleChest()
    {
        chest.SetActive(!chest.activeSelf);
    }

    
}
