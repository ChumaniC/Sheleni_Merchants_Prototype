using Sheleni_Merchants.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sheleni_Merchants.Services
{
    public class DataService
    {
        public static event EventHandler<Item> ItemRemoved;
        public static event EventHandler<Item> ItemUpdated;

        public static void RemoveItem(Item item)
        {
            ItemRemoved?.Invoke(null, item);
        }

        public static void UpdateItem(Item item)
        {
            ItemUpdated?.Invoke(null, item);
        }
    }

}
