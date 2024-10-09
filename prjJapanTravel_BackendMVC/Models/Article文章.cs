﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace prjJapanTravel_BackendMVC.Models;

[Table("Article文章")]
public partial class Article文章
{
    [Key]
    public int 文章編號 { get; set; }

    [Column("會員ID")]
    public int 會員id { get; set; }

    [Required]
    [StringLength(50)]
    public string 文章標題 { get; set; }

    public DateOnly 發布時間 { get; set; }

    public int 文章狀態編號 { get; set; }

    [InverseProperty("文章編號Navigation")]
    public virtual ICollection<Article圖片> Article圖片s { get; set; } = new List<Article圖片>();

    [InverseProperty("文章編號Navigation")]
    public virtual ICollection<文章hashtag中介表> 文章hashtag中介表s { get; set; } = new List<文章hashtag中介表>();

    [ForeignKey("文章狀態編號")]
    [InverseProperty("Article文章s")]
    public virtual Article狀態 文章狀態編號Navigation { get; set; }
}