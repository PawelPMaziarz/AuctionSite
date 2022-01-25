using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionSite.Data;


namespace AuctionSite
{
    class RatingRepository
    {
        private AuctionSiteEntities db;
        public List<RatingDTO> Ratings { get; private set; }
        public RatingRepository()
        {
            RefreshRatings();
        }
        public void RefreshRatings()
        {
            if (db != null) db.Dispose();
            db = new AuctionSiteEntities();
            if (Ratings != null && Ratings.Count > 0)
                Ratings.Clear();
            else
                Ratings = new List<RatingDTO>();
            db.Rating.ToList().ForEach(x => Ratings.Add(new RatingDTO(x)));
        }
        public List<RatingDTO> GetRatingsByRatingUser(int idUser)
        {
            return Ratings.Where(x => x.IdRatingUser == idUser).ToList();
        }
        public List<RatingDTO> GetRatingsByRatedUser(int idUser)
        {
            return Ratings.Where(x => x.IdRatedUser == idUser).ToList();
        }
        public RatingDTO GetRatingByIdPurchase(int idPurchase)
        {
            return Ratings.FirstOrDefault(x => x.IdPurchase == idPurchase);
        }
        /// <summary>Pass null in parameter if u want all unrated auction</summary>
        public List<AuctionDTO> GetUnratedAuctions(int? idUser=null)
        {
            AuctionRepository auctionRepo = new AuctionRepository();
            return auctionRepo.Filter(null, null, null, null, null, null, null, null, null, null, idUser, false);
        }
        public void ShowAllAvargeRatings()
        {
            int maxLen = db.AvargeUserRating.Max(x => x.Name.Length);
            Console.WriteLine($"osoba{new String(' ', maxLen-"osoba".Length)}\tobsluga\t\topis\t\tczas dostawy");
            db.AvargeUserRating.ToList().ForEach(x=> {
                Console.Write(x.Name+new String(' ',maxLen-x.Name.Length));
                Console.WriteLine($"\t{x.BuyerService} \t{x.TrueDescription} \t{x.ShippingTime}");
            });
        }
        public bool AddRating(int idPurchase, int BuyerService, int TrueDescription, int ShippingTime)
        {
            if(!db.Purchase.Any(x=> x.idPurchase==idPurchase))
            {
                Console.WriteLine("Nieprawidlowe id zakupu");
                return false;
            }
            if(GetRatingByIdPurchase(idPurchase) != null)
            {
                Console.WriteLine("Ten zakup juz jest oceniony");
                return false;
            }
            if(BuyerService < 1 || BuyerService > 5 || TrueDescription < 1 || TrueDescription > 5 || ShippingTime < 1 || ShippingTime > 5)
            {
                Console.WriteLine("Wartosci ocen powinny wynosic od 1 do 5");
                return false;
            }
            db.Rating.Add(new Rating() { 
                idPurchase = idPurchase,
                buyerService = BuyerService,
                trueDescription = TrueDescription,
                shippingTime = ShippingTime, 
                date = DateTime.Now
            });
            int result = db.SaveChanges();

            if (result < 1)
            {
                Console.WriteLine("Ocenianie zakupu sie nie powiodlo");
                return false;
            }
            else
            {
                Console.WriteLine("Oceniono zakup");
                RefreshRatings();
                return true;
            }
        }
    }
}
