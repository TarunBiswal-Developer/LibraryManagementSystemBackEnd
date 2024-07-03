namespace BackEnd.Model
{
    public class CreateStudentRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class PaginationParams
    {
        public int PageIndex { get; set; } = 1; // Default to first page
        public int PageSize { get; set; } = 10; // Default page size
    }


    public class CreateUsers
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string GuardianName { get; set; }
        public string GuardianPhone { get; set; }
    }

    public class BookViewModel
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public DateTime? PublishedYear { get; set; }
        public string Genre { get; set; }
        public int Copies { get; set; }
        public int AvailableCopies { get; set; }
    }

}
