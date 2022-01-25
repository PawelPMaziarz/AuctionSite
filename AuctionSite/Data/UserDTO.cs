using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite.Data
{
    class UserDTO
    {
        public int IdUser { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public PermissionsDTO Permissions { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AddressDTO DefaultAddress { get; set; }
        public override string ToString()
        {
            return $"{IdUser} {FirstName} {LastName} {Permissions.Description}";
        }
        public UserDTO(User user)
        {
            IdUser = user.idUser;
            Email = user.email;
            Password = user.password;
            FirstName = user.firstName;
            LastName = user.lastName;
            Permissions = new PermissionsDTO(user.Permissions);
            DefaultAddress = user.DefaultAddress==null ? null : new AddressDTO(user.DefaultAddress);
        }
    }
}
