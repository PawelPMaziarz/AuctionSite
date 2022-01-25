using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite.Data
{
    class PurchaseDTO
    {
        public int IdPurchase { get; set; }
        public int IdUser { get; set; }
        public int IdAuction { get; set; }
        public DeliveryMethodDTO DeliveryMethod { get; set; }
        public PaymentMethodDTO PaymentMethod { get; set; }
        public AddressDTO AddressBuyer { get; set; }
        public AddressDTO AddressSeller { get; set; }
        public int IdPurchaseStatus { get; set; }
        public string NamePurchaseStatus { get; set; }
        public RatingDTO Rating { get; set; }
        public DateTime Date { get; set; }
        public decimal? Price { get; set; }
        public PurchaseDTO(Purchase purchase)
        {
            IdPurchase = purchase.idPurchase;
            IdUser = purchase.idUser;
            IdAuction = purchase.idAuction;
            DeliveryMethod = new DeliveryMethodDTO(purchase.DeliveryMethod);
            PaymentMethod = new PaymentMethodDTO(purchase.PaymentMethod);
            AddressBuyer = new AddressDTO(purchase.AddressBuyer);
            AddressSeller = new AddressDTO(purchase.AddressSeller);
            IdPurchaseStatus = purchase.idPurchaseStatus;
            NamePurchaseStatus = purchase.PurchaseStatus.description;
            Date = purchase.date;
            Price = purchase.price;
            Rating = !purchase.Rating.Any() ? null : new RatingDTO(purchase.Rating.First());
        }

    }
}
