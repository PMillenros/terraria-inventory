
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Item storedItem;
    protected Vector2 slotPosition;
    protected bool storingItem = true;
    [SerializeField] protected Sprite normalTexture;
    [SerializeField] protected Sprite favoriteTexture;
    protected Image image;
    protected bool favorite;

    public bool StoringItem
    {
        get => storingItem;
        set
        {
            storingItem = value;
            if (!storingItem)
            {
                SetFavorite(false);
               // storedItem = null;
               return;
            }
            if(storedItem.favorite)
                SetFavorite(true);
            storedItem.transform.SetParent(transform);
            storedItem.transform.parent.SetAsLastSibling();
            PlayerCursor.Instance.transform.SetAsLastSibling();
            TrashSlot.Instance.transform.SetAsFirstSibling();
            
        }
    }
    public bool Favorite
    {
        get => favorite;
        private set => favorite = value;
    }
    public Vector2 SlotPosition
    {
        get => slotPosition;
        protected set => slotPosition = value;
    }
    protected void Awake()
    {
        image = GetComponent<Image>();
    }

    public void ToggleFavorite()
    {
        if (storingItem)
        {
            favorite = !favorite;
            image.sprite = favorite ? favoriteTexture : normalTexture;
            storedItem.favorite = favorite;
        }
    }

    protected void SetFavorite(bool favorite)
    {
        image.sprite = favorite ? favoriteTexture : normalTexture;
        this.favorite = favorite;
    }
}
