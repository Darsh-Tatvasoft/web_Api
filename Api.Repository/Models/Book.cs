using System;
using System.Collections.Generic;

namespace Api.Repository.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string? Isbn { get; set; }

    public string? Genre { get; set; }

    public string? Language { get; set; }

    public string? Publisher { get; set; }

    public decimal Price { get; set; }

    public int? Pagecount { get; set; }

    public int? Stockquantity { get; set; }

    public bool? Isavailable { get; set; }

    public DateOnly Publisheddate { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public bool? Isdeleted { get; set; }

    public int? Updatedby { get; set; }
}
