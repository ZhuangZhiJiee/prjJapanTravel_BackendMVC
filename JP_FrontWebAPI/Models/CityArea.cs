﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace JP_FrontWebAPI.Models;

public partial class CityArea
{
    public int CityAreaId { get; set; }

    public string CityAreaName { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}