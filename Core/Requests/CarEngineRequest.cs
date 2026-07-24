using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Requests
{
    public class CarEngineRequest
    {
        public Guid Id { get; set; }
        public string EngineType { get; set; }
    }
}