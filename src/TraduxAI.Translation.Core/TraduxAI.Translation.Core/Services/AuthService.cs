using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Data;
using TraduxAI.Shared.DTOs;
using TraduxAI.Shared.Enumerations;
using TraduxAI.Shared.Helpers;
using TraduxAI.Shared.Models;
using TraduxAI.Translation.Core.Interfaces;
using TraduxAI.Translation.Core.Repositories;

namespace TraduxAI.Translation.Core.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _userRepository;
		private readonly JwtService _jwtService;

		public AuthService(IUserRepository userRepository, JwtService jwtService)
		{
			_userRepository = userRepository;
			_jwtService = jwtService;
		}

		public async Task<(bool Success, string Message, string? Token)> LoginUserAsync(LoginUserDto dto)
		{
			var user = await _userRepository.GetUserByEmailAsync(dto.Email);
			if (user == null)
			{
				return (false, "Ivalid email or password", null);
			}

			var isPasswordValid = PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash);
			if (!isPasswordValid)
			{
				return (false, "Ivalid email or password", null);
			}

			var toke = _jwtService.GenerateToken(user);

			return (true, "user logged in successfully", toke);
		}

		public async Task<(bool Success, string Message)> RegisterUserAsync(RegisterUserDto registerUserDto)
		{
			// Check if user already exists
			var existingUser = await _userRepository.GetUserByEmailAsync(registerUserDto.Email);
			if (existingUser != null)
			{
				return (false, "Email is already registered.");
			}

			// Check password match
			if (registerUserDto.Password != registerUserDto.ConfirmPassword)
			{
				return (false, "Passwords do not match.");
			}

			// Hash password
			var hashedPassword = PasswordHasher.HashPassword(registerUserDto.Password);

			// Create user
			var newUser = new User
			{
				Email = registerUserDto.Email,
				PasswordHash = hashedPassword,
				Role = UserRole.User
			};

			await _userRepository.CreateUserAsync(newUser);

			return (true, "User registered successfully.");
		}
	}
}
