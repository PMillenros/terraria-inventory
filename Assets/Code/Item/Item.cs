using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;


public class Item : MonoBehaviour
{
    public enum Rarity { White, Green, Pink, Lime, Purple };
    public enum Type { Tile, Consumable, Furniture, Weapon, Tool, Equipable };
    
    [SerializeField] public Rarity rarity = Rarity.White;
    [SerializeField] public Type type;
    [SerializeField] private int quantity = 1;
    [SerializeField] public bool stackable;
    [SerializeField] TextMeshProUGUI quantityText;
    
    public int maxQuantity = 999;
    public int id;
    public int Quantity
    { 
        get => quantity; 
        private set
        {
            quantity = value;
            quantityText.text = quantity == 1 ? "" : quantity.ToString();
        }
    }
    
    public void Stack(Item item)
    {
        Quantity += item.Quantity;
        if (Quantity > maxQuantity)
        {
            item.Quantity = Quantity - maxQuantity;
            Quantity = maxQuantity;
        }
        else
            item.Quantity = 0;
    }

    public void AddQuantity(int value)
    {
        Quantity += value;
    }

    public void SetQuantity(int value)
    {
        Quantity = value;
    }
    private void Awake()
    {
        SetupQuantityText();
        gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
    }

    private void SetupQuantityText()
    {
        if (!stackable)
        {
            return;
        }
        quantityText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        quantityText.text = quantity.ToString();
    }
}
