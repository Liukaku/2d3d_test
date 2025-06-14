using System.Collections;
using System.Collections.Generic;
using SpriteGame;
using UnityEngine;

namespace SpriteGame
{
    public class Inventory : MonoBehaviour
    {
        public List<InventoryItem> Items = new List<InventoryItem>();

        public void AddToInventory(Interactable item)
        {
            int existingIndex = Items.FindIndex(i => i.item == item);
            if (existingIndex >= 0)
            {
                // Item already exists, increase quantity
                Items[existingIndex].quantity++;
            }
            else
            {
                // Item does not exist, add new item
                InventoryItem newItem = new InventoryItem { item = item, quantity = 1 };
                Items.Add(newItem);
            }


            Debug.Log($"Added {item.Name} to inventory. Total quantity: {Items.Find(i => i.item == item).quantity}");
        }

        public void RemoveFromInventory(Interactable item)
        {
            int existingIndex = Items.FindIndex(i => i.item == item);
            if (existingIndex >= 0)
            {
                // Item exists, decrease quantity
                Items[existingIndex].quantity--;
                if (Items[existingIndex].quantity <= 0)
                {
                    // Remove item if quantity is zero or less
                    Items.RemoveAt(existingIndex);
                }
            }
            else
            {
                Debug.LogWarning($"Item {item.Name} not found in inventory.");
            }
        }

        public class InventoryItem
        {
            public Interactable item;
            public int quantity;
        }
    }
}
