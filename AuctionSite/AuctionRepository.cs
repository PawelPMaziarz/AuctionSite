using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionSite.Data;

namespace AuctionSite
{
    class AuctionRepository
    {
        private AuctionSiteEntities db;
        public List<AuctionDTO> Auctions { get; private set; }
        public AuctionRepository()
        {
            RefreshAuctions();
        }
        public void RefreshAuctions()
        {
            if(db!=null) db.Dispose();
            db = new AuctionSiteEntities();
            if (Auctions != null && Auctions.Count > 0)
                Auctions.Clear();
            else 
                Auctions = new List<AuctionDTO>();
            db.Auction.ToList().ForEach(x=>Auctions.Add(new AuctionDTO(x)));
        }
        public AuctionDTO GetAuctionById(int id)
        {
            var el = Auctions.Where(x => x.IdAuction == id).FirstOrDefault();
            if (el == null)
            {
                RefreshAuctions();
                el = Auctions.Where(x => x.IdAuction == id).FirstOrDefault();
            }
            return el;
        }
        public List<AuctionDTO> UnfinishedWinningBiddings()
        {
            return Filter(null, null, null, null, false, null, null, null, false, false,null,null);
        }
        private bool FilterPriceCheck(AuctionDTO x, bool? onlyBuyNow, decimal? minPrice, decimal? maxPrice)
        {
            decimal? priceBuyNow=null, priceBidding=null;
            if (x.BuyNowPrice.HasValue) 
                priceBuyNow = x.BuyNowPrice;
            if(x.StartingBiddingPrice.HasValue)
            {
                if (x.BiddingOffer != null)
                    priceBidding = x.BiddingOffer.Price;
                else
                    priceBidding = x.StartingBiddingPrice;
            }
            //if there is option onlyBuyNow, check the buy type of the auction,
            //and remove possibility to check the second price
            if(onlyBuyNow.HasValue)
            {
                //only buy now
                if (onlyBuyNow.Value)
                {
                    if (!x.BuyNowPrice.HasValue)
                        return false; //but there is no this option
                    else
                        priceBidding = null;
                }
                //only bidding
                else if (!onlyBuyNow.Value)
                {
                    if (!x.StartingBiddingPrice.HasValue)
                        return false; //but there is no this option
                    else
                        priceBuyNow = null;
                }
            }
            //check both prices if not null
            if (minPrice.HasValue)
            {
                if (priceBuyNow.HasValue && priceBuyNow < minPrice)
                    return false;
                if (priceBidding.HasValue && priceBidding < minPrice)
                    return false;
            }
            if (maxPrice.HasValue)
            {
                if (priceBuyNow.HasValue && priceBuyNow > maxPrice)
                    return false;
                if (priceBidding.HasValue && priceBidding > maxPrice)
                    return false;
            }
            //auction match all required conditions
            return true;
        }
        ///<summary>If you won't use some filtering parameter, just pass null</summary>
        public List<AuctionDTO> Filter(string words, DateTime? beforeDate, DateTime? afterDate, int? idCategory,
            bool? onlyBuyNow, decimal? minPrice, decimal? maxPrice, int? idUserSeller, bool? active, bool? sold, int? idUserBuyer, bool? rated)
        {
            if(words!=null) words=words.ToLowerInvariant();
            return Auctions.Where(x => 
                (words==null ? true : (x.Title.ToLowerInvariant().Contains(words) || x.Description.ToLowerInvariant().Contains(words))) &&
                (!idCategory.HasValue ? true : (x.ProductCategory.IdProductCategory==idCategory || 
                    (x.ProductCategory.ParentCategory!=null && x.ProductCategory.ParentCategory.IdProductCategory==idCategory)) ) &&
                (!active.HasValue ? true : active.Value == x.EndDate>DateTime.Now) &&
                (!sold.HasValue ? true : sold.Value == x.IsSold()) &&
                (beforeDate == null ? true : x.CreationDate <= beforeDate) &&
                (afterDate == null ? true : x.CreationDate >= afterDate) &&
                (!idUserSeller.HasValue ? true : x.UserSeller.IdUser == idUserSeller) &&
                (!idUserBuyer.HasValue ? true : x.Purchase != null && x.Purchase.IdUser == idUserBuyer) &&
                (!rated.HasValue ? true : x.Purchase != null && rated.Value == (x.Purchase.Rating != null)) &&
                FilterPriceCheck(x, onlyBuyNow, minPrice, maxPrice) //to simplicity, i avoid many nested conditions by using func
            ).ToList();
        }
        public bool AddAuction(int idUserSeller, int idProductCategory, string title, string description, decimal? startingBiddingPrice, decimal? buyNowPrice)
        {
            if (db.User.Where(x => x.idUser == idUserSeller).First().idDefaultAddress == null)
            {
                Console.WriteLine("Aby dodac aukcje musisz ustawic adres domyslny lub dodac nowy");
                return false;
            }
            //using stored procedure
            //returns int code and string message:
            //-1 missing data, -2 wrong user, -3 wrong category, 1 success
            var result = db.CreateAuction(idUserSeller, idProductCategory, title, description, (decimal?)startingBiddingPrice, (decimal?)buyNowPrice).First();
            Console.WriteLine(result.message);
            if(result.code == 1)
            {
                RefreshAuctions();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buyNow">True for buy now, false for win bidding</param>
        public bool BuyAuction(int idUserBuyer, int idAuction, int idPaymentMethod, int idDeliveryMethod, int idDeliveryAddress, bool buyNow)
        {
            var user = db.User.Where(x => x.idUser == idUserBuyer).FirstOrDefault();
            if (user==null)
            {
                Console.WriteLine("Nieprawidlowy uzytkownik");
                return false;
            }
            if(!user.Addresses.Where(x=>x.idAddress==idDeliveryAddress).Any())
            {
                Console.WriteLine("Nieprawidlowy adres dostawy");
                return false;
            }
            var auction = GetAuctionById(idAuction);
            if (auction==null)
            {
                Console.WriteLine("Nieprawidlowa aukcja");
                return false;
            }
            if(buyNow && (auction.EndDate < DateTime.Now || auction.Purchase!=null))
            {
                Console.WriteLine("Aukcja jest zakonczona lub sprzedana");
                return false;
            }
            else if (!buyNow)
            {
                if(auction.EndDate > DateTime.Now)
                {
                    Console.WriteLine("Licytacja aukcji jeszcze się nie zakończyła");
                    return false;
                } 
                if(auction.Purchase != null)
                {
                    Console.WriteLine("Aukcja jest juz sprzedana");
                    return false;
                }
                if(auction.BiddingOffer!=null && auction.BiddingOffer.IdUser!=idUserBuyer)
                {
                    Console.WriteLine("Nie wygrales tej aukcji");
                    return false;
                }
            }
            if (!db.PaymentMethod.Where(x => x.idPaymentMethod == idPaymentMethod).Any())
            {
                Console.WriteLine("Nieprawidlowa metoda platnosci");
                return false;
            }
            if (!db.DeliveryMethod.Where(x => x.idDeliveryMethod == idDeliveryMethod).Any())
            {
                Console.WriteLine("Nieprawidlowa metoda dostawy");
                return false;
            }

            db.Purchase.Add(new Purchase() { 
                idUser = idUserBuyer,
                idAuction = idAuction,
                idPaymentMethod = idPaymentMethod,
                idDeliveryMethod = idDeliveryMethod,
                idBuyerAddress = idDeliveryAddress,
                idSellerAddress = (auction.UserSeller.DefaultAddress==null ?
                    auction.UserSeller.DefaultAddress.IdAddress :
                    db.Address.Where(x=>x.idUser==auction.UserSeller.IdUser).Select(x=>x.idAddress).FirstOrDefault()),
                idPurchaseStatus=1,
                date = DateTime.Now,
                price = (buyNow ? auction.BuyNowPrice : auction.BiddingOffer.Price)
            });
            int result = db.SaveChanges();

            if (result < 1)
            {
                Console.WriteLine("Kupowanie aukcji sie nie powiodlo");
                return false;
            }
            else
            {
                Console.WriteLine("Kupiono aukcje");
                RefreshAuctions();
                return true;
            }
        }
        /// <summary>If the price is zero, it is set by default 5 more than the last bid</summary>
        public bool BidAuction(int idUserBuyer, int idAuction, decimal? price)
        {
            if (!db.User.Where(x => x.idUser == idUserBuyer).Any())
            {
                Console.WriteLine("Nieprawidlowy uzytkownik");
                return false;
            }
            var auction = GetAuctionById(idAuction);
            if (auction == null || auction.EndDate < DateTime.Now || auction.Purchase != null)
            {
                Console.WriteLine("Aukcja nieprawidlowa, zakonczona lub sprzedana");
                return false;
            } 
            if (!auction.StartingBiddingPrice.HasValue)
            {
                Console.WriteLine("Aukcja nie jest licytacja");
                return false;
            }

            if(idUserBuyer==auction.UserSeller.IdUser)
            {
                Console.WriteLine("Nie mozna licytowac wlasnej aukcji");
                return false;
            }

            if (!price.HasValue)
                price = (auction.BiddingOffer != null ? auction.BiddingOffer.Price : auction.StartingBiddingPrice.Value)+5;
            if ((auction.BiddingOffer != null && auction.BiddingOffer.Price > price) || auction.StartingBiddingPrice.Value > price)
            {
                Console.WriteLine("Podana cena jest za niska");
                return false;
            }

            db.BiddingOffer.Add(new BiddingOffer()
            {
                idUser = idUserBuyer,
                idAuction = idAuction,
                date = DateTime.Now,
                price = (decimal)price
            });
            int result = db.SaveChanges();

            if (result < 1)
            {
                Console.WriteLine("Licytowanie aukcji sie nie powiodlo");
                return false;
            }
            else
            {
                Console.WriteLine("Zalicytowano aukcje");
                RefreshAuctions();
                return true;
            }
        }
        public List<PaymentMethodDTO> GetPaymentMethods()
        {
            return db.PaymentMethod.ToList().ConvertAll(x => new PaymentMethodDTO(x));
        }
        public List<DeliveryMethodDTO> GetDeliveryMethods()
        {
            return db.DeliveryMethod.ToList().ConvertAll(x => new DeliveryMethodDTO(x));
        }
    }
}
