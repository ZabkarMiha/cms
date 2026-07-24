using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Requests
{
    public class CarBodyRequest
    {
        public Guid Id { get; set; }
        public string BodyType { get; set; }
    }
}