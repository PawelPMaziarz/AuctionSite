using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite.Data
{
    class RatingDTO
    {
        public int IdRating { get; set; }
        public int IdPurchase { get; set; }
        public int IdRatingUser { get; set; }
        public int IdRatedUser { get; set; }
        public string TitleRatedAuction { get; set; }
        public DateTime Date { get; set; }
        public int BuyerService { get; set; }
        public int TrueDescription { get; set; }
        public int ShippingTime { get; set; }
        public RatingDTO(Rating r)
        {
            IdRating = r.idRating;
            IdPurchase = r.idPurchase;
            IdRatingUser = r.Purchase.idUser;
            IdRatedUser = r.Purchase.Auction.idUserSeller;
            TitleRatedAuction = r.Purchase.Auction.AuctionDescription.First().title;
            Date = r.date;
            BuyerService = r.buyerService;
            TrueDescription = r.trueDescription;
            ShippingTime = r.shippingTime;
        }
        public override string ToString()
        {
            return $"{TitleRatedAuction} Obsluga:{BuyerService} Opis:{TrueDescription} Dostawa:{ShippingTime}";
        }
    }
}
