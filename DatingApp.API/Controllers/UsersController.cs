using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dto;
using DatingApp.API.Helper;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly IDatingRepository _datingRepo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository datingRepo, IMapper mapper)
        {
            _datingRepo = datingRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery]UserParams userParams)
        {
            var userId =  int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _datingRepo.GetUser(userId);

            userParams.UserId = userId;

            if(string.IsNullOrEmpty(userParams.Gender)){
                userParams.Gender = userFromRepo.Gender == "male" ?  "female" : "male";
            }

            var users = await _datingRepo.GetUsers(userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize,users.TotalCount, users.TotalPage);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name="GetUser")]
        public  async Task<IActionResult> Getuser(int id)
        {
            var user =await _datingRepo.GetUser(id);

            var userToReturn = _mapper.Map<UserForDetailsDto>(user);

            return Ok(userToReturn);
        }

       [HttpPut("{id}")]
       public async Task<IActionResult> UpdateUserData(int id, UserForUpdateDto userForUpdateDto)
       {
           if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userForRepo = await _datingRepo.GetUser(id);

            _mapper.Map(userForUpdateDto, userForRepo);

            if(await _datingRepo.SaveAll())
                return NoContent();

            throw new System.Exception($"Updating user {id} failed to save");
       }

       [HttpPost("{id}/like/{recipientId}")]
       public async Task<IActionResult> Likeuser(int id, int recipientId)
       {
           if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _datingRepo.getLike(id, recipientId);

            if (like != null)
                return BadRequest("You have already like this user");
                
            if ( await _datingRepo.GetUser(recipientId) == null)
                return NotFound();

            like = new Like {
                LikerId= id,
                LikeeId = recipientId
            };

            _datingRepo.Add<Like>(like);

            if (await _datingRepo.SaveAll())
                return Ok();

            return BadRequest("failed to like user");
       }
       
    }
}