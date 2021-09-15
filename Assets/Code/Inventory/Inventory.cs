using System;
using System.Collections;
using System.Collections.Generic;
using Code.Inventory;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Inventory : MonoBehaviour
{
    private ItemSlot[,] _itemSlots;
    private int height = 5;
    private int width = 10;
    [SerializeField] private Vector2 offset, origin;
    [SerializeField] private GameObject inventorySlot;
    [SerializeField] private GameObject _hotbarIndex;
    [SerializeField] private Texture2D texture;
    [SerializeField] private Texture2D favoriteTexture;
    private GameObject[] _hotbarIndexes;
    private bool setupComplete = false;
    private Canvas canvas;
    
    private void SetupInventory()
    {
        canvas = GetComponent<Canvas>();
        _itemSlots = new ItemSlot[width, height];
        _hotbarIndexes = new GameObject[width];
        ItemManager itemManager = ItemManager.Instance;
        float canvasWidth = canvas.pixelRect.width;
        float canvasHeight = canvas.pixelRect.height;
        const int screenSizeConstant = 1000;
        
        for (int i = 0; i < width; i++)
        {
            for (int y = 0; y < height; y++)
            {
                _itemSlots[i, y] = Instantiate(inventorySlot,
                    new Vector3(i * offset.x * canvasWidth / screenSizeConstant + origin.x * canvasWidth / screenSizeConstant,
                        y * -offset.y * canvasWidth / screenSizeConstant - origin.y * canvasWidth / screenSizeConstant + canvasHeight, 0),
                    Quaternion.identity, transform).AddComponent<ItemSlot>();
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
                    item.AddQuantity(Random.Range(0, item.maxQuantity + 1));
                newItem.transform.SetAsLastSibling();
            }

        }
        setupComplete = true;
    }

    private void OnValidate()
    {
        if(Application.isPlaying && setupComplete)
            UpdatePosition();
    }

    private void UpdatePosition()
    {
        canvas = GetComponent<Canvas>();
        float canvasWidth = canvas.pixelRect.width;
        float canvasHeight = canvas.pixelRect.height;
        const int screenSizeConstant = 1000;
        for (int i = 0; i < width; i++)
        {
            for (int y = 0; y < height; y++)
            {
                _itemSlots[i, y].transform.position = new Vector3(i * offset.x * canvasWidth / screenSizeConstant + origin.x * canvasWidth / screenSizeConstant,
                    y * -offset.y * canvasWidth / screenSizeConstant - origin.y * canvasWidth / screenSizeConstant + canvasHeight);
                _itemSlots[i, y].storedItem.transform.position = _itemSlots[i, y].transform.position;
            }

            _hotbarIndexes[i].transform.position = _itemSlots[i, 0].transform.position;
        }
    }
    private void Start()
    {
        SetupInventory();
    }

    private void SortInventory()
    {
        
    }
}
