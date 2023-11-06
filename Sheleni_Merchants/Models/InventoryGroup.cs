using System;
using System.Collections.Generic;
using System.Text;

namespace Sheleni_Merchants.Models
{
    public class InventoryGroup
    {
        public string Subcategory { get; set; }
        public List<Item> Items { get; set; }
        public string SubcategoryIcon { get; set; }
    }
}
