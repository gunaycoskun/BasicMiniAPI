using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniAPI.Data;
using MiniAPI.Models;
using MiniAPI.Models.DTO;
using MiniAPI.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniAPI.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private string secretKey;
        public AuthRepository(ApplicationDbContext db, IMapper mapper, IConfiguration config)
        {
            _db = db;
            _mapper = mapper;
            _config = config;
            secretKey = _config.GetValue<string>("ApiSettings:Secret");

        }
        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _db.LocalUser.FirstOrDefaultAsync(u => u.Username == loginRequestDTO.Username && u.Password == loginRequestDTO.Password);
            if (user == null)
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim(ClaimTypes.Role,user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var userDTO = _mapper.Map<UserDTO>(user);
            return new LoginResponseDTO { Token = new JwtSecurityTokenHandler().WriteToken(token), User = userDTO };

        }

        public async Task<bool> IsUniqueUser(string username)
        {
            var user = await _db.LocalUser.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            LocalUser user = new()
            {
                Username = registerationRequestDTO.Username,
                Password = registerationRequestDTO.Password,
                Name = registerationRequestDTO.Name,
                Role = "admin"
                //Role = "customer"
            };
            _db.LocalUser.Add(user);
            _db.SaveChanges();
            return _mapper.Map<UserDTO>(user);
        }
    }
}
