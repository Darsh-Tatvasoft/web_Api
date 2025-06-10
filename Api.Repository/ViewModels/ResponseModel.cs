using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Repository.ViewModels;

public partial class ResponseModel
{
    public Object Data { get; set; } = new Object();
    public bool Result { get; set; }
    public string Message { get; set; } = string.Empty;
}