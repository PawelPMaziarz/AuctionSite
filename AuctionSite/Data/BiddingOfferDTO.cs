using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite.Data
{
    class BiddingOfferDTO
    {
        public int IdBiddingOffer { get; set; }
        public int IdAuction { get; set; }
        public int IdUser { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public BiddingOfferDTO(BiddingOffer biddingOffer)
        {
            IdBiddingOffer = biddingOffer.idBiddingOffer;
            IdAuction = biddingOffer.idAuction;
            IdUser = biddingOffer.idUser;
            Price = biddingOffer.price;
            Date = biddingOffer.date;
        }
    }
}
