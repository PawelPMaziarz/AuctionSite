using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite.Data
{
    public class AddressDTO
    {
        public int IdAddress { get; set; }
        public int IdUser { get; set; }
        public string PhoneNo { get; set; }
        public string Apartment { get; set; }
        public string Building { get; set; }
        public string Street { get; set; }
        public string Town { get; set; }
        public string PostCode { get; set; }
        public AddressDTO(Address adr)
        {
            IdAddress = adr.idAddress;
            IdUser = adr.idUser;
            PhoneNo = adr.phoneNo;
            Apartment = adr.apartment;
            Building = adr.building;
            Street = adr.street;
            Town = adr.town;
            PostCode = adr.postCode;
        }
        public override string ToString()
        {
            return $"ul. {Street} {Building}{(String.IsNullOrEmpty(Apartment)?"":"/"+Apartment)} w {Town}";
        }
    }
}
