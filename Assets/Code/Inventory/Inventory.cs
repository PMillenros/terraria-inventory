
using System.Collections.Generic;
using Code.Inventory;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Inventory : MonoBehaviour
{
    protected ItemSlot[,] _itemSlots;
    [SerializeField] protected int height = 5;
    [SerializeField] protected int width = 10;
    protected Vector2 previousDimensions;
    [SerializeField] protected Vector2 offset, origin;
    [SerializeField] protected GameObject inventorySlot;
    [SerializeField] protected GameObject _hotbarIndex;
    [SerializeField] protected GameObject trashSlotPrefab;
    [SerializeField] protected GameObject titlePrefab;
    protected GameObject trashSlot;
    protected GameObject title;
    protected GameObject[] _hotbarIndexes;
    protected bool setupComplete;
    protected Canvas canvas;
    
    
    protected virtual void SetupInventory()
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
            }
            _hotbarIndexes[i] = Instantiate(_hotbarIndex, _itemSlots[i, 0].transform.position, Quaternion.identity, transform);
            int hotbarIndex = i;
            hotbarIndex = hotbarIndex < width - 1? ++hotbarIndex : 0;
            _hotbarIndexes[i].GetComponent<TextMeshProUGUI>().text = (hotbarIndex).ToString();
            _hotbarIndexes[i].transform.position = _itemSlots[i, 0].transform.position;
        }
        for (int i = 0; i < width; i++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newItem = Instantiate(itemManager.items[Random.Range(0, itemManager.items.Length)].gameObject,
                    _itemSlots[i, y].transform.position, Quaternion.identity, transform);
                Item item = newItem.GetComponent<Item>();
                _itemSlots[i, y].storedItem = item;
                if(item.stackable)
                    item.AddQuantity(Random.Range(1, item.maxQuantity + 1));
                newItem.transform.SetAsLastSibling();
            }
        }
        trashSlot = Instantiate(trashSlotPrefab, SetGridPosition(width - 1, height), Quaternion.identity, transform);
        trashSlot.GetComponent<ItemSlot>().StoringItem = false;
        trashSlot.AddComponent<ClickHandler>();
        title = Instantiate(titlePrefab, _itemSlots[0, 0].transform.position, Quaternion.identity, transform);
        setupComplete = true;
    }
    private void OnValidate()
    {
        if (setupComplete)
        {
            if (previousDimensions.x != width || previousDimensions.y != height)
            {
                ResetInventory();
                return;
            }
            UpdatePosition();
        }
    }
    
    protected Vector3 SetGridPosition(int width, int height)
    {
        var pixelRect = canvas.pixelRect;
        float canvasWidth = pixelRect.width;
        float canvasHeight = pixelRect.height;
        const int screenSizeConstant = 1000;
        
        return new Vector3(width * offset.x * canvasWidth / screenSizeConstant + origin.x * canvasWidth / screenSizeConstant,
            height * -offset.y * canvasWidth / screenSizeConstant - origin.y * canvasWidth / screenSizeConstant +
            canvasHeight, 0);
    }
    private void UpdatePosition()
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
        }

        trashSlot.transform.position = SetGridPosition(width - 1, height);
        title.transform.position = _itemSlots[0, 0].transform.position;
    }
    protected void Start()
    {
        canvas = GetComponent<Canvas>();
        SetupInventory();
    }

    protected void ResetInventory()
    {
        PlayerCursor.Instance.transform.SetAsLastSibling();
        for (int i = 0; i < transform.childCount - 2; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        SetupInventory();
    }

    private void CombineItems(List<Item> items)
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
    public void SortInventory()
    {
        List<Item> items = new List<Item>();
        
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
        QuickSortById(items, 0, items.Count - 1);
        for (int i = 0; i < items.Count; i++)
        {
            CombineItems(items);
        }
        QuickSortByType(items,0, items.Count - 1);
        
        
        List<Item> itemType = new List<Item>();
        int index = 0;
        for (int i = 0; i < _itemSlots.GetLength(1); i++)
        {
            for (int y = 0; y < _itemSlots.GetLength(0); y++)
            {
                if (!_itemSlots[y, i].StoringItem && !_itemSlots[y, i].Favorite)
                {
                    if (index < items.Count)
                    {
                        _itemSlots[y, i].storedItem = items[index];
                        _itemSlots[y, i].StoringItem = true;
                        _itemSlots[y, i].storedItem.transform.position = _itemSlots[y, i].transform.position;
                        index++;
                    }
                    else
                        return;
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
}
