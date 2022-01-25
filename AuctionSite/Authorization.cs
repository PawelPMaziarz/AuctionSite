using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AuctionSite.Data;

namespace AuctionSite
{
    class Authorization
    {
        private AuctionSiteEntities db;
        public UserDTO User { get; private set; }
        public Authorization()
        {
            db = new AuctionSiteEntities();
        }
        public bool IsLogged()
        {
            return User != null;
        }
        public int GetUserId()
        {
            return (User == null) ? -1 : User.IdUser;
        }
        public int GetAddressId()
        {
            return (User == null) ? -1 : (User.DefaultAddress==null? -1: User.DefaultAddress.IdAddress);
        }
        public int GetUserPermissions(string email)
        {
            return db.User.Where(x => x.email.Equals(email)).Select(x => x.idPermissions).DefaultIfEmpty(-1).First();
        }
        public int GetUserPermissions()
        {
            return (User == null) ? -1 : User.Permissions.IdPermissions;
        }
        public void Logout()
        {
            User = null;
            Console.WriteLine("Wylogowano");
        }
        /// <summary>Doesn't allow logging in with a blocked or deleted user</summary>
        public bool Login(string email, string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Podaj dane logowania");
                return false;
            }
            var hash = MakeHash(password);
            User u = db.User.Where(x => x.email.Equals(email) && x.password.Equals(hash)).FirstOrDefault();
            if(u==null)
            {
                Console.WriteLine("Niepoprawne dane logowania");
                return false;
            }
            if (u.idPermissions<=2)
            {
                Console.WriteLine("Konto ma status: "+u.Permissions.description);
                return false;
            }

            //login successful
            User = new UserDTO(u);
            Console.WriteLine($"Witaj {User.FirstName} {User.LastName}");
            return true;
        }
        static public string MakeHash(string text) {
            if (text == null)
                return string.Empty;
            else
            {
                return BitConverter.ToString(new System.Security.Cryptography.SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", string.Empty);
            }
        }
        static public bool IsValidEmail(string email)
        {
            try
            {
                return new System.Net.Mail.MailAddress(email).Address.Equals(email);
            }
            catch
            {
                return false;
            }
        }
        static public bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password)
                && password.Length > 8
                && new Regex("[0-9]+").IsMatch(password)
                && new Regex("[a-z]+").IsMatch(password)
                && new Regex("[A-Z]+").IsMatch(password)
                && new Regex("[#?!@$%^&*-]+").IsMatch(password);
        }
        public bool Register(string email, string password, string firstName, string lastName)
        {
            if (!IsValidEmail(email))
            {
                Console.WriteLine("Email jest niepoprawny");
                return false;
            }
            if (!IsValidPassword(password))
            {
                Console.WriteLine("Haslo jest zbyt proste");
                return false;
            }
            //using stored procedure
            //returns int code and string message:
            //-1 existing user, -2 missing data, 1 success
            var result = db.CreateUser(email, MakeHash(password), firstName, lastName).First();
            Console.WriteLine(result.message);
            return result.code==1;
        }
        public bool AddAddress(string phoneNo, string apartment, string building, string street, string town, string postCode)
        {
            if(User==null)
            {
                Console.WriteLine("Aby dodac adres trzeba sie zalogowac");
                return false;
            }
            var adr = db.Address.Add(new Address() {
                idUser=User.IdUser,
                phoneNo=phoneNo,
                apartment=apartment,
                building=building,
                street=street,
                town=town,
                postCode=postCode
            });
            if(db.SaveChanges()>0)
            {
                var u = db.User.Where(x => x.idUser == User.IdUser).FirstOrDefault();
                u.idDefaultAddress = adr.idAddress;
                db.SaveChanges();
                User.DefaultAddress = new AddressDTO(adr);
                Console.WriteLine("Adres dodano poprawnie");
                return true;
            }
            Console.WriteLine("Dodawanie adresu niepowiodlo sie");
            return false;
        }
        
    }
}
