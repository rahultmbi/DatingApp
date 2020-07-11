using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Dto
{
    public class MessageForCreationDto
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime MessageSent { get; set; }
        public string Content { get; set; }

        public DateTime DateAdded { get; set; }

        public MessageForCreationDto()
        {
            MessageSent = DateTime.Now;
        }
    }
}