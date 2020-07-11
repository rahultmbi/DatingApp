using System;
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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        public readonly IDatingRepository _datingRepo;
        private readonly IMapper _mapper;

        public MessageController(IDatingRepository datingRepo, IMapper mapper)
        {
            _datingRepo = datingRepo;
            _mapper = mapper;
        }

        [HttpGet("{id}",Name="GetMessage")]
        public async Task<IActionResult> GetMessage(int id){
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _datingRepo.GetMessage(id);

            if(messageFromRepo == null)
                return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;

            var recipient = await _datingRepo.GetUser(messageForCreationDto.RecipientId);

            if(recipient == null)    
                return BadRequest("Could not find user");

            var message = _mapper.Map<Message>(messageForCreationDto);

            _datingRepo.Add(message);

            var messageToReturn = _mapper.Map<MessageForCreationDto>(message);


            if(await _datingRepo.SaveAll())
                return CreatedAtRoute("GetMessage",new {id = message.Id}, messageToReturn);

            throw new Exception("Createing the message failed on save");
            
        }
    }
}