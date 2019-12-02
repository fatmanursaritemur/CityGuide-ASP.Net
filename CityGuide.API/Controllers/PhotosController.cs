using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CityGuide.API.Data;
using CityGuide.API.Dtos;
using CityGuide.API.Helpers;
using CityGuide.API.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CityGuide.API.Controllers
{
    [Produces("application/json")]
    [Route("api/cities/{cityId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private IAppRepository appRepository;
        private IMapper mapper;
        private IOptions<ClodinarySettings> cloudinarySettings;
        private Cloudinary cloudinary;

        public PhotosController(IAppRepository appRepository, IMapper mapper, IOptions<ClodinarySettings> cloudinarySettings)
        {
            this.appRepository = appRepository;
            this.mapper = mapper;
            this.cloudinarySettings = cloudinarySettings;

            Account account = new Account(
                cloudinarySettings.Value.CloudName,
                cloudinarySettings.Value.ApiKey,
                cloudinarySettings.Value.ApiSecret );

            cloudinary = new Cloudinary(account);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public ActionResult AddPhotosForCity(int cityId, [FromBody]  PhotoForCreationDto photoForCreationDto)
        {
            var city = appRepository.GetCityById(cityId);
            if(city==null)
            {
                return BadRequest("Could not find the city");
            }
            var currentUserId=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if(currentUserId!=city.UserId)
            {
                return Unauthorized();
            }
            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();
            
            if(file.Length>0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File=new FileDescription(file.Name,stream)
                    };
                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;
            var photo = mapper.Map<Photo>(photoForCreationDto);
            photo.City = city;
            if(!city.Photos.Any(p=>p.IsMain))
            {
                photo.IsMain = true;
            }
            city.Photos.Add(photo);
            if(appRepository.SaveAll())
            {
                var photoToReturn = mapper.Map<PhotoForReturnDto>(photo);
                 return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
               // return GetPhoto(photo.Id);
            }
            return BadRequest("Could not add the photo");
        }
        [HttpGet("{id}",Name = "GetPhoto")]
        public ActionResult GetPhoto(int id)
        {
            var photoFormDb = appRepository.GetPhoto(id);
            var photo = mapper.Map<PhotoForReturnDto>(photoFormDb);

            return Ok(photo);
        }
    }
}