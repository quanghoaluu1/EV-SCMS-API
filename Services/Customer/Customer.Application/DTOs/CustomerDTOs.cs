using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.DTOs
{
    public class CustomerDTOs
    {
        public class CustomerCreateDto
        {
            public Guid UserId { get; set; }
            public string? Address { get; set; }
        }

        public class CustomerUpdateDto
        {
            public string? Address { get; set; }
            public string? Status { get; set; }
        }

        public class CustomerResponseDto
        {
            public Guid Id { get; set; }
            public Guid UserId { get; set; }
            public string FullName { get; set; } 
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string? Address { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public class CustomerListDto
        {
            public Guid Id { get; set; }
            public Guid UserId { get; set; }
            public string? Address { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
        }
    }
}
