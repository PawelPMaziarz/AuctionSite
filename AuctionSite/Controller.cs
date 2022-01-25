using AuctionSite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite
{
    class Controller
    {
        private Authorization Auth { get; set; }
        private AuctionRepository AuctionRepo { get; set; }
        private RatingRepository RatingRepo { get; set; }
        private CategoriesRepository CategoriesRepo { get; set; }
        private Backup Backup { get; set; }
        private Dictionary<string, string> paramDict = new Dictionary<string, string>();
        public Controller()
        {
            try
            {
                Auth = new Authorization();
                AuctionRepo = new AuctionRepository();
                RatingRepo = new RatingRepository();
                CategoriesRepo = new CategoriesRepository();
                Backup = new Backup();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\n\t{e.StackTrace}");
                Console.ReadLine();
                return;
            }
        }

        private int CreateMenu(List<string> list, string name="element", bool header=true)
        {
            if(list.Count()==0)
            {
                Console.WriteLine("Brak elementow do wyswietlenia.");
                return -1;
            }
            if(header) 
                Console.WriteLine($"Wybierz {name}, wpisujac numer i naciskajac enter: ");
            for (int i = 0; i < list.Count(); i++)
            {
                Console.WriteLine($"\t{i + 1}. {list[i]}");
            }
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > list.Count())
            {
                Console.WriteLine("Nieprawidlowy wybor, sprobuj ponownie.");
            }
            return choice-1;
        }
        public void Main()
        {
            //only for testing:
            //Auth.Login("drugitestet@gmail.com", "drugitesterQ1!"); //id=22
            //Auth.Login("supertester", "supertester"); //id = 21
            //Auth.Login("FrydrychWojciechowski@onet.pl", "FrydrychWojciechowski@onet.pl");
            //Auth.Login("w64155@student.wsiz.edu.pl", "w64155@student.wsiz.edu.pl");
            while (true)
            {
                //Console.Clear();
                Console.WriteLine();
                if (Auth == null || AuctionRepo== null || RatingRepo == null || CategoriesRepo == null)
                {
                    Console.WriteLine("Wystapil blad, uruchom aplikacje ponownie");
                    return;
                }

                List<string> options = new List<string>() { "Wszystkie aukcje", "Filtruj aukcje" };
                if (!Auth.IsLogged())
                {
                    options.Add("Logowanie");
                    options.Add("Rejestracja");
                } 
                else
                {
                    options.Add("Dodawanie aukcji");
                    options.Add("Oceny");
                    options.Add("Dodawanie adresu");
                    if(RatingRepo.GetUnratedAuctions(Auth.GetUserId()).Any())
                        options.Add("Dodawanie oceny");
                    if (AuctionRepo.UnfinishedWinningBiddings().Where(x => x.BiddingOffer.IdUser == Auth.GetUserId()).Any())
                        options.Add("Finalizacja zakupu wygranej aukcji");
                    if (Auth.GetUserPermissions() == 4)
                    {
                        options.Add("Srednie oceny sprzedawcow");
                        options.Add("Import");
                        options.Add("Export");
                    }
                    options.Add("Wylogowanie");
                }
                options.Add("Wyjscie");

                switch(options.ElementAt(CreateMenu(options, "akcje")))
                {
                    case "Wszystkie aukcje":
                        SelectAuction(false);
                        break;
                    case "Filtruj aukcje":
                        SelectAuction(true);
                        break;
                    case "Logowanie":
                        Login();
                        break;
                    case "Rejestracja":
                        Register();
                        break;
                    case "Oceny":
                        AllRatings();
                        break;
                    case "Dodawanie aukcji":
                        AddAuction();
                        break;
                    case "Dodawanie oceny":
                        AddRating();
                        break;
                    case "Finalizacja zakupu wygranej aukcji":
                        FinishWinnedBid();
                        break;
                    case "Dodawanie adresu":
                        AddAddress();
                        break;
                    case "Srednie oceny sprzedawcow":
                        RatingRepo.ShowAllAvargeRatings();
                        Console.WriteLine("Wcisnij enter by wyjsc");
                        Console.Read();
                        break;
                    case "Wylogowanie":
                        Auth.Logout();
                        break;
                    case "Import":
                        Console.WriteLine(Backup.ImportUsers(GetNewOrOldInput("sciezke pliku")) ? "Sukces" : "Niepowodzenie");
                        break;
                    case "Export":
                        Console.WriteLine(Backup.ExportUsers(GetNewOrOldInput("sciezke pliku")) ? "Sukces" : "Niepowodzenie" );
                        break;
                    case "Wyjscie":
                        return;
                }
            }
        }
        private string GetNewOrOldInput(string name, string oldValue="", bool forceToGetAnyValue=true)
        {
            string newValue = "";
            if (oldValue.Length == 0)
            {
                Console.WriteLine($"Podaj {name}: ");
                if (forceToGetAnyValue)
                    while (newValue.Length == 0) newValue = Console.ReadLine();
                else
                    newValue = Console.ReadLine();
                return newValue;
            }
            else
            {
                Console.WriteLine($"Podaj {name}: (wcisnij enter jesli nie chcesz zmieniac '{oldValue}')");
                newValue = Console.ReadLine();
                if (newValue.Length == 0) return oldValue;
                else return newValue;
            }
        }
        /// <summary>paramList eq: "email,password,first name"</summary>
        private void GetInputForParameters(string paramList, bool forceToGetAnyValue = true)
        {
            if (!String.Join(",", paramDict.Keys).Equals(paramList))
                paramDict = paramList.Split(',').ToDictionary(key => key, value => "");
            paramDict.Keys.ToList().ForEach(key => paramDict[key] = GetNewOrOldInput(key, paramDict[key],forceToGetAnyValue));
        }
        private void Login()
        {
            paramDict.Clear();
            do
            {
                GetInputForParameters("email,haslo");
                Console.WriteLine("Zalogowac na podane dane? y/n");
            } while (Console.ReadLine() == "y" && !Auth.Login(paramDict["email"], paramDict["haslo"]));
        }
        private void Register()
        {
            bool success;
            paramDict.Clear();
            do
            {
                GetInputForParameters("email,haslo,imie,nazwisko");
                Console.WriteLine("Zarejestrowac na podane dane? y/n");
                if (Console.ReadLine() == "n") return;
                success = Auth.Register(paramDict["email"], paramDict["haslo"], paramDict["imie"], paramDict["nazwisko"]);
            } while (!success);

            if(Auth.Login(paramDict["email"], paramDict["haslo"])) {
                Console.WriteLine("Dodaj swoj pierwszy adres: ");
                AddAddress();
            }

        }
        private void AddAddress()
        {
            paramDict.Clear();
            do
            {
                GetInputForParameters("numer telefonu,mieszkanie,budynek,ulica,miasto,kod pocztowy");
                Console.WriteLine("Utworzyc adres na podane dane? y/n");
            } while (Console.ReadLine() == "y" && !Auth.AddAddress(paramDict["numer telefonu"], paramDict["mieszkanie"],
                    paramDict["budynek"], paramDict["ulica"], paramDict["miasto"], paramDict["kod pocztowy"]));
        }
        private void AllRatings()
        {
            Console.WriteLine("Moje otrzymane oceny: ");
            RatingRepo.GetRatingsByRatedUser(Auth.User.IdUser).ForEach(x => Console.WriteLine(x));
            Console.WriteLine("Moje wystawione oceny: ");
            RatingRepo.GetRatingsByRatingUser(Auth.User.IdUser).ForEach(x => Console.WriteLine(x));
            Console.WriteLine("Wcisnij enter by wyjsc");
            Console.Read();
            return;
        }
        private void AddRating()
        {
            var list = RatingRepo.GetUnratedAuctions(Auth.GetUserId());
            Console.WriteLine("Masz nieocenione zakupy");
            int idPurchase = list.ElementAt(CreateMenu(list.ConvertAll(x => x.ToString()), "aukcje")).Purchase.IdPurchase;
            paramDict.Clear();
            do
            {
                GetInputForParameters("ocene obslugi,ocene opisu,ocene dostawy");
                Console.WriteLine("Chcesz dodac ocene? y/n");
                if (Console.ReadLine() == "n") return;

                if (!int.TryParse(paramDict["ocene obslugi"], out int service) ||
                    !int.TryParse(paramDict["ocene opisu"], out int description) ||
                    !int.TryParse(paramDict["ocene dostawy"], out int shipping))
                {
                    Console.WriteLine("Wartosci ocen powinny wynosic od 1 do 5");
                    continue;
                }
                else
                {
                    if (RatingRepo.AddRating(idPurchase, service, description, shipping))
                        return;
                }
            } while (true);
        }
        private void AddAuction()
        {
            int idCategory = -1;
            paramDict.Clear();
            do
            {
                if(idCategory > 0)
                {
                    Console.WriteLine("Wybrana kategoria: " + CategoriesRepo.GetCategoryById(idCategory));
                    Console.WriteLine("Chcesz zmienic kategorie? y/n");
                    if (Console.ReadLine() == "y") idCategory = ChooseCategory(false);
                } 
                else
                    idCategory = ChooseCategory(false);
                GetInputForParameters("tytul,opis,cena startowa licytacji,cena kup teraz",false);
                Console.WriteLine("Utworzyc aukcje na podane dane? y/n");
                if (Console.ReadLine() == "n") return;
            } while (!AuctionRepo.AddAuction(Auth.GetUserId(),idCategory,paramDict["tytul"], paramDict["opis"]
                    ,TryParseDecimal(paramDict["cena startowa licytacji"]), TryParseDecimal(paramDict["cena kup teraz"])));

        }
        private void FinishWinnedBid()
        {
            var list = AuctionRepo.UnfinishedWinningBiddings().Where(x => x.BiddingOffer.IdUser == Auth.GetUserId()).ToList();
            Console.WriteLine("Masz wygrane licytacje, ktorych nie sfinalizowales: ");
            int idAuction = list.ElementAt(CreateMenu(list.ConvertAll(x => x.ToString()),"aukcje")).IdAuction;
            Console.WriteLine("Chcesz teraz sfinalizowac? y/n");
            if (Console.ReadLine() == "y")
                BuyAuction(idAuction, false);
        }
        private void SelectAuction(bool filter)
        {
            List<AuctionDTO> list = filter ? GetFilteredAuctions() : GetAllActiveAuctions();
            if (list.Count() == 0)
            {
                Console.WriteLine("Brak aukcji do wyswietlenia");
                return;
            }
            while(true)
            {
                Console.WriteLine();
                AuctionDTO auction = list.ElementAt(CreateMenu(list.ConvertAll(x => x.ToString()), "aukcje"));

                Console.WriteLine(auction.FullInfo());

                List<string> options = new List<string> { };
                if(!auction.IsSold())
                {
                    if (auction.IsBuyNow()) options.Add("Kup teraz");
                    if (auction.IsBidding()) options.Add("Zalicytuj");
                }
                options.Add("Wyjscie do aukcji");
                options.Add("Wyjscie do menu glownego");
                string choose = options.ElementAt(CreateMenu(options, "akcje"));
                switch(choose)
                {
                    case "Kup teraz":
                        if (BuyAuction(auction.IdAuction, true)) return;
                        break;
                    case "Zalicytuj":
                        if(BidAuction(auction.IdAuction)) return;
                        break;
                    case "Wyjscie do aukcji":
                        break;
                    case "Wyjscie do menu glownego":
                        return;
                }
            }
        }
        private List<AuctionDTO> GetAllActiveAuctions()
        {
            return AuctionRepo.Filter(null, null, null, null, null, null, null, null, true, false, null, null);
        }
        public static DateTime? TryParseDateTime(string text) =>
            DateTime.TryParse(text, out var date) ? date : (DateTime?)null;
        public static bool? TryParseBoolean(string text) =>
            text.Equals("y") ? true : (text.Equals("n") ? false : (bool?)null);
        public static decimal? TryParseDecimal(string text) =>
            decimal.TryParse(text, out var val) ? val : (decimal?)null;
        public static int? TryParseInt(string text) =>
            int.TryParse(text, out var val) ? val : (int?)null;
        private List<AuctionDTO> GetFilteredAuctions()
        {
            int categoryId=-1;
            paramDict.Clear();
            do
            {
                if(categoryId==-1)
                    Console.WriteLine("Chcesz wybrac kategorie? y/n");
                else
                {
                    Console.WriteLine("Wybrana kategoria: " + CategoriesRepo.GetCategoryById(categoryId));
                    Console.WriteLine("Chcesz zmienic kategorie? y/n");
                }
                if (Console.ReadLine() == "y") categoryId = ChooseCategory();

                Console.WriteLine("Jesli nie chcesz uzyc danego filtra, wcisnij enter");

                GetInputForParameters("slowa kluczowe,przed data,po dacie,tylko aukcje kup teraz (y/n),minimalna cena,maksymalna cena,aktywna (y/n),sprzedana (y/n)", false);
                Console.WriteLine("Chcesz wyszukac wg wybranych filtrow? y/n");
            } while (Console.ReadLine() == "n");
            //parameters validation
            string words = paramDict["slowa kluczowe"].Length==0 ? null : paramDict["slowa kluczowe"];
            DateTime? beforeDate = TryParseDateTime(paramDict["przed data"]);
            DateTime? afterDate = TryParseDateTime(paramDict["po dacie"]);
            bool? onlyBuyNow = TryParseBoolean(paramDict["tylko aukcje kup teraz (y/n)"]);
            decimal? minPrice = TryParseDecimal(paramDict["minimalna cena"]);
            decimal? maxPrice = TryParseDecimal(paramDict["maksymalna cena"]);
            bool? active = TryParseBoolean(paramDict["aktywna (y/n)"]);
            bool? sold = TryParseBoolean(paramDict["sprzedana (y/n)"]);

            return AuctionRepo.Filter(words, beforeDate, afterDate, categoryId == -1 ? (int?)null : categoryId, 
                onlyBuyNow, minPrice, maxPrice, null, active, sold, null, null);
        }
        private int ChooseCategory(bool canChooseMainCategory=true)
        {
            int choose;
            List<ProductCategoryDTO> list;
            do
            {
                list = CategoriesRepo.GetMainCategories();
                choose = list.ElementAt(CreateMenu(list.ConvertAll(x => x.ToString()), "glowna kategorie")).IdProductCategory;

                if(canChooseMainCategory)
                    Console.WriteLine("Chcesz wybrac podkategorie? y/n");
                if (!canChooseMainCategory || Console.ReadLine() == "y")
                {
                    list = CategoriesRepo.GetSubCategories(choose);
                    choose = list.ElementAt(CreateMenu(list.ConvertAll(x => x.ToString()), "podkategorie")).IdProductCategory;
                }
                Console.WriteLine(CategoriesRepo.GetCategoryById(choose));
                Console.WriteLine("Chcesz wybrac ta kategorie? y/n");
            } while (Console.ReadLine() == "n");
            return choose;
        }
        private bool BuyAuction(int idAuction, bool buyNow)
        {
            bool success = false;
            if (!Auth.IsLogged())
            {
                Console.WriteLine("Aby kupic musisz sie zalogowac");
                return success;
            }
            int paymentId, deliveryId;
            var listPayment = AuctionRepo.GetPaymentMethods();
            var listDelivery = AuctionRepo.GetDeliveryMethods();
            do
            {
                paymentId = listPayment.ElementAt(CreateMenu(listPayment.ConvertAll(x => x.ToString()), "sposob platnosci")).IdPaymentMethod;
                deliveryId = listDelivery.ElementAt(CreateMenu(listDelivery.ConvertAll(x => x.ToString()), "sposob dostawy")).IdDeliveryMethod;
                Console.WriteLine("Dostawa pod adres: \n"+Auth.User.DefaultAddress);
                Console.WriteLine("Chcesz kupic? y/n");
                if (Console.ReadLine() == "n")
                    return false;
                //the rest of the validation is done by the auction repository
                success = AuctionRepo.BuyAuction(Auth.GetUserId(), idAuction, paymentId, deliveryId, Auth.GetAddressId(), buyNow);
            } while (!success);
            return success;
        }
        private bool BidAuction(int idAuction)
        {
            bool success=false;
            if(!Auth.IsLogged())
            {
                Console.WriteLine("Aby licytowac musisz sie zalogowac");
                return success;
            }
            string price="";
            do
            {
                Console.WriteLine("Nie musisz wpisywac ceny, wcisnij enter a cena zostanie ustawiona domyslnie 5zl wyzsza od ostatniej oferty");
                price = GetNewOrOldInput("twoja cene", price, false);
                Console.WriteLine("Chcesz zalicytowac? y/n");
                if (Console.ReadLine() == "n")
                    return false;
                //the rest of the validation is done by the auction repository
                success = AuctionRepo.BidAuction(Auth.GetUserId(), idAuction, TryParseDecimal(price));
            } while (!success);
            return success;
        }
    }
}
