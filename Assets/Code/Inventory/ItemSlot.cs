using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public Item storedItem;
    private Vector2 slotPosition;
    public bool storingItem = true;

    public Vector2 SlotPosition
    {
        get => slotPosition;
        private set => slotPosition = value;
    }
    private void SwitchItem(Item itemInHand)
    {
        Item placeholderItem = itemInHand;
        itemInHand = storedItem;
        storedItem = placeholderItem;
    }
}
