using Microsoft.AspNetCore.Mvc;
using MiniAPI.Models;
using MiniAPI.Models.DTO;
using MiniAPI.Repository.IRepository;
using System.Net;

namespace MiniAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/api/login", async (IAuthRepository _authRepository, [FromBody] LoginRequestDTO loginRequestDTO) =>
            {
                APIResponse response = new APIResponse();
                var user = await _authRepository.Login(loginRequestDTO);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Invalid username or password");
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    return Results.BadRequest(response);
                }
                response.IsSuccess = true;
                response.Result = user;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("Login").Accepts<LoginRequestDTO>("application/json").Produces<APIResponse>(200).Produces(400);

            app.MapPost("/api/register", async (IAuthRepository _authRepository, [FromBody] RegisterationRequestDTO registerationRequestDTO) =>
            {
                APIResponse response = new APIResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
                bool isUnique = await _authRepository.IsUniqueUser(registerationRequestDTO.Username);
                if (!isUnique)
                {
                    response.ErrorMessages.Add("Username already exists");
                    return Results.BadRequest(response);
                }
                var user = await _authRepository.Register(registerationRequestDTO);
                if (user == null || string.IsNullOrEmpty(registerationRequestDTO.Username))
                {
                    response.ErrorMessages.Add("Something went wrong");
                    return Results.BadRequest(response);
                }
                response.IsSuccess = true;
                response.Result = user;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("Register").Accepts<RegisterationRequestDTO>("application/json").Produces<APIResponse>(201).Produces(400);


        }
    }
}
