using AutoMapper;
using FluentValidation;
using MiniAPI.Data;
using MiniAPI.Models.DTO;
using MiniAPI.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MiniAPI.Endpoints
{
    public static class CouponEndpoints
    {
        public static void MapCouponEndpoints(this WebApplication app)
        {
            app.MapGet("/api/coupon", GetAllCoupon).WithName("GetAlCoupons").Produces<APIResponse>(200).RequireAuthorization("AdminOnly"); ;


            app.MapGet("/api/coupon/{id:int}", (ApplicationDbContext _db, int id) =>
            {
                APIResponse response = new APIResponse();
                response.IsSuccess = true;
                response.Result = _db.Coupon.FirstOrDefault(c => c.Id == id);
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);

            }).WithName("GetCoupon").Produces<APIResponse>(200)
            .AddEndpointFilter(async (context, next) =>
            {
                var id=context.GetArgument<int>(1);
                if(id==0)
                {
                    return Results.BadRequest(new APIResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest, ErrorMessages = new List<string>() { "Id cannot be 0" } });
                }
                Console.WriteLine("Before 1");
                var result=await next(context);
                Console.WriteLine("After 1");
                return result;
            })
            .AddEndpointFilter(async (context, next) =>
            {
                Console.WriteLine("Before 2");
                var result=await next(context);
                Console.WriteLine("After 2");
                return result;
            })
            ;

            app.MapPost("/api/coupon", CreateCoupon).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);

            app.MapPut("/api/coupon/{id:int}", UpdateCoupon).WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400).Produces(404);

            app.MapDelete("/api/coupon/{id:int}", async (ApplicationDbContext _db, ILogger<Program> _logger, int id) =>
            {
                APIResponse response = new APIResponse();
                Coupon coupon = _db.Coupon.FirstOrDefault(c => c.Id == id);
                if (coupon == null)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add($"Coupon with id {id} not found");
                    response.StatusCode = HttpStatusCode.NotFound;
                    return Results.NotFound(response);
                }

                _db.Coupon.Remove(coupon);
                await _db.SaveChangesAsync();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("DeleteCoupon").Produces<APIResponse>(200).Produces(404);
        }

        private async static Task<IResult> GetAllCoupon(ApplicationDbContext _db, ILogger<Program> _logger)
        {
            APIResponse response = new APIResponse();
            response.IsSuccess = true;
            response.Result = await _db.Coupon.ToListAsync();
            response.StatusCode = HttpStatusCode.OK;
            _logger.LogInformation("Getting all coupons");
            return Results.Ok(response);
        }
        [Authorize(Policy = "AdminOnly")]
        private async static Task<IResult> CreateCoupon(ApplicationDbContext _db, IMapper _mapper, IValidator<CouponCreateDTO> _validator, [FromBody] CouponCreateDTO data)
        {
            APIResponse response = new APIResponse();
            var validationResult = await _validator.ValidateAsync(data);
            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                response.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(response);
            }

            Coupon coupon = _mapper.Map<Coupon>(data);

            coupon.Created = DateTime.Now;
            await _db.Coupon.AddAsync(coupon);
            await _db.SaveChangesAsync();

            CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);
            response.IsSuccess = true;
            response.Result = couponDTO;
            response.StatusCode = HttpStatusCode.Created;
            return Results.Created($"/api/coupon/{coupon.Id}", coupon);
            //return Results.Created($"/api/coupon/{coupon.Id}",coupon); 
            //return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDTO);
        }
        [Authorize(Policy = "AdminOnly")]
        private async static Task<IResult> UpdateCoupon(ApplicationDbContext _db, IMapper _mapper, IValidator<CouponUpdateDTO> _validator, [FromBody] CouponUpdateDTO data, int id)
        {
            APIResponse response = new APIResponse();
            var validationResult = await _validator.ValidateAsync(data);
            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                response.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(response);
            }

            Coupon coupon = _db.Coupon.FirstOrDefault(c => c.Id == id);
            if (coupon == null)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add($"Coupon with id {id} not found");
                response.StatusCode = HttpStatusCode.NotFound;
                return Results.NotFound(response);
            }

            _mapper.Map(data, coupon);
            coupon.LastUpdated = DateTime.Now;
            _db.Coupon.Update(coupon);
            await _db.SaveChangesAsync();
            CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);
            response.IsSuccess = true;
            response.Result = couponDTO;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }
    }
}
