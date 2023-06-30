﻿using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.DTO.Output
{
    public class UserPrivateDTO
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Discriminator { get; set; }
        public required string Email { get; set; } 
        public Role Role { get; set; } = Role.USER;
        public string? Token { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Permet de caster un User en USerPrivateDTO

        public static explicit operator UserPrivateDTO(User user)
        {
            return new UserPrivateDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Discriminator = user.Discriminator,
                Email = user.Email,
                Token = user.Token,
                Role = user.Role,
                CreationDate = user.CreationDate,
                UpdatedDate = user.UpdatedDate
            };
        }
    }
}
