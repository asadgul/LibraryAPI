using LibraryAPI.Models;

namespace LibraryAPI.DbConnection
{
    public interface IDatabaseConnection
    {
        public int CreateUser(User user);
        public bool IsEmailAvailable(string Email);
        public void CreateConnection();
        public bool AuthenticateUser(string email,string password,out User?user);
        public IList<Book> GetBooks();
        public string OrderBook(int userId, int bookId);
        public IList<Order> GetOrders(int id);
        public IList<Order> AllOrders();
        public int ReturnBook(int userId,int BookId);
        public List<User> GetAlUsers();
        public void AddBook(Book book);
        public bool DeleteBook(int bookId);
        public bool AddCategories(BookCategory bookCategory);

    }
}
