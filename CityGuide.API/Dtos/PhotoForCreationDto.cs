using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityGuide.API.Dtos
{
    public class PhotoForCreationDto
    {
        public PhotoForCreationDto()
        {
            DateAdd = DateTime.Now;
        }

        public string Url {get; set;}
        public IFormFile File { get; set; }
        public string Descriptipn {get; set;}
        public DateTime DateAdd { get; set; }
        public string  PublicId {get; set;}
    }
}
