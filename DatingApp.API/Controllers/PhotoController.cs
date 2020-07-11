using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dto;
using DatingApp.API.Helper;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        public readonly IDatingRepository _datingRepo;
        public readonly IMapper _mapper;
        public readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly Cloudinary _cloudinary;

        public PhotoController(IDatingRepository datingRepository, IMapper mapper,
        IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
           _mapper = mapper;
            _datingRepo = datingRepository;

            Account acc = new Account(
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiSecret
            );

        _cloudinary = new Cloudinary(acc);

        }

        [HttpGet("{id}", Name = "GetPhoto")]
       public async Task<IActionResult> GetPhoto(int id)
       {
           var photoFromRepo = await _datingRepo.GetPhoto(id);

           var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

           return Ok(photo);
       }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId,
                     [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userForRepo = await _datingRepo.GetUser(userId);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if(file.Length > 0)
            {
                using( var stream = file.OpenReadStream())
                {
                    var uploadParam = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParam);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if(!userForRepo.Photos.Any(x => x.IsMain))
               photo.IsMain = true;

            userForRepo.Photos.Add(photo);

            if(await _datingRepo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForCreationDto>(photo);
                return CreatedAtRoute("GetPhoto", new {id= photo.Id},photoToReturn);
            }

        return BadRequest("Could not add the photo.");

        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int id, int userId )
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _datingRepo.GetUser(userId);

            if(!user.Photos.Any(x=> x.Id == id))
                return Unauthorized();
            
            var photoFromRepo = await _datingRepo.GetPhoto(id);

            if(photoFromRepo.IsMain)    
               return BadRequest("This is already main photo");

            var currentMainPhoto = await _datingRepo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if(await _datingRepo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");
        }        

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _datingRepo.GetUser(userId);

            if(!user.Photos.Any(x=> x.Id == id))
                return Unauthorized();
            
            var photoFromRepo = await _datingRepo.GetPhoto(id);

            if(photoFromRepo.IsMain)    
               return BadRequest("you cannot delete your main photo");
            if(photoFromRepo.PublicId != null)
            {
            var deleteParam = new DeletionParams(photoFromRepo.PublicId);

            var result = _cloudinary.Destroy(deleteParam);

            if(result.Result == "ok")
                _datingRepo.Delete(photoFromRepo);
            }
            
            if(photoFromRepo.PublicId == null)
            {
                _datingRepo.Delete(photoFromRepo);
            }
            
            if(await _datingRepo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete photo");

            
        }

    }
}