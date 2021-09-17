
using Code.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCursor : MonoBehaviour
{
    [SerializeField] private Sprite favoriteCursor, normalCursor;
    private Image cursorImage;
    private static Vector3 _position;
    [HideInInspector] public Item heldItem;
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
        cursorImage = GetComponent<Image>();
        Cursor.visible = false;
        heldItemOrigin = transform.GetChild(0);
        Instance = this;
        transform.SetAsLastSibling();
    }
    public void Click(ItemSlot itemSlot, bool leftClick)
    {
        if (!_holdingItem && !itemSlot.StoringItem)
            return;
        if (leftClick && Input.GetKey(KeyCode.LeftControl))
        {
            TrashSlot.Instance.Trash(itemSlot.storedItem, itemSlot);
            return;
        }

        if(leftClick)
            if(_holdingItem)
                Place(itemSlot);
            else
                GrabItem(itemSlot);
        else
            TakeItems(itemSlot, 1);
        transform.SetAsLastSibling();
    }

    private void TakeItems(ItemSlot itemSlot, int amount)
    {
        if (!itemSlot.StoringItem || !itemSlot.storedItem.stackable)
            return;
        if (_holdingItem)
        {
            if (heldItem.Quantity > heldItem.maxQuantity - 1)
                return;
            if (heldItem.id == itemSlot.storedItem.id)
            {
                heldItem.AddQuantity(amount);
                itemSlot.storedItem.AddQuantity(-amount);
            }
        }
        else
        {
            Transform itemSlotTransform = itemSlot.transform;
            heldItem = Instantiate(ItemManager.Instance.items[itemSlot.storedItem.id], 
                itemSlotTransform.position, Quaternion.identity, itemSlotTransform.parent);
            heldItem.SetQuantity(amount);
            itemSlot.storedItem.AddQuantity(-amount);
            _holdingItem = true;
        }
        if (itemSlot.storedItem.Quantity < 1)
        {
            Destroy(itemSlot.storedItem.gameObject);
            itemSlot.StoringItem = false;
        }
    }
    
    private void Place(ItemSlot itemSlot)
    {
        if (!itemSlot.StoringItem)
        {
            itemSlot.storedItem = heldItem;
            itemSlot.StoringItem = true;
            heldItem = null;
            _holdingItem = false;
            itemSlot.storedItem.transform.position = itemSlot.transform.position;
        }
        else if (itemSlot.storedItem.stackable && heldItem.stackable && heldItem.id == itemSlot.storedItem.id)
        {
            itemSlot.storedItem.Stack(heldItem);
            if (heldItem.Quantity < 1)
            {
                _holdingItem = false;
                Destroy(heldItem.gameObject);
            }
        }
        else
            SwapItems(itemSlot);
    }
    private void SwapItems(ItemSlot itemSlot)
    {
        Item temporaryItem = itemSlot.storedItem;
        if (itemSlot.Equals(TrashSlot.Instance))
        {
            Destroy(itemSlot.storedItem.gameObject);
            itemSlot.storedItem = heldItem;
            heldItem = null;
            _holdingItem = false;
            itemSlot.storedItem.transform.position = itemSlot.transform.position;
        }
        else
        {
            itemSlot.storedItem = heldItem;
            itemSlot.StoringItem = true;
            heldItem = temporaryItem;
            itemSlot.storedItem.transform.position = itemSlot.transform.position;
        }
        
        
    }
    private void GrabItem(ItemSlot itemSlot)
    {
        heldItem = itemSlot.storedItem;
        itemSlot.storedItem = null;
        itemSlot.StoringItem = false;
        
        heldItem.transform.SetAsLastSibling();
        _holdingItem = true;
    }
    private void Update()
    {
        transform.position = Position;
        if(_holdingItem)
            heldItem.transform.position = heldItemOrigin.transform.position;
        if (Input.GetKey(KeyCode.LeftAlt))
            cursorImage.sprite = favoriteCursor;
        else
            cursorImage.sprite = normalCursor;

    }
}
