using System.Collections;
using System.Collections.Generic;
using Code.Inventory;
using TMPro;
using UnityEngine;

public class PlayerInventory : Inventory
{
    [SerializeField] private GameObject trashSlotPrefab;
    protected GameObject trashSlot;
    protected GameObject title;
    [SerializeField] protected GameObject titlePrefab;
    #region Setup
    protected override void SetupInventory()
    {
        _itemSlots = new ItemSlot[width, height];
        _hotbarIndexes = new GameObject[width];
        previousDimensions.x = width;
        previousDimensions.y = height;
        ItemManager itemManager = ItemManager.Instance;
        SetupInventorySlots();
        SetUpItemIndexes();
        SetupItems();
        SetUpTrashSlot();
        title = Instantiate(trashSlotPrefab, SetGridPosition(width - 1, height), Quaternion.identity, transform);
        trashSlot = Instantiate(titlePrefab, _itemSlots[0, 0].transform.position, Quaternion.identity, transform);
    }
    private void SetupItems()
    {
        ItemManager itemManager = ItemManager.Instance;
        
        for (int i = 0; i < width; i++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newItem = Instantiate(itemManager.items[Random.Range(0, itemManager.items.Length)].gameObject,
                    _itemSlots[i, y].transform.position, Quaternion.identity, transform);
                Item item = newItem.GetComponent<Item>();
                _itemSlots[i, y].storedItem = item;
                if(item.stackable)
                    item.AddQuantity(Random.Range(0, item.maxQuantity + 1));
                newItem.transform.SetAsLastSibling();
            }
        }
    }
    private void SetUpItemIndexes()
    {
        for (int i = 0; i < width; i++)
        {
            _hotbarIndexes[i] = Instantiate(_hotbarIndex, _itemSlots[i, 0].transform.position, Quaternion.identity,
                transform);
            int hotbarIndex = i;
            hotbarIndex = hotbarIndex < width - 1 ? ++hotbarIndex : 0;
            _hotbarIndexes[i].GetComponent<TextMeshProUGUI>().text = (hotbarIndex).ToString();
            _hotbarIndexes[i].transform.position = _itemSlots[i, 0].transform.position;
        }
    }
    private void SetUpTrashSlot()
    {
        trashSlot = Instantiate(trashSlotPrefab, SetGridPosition(width - 1, height), Quaternion.identity, transform);
        trashSlot.GetComponent<ItemSlot>().StoringItem = false;
        trashSlot.AddComponent<ClickHandler>();
        title = Instantiate(titlePrefab, _itemSlots[0, 0].transform.position, Quaternion.identity, transform);
        setupComplete = true;
    }
    #endregion
    protected override void UpdatePosition()
    {
        canvas = GetComponent<Canvas>();
        for (int i = 0; i < width; i++)
        {
            for (int y = 0; y < height; y++)
            {
                _itemSlots[i, y].transform.position = SetGridPosition(i, y);
                _itemSlots[i, y].storedItem.transform.position = _itemSlots[i, y].transform.position;
            }
            _hotbarIndexes[i].transform.position = _itemSlots[i, 0].transform.position;
            trashSlot.transform.position = SetGridPosition(width - 1, height);
            title.transform.position = _itemSlots[0, 0].transform.position;
        }
    }
}
