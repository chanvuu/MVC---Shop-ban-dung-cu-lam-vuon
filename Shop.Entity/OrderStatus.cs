using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class OrderStatus
    {
        public const string StatusPending = "Pending";
        public const string StatusCancelled = "Cancelled";
        public const string StatusApproved = "Approved";
        public const string StatusShipped = "Shipped";
        public const string StatusInProcess = "UnderProcessing";


    }
}
