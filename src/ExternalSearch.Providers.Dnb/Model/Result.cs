using CluedIn.ExternalSearch.Providers.DnB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CluedIn.ExternalSearch.Providers.DnB.Model
{
    public class FullResult
    {
        public DnBResponse DnBResponse { get; set; }
        public DunsDataResponse DunsDataResponse { get; set; }
    }
}
