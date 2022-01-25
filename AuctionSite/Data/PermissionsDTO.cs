using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite.Data
{
    public class PermissionsDTO 
    {
        public int IdPermissions { get; set; }
        public string Description { get; set; }
        public PermissionsDTO(Permissions p)
        {
            IdPermissions = p.idPermissions;
            Description = p.description;
        }
    }
}
