using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Requests
{
    public class CarBrandRequest
    {
        public Guid Id { get; set; }
        public string Brand { get; set; }
    }
}