//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AuctionSite.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class AuctionDescription
    {
        public int idAuctionDescription { get; set; }
        public int idAuction { get; set; }
        public System.DateTime date { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Nullable<decimal> startingBiddingPrice { get; set; }
        public Nullable<decimal> buyNowPrice { get; set; }
    
        public virtual Auction Auction { get; set; }
    }
}