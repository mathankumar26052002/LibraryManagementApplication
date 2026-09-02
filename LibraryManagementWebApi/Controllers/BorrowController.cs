using LibraryManagementWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BorrowController : ControllerBase
    {
        private readonly IBorrowService _borrowService;

        public BorrowController(
            IBorrowService borrowService)
        {
            _borrowService = borrowService;
        }

        [HttpPost("{bookId}")]
        public async Task<IActionResult> BorrowBook(
            int bookId)
        {
            try
            {
                var userIdClaim =
                    User.FindFirst("UserId");

                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId =
                    int.Parse(userIdClaim.Value);

                var dueDate =
                    await _borrowService.BorrowBook(
                        bookId,
                        userId);

                return Ok(new
                {
                    message =
                        "Book borrowed successfully.",

                    dueDate = dueDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("return/{bookId}")]
        public async Task<IActionResult> ReturnBook(
            int bookId)
        {
            try
            {
                var userIdClaim =
                    User.FindFirst("UserId");

                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId =
                    int.Parse(userIdClaim.Value);

                await _borrowService.ReturnBook(
                    bookId,
                    userId);

                return Ok(new
                {
                    message =
                        "Book returned successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}