using MiniAPI.Models.DTO;

namespace MiniAPI.Repository.IRepository
{
    public interface IAuthRepository
    {
        Task<bool> IsUniqueUser(string username);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    }
}
