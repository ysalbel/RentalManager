﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentalManagement.Models
{
    public class Rental
    {
        public int ID { get; set; }
        public Asset AssetID { get; set; }
        public Tenant ClientID { get; set; }
        public DateTime NegotiatedOn { get; set; }
        public string Details { get; set; }
    }
}
