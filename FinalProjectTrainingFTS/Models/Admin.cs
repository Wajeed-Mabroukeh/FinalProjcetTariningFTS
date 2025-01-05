using System;
using System.Collections.Generic;

namespace FinalProjectTrainingFTS.Models;

public partial class Admin
{
    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Id { get; set; }
}
