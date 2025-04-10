using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Models.Dto.Response;
using SocialMedia.Models.Entities;
using SocialMedia.Repositories;
using BCrypt.Net;

namespace SocialMedia.Services.Users
{
  public class UserService : IUserService
  {
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
      _userRepository = userRepository;
      _roleRepository = roleRepository;
    }

    public async Task<ApiResponse<string>> RegisterUser(RegisterRequest request)
    {
      //check request
      if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
      {
        return new ApiResponse<string>(400, "Email and password are required", null);
      }
      //Check user already exists
      var user = await _userRepository.GetUserByEmailAsync(request.Email);
      if (user != null)
      {
        return new ApiResponse<string>(409, "User already exists", null);
      }
      //check Role User
      var role = await _roleRepository.GetRoleByNameAsync("User");
      if (role == null)
      {
        return new ApiResponse<string>(404, "Role User not found", null);
      }
      //Create new user
      string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
      var newUser = new User
      {
        Email = request.Email,
        Password = hashedPassword,
        UserRoles = new List<UserRole>
         {
             new UserRole
             {
                 RoleId = role.Id
             }
         }
      };
      //Add user to database
      await _userRepository.AddUserAsync(newUser);
      await _userRepository.SaveChangesAsync();

      return new ApiResponse<string>(201, "User created successfully", null);
    }
  }
}
