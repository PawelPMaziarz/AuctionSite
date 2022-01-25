using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite.Data
{
    class DeliveryMethodDTO
    {
        public int IdDeliveryMethod { get; set; }
        public string Name { get; set; }
        public decimal Fee { get; set; }
        public DeliveryMethodDTO(DeliveryMethod deliveryMethod)
        {
            IdDeliveryMethod = deliveryMethod.idDeliveryMethod;
            Name = deliveryMethod.name;
            Fee = deliveryMethod.fee;
        }
        public override string ToString()
        {
            return Name + " oplaty: " + Fee;
        }
    }
}
