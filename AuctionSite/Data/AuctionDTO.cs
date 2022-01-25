using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite.Data
{
    class AuctionDTO
    {
        //basic data
        public int IdAuction { get; set; }
        public UserDTO UserSeller { get; set; }
        public ProductCategoryDTO ProductCategory { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EndDate { get; set; }
        //latest auction description
        public int IdAuctionDescription { get; set; }
        public DateTime DateAuctionDescription { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> StartingBiddingPrice { get; set; }
        public Nullable<decimal> BuyNowPrice { get; set; }
        //if not exists - null
        public BiddingOfferDTO BiddingOffer { get; set; } //only last offer
        public PurchaseDTO Purchase { get; set; }
        /// <summary>By default minimal info and prices</summary>
        public override string ToString()
        {
            return MinInfo() + Prices();
        }
        public string MinInfo()
        {
            return $"{Title} od {UserSeller.FirstName} {UserSeller.LastName}, {CreationDate}\n";
        }
        public string Prices()
        {
            StringBuilder s = new StringBuilder();
            if (IsBuyNow())
                s.AppendLine($"\tCena kup teraz: {BuyNowPrice}");
            if (IsBidding())
                s.AppendLine($"\tPoczatkowa cena licytacji: {StartingBiddingPrice}");

            if(IsSold())
                s.AppendLine($"\tKupiono za {Purchase.Price}, {Purchase.Date}");
            else if (BiddingOffer != null)
                s.AppendLine($"\tOstatnia oferta licytacji: {BiddingOffer.Price}, {BiddingOffer.Date}");

            return s.ToString();
        }
        /// <summary>Don't use with toString, minInfo or prices, because this include all informations</summary>
        public string FullInfo()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine($"{Title} od {UserSeller.FirstName} {UserSeller.LastName} w {UserSeller.DefaultAddress.Town}");
            s.AppendLine($"Stworzona {CreationDate}, zakonczy sie {EndDate}");
            s.AppendLine($"Kategoria: {ProductCategory}");
            s.Append(Prices());
            s.AppendLine('\t'+Description);
            return s.ToString();
        }
        public AuctionDTO(Auction auction)
        {
            var lastOffer = auction.BiddingOffer.OrderByDescending(x => x.idBiddingOffer).DefaultIfEmpty(null).First();
            var lad = auction.AuctionDescription.OrderByDescending(x => x.idAuctionDescription).DefaultIfEmpty(null).First();
            IdAuction = auction.idAuction;
            UserSeller = new UserDTO(auction.User);
            ProductCategory = new ProductCategoryDTO(auction.ProductCategory);
            CreationDate = auction.creationDate;
            EndDate = auction.endDate;
            Purchase = !auction.Purchase.Any() ? null : new PurchaseDTO(auction.Purchase.First());
            BiddingOffer = lastOffer == null ? null : new BiddingOfferDTO(lastOffer);
            if(lad!=null)
            {
                IdAuctionDescription = lad.idAuctionDescription;
                DateAuctionDescription = lad.date;
                Title = lad.title;
                Description = lad.description;
                StartingBiddingPrice = lad.startingBiddingPrice;
                BuyNowPrice = lad.buyNowPrice;
            }
        }
        public bool IsSold() => Purchase != null;
        public bool IsBuyNow() => BuyNowPrice != null;
        public bool IsBidding() => StartingBiddingPrice != null;

    }
}
