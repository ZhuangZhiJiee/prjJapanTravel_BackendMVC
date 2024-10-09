﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace prjJapanTravel_BackendMVC.Models;

[Table("Activity")]
public partial class Activity
{
    [Key]
    public int ActivitySystemId { get; set; }

    [StringLength(50)]
    public string ActivityName { get; set; }

    public string ActivityDetail { get; set; }

    [InverseProperty("ActivitySystem")]
    public virtual ICollection<Itinerary> Itineraries { get; set; } = new List<Itinerary>();
}