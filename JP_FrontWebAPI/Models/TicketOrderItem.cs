﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace JP_FrontWebAPI.Models;

public partial class TicketOrderItem
{
    public int TicketOrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ScheduleId { get; set; }

    public int Quantity { get; set; }

    public int? CommentStar { get; set; }

    public string CommentContent { get; set; }

    public DateTime? CommentTime { get; set; }

    public virtual Order Order { get; set; }

    public virtual Schedule Schedule { get; set; }
}