using System;
using System.Collections;
using Code.Inventory;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    [SerializeField] private Sprite[] _cursorVariants;
    private static Vector3 _position;
    private Item _heldItem;
    public static PlayerCursor Instance;
    private bool _holdingItem = false;
    private Transform heldItemOrigin;
    public static Vector3 Position
    {
        get
        {
            _position = Input.mousePosition;
            return _position;
        }
        private set => _position = value;
    }

    private void Start()
    {
        Cursor.visible = false;
        heldItemOrigin = transform.GetChild(0);
        Instance = this;
        transform.SetAsLastSibling();
    }
    public void Click(ItemSlot itemSlot, bool leftClick)
    {
        if (!_holdingItem && !itemSlot.storingItem)
            return;
        
        if(leftClick)
            if(_holdingItem)
                Place(itemSlot);
            else
                GrabItem(itemSlot);
        else
            TakeItems(itemSlot, 1);
    }

    private void TakeItems(ItemSlot itemSlot, int amount)
    {
        if (!itemSlot.storingItem || !itemSlot.storedItem.stackable)
            return;
        if (_holdingItem)
        {
            if (_heldItem.Quantity > _heldItem.maxQuantity - 1)
                return;
            if (_heldItem.id == itemSlot.storedItem.id)
            {
                _heldItem.AddQuantity(amount);
                itemSlot.storedItem.AddQuantity(-amount);
            }
        }
        else
        {
            Transform itemSlotTransform = itemSlot.transform;
            _heldItem = Instantiate(ItemManager.Instance.items[itemSlot.storedItem.id], 
                itemSlotTransform.position, Quaternion.identity, itemSlotTransform.parent);
            _heldItem.SetQuantity(amount);
            itemSlot.storedItem.AddQuantity(-amount);
            _holdingItem = true;
        }
        if (itemSlot.storedItem.Quantity < 1)
        {
            Destroy(itemSlot.storedItem.gameObject);
            itemSlot.storingItem = false;
        }
    }
    
    private void Place(ItemSlot itemSlot)
    {
        if (!itemSlot.storingItem)
        {
            itemSlot.storedItem = _heldItem;
            itemSlot.storingItem = true;
            _heldItem = null;
            _holdingItem = false;
            itemSlot.storedItem.transform.position = itemSlot.transform.position;
        }
        else if(itemSlot.storedItem.id != _heldItem.id)
            SwitchItems(itemSlot);
        else if (itemSlot.storedItem.stackable && _heldItem.stackable)
        {
            itemSlot.storedItem.Stack(_heldItem);
            if (_heldItem.Quantity < 1)
            {
                _holdingItem = false;
                Destroy(_heldItem.gameObject);
            }
        }
    }
    private void SwitchItems(ItemSlot itemSlot)
    {
        Item temporaryItem = itemSlot.storedItem;
        itemSlot.storedItem = _heldItem;
        itemSlot.storingItem = true;
        _heldItem = temporaryItem;
        itemSlot.storedItem.transform.position = itemSlot.transform.position;
    }
    private void GrabItem(ItemSlot itemSlot)
    {
        _heldItem = itemSlot.storedItem;
        itemSlot.storedItem = null;
        itemSlot.storingItem = false;
        
        _heldItem.transform.SetAsLastSibling();
        _holdingItem = true;
    }
    private void Update()
    {
        transform.position = Position;
        if(_holdingItem)
            _heldItem.transform.position = heldItemOrigin.transform.position;
    }
}
