﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OzymandiasInvestments.Areas.Identity.Data;

// Add profile data for application users by adding properties to the OzymandiasInvestmentsUser class
public class OzymandiasInvestmentsUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AlpacaApiKey { get; set; }
    public string AlpacaApiSecret { get; set; }
}

