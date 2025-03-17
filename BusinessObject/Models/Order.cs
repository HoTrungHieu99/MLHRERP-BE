﻿using Azure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public long SalesAgentId { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalPrice { get; set; }
        public string Status { get; set; } // PENDING, PROCESSING, CANCELED
        public long RequestId { get; set; }

        [ForeignKey("RequestId")]
        public RequestProduct Request { get; set; }
        public ICollection<PaymentHistory> PaymentHistories { get; set; }
        public ICollection<RequestExport> RequestExports { get; set; }
    }
}
