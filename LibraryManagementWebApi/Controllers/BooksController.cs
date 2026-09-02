using LibraryManagementWebApi.DTOs;
using LibraryManagementWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(
            IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooks(
            [FromQuery] string search = null)
        {
            var books =
                await _bookService.GetBooks(search);

            return Ok(books);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBook(
            int id)
        {
            try
            {
                var book =
                    await _bookService.GetBook(id);

                return Ok(book);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook(
            BookDto dto)
        {
            try
            {
                var book =
                    await _bookService.AddBook(dto);

                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(
            int id,
            BookDto dto)
        {
            try
            {
                await _bookService.UpdateBook(
                    id,
                    dto);

                return Ok(new
                {
                    message =
                        "Book updated successfully."
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(
            int id)
        {
            try
            {
                await _bookService.DeleteBook(id);

                return Ok(new
                {
                    message =
                        "Book deleted successfully."
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