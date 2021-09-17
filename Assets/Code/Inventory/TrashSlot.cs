
using UnityEngine.UI;

public class TrashSlot : ItemSlot
{
    public static TrashSlot Instance;
    public new bool StoringItem
    {
        get => storingItem;
        set
        {
            transform.SetAsFirstSibling();
            storingItem = value;
        }
    }
    protected void Start()
    {
        image = GetComponent<Image>();
        Instance = this;
        transform.SetAsFirstSibling();
    }

    public void Trash(Item item, ItemSlot itemSlot)
    {
        if (item.favorite)
            return;
        if(StoringItem)
            Destroy(storedItem.gameObject);
        storedItem = item;
        storedItem.transform.position = transform.position;
        StoringItem = true;
        storedItem.transform.SetAsLastSibling();
        itemSlot.storedItem = null;
        itemSlot.StoringItem = false;
    }
}
