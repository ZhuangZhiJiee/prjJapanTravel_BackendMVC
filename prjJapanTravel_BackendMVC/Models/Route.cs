﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjJapanTravel_BackendMVC.Models;

public partial class Route
{
    public int RouteId { get; set; }

    public int OriginPortId { get; set; }

    public int DestinationPortId { get; set; }

    public decimal Price { get; set; }

    public string RouteDescription { get; set; }

    public virtual Port DestinationPort { get; set; }

    public virtual Port OriginPort { get; set; }

    public virtual ICollection<RouteImage> RouteImages { get; set; } = new List<RouteImage>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}