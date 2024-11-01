﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace JP_FrontWebAPI.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int RouteId { get; set; }

    public DateTime DepartureTime { get; set; }

    public DateTime ArrivalTime { get; set; }

    public int Seats { get; set; }

    public int Capacity { get; set; }

    public virtual Route Route { get; set; }

    public virtual ICollection<TicketOrderItem> TicketOrderItems { get; set; } = new List<TicketOrderItem>();
}