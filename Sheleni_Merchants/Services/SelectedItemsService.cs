using Sheleni_Merchants.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sheleni_Merchants.Services
{
    public class SelectedItemsService
    {
        public List<Item> SelectedItems { get; private set; }

        public SelectedItemsService()
        {
            SelectedItems = new List<Item>();
        }

        public void AddSelected(Item item)
        {
            SelectedItems.Add(item);
        }

        public void RemoveSelected(Item item)
        {
            SelectedItems.Remove(item);
        }

        // Update the list of selected items with the new items
        public void UpdateSelectedItems(List<Item> items)
        {
            // Clear the previous list and add the new items
            SelectedItems.Clear();
            SelectedItems.AddRange(items);
        }
    }

}
