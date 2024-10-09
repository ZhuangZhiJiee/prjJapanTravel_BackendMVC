﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace prjJapanTravel_BackendMVC.Models;

[Table("MyCollection")]
public partial class MyCollection
{
    [Key]
    [Column("MyCollectionID")]
    public int MyCollectionId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("ItineraryID")]
    public int ItineraryId { get; set; }

    [ForeignKey("ItineraryId")]
    [InverseProperty("MyCollections")]
    public virtual Itinerary Itinerary { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("MyCollections")]
    public virtual Member Member { get; set; }
}