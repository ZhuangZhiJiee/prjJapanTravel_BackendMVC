﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace JP_FrontWebAPI.Models;

public partial class Port
{
    public int PortId { get; set; }

    public string PortName { get; set; }

    public string City { get; set; }

    public string CityDescription1 { get; set; }

    public string CityDescription2 { get; set; }

    public string PortGoogleMap { get; set; }

    public virtual ICollection<PortImage> PortImages { get; set; } = new List<PortImage>();

    public virtual ICollection<Route> RouteDestinationPorts { get; set; } = new List<Route>();

    public virtual ICollection<Route> RouteOriginPorts { get; set; } = new List<Route>();
}