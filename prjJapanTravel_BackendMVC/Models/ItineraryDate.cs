﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjJapanTravel_BackendMVC.Models;

public partial class ItineraryDate
{
    public int ItineraryDateSystemId { get; set; }

    public int? ItinerarySystemId { get; set; }

    public string DepartureDate { get; set; }

    public virtual ICollection<ItineraryOrder> ItineraryOrders { get; set; } = new List<ItineraryOrder>();

    public virtual Itinerary ItinerarySystem { get; set; }
}