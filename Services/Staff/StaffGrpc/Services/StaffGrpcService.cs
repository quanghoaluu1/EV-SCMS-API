using Application.DTOs;
using Application.IService;
using Grpc.Core;

namespace StaffGrpc.Services;

public class StaffGrpcService : Staff.StaffBase
{
    private readonly IStaffService _staffService;
    public StaffGrpcService(IStaffService staffService)
    {
        _staffService = staffService;
    }

    public override async Task<CreateStaffReply> CreateStaff(CreateStaffRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                return new CreateStaffReply { Success = false, Message = "Invalid UserId" };
            }

            if (!DateOnly.TryParseExact(request.HireDate, "yyyy-MM-dd", out var hireDate))
            {
                return new CreateStaffReply { Success = false, Message = "Invalid HireDate format (yyyy-MM-dd)" };
            }

            var dto = new CreateStaffDto
            {
                UserId = userId,
                HireDate = hireDate
            };

            var staff = await _staffService.CreateStaffAsync(dto);

            return new CreateStaffReply
            {
                Success = true,
                Message = "Staff created",
                StaffId = staff.StaffId.ToString(),
                UserId = staff.UserId.ToString()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("Staff Grpc exception: ", ex);
            return new CreateStaffReply { Success = false, Message = ex.Message };
        }
    }

    public override async Task<GetStaffByIdReply> GetStaffById(GetStaffByIdRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.StaffId, out var staffId))
            {
                return new GetStaffByIdReply { Success = false, Message = "Invalid StaffId format" };
            }

            var result = await _staffService.GetStaffByIdAsync(staffId);

            if (result == null || !result.IsSuccess)
            {
                return new GetStaffByIdReply
                {
                    Success = false,
                    Message = result?.Message ?? "Staff not found"
                };
            }

            return new GetStaffByIdReply
            {
                Success = true,
                Message = "Staff found",
                Data = new StaffInfo
                {
                    StaffId = result.Data.StaffId.ToString(),
                    UserId = result.Data.UserId.ToString(),
                    FullName = result.Data.FullName,
                    Email = result.Data.Email,
                    PhoneNumber = result.Data.PhoneNumber ?? "",
                    Role = result.Data.Role,
                    HireDate = "" // You might want to add HireDate to StaffDto if needed
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("GetStaffById Grpc exception: ", ex);
            return new GetStaffByIdReply { Success = false, Message = ex.Message };
        }
    }
}