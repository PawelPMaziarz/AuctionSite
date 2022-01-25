using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionSite.Data;
using Newtonsoft.Json;

namespace AuctionSite
{
    class Backup
    {
        private AuctionSiteEntities db;
        public Backup()
        {
            try { 
                db = new AuctionSiteEntities();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\n\t{e.StackTrace}");
                Console.ReadLine();
                return;
            }
        }
        public bool ExportUsers(string fileName)
        {
            try
            {
                StreamWriter file = new StreamWriter(fileName);
                var list = db.User.ToList().ConvertAll(x => new UserDTO(x));
                file.Write(JsonConvert.SerializeObject(list));
                file.Close();
                return true;
            } 
            catch (Exception e)
            {

                Console.WriteLine($"{e.Message}\n\t{e.StackTrace}");
                Console.ReadLine();
                return false;
            }
        }
        public bool ImportUsers(string fileName)
        {
            try
            {
                StreamReader file = new StreamReader(fileName);
                string text = file.ReadToEnd();
                file.Close();

                //not working, idk why (throws in Permissionsdto constructor that parameter p is null, but every user have permissions)
                List<UserDTO> source = JsonConvert.DeserializeObject<List<UserDTO>>(text);

                int userId, addressId;
                foreach (var el in source)
                {
                    if(!db.User.Where(x=>x.email.Equals(el.Email)).Any())
                    {
                        userId = db.User.Add(new User()
                        {
                            email = el.Email,
                            password = el.Password,
                            idPermissions = el.Permissions.IdPermissions,
                            firstName = el.FirstName,
                            lastName = el.LastName
                        }).idUser;
                        if(el.DefaultAddress!=null)
                        {
                            addressId = db.Address.Add(new Address()
                            {
                                idUser = el.DefaultAddress.IdUser,
                                phoneNo = el.DefaultAddress.PhoneNo,
                                apartment = el.DefaultAddress.Apartment,
                                building = el.DefaultAddress.Building,
                                street = el.DefaultAddress.Street,
                                town = el.DefaultAddress.Town,
                                postCode = el.DefaultAddress.PostCode
                            }).idAddress;
                            db.User.Where(x => x.idUser == userId).First().idDefaultAddress = addressId;
                        }
                    }
                }
                Console.WriteLine("Dodano "+db.SaveChanges()+" rekordow");
                return true;
            }
            catch (Exception e)
            {

                Console.WriteLine($"{e.Message}\n\t{e.StackTrace}");
                Console.ReadLine();
                return false;
            }
        }
    }
}
