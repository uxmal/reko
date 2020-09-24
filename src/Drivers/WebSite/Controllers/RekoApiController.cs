using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace website.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RekoApiController
    {

        [HttpGet]
        public string GetString()
        {
            return "string";
        }
    }

}
