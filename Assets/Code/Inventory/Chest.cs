
using Code.Inventory;
using UnityEngine;

public class Chest : Inventory
{
    protected override void SetupInventory()
    {
        _itemSlots = new ItemSlot[width, height];
        _hotbarIndexes = new GameObject[width];
        previousDimensions.x = width;
        previousDimensions.y = height;
        ItemManager itemManager = ItemManager.Instance;

        for (int i = 0; i < width; i++)
        {
            for (int y = 0; y < height; y++)
            {
                _itemSlots[i, y] = Instantiate(inventorySlot, SetGridPosition(i, y),
                    Quaternion.identity, transform).GetComponent<ItemSlot>();
                _itemSlots[i, y].gameObject.AddComponent<ClickHandler>();
                _itemSlots[i, y].StoringItem = false;
            }
        }
    }
}
