using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Item> items = null;
    [SerializeField] private Transform itemsParent = null;
    [SerializeField] private ItemSlot[] itemSlots = null;

    private void OnValidate()
    {
        if (itemsParent != null)
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();

        RefreshUI();
    }

    private void RefreshUI()
    { //Assigns an item slot its item. Is called everytime an item is added or removed from the inventory.
        int i = 0;
        for (; i < items.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].item = items[i];
        }

        for (; i < itemSlots.Length; i++)
        {
            itemSlots[i].item = null;
        }
    }

    public bool AddItem(Item item)
    {
        if (IsFull())
            return false;

        items.Add(item);
        RefreshUI();
        return true;
    }

    public bool RemoveItem(Item item)
    {
        if (items.Remove(item))
        {
            RefreshUI();
            return true;
        }
        return false;
    }

    public bool IsFull()
    {
        return items.Count >= itemSlots.Length;
    }
}
