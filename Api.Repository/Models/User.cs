﻿using System;
using System.Collections.Generic;

namespace Api.Repository.Models;

public partial class User
{
    public int Id { get; set; }

    public int Role { get; set; }

    public string Name { get; set; } = null!;

    public string Mobilenumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}
