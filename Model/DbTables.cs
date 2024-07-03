using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Model
{
    [Table("Reservations")]
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }
        public int MemberID { get; set; }
        public int BookID { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("Members")]
    public class Member
    {
        [Key]
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("Fines")]
    public class Fine
    {
        [Key]
        public int FineID { get; set; }
        public int MemberID { get; set; }
        public int FineDateId { get; set; }
        public decimal Amount { get; set; }
        public DateTime FineDate { get; set; }
        public DateTime? PaidDate { get; set; } 
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("FineDate")]
    public class FineDate
    {
        [Key]
        public int FineDateId { get; set; }
        public int FineDays { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }


    [Table("Borrowing")]
    public class Borrow
    {
        [Key]
        public int BorrowID { get; set; }
        public int MemberID { get; set; }
        public int BookID { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal? FineAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("Books")]
    public class Book
    {
        [Key]
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedYear { get; set; }
        public string Genre { get; set; }
        public int Copies { get; set; }
        public int AvailableCopies { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
    }

}
