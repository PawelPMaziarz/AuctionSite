using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite.Data
{
    class PaymentMethodDTO
    {
        public int IdPaymentMethod { get; set; }
        public string Name { get; set; }
        public decimal? AmountFee { get; set; }
        public decimal? PercentageFee { get; set; }
        public PaymentMethodDTO(PaymentMethod paymentMethod)
        {
            IdPaymentMethod = paymentMethod.idPaymentMethod;
            Name = paymentMethod.name;
            AmountFee = paymentMethod.amountFee;
            PercentageFee = paymentMethod.percentageFee;
        }
        public override string ToString()
        {
            //it can be null or zero
            if (AmountFee.HasValue && PercentageFee.HasValue)
                return Name + " oplaty: "+(AmountFee > PercentageFee ? AmountFee + "zl" : PercentageFee + "%");
            else if (AmountFee.HasValue)
                return Name + " oplaty: " + AmountFee + "zl";
            else if (AmountFee.HasValue)
                return Name + " oplaty: " + PercentageFee + "%";
            else return Name + " oplaty: 0";
        }
    }
}
