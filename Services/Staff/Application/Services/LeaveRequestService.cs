using Application.DTOs;
using Application.IService;
using Domain.Entities;
using Domain.Enum;
using GrpcContract;
using Infrastructure.Interfaces;
using Infrastructure.IRepository;
using Mapster;

namespace Application;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _leaveRepository;
    private readonly IAuthGrpcGateway _authGrpcGateway;
    private readonly IStaffRepository _staffRepository;

    public LeaveRequestService(ILeaveRequestRepository leaveRepository, IAuthGrpcGateway authGrpcGateway, IStaffRepository staffRepository)
    {
        _leaveRepository = leaveRepository;
        _authGrpcGateway = authGrpcGateway;
        _staffRepository = staffRepository;
    }

    public async Task<ResultDto<LeaveRequestDto>> GetByIdAsync(Guid id)
    {
        var lr = await _leaveRepository.GetByIdAsync(id);
        if (lr == null) return null;
        var dto = lr.Adapt<LeaveRequestDto>();
        var userReply = await _authGrpcGateway.GetUserById(lr.Staff.UserId);
        if (!userReply.IsSuccess)
        {
            return new ResultDto<LeaveRequestDto>()
            {
                IsSuccess = false,
                Message = "Failed to retrieve user information through gRPC",
                Data = null,
            }; 
        }

        var staffdto = new StaffDto()
        {
            StaffId = lr.StaffId,
            UserId = lr.Staff.UserId,
            FullName = userReply.Data.FullName,
            Email = userReply.Data.Email,
            PhoneNumber = string.Empty,
            Role = userReply.Data.Role
        };
        dto.Staff = staffdto;
        return new ResultDto<LeaveRequestDto>()
        {
            IsSuccess = true,
            Message = "Leave request retrieved successfully",
            Data = dto,
        };
    }

    public async Task<ResultDto<IEnumerable<LeaveRequestDto>>> GetAllAsync()
    {
        var list = await _leaveRepository.GetAllAsync();

        if (list == null || !list.Any())
        {
            return new ResultDto<IEnumerable<LeaveRequestDto>>
            {
                IsSuccess = true,
                Message = "No leave requests found.",
                Data = Enumerable.Empty<LeaveRequestDto>()
            };
        }

        var dtos = list.Adapt<List<LeaveRequestDto>>();

        foreach (var dto in dtos)
        {
            var leave = list.FirstOrDefault(x => x.Id == dto.Id);
            if (leave?.Staff == null) continue;

            var userReply = await _authGrpcGateway.GetUserById(leave.Staff.UserId);

            if (!userReply.IsSuccess)
            {
                dto.Staff = new StaffDto
                {
                    StaffId = leave.StaffId,
                    UserId = leave.Staff.UserId,
                    FullName = "(Unknown User)",
                    Email = string.Empty,
                    PhoneNumber = string.Empty,
                    Role = -1
                };
                continue;
            }

            dto.Staff = new StaffDto
            {
                StaffId = leave.StaffId,
                UserId = leave.Staff.UserId,
                FullName = userReply.Data.FullName,
                Email = userReply.Data.Email,
                Role = userReply.Data.Role
            };
        }

        return new ResultDto<IEnumerable<LeaveRequestDto>>
        {
            IsSuccess = true,
            Message = "Leave requests retrieved successfully",
            Data = dtos
        };
    }

    public async Task<ResultDto<IEnumerable<LeaveRequestDto>>> GetByStaffAsync(Guid staffId)
    {
        var list = await _leaveRepository.GetByStaffIdAsync(staffId);
        if (list == null || !list.Any())
        {
            return new ResultDto<IEnumerable<LeaveRequestDto>>
            {
                IsSuccess = true,
                Message = "No leave requests found.",
                Data = Enumerable.Empty<LeaveRequestDto>()
            };
        }

        var dtos = list.Adapt<List<LeaveRequestDto>>();

        foreach (var dto in dtos)
        {
            var leave = list.FirstOrDefault(x => x.Id == dto.Id);
            if (leave?.Staff == null) continue;

            var userReply = await _authGrpcGateway.GetUserById(leave.Staff.UserId);

            if (!userReply.IsSuccess)
            {
                dto.Staff = new StaffDto
                {
                    StaffId = leave.StaffId,
                    UserId = leave.Staff.UserId,
                    FullName = "(Unknown User)",
                    Email = string.Empty,
                    PhoneNumber = string.Empty,
                    Role = -1
                };
                continue;
            }

            dto.Staff = new StaffDto
            {
                StaffId = leave.StaffId,
                UserId = leave.Staff.UserId,
                FullName = userReply.Data.FullName,
                Email = userReply.Data.Email,
                Role = userReply.Data.Role
            };
        }

        return new ResultDto<IEnumerable<LeaveRequestDto>>
        {
            IsSuccess = true,
            Message = "Leave requests retrieved successfully",
            Data = dtos
        };
    }

    public async Task<ResultDto<IEnumerable<LeaveRequestDto>>> GetByStatusAsync(LeaveRequestStatus status)
    {
        var list = await _leaveRepository.GetByStatusAsync(status);
        if (list == null || !list.Any())
        {
            return new ResultDto<IEnumerable<LeaveRequestDto>>
            {
                IsSuccess = true,
                Message = "No leave requests found.",
                Data = Enumerable.Empty<LeaveRequestDto>()
            };
        }

        var dtos = list.Adapt<List<LeaveRequestDto>>();

        foreach (var dto in dtos)
        {
            var leave = list.FirstOrDefault(x => x.Id == dto.Id);
            if (leave?.Staff == null) continue;

            var userReply = await _authGrpcGateway.GetUserById(leave.Staff.UserId);

            if (!userReply.IsSuccess)
            {
                dto.Staff = new StaffDto
                {
                    StaffId = leave.StaffId,
                    UserId = leave.Staff.UserId,
                    FullName = "(Unknown User)",
                    Email = string.Empty,
                    PhoneNumber = string.Empty,
                    Role = -1
                };
                continue;
            }

            dto.Staff = new StaffDto
            {
                StaffId = leave.StaffId,
                UserId = leave.Staff.UserId,
                FullName = userReply.Data.FullName,
                Email = userReply.Data.Email,
                Role = userReply.Data.Role
            };
        }

        return new ResultDto<IEnumerable<LeaveRequestDto>>
        {
            IsSuccess = true,
            Message = "Leave requests retrieved successfully",
            Data = dtos
        };
    }

    public async Task<ResultDto<IEnumerable<LeaveRequestDto>>> GetPendingAsync()
    {
        var list = await _leaveRepository.GetPendingRequestsAsync();
        if (list == null || !list.Any())
        {
            return new ResultDto<IEnumerable<LeaveRequestDto>>
            {
                IsSuccess = true,
                Message = "No leave requests found.",
                Data = Enumerable.Empty<LeaveRequestDto>()
            };
        }

        var dtos = list.Adapt<List<LeaveRequestDto>>();

        foreach (var dto in dtos)
        {
            var leave = list.FirstOrDefault(x => x.Id == dto.Id);
            if (leave?.Staff == null) continue;

            var userReply = await _authGrpcGateway.GetUserById(leave.Staff.UserId);

            if (!userReply.IsSuccess)
            {
                dto.Staff = new StaffDto
                {
                    StaffId = leave.StaffId,
                    UserId = leave.Staff.UserId,
                    FullName = "(Unknown User)",
                    Email = string.Empty,
                    PhoneNumber = string.Empty,
                    Role = -1
                };
                continue;
            }

            dto.Staff = new StaffDto
            {
                StaffId = leave.StaffId,
                UserId = leave.Staff.UserId,
                FullName = userReply.Data.FullName,
                Email = userReply.Data.Email,
                Role = userReply.Data.Role
            };
        }

        return new ResultDto<IEnumerable<LeaveRequestDto>>
        {
            IsSuccess = true,
            Message = "Leave requests retrieved successfully",
            Data = dtos
        };
    }

    public async Task<ResultDto<LeaveRequestDto>> CreateAsync(CreateLeaveRequestDto dto)
    {
        if (dto.EndDate < dto.StartDate)
        {
            throw new ArgumentException("EndDate must be on or after StartDate");
        }
        if (dto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
        {
            throw new ArgumentException("StartDate cannot be in the past");
        }
        if (await _leaveRepository.HasOverlappingAsync(dto.StaffId, dto.StartDate, dto.EndDate))
        {
            throw new InvalidOperationException("Leave request overlaps with an existing request");
        }
        var entity = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            StaffId = dto.StaffId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        await _leaveRepository.AddAsync(entity);
        var lrdto = entity.Adapt<LeaveRequestDto>();
        var leave = await _leaveRepository.GetByIdAsync(entity.Id);
        var userReply = await _authGrpcGateway.GetUserById(leave.Staff.UserId);

        if (!userReply.IsSuccess)
        {
            lrdto.Staff = new StaffDto
            {
                StaffId = leave.StaffId,
                UserId = leave.Staff.UserId,
                FullName = "(Unknown User)",
                Email = string.Empty,
                PhoneNumber = string.Empty,
                Role = -1
            };
        }

        lrdto.Staff = new StaffDto
        {
            StaffId = leave.StaffId,
            UserId = leave.Staff.UserId,
            FullName = userReply.Data.FullName,
            Email = userReply.Data.Email,
            Role = userReply.Data.Role
        };
        return new ResultDto<LeaveRequestDto>()
        {
            IsSuccess = true,
            Message = "Leave request created successfully",
            Data = lrdto,
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateLeaveRequestDto dto)
    {
        var entity = await _leaveRepository.GetByIdAsync(id);
        if (entity == null) return false;
        var newStart = dto.StartDate ?? entity.StartDate;
        var newEnd = dto.EndDate ?? entity.EndDate;
        if (newEnd < newStart)
        {
            throw new ArgumentException("EndDate must be on or after StartDate");
        }

        if (newStart < DateOnly.FromDateTime(DateTime.UtcNow.Date))
        {
            throw new ArgumentException("StartDate cannot be in the past");
        }

        if (dto.StartDate.HasValue) entity.StartDate = dto.StartDate.Value;
        if (dto.EndDate.HasValue) entity.EndDate = dto.EndDate.Value;
        if (dto.Reason != null) entity.Reason = dto.Reason;
        if (dto.Status.HasValue) entity.Status = dto.Status.Value;
        entity.UpdatedAt = DateTime.UtcNow;
        await _leaveRepository.UpdateAsync(entity);
        return true;
    }

    private async Task<StaffDto?> MapStaffAsync(Guid staffId)
    {
        var staff = await _staffRepository.GetByIdAsync(staffId);
        if (staff == null) return null;
        var user = await _authGrpcGateway.GetUserById(staff.UserId);
        if (user == null || !user.IsSuccess) return null;
        return new StaffDto
        {
            StaffId = staffId,
            UserId = staff.UserId,
            FullName = user.Data.FullName,
            Email = user.Data.Email,
            PhoneNumber = user.Data.PhoneNumber,
            Role = user.Data.Role
        };
    }

    private async Task EnrichStaffAsync(List<LeaveRequestDto> items)
    {
        foreach (var dto in items)
        {
            dto.Staff = await MapStaffAsync(dto.StaffId);
        }
    }
    
    public async Task<bool> ApproveAsync(Guid id, ApproveLeaveRequestDto dto)
    {
        var entity = await _leaveRepository.GetByIdAsync(id);
        if (entity == null) return false;
        entity.Status = dto.IsApproved ? LeaveRequestStatus.Approved : LeaveRequestStatus.Rejected;
        entity.ApprovedBy = dto.ApprovedBy;
        entity.ApprovedAt = DateTime.UtcNow;
        entity.ApprovalNotes = dto.ApprovalNotes;
        entity.UpdatedAt = DateTime.UtcNow;
        await _leaveRepository.UpdateAsync(entity);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _leaveRepository.ExistsAsync(id)) return false;
        await _leaveRepository.DeleteAsync(id);
        return true;
    }
}


