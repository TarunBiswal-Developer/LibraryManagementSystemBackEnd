using BackEnd.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    public class LibrarianController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public LibrarianController ( UserManager<IdentityUser> userManager, IConfiguration configuration, ApplicationDbContext context )
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;

        }

        [HttpPost("createStudent")]
        public async Task<IActionResult> CreateStudent ( [FromBody] CreateUsers request )
        {

            string? token = HttpContext.Request.Headers ["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            var key = _configuration ["Jwt:Key"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            ClaimsPrincipal principal;
            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            var roleClaim = principal.FindFirst(ClaimTypes.Role);
            if (roleClaim?.Value != "Librarian")
            {
                return Unauthorized();
            }
            var usernameClaim = principal.FindFirst(ClaimTypes.Name);
            var username = usernameClaim?.Value;

            var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");
                Member member = new Member
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                        Address = request.Address,
                        Phone = request.Phone,
                        JoinDate = DateTime.Now,
                        CreatedBy = username,
                        CreatedOn = DateTime.Now,
                        IsActive = true
                    };
                _context.members.Add(member);
                int i =  await _context.SaveChangesAsync();
                if (i > 0)
                {
                    return Ok("Student created successfully.");
                }
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("createMember")]
        public async Task<IActionResult> CreateMember (  [FromBody] CreateUsers request  )
        {
            string? token = HttpContext.Request.Headers ["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }
            var key = _configuration ["Jwt:Key"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            ClaimsPrincipal principal;
            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            var usernameClaim = principal.FindFirst(ClaimTypes.Name);
            var username = usernameClaim?.Value;
            var roleClaim = principal.FindFirst(ClaimTypes.Role);
            if (roleClaim?.Value != "Librarian")
            {
                return Unauthorized();
            }

            var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Member");
                Member member = new Member
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Address = request.Address,
                    Phone = request.Phone,
                    JoinDate = DateTime.Now,
                    CreatedBy = username,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };
                _context.members.Add(member);
                int i = await _context.SaveChangesAsync();
                if (i > 0)
                {
                    return Ok("Member created successfully.");
                }
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("createLibrarian")]
        //[Authorize(Roles = "Librarian")]
        public async Task<IActionResult> CreateLibrarian ( [FromBody] CreateStudentRequest request )
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest("User with this email already exists.");
            }

            var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                // Assign the "Librarian" role to the user
                await _userManager.AddToRoleAsync(user, "Librarian");
                return Ok("Librarian Created Successfully.");
            }

            return BadRequest(result.Errors);
        }


        //[HttpPost("viewbooks")]
        //public async Task<IActionResult> ViewBooks ()
        //{
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        var allBooks = await _context.books.Where(x => x.IsActive).ToListAsync();
        //        if (allBooks.Count > 0)
        //        {
        //            apiResult.Data = allBooks;
        //            apiResult.IsSuccessfull = true;
        //            apiResult.Message = "Books Found";
        //        }
        //        else
        //        {
        //            apiResult.Data = new object [0];
        //            apiResult.IsSuccessfull = false;
        //            apiResult.Message = "Books Not Found";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResult.IsSuccessfull = false;
        //        apiResult.Message = $"An error occurred: {ex.Message}";
        //    }

        //    return Ok(apiResult);
        //}


        [HttpPost("viewbooks")]
        public async Task<IActionResult> ViewBooks ( [FromBody] PaginationParams paginationParams )
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                int pageIndex = paginationParams.PageIndex > 0 ? paginationParams.PageIndex - 1 : 0;
                int pageSize = paginationParams.PageSize > 0 ? paginationParams.PageSize : 10;
                var query = _context.books.Where(x => x.IsActive);
                var totalBooks = await query.CountAsync();
                var books = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();

                if (books.Count > 0)
                {
                    apiResult.Data = new { TotalItems = totalBooks, Items = books };
                    apiResult.IsSuccessfull = true;
                    apiResult.Message = "Books Found";
                }
                else
                {
                    apiResult.Data = new { TotalItems = 0, Items = new object [0] };
                    apiResult.IsSuccessfull = false;
                    apiResult.Message = "Books Not Found";
                }
            }
            catch (Exception ex)
            {
                apiResult.IsSuccessfull = false;
                apiResult.Message = $"An error occurred: {ex.Message}";
            }

            return Ok(apiResult);
        }


        [HttpGet("getbookbyid")]
        public async Task<IActionResult> RetrieveBookById ( int bookId )
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var bookDetails = await _context.books
                    .FirstOrDefaultAsync(x => x.BookID == bookId && x.IsActive);

                if (bookDetails != null)
                {
                    apiResult.IsSuccessfull = true;
                    apiResult.Data = bookDetails;
                    apiResult.Message = "Record Found";
                }
                else
                {
                    apiResult.IsSuccessfull = false;
                    apiResult.Data = new object [0];
                    apiResult.Message = "Record Not Found";
                }
            }
            catch (Exception ex)
            {
                apiResult.IsSuccessfull = false;
                apiResult.Message = $"An error occurred: {ex.Message}";
                apiResult.Data = new object [0];
            }

            return Ok(apiResult);
        }

        [HttpPost("updatebook")]
        public async Task<IActionResult> UpdateBookById ( [FromBody] BookViewModel model )
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var book =  await _context.books.Where(x => x.BookID == model.BookID && x.IsActive).FirstOrDefaultAsync();
                if (book != null)
                {
                    book.Title = model.Title;
                    book.Author = model.Author;
                    book.ISBN = model.ISBN;
                    book.PublishedYear = Convert.ToDateTime(model.PublishedYear);
                    book.Genre = model.Genre;
                    book.Copies = model.Copies;
                    book.AvailableCopies = model.AvailableCopies;
                    book.UpdatedBy = User.Identity.Name;
                    book.UpdatedOn = DateTime.Now;

                    int changes = await _context.SaveChangesAsync();
                    if (changes > 0)
                    {
                        apiResult.Data = new object [0];
                        apiResult.IsSuccessfull = true;
                        apiResult.Message = "Updated Successfully";
                    }
                    else
                    {
                        apiResult.Data = new object [0];
                        apiResult.IsSuccessfull = false;
                        apiResult.Message = "Failed To Update";
                    }
                }
                else
                {
                    apiResult.Data = new object [0];
                    apiResult.IsSuccessfull = false;
                    apiResult.Message = "Book not found or is inactive.";
                }
            }
            catch (Exception ex)
            {
                apiResult.Data = new object [0];
                apiResult.IsSuccessfull = false;
                apiResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(apiResult);
        }

        [HttpGet("deletebook")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var activeBook = await _context.books.Where(x => x.BookID == bookId && x.IsActive).FirstOrDefaultAsync();
                if (activeBook != null)
                {
                    activeBook.IsActive = false;
                    int activeStatus = await _context.SaveChangesAsync();
                    if ( activeStatus > 0)
                    {
                        apiResult.Data = new object [0];
                        apiResult.IsSuccessfull = true;
                        apiResult.Message = "Deleted Successfully";
                    }
                    else
                    {
                        apiResult.Data = new object [0];
                        apiResult.IsSuccessfull = false;
                        apiResult.Message = "Failed to Delete";
                    }
                }
                else
                {
                    apiResult.Data = new object [0];
                    apiResult.IsSuccessfull = false;
                    apiResult.Message = "No Record found for the Book";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(apiResult);
        }

        [HttpPost("addbooks")]
        public async Task<IActionResult> AddBook([FromBody] BookViewModel model)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var existingBook = await _context.books.FirstOrDefaultAsync(x => x.ISBN == model.ISBN);
                if (existingBook != null)
                {
                    apiResult.IsSuccessfull = false;
                    apiResult.Message = "A book with the same ISBN already exists.";
                    apiResult.Data = new object [0];
                }
                else
                {
                    Book book = new Book
                    {
                        Title = model.Title,
                        Author = model.Author,
                        ISBN = model.ISBN,
                        PublishedYear = Convert.ToDateTime(model.PublishedYear),
                        Genre = model.Genre,
                        Copies = model.Copies,
                        AvailableCopies = model.AvailableCopies,
                        CreatedOn = DateTime.Now,
                        CreatedBy = User.Identity?.Name ?? "Unknown",
                        IsActive = true
                    };
                    _context.books.Add(book);
                    int changes = await _context.SaveChangesAsync();
                    if (changes > 0)
                    {
                        apiResult.Data = new object [0];
                        apiResult.IsSuccessfull = true;
                        apiResult.Message = "Book added successfully.";
                    }
                    else
                    {
                        apiResult.IsSuccessfull = false;
                        apiResult.Message = "Failed to add book.";
                        apiResult.Data = new object [0];
                    }
                }
            }
            catch (Exception ex)
            {
                apiResult.IsSuccessfull = false;
                apiResult.Message = $"An error occurred: {ex.Message}";
                apiResult.Data = new object [0];
            }

            return Ok(apiResult);

        }

        [HttpPost("viewfinedays")]
        public async Task<IActionResult> ViewFineDays ()
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var allfinesDates = await _context.fineDates.Where(x => x.IsActive).ToListAsync();
                if (allfinesDates.Count > 0)
                {
                    apiResult.Data = allfinesDates;
                    apiResult.IsSuccessfull = true;
                    apiResult.Message = "Fine Dates Found";
                }
                else
                {
                    apiResult.Data = new object [0];
                    apiResult.IsSuccessfull = false;
                    apiResult.Message = "Fine Dates Not Found";
                }
            }
            catch (Exception ex)
            {
                apiResult.IsSuccessfull = false;
                apiResult.Message = $"An error occurred: {ex.Message}";
            }

            return Ok(apiResult);
        }

        [HttpGet("getfinedatesbyid")]
        public async Task<IActionResult> RetrieveFineDateById ( int fineDateId )
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var fineDateDetails = await _context.fineDates
                    .FirstOrDefaultAsync(x => x.FineDateId == fineDateId && x.IsActive);

                if (fineDateDetails != null)
                {
                    apiResult.IsSuccessfull = true;
                    apiResult.Data = fineDateDetails;
                    apiResult.Message = "Record Found";
                }
                else
                {
                    apiResult.IsSuccessfull = false;
                    apiResult.Data = new object [0];
                    apiResult.Message = "Record Not Found";
                }
            }
            catch (Exception ex)
            {
                apiResult.IsSuccessfull = false;
                apiResult.Message = $"An error occurred: {ex.Message}";
            }

            return Ok(apiResult);
        }

        [HttpPost("updatefinedate")]
        public async Task<IActionResult> UpdateFineDateById ( [FromBody] FineDate  model )
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var fineDateDetails = await _context.fineDates.Where(x => x.FineDateId == model.FineDateId && x.IsActive).FirstOrDefaultAsync();
                if (fineDateDetails != null)
                {
                    fineDateDetails.FineDays = model.FineDays;
                    fineDateDetails.UpdatedBy = User.Identity.Name;
                    fineDateDetails.UpdatedOn = DateTime.Now;
                    int changes = await _context.SaveChangesAsync();
                    if (changes > 0)
                    {
                        apiResult.Data = new object [0];
                        apiResult.IsSuccessfull = true;
                        apiResult.Message = "Updated Successfully";
                    }
                    else
                    {
                        apiResult.Data = new object [0];
                        apiResult.IsSuccessfull = false;
                        apiResult.Message = "Failed To Update";
                    }
                }
                else
                {
                    apiResult.Data = new object [0];
                    apiResult.IsSuccessfull = false;
                    apiResult.Message = "Book not found or is inactive.";
                }
            }
            catch (Exception ex)
            {
                apiResult.Data = new object [0];
                apiResult.IsSuccessfull = false;
                apiResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(apiResult);
        }

        [HttpDelete("deletefinedate")]
        public async Task<IActionResult> DeleteFineDate ( int fineDateId )
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var activefineDate = await _context.fineDates.Where(x => x.FineDateId == fineDateId && x.IsActive).FirstOrDefaultAsync();
                if (activefineDate != null)
                {
                    activefineDate.IsActive = false;
                    int activeStatus = await _context.SaveChangesAsync();
                    if (activeStatus > 0)
                    {
                        apiResult.Data = new object [0];
                        apiResult.IsSuccessfull = true;
                        apiResult.Message = "Deleted Successfully";
                    }
                    else
                    {
                        apiResult.Data = new object [0];
                        apiResult.IsSuccessfull = false;
                        apiResult.Message = "Failed to Delete";
                    }
                }
                else
                {
                    apiResult.Data = new object [0];
                    apiResult.IsSuccessfull = false;
                    apiResult.Message = "No Record found for the Fine Date";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(apiResult);
        }

        [HttpPost("addfinedate")]
        public async Task<IActionResult> AddFineDate ( [FromBody] FineDate model )
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var existingBook = await _context.fineDates.FirstOrDefaultAsync(x => x.FineDays == model.FineDays);
                if (existingBook != null)
                {
                    apiResult.IsSuccessfull = false;
                    apiResult.Message = "Fine Date already exists.";
                    apiResult.Data = new object [0];
                }
                else
                {
                    FineDate finedate = new FineDate
                    {
                        FineDays = model.FineDays,
                        CreatedOn = DateTime.Now,
                        CreatedBy = User.Identity?.Name ?? "Unknown",
                        IsActive = true
                    };
                    _context.fineDates.Add(finedate);
                    int changes = await _context.SaveChangesAsync();
                    if (changes > 0)
                    {
                        apiResult.Data = new object [0];
                        apiResult.IsSuccessfull = true;
                        apiResult.Message = "Fine Date added successfully.";
                    }
                    else
                    {
                        apiResult.IsSuccessfull = false;
                        apiResult.Message = "Failed to add Fine Date.";
                        apiResult.Data = new object [0];
                    }
                }
            }
            catch (Exception ex)
            {
                apiResult.IsSuccessfull = false;
                apiResult.Message = $"An error occurred: {ex.Message}";
                apiResult.Data = new object [0];
            }

            return Ok(apiResult);

        }

    }   

}
