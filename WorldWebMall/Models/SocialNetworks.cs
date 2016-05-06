using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WorldWebMall.Models
{
    public class SocialNetworks
    {
        [Key, Column(Order = 1)]
        public int BroadcastId { get; set; }
        [Key, Column(Order=2)]
        public string Network { get; set; }
        public string CompanyId { get; set; }
        public string ExtenalId { get; set; }

        public virtual Company Company { get; set; }
    }

    public class FacebookPageTokens
    {
        [Key]
        public string CompanyId { get; set; }
        public string PageName { get; set; }
        public string Token { get; set; }
        public DateTime exp { get; set; }
    }
}