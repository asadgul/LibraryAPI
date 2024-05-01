using LibraryAPI.DbConnection;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        IDatabaseConnection _databaseConnection;
        public LibraryController(IDatabaseConnection _databaseConnection)
        {
            this._databaseConnection = _databaseConnection;
        }
        [HttpPost("CreateUser")]

        public IActionResult CreateUser(User user)
        {
            user.CreatedOn = DateTime.Now.ToString();
            user.UserType = UserType.USER;
            user.Active=true;
            if (_databaseConnection.CreateUser(user) < 0)
            {
                return Ok("Unable To Create user Please try again");
            }
           return Ok(user);
        }
        [HttpGet("Login")]
        public IActionResult Login(string email,string password) {

            if (_databaseConnection.AuthenticateUser(email, password,out User?user))
            {
                if (user != null)
                {
                    var createtoken = new Jwt("This is my test key and my name is asad Gul", "20");
                 var token=   createtoken.GenerateToken(user);
                    return Ok(token);
                }
            }
            return Ok("Invalid");
        }
        [HttpGet("GetAllBooks")]
        public IActionResult GetBooks()
        {
            var books=_databaseConnection.GetBooks();
            var booksToSend = books.Select(book => new
            {
                book.Id, book.Title, book.Category.Category,book.Category.SubCategory,
                book.Price,Available=!book.Ordered,
                book.Author
            }).ToList();
            return Ok(booksToSend);

        }
        [HttpPost("OrderBooks/{userId}/{bookId}")]
        public IActionResult OrderBook(int userId,int bookId)
        {
            var orderbook=_databaseConnection.OrderBook(userId,bookId);
            return Ok(orderbook);
        }

        [HttpGet("GetOrder/{userId}")]
        public IActionResult GetOrder(int userId)
        {
            var getorderbook = _databaseConnection.GetOrders(userId);
            return Ok(getorderbook);
        }
        [HttpGet("AllOrders")]
        public IActionResult AllOrders()
        {
            var getorderbook = _databaseConnection.AllOrders();
            return Ok(getorderbook);
        }
        [HttpGet("ReturnOrder/{userId}/{bookId}")]
        public IActionResult ReturnOrder(string userId,string bookId)
        {
            var returnbook = _databaseConnection.ReturnBook(int.Parse(userId),int.Parse(bookId));
            if (returnbook == 1)
            {
                return Ok("Book Returned");
            }
            return Ok("failed");
        }
        [HttpGet("GetAllUsers")]
        public IActionResult GetUsers()
        {
            var users = _databaseConnection.GetAlUsers();
            return Ok(users);

        }
        [HttpPost("InsertBook")]
        public IActionResult InsertBook(Book book)
        {
            _databaseConnection.AddBook(book);
            return Ok("Book Inserted");
        }
        [HttpDelete("DeleteBook/{id}")]
        public IActionResult DeleteBook(int id)
        {
            _databaseConnection.DeleteBook(id);
            return Ok("success");
        }

        [HttpPost("AddCategories")]
        public IActionResult AdCategories(BookCategory bookCategory)
        {
            _databaseConnection.AddCategories(bookCategory);
            return Ok("Category Added");
        }

    }
}
