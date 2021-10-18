using System.Collections.Generic;
using Code.Inventory;
using TMPro;

using UnityEngine;

using Random = UnityEngine.Random;

public class Inventory : MonoBehaviour
{
    [SerializeField] protected int height = 5;
    [SerializeField] protected int width = 10;
    [SerializeField] protected Vector2 offset, origin;
    [SerializeField] protected GameObject inventorySlot;
    [SerializeField] protected GameObject _hotbarIndex;
    
    protected ItemSlot[,] _itemSlots;
    protected Vector2 previousDimensions;
    protected GameObject[] _hotbarIndexes;
    protected Canvas canvas;
    
    protected bool setupComplete;
    
    
    
    #region setup
    protected void Start()
    {
        canvas = GetComponent<Canvas>();
        SetupInventory();
    }
    protected virtual void SetupInventory()
    {
        _itemSlots = new ItemSlot[width, height];
        _hotbarIndexes = new GameObject[width];
        previousDimensions.x = width;
        previousDimensions.y = height;
        SetupInventorySlots();
        setupComplete = true;
    }
    protected void SetupInventorySlots()
    {
        for (int i = 0; i < width; i++)
        {
            for (int y = 0; y < height; y++)
            {
                _itemSlots[i, y] = Instantiate(inventorySlot, SetGridPosition(i, y),
                    Quaternion.identity, transform).GetComponent<ItemSlot>();
                _itemSlots[i, y].gameObject.AddComponent<ClickHandler>();
            }
        }
    }
    #endregion
    private void OnValidate()
    {
        if (setupComplete)
        {
            //if (previousDimensions.x != width || previousDimensions.y != height)
            //{
            //   ResetInventory(); 
            // return;
            //} Experimental code
            UpdatePosition();
        }
    }

    #region PositionLogic
    protected virtual void UpdatePosition()
    {
        canvas = GetComponent<Canvas>();
        for (int i = 0; i < width; i++)
        {
            for (int y = 0; y < height; y++)
            {
                _itemSlots[i, y].transform.position = SetGridPosition(i, y);
                _itemSlots[i, y].storedItem.transform.position = _itemSlots[i, y].transform.position;
            }
        }
    }
    protected Vector3 SetGridPosition(int width, int height)
    {
        var pixelRect = canvas.pixelRect;
        float canvasWidth = pixelRect.width;
        float canvasHeight = pixelRect.height;
        const int screenSizeConstant = 1000;
        float screenSizeMultiplier = canvasWidth / screenSizeConstant;
        
        return new Vector3(width * offset.x * screenSizeMultiplier + origin.x * screenSizeMultiplier,
            height * -offset.y * screenSizeMultiplier - origin.y * screenSizeMultiplier +
            canvasHeight, 0);
    }
    #endregion

    
    protected void ResetInventory()
    {
        PlayerCursor.Instance.transform.SetAsLastSibling();
        for (int i = 0; i < transform.childCount - 2; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        SetupInventory();
    }

    #region Sorting
    public void SortInventory()
    {
        List<Item> items = new List<Item>();
        GetSortableItems(items);
        QuickSortById(items, 0, items.Count - 1);
        CombineItems(items);
        QuickSortByType(items,0, items.Count - 1);
        PickUpItems(items);
    }
    protected void CombineItems(List<Item> items)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if(items[i].stackable)
                if(0 < i - 1 && items[i].id == items[i - 1].id) 
                    items[i - 1].Stack(items[i]);
            if (items[i].Quantity < 1)
            {
                Destroy(items[i].gameObject);
                items.RemoveAt(i);
            }
        }
    }

    protected void GetSortableItems(List<Item> items)
    {
        for (int i = 0; i < _itemSlots.GetLength(0); i++)
        {
            for (int y = 0; y < _itemSlots.GetLength(1); y++)
            {
                if (_itemSlots[i, y].StoringItem && !_itemSlots[i, y].Favorite)
                {
                    items.Add(_itemSlots[i, y].storedItem);
                    _itemSlots[i, y].storedItem = null;
                    _itemSlots[i, y].StoringItem = false;
                }
            }
        }
    }
    public void PickUpItems(List<Item> items)
    {
        int index = 0;
        for (int i = 0; i < _itemSlots.GetLength(1); i++)
        {
            for (int y = 0; y < _itemSlots.GetLength(0); y++)
            {
                if (_itemSlots[y, i].StoringItem && _itemSlots[y, i].Favorite)
                    return;
                if (index < items.Count)
                {
                    _itemSlots[y, i].storedItem = items[index];
                    _itemSlots[y, i].StoringItem = true;
                    _itemSlots[y, i].storedItem.transform.position = _itemSlots[y, i].transform.position;
                    index++;
                }
            }
        }
    }
    
    protected void QuickSortByType(List<Item> items, int left, int right)
    {
        int pivot = (int) items[(left + right) / 2].type;
        int leftHold = left;
        int rightHold = right;
        
        while (leftHold < rightHold)
        {
            while (((int)items[leftHold].type < pivot) && (leftHold <= rightHold)) leftHold++;
            while (((int)items[rightHold].type > pivot) && (rightHold >= leftHold)) rightHold--;
            
            if (leftHold < rightHold)
            {
                Item tmp = items[leftHold];
                items[leftHold] = items[rightHold];
                items[rightHold] = tmp;
                if ((int)items[leftHold].type == pivot && (int)items[rightHold].type == pivot)
                    leftHold++;
            }
        }
        if (left < leftHold -1) QuickSortByType(items, left, leftHold - 1);
        if (right > rightHold + 1) QuickSortByType(items, rightHold + 1, right);
    }
    protected void QuickSortById(List<Item> items, int left, int right)
    {
        int pivot = items[(left + right) / 2].id;
        int leftHold = left;
        int rightHold = right;
        
        while (leftHold < rightHold)
        {
            while ((items[leftHold].id < pivot) && (leftHold <= rightHold)) leftHold++;
            while ((items[rightHold].id > pivot) && (rightHold >= leftHold)) rightHold--;
            
            if (leftHold < rightHold)
            {
                Item tmp = items[leftHold];
                items[leftHold] = items[rightHold];
                items[rightHold] = tmp;
                if (items[leftHold].id == pivot && items[rightHold].id == pivot)
                    leftHold++;
            }
        }
        if (left < leftHold -1) QuickSortById(items, left, leftHold - 1);
        if (right > rightHold + 1) QuickSortById(items, rightHold + 1, right);
    }
    #endregion
}
