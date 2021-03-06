﻿using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Authorize]
    [EnableCors("CorsPolicy")]
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepository = await _repository.GetUser(currentUserId);

            userParams.UserId = currentUserId;

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = userFromRepository.Gender == "male" ? "female" : "male";

            var users = await _repository.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage,
                                   users.PageSize,
                                   users.TotalCount,
                                   users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repository.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepository = await _repository.GetUser(id);

            _mapper.Map(userForUpdateDto, userFromRepository);

            if (await _repository.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save.");
        }

        [HttpPost("{userId}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repository.GetLike(userId, recipientId);

            if (like != null)
                return BadRequest("You already like this user");
            if (await _repository.GetUser(recipientId) == null)
                return NotFound();

            like = new Like
            {
                LikerId = userId,
                LikeeId = recipientId
            };

            _repository.Add(like);

            if (await _repository.SaveAll())
                return Ok();
            else
                return BadRequest("Failed to like user");
        }
    }
}