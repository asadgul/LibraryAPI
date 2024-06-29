using LibraryAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;

namespace LibraryAPI.DbConnection
{
    public class DatabaseConnection : IDatabaseConnection
    {
        public bool AuthenticateUser(string email, string password, out User? user)
        {
            var result = false;
            user = null;
            string ConnectionString = @"data source=DESKTOP-RRDT0KB; database=LibraryDatabase;TrustServerCertificate=true; integrated security=true";
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
               
                string sql = $"Select * from Users Where Email='{email}' and Password='{password}'";
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                sqlConnection.Open();
                SqlDataReader dataReader = sqlCommand.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        //                        user = null;
                        user = new User()
                        {
                            
                            Id = dataReader.GetInt32(0),
                            FirstName = dataReader.GetString(1),
                            LastName = dataReader.GetString(2),
                            Email = dataReader.GetString(3),
                            Mobile = dataReader.GetString(4),
                            Password = dataReader.GetString(5),
                            Blocked = dataReader.GetBoolean(6),
                            Active = dataReader.GetBoolean(7),
                            CreatedOn = dataReader.GetSqlDateTime(8).Value.ToString(),
                         
                        };
                        if (dataReader.GetString(9).ToString().Equals(UserType.USER.ToString()))
                        {
                            user.UserType = UserType.USER;
                        }
                        else
                        {
                            user.UserType = UserType.ADMIN;
                        }
                    }
                    result = true;
                }
                else
                {
                    user = null;
                }
            }
           return result;


        }

        public void CreateConnection()
        {
            string ConnectionString = @"data source=DESKTOP-RRDT0KB; database=LibraryDatabase; integrated security=true";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch(Exception e) {
                
                }

            }
        }

        public int CreateUser(User user)
        {
            int x = -1;
            string ConnectionString = @"data source=DESKTOP-RRDT0KB; database=LibraryDatabase;TrustServerCertificate=true; integrated security=true";
            try
            {
                string sqlQuery = $"insert into Users(FirstName,LastName,Email,Mobile,Password,Blocked,Active,CreatedOn,UserType) Values('{user.FirstName}','{user.LastName}','{user.Email}','{user.Mobile}','{user.Password}','{user.Blocked}','{user.Active}','{user.CreatedOn}','{user.UserType}')";
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConnection);
                sqlConnection.Open();
               return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {

            }
            return x;

        }

        public IList<Book> GetBooks()
        {
            
            string ConnectionString = @"data source=DESKTOP-RRDT0KB;MultipleActiveResultSets=True; database=LibraryDatabase;TrustServerCertificate=true; integrated security=true";
            IList<Book> books = new List<Book>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    string sql = $"select * from Books";
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                    sqlConnection.Open();
                    SqlDataReader dataReader = sqlCommand.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            books.Add(new Book()
                            {
                                Id = dataReader.GetInt32(0),
                                Title = dataReader.GetString(1),
                                Author = dataReader.GetString(2),
                                Price = dataReader.GetDouble(3),
                                Ordered = dataReader.GetBoolean(4),
                                CategoryId = dataReader.GetInt32(5),
                            });                        
                        }
                    }
                   sqlConnection.Close();
                    if (books?.Count > 0)
                    {
  //                      sqlConnection.Open();
                        foreach (var book in books)
                        {
                            string query = $"select * from BookCategories where Id='{book.CategoryId}'";
                            sqlCommand = new SqlCommand(query, sqlConnection);
                            if (sqlConnection.State==ConnectionState.Closed)
                            {
                                sqlConnection.Open();
                            }
                            dataReader = sqlCommand.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    book.Category.Id = dataReader.GetInt32(0);
                                    book.Category.Category = dataReader.GetString(1);
                                    book.Category.SubCategory = dataReader.GetString(2);
                                }
                            }

                        }

                    }
                    sqlConnection.Close();

                }
                
            }
            catch(Exception e)
            {

            }
            return books.ToList();
            
        }

        
        public IList<Order> GetOrders(int id)
        {
            IList<Order> orders = new List<Order>();

            try
            {
                string ConnectionString = @"data source=DESKTOP-RRDT0KB;MultipleActiveResultSets=True; database=LibraryDatabase;TrustServerCertificate=true; integrated security=true";
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    string query = $"select o.Id, u.Id as UserId, CONCAT(u.FirstName, ' ', u.LastName) as Name,o.BookId as BookId, b.Title as BookName,o.OrderedOn as OrderDate, o.Returned as Returned from Users u LEFT JOIN Orders o ON u.Id=o.UserId LEFT JOIN Books b ON o.BookId=b.Id where o.UserId='{id}'";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            orders.Add(new Order()
                            {
                                Id = sqlDataReader.GetInt32(0),
                                UserId = sqlDataReader.GetInt32(1),
                                Name = sqlDataReader.GetString(2),
                                BookId = sqlDataReader.GetInt32(3),
                                BookName = sqlDataReader.GetString(4),
                                OrderDate = sqlDataReader.GetDateTime(5),
                                Returned = sqlDataReader.GetBoolean(6)

                            });
                        }
                    }
                }
            }
            catch(Exception e)
            {

            }
            return orders.ToList();

        }

        public IList<Order> AllOrders()
        {
            IList<Order> orders = new List<Order>();
            try
            {
                string ConnectionString = @"data source=DESKTOP-RRDT0KB;MultipleActiveResultSets=True; database=LibraryDatabase;TrustServerCertificate=true; integrated security=true";
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    string query = $"select o.Id, u.Id as UserId, CONCAT(u.FirstName, ' ', u.LastName) as Name,o.BookId as BookId, b.Title as BookName,o.OrderedOn as OrderDate, o.Returned as Returned from Users u LEFT JOIN Orders o ON u.Id=o.UserId LEFT JOIN Books b ON o.BookId=b.Id";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            orders.Add(new Order()
                            {
                                Id = sqlDataReader.GetInt32(0),
                                UserId = sqlDataReader.GetInt32(1),
                                Name = sqlDataReader.GetString(2),
                                BookId = sqlDataReader.GetInt32(3),
                                BookName = sqlDataReader.GetString(4),
                                OrderDate = sqlDataReader.GetDateTime(5),
                                Returned = sqlDataReader.GetBoolean(6)
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return orders.ToList();

        }

        public bool IsEmailAvailable(string Email)
        {
            bool result=false;
            string ConnectionString = @"data source=DESKTOP-RRDT0KB; database=LibraryDatabase; integrated security=true";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter($"select count(*) from Users where Email={Email}", connection);
                    DataTable dataTable = new DataTable();
                    sqlDataAdapter.Fill(dataTable);
                    if(dataTable.Rows.Count > 0 ) { 
                    
                        return true;
                    }

                }
                catch (Exception e)
                {

                }
            }
            return result;
        }

        public string OrderBook(int userId, int bookId)
        {
            string ConnectionString = @"data source=DESKTOP-RRDT0KB;MultipleActiveResultSets=True; database=LibraryDatabase;TrustServerCertificate=true; integrated security=true";
            using (SqlConnection sqlConnection=new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                string insertOrder = $"insert into Orders(UserId,BookId,OrderedOn,Returned) values('{userId}','{bookId}','{DateTime.Now.ToString()}','0')";
                SqlCommand sqlCommand=new SqlCommand(insertOrder,sqlConnection);
                int insertBook = sqlCommand.ExecuteNonQuery();
                if (insertBook > 0)
                {
                    string updateBook = $"update Books set Ordered='1' where Id='{bookId}'";
                    sqlCommand = new SqlCommand(updateBook, sqlConnection);
                    int update=sqlCommand.ExecuteNonQuery();
                    if (update > 0)
                    {
                        return "Success";
                    }
                }
            }
            return "";

        }

        public int ReturnBook(int userId, int BookId)
        {
            string ConnectionString = @"data source=DESKTOP-RRDT0KB;MultipleActiveResultSets=True; database=LibraryDatabase;TrustServerCertificate=true; integrated security=true";
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string query = $"update Books set Ordered='1' where Id='{BookId}'";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                int update= sqlCommand.ExecuteNonQuery();
                if(update > 0) {
                    query = $"update Orders set Returned='1' where UserId={userId} and BookId={BookId}";
                    sqlCommand = new SqlCommand(query, sqlConnection);
                    update =sqlCommand.ExecuteNonQuery();
                    if( update > 0 )
                    {
                        return 1;
                    }
                }
            }
            return -1;


        }

        public List<User> GetAlUsers()
        {
            List<User> users = new List<User>() ;
            try
            {
              
                string ConnectionString = @"data source=DESKTOP-RRDT0KB;MultipleActiveResultSets=True; database=LibraryDatabase;TrustServerCertificate=true; integrated security=true";
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    string query = $"select * from users";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            users.Add(new User()
                            {
                                FirstName = sqlDataReader.GetString(1),
                                LastName = sqlDataReader.GetString(2),
                                Email = sqlDataReader.GetString(3),
                                Mobile = sqlDataReader.GetString(4),
                                Blocked = sqlDataReader.GetBoolean(6),
                                Active = sqlDataReader.GetBoolean(7),
                                CreatedOn = sqlDataReader.GetDateTime(8).ToString()
                            });

                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return users.ToList();

        }

        public void AddBook(Book book)
        {
            string ConnectionString = @"data source=DESKTOP-RRDT0KB;MultipleActiveResultSets=True; database=LibraryDatabase;TrustServerCertificate=true; integrated security=true";
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string query = $"select Id from BookCategories where Category='{book.Category.Category}' and SubCategory='{book.Category.SubCategory}'";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        string insert = $"Insert into Books(Title,Author,Price,Ordered,CategoryId) Values('{book.Title}','{book.Author}','{book.Price}','{book.Ordered}','{sqlDataReader.GetInt32(0)}')";
                        sqlCommand = new SqlCommand(insert, sqlConnection);
                        int insertdata = sqlCommand.ExecuteNonQuery();
                        if (insertdata > 0)
                        {
                            return;
                        }
                    }
                }

            }
//            return ;

        }

        public bool DeleteBook(int bookId)
        {
            string ConnectionString = @"data source=DESKTOP-RRDT0KB;MultipleActiveResultSets=True; database=LibraryDatabase;TrustServerCertificate=true; integrated security=true";
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string query = $"Delete Books where Id='{bookId}'";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery ();
                return true;
            }
        }

        public bool AddCategories(BookCategory bookCategory)
        {
            string ConnectionString = @"data source=DESKTOP-RRDT0KB;MultipleActiveResultSets=True; database=LibraryDatabase;TrustServerCertificate=true; integrated security=true";
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string query = $"Insert into BookCategories(Category,SubCategory) values('{bookCategory.Category}','{bookCategory.SubCategory}')";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
             int insert= sqlCommand.ExecuteNonQuery();
                if (insert > 0)
                {
                    return true;
                }
            }
            return false;
        }
        
    }
}
