using System;
using System.Collections.Generic;

namespace GroceryStoreAPI.Entities
{
    public class Order : Entity
    {
        public int CustomerId { get; set; }

        public IList<OrderItem> Items { get; set; }

        public DateTime? DateCreated { get; set; }
    }
}
