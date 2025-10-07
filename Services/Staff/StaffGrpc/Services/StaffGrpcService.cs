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
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            return new CreateStaffReply { Success = false, Message = ex.Message };
        }
    }
}