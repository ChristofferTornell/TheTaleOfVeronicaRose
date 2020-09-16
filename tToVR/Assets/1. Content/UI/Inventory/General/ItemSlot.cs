using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    private Item Item;
    public Item item
    {
        get { return Item; }
        set 
        {
            Item = value;
            if (Item == null)
            {
                image.enabled = false;
            }
            else
            {
                image.sprite = Item.icon;
                image.enabled = true;
            }
        }
    }
    [SerializeField] private Image image = null;

    protected virtual void OnValidate()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }
}
