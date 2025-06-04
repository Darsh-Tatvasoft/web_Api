using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Repository.ViewModels;

public partial class BookDetails
{

    public int Id { get; set; }
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = null!;
    [Required(ErrorMessage = "Author is required.")]
    public string Author { get; set; } = null!;
    [Required(ErrorMessage = "ISBN is required.")]
    public string? Isbn { get; set; }
    [Required(ErrorMessage = "Genre is required.")]
    public string? Genre { get; set; }
    [Required(ErrorMessage = "Language is required.")]
    public string? Language { get; set; }
    [Required(ErrorMessage = "Publisher is required.")]
    public string? Publisher { get; set; }
    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
    public decimal Price { get; set; }
    [Required(ErrorMessage = "Description is required.")]
    public int? Pagecount { get; set; }
    [Required(ErrorMessage = "Stock quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative integer.")]
    public int? Stockquantity { get; set; }

    public bool? Isavailable { get; set; }
    [Required(ErrorMessage = "Published date is required.")]
    public DateOnly Publisheddate { get; set; }
}