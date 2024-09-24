using BL.Interfaces;
using Entities;
using Microsoft.AspNetCore.Mvc;
using PasswordSystem.API.Infrastructure.Models;

namespace PasswordSystem.API.Controllers
{
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IUsersBL _usersBL;
        private readonly ITokensBL _tokensBL;

        public AuthController(IUsersBL usersBL, ITokensBL tokensBL)
        {
            _usersBL = usersBL;
            _tokensBL = tokensBL;
        }

        [Route("sign-up")]
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] UserRequestDto request)
        {
            var user = new User(request.Username, request.Password);

            try
            {
                var result = await _usersBL.RegisterUserAsync(user);
                if (result)
                {
                    return StatusCode(201);
                }
                return BadRequest("User registration failed.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("auth")]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] UserRequestDto request)
        {
            var isAuthenticated = await _usersBL.AuthenticateUserAsync(request.Username, request.Password);
            if (!isAuthenticated)
            {
                return Forbid();
            }

            var accessToken = _tokensBL.GenerateAccessToken(request.Username);
            var refreshToken = _tokensBL.GenerateRefreshToken(request.Username);

            Response.Cookies.Append("Access-Token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("Refresh-Token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Path = "/refresh",
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok();
        }

        [Route("refresh")]
        [HttpPost]
        public IActionResult RefreshTokens()
        {
            var refreshToken = Request.Cookies["Refresh-Token"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Forbid();
            }

            var newTokens = _tokensBL.RefreshTokens(refreshToken);
            if (newTokens == null)
            {
                return Forbid();
            }

            Response.Cookies.Append("Access-Token", newTokens.Value.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("Refresh-Token", newTokens.Value.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Path = "/refresh",
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok();
        }
    }
}
