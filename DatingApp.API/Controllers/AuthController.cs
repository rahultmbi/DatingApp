using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dto;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace DatingApp.API.Controllers
{
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IAuthRepository _authRepository { get; set; }
        public IConfiguration _config { get; set; }

        public IMapper _mapper { get; set; }

        public AuthController(IAuthRepository authRepository, IMapper mapper , IConfiguration config)
        {
            _config = config;
            _authRepository = authRepository;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _authRepository.UserExists(userForRegisterDto.Username))
                return BadRequest("Username alreary exists!");

            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var createdUser = await _authRepository.Register(userToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailsDto>(createdUser);

            return CreatedAtRoute("GetUser", new {
                controller = "Users", id = createdUser.Id
            },userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            //throw new Exception("Server says No!.");
            //checking user exist or not
            var UserFromRepo = await _authRepository.Login(userForLoginDto.username.ToLower(), userForLoginDto.password);

            if (UserFromRepo == null)
                return Unauthorized();

            // Creating token

            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier,UserFromRepo.Id.ToString()),
                 new Claim(ClaimTypes.Name,UserFromRepo.Username)
             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _config.GetSection("AppSettings:Token").Value));

            var creds =  new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tockenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tockenHandler = new JwtSecurityTokenHandler();

            var token = tockenHandler.CreateToken(tockenDescriptor);

            var user = _mapper.Map<UserForListDto>(UserFromRepo);

            return Ok(new {
               token = tockenHandler.WriteToken(token),
               user
            });
        }
    }
}