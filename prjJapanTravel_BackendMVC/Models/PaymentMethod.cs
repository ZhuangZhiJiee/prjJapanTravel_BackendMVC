﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjJapanTravel_BackendMVC.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string PaymentMethod1 { get; set; }

    public virtual ICollection<ItineraryOrder> ItineraryOrders { get; set; } = new List<ItineraryOrder>();

    public virtual ICollection<TicketOrder> TicketOrders { get; set; } = new List<TicketOrder>();
}