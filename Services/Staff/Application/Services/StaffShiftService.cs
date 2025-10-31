using Application.DTOs;
using Application.IService;
using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.IRepository;
using Mapster;
using GrpcContract;

namespace Application;

public class StaffShiftService : IStaffShiftService
{
    private readonly IStaffShiftRepository _staffShiftRepository;
    private readonly IAuthGrpcGateway _authGrpcGateway;
    private readonly IStaffRepository _staffRepository;
    private readonly ILeaveRequestRepository _leaveRepository;

    public StaffShiftService(IStaffShiftRepository staffShiftRepository, IAuthGrpcGateway authGrpcGateway, IStaffRepository staffRepository, ILeaveRequestRepository leaveRepository)
    {
        _staffShiftRepository = staffShiftRepository;
        _authGrpcGateway = authGrpcGateway;
        _staffRepository = staffRepository;
        _leaveRepository = leaveRepository;
    }

    public async Task<ResultDto<StaffShiftDto>> GetByIdAsync(Guid id)
    {
        var item = await _staffShiftRepository.GetByIdAsync(id);
        if (item == null)
        {
            return new ResultDto<StaffShiftDto> { IsSuccess = false, Message = "StaffShift not found" };
        }
        var dto = item.Adapt<StaffShiftDto>();
        dto.Staff = await MapStaffAsync(item.StaffId);
        return new ResultDto<StaffShiftDto> { IsSuccess = true, Message = "OK", Data = dto };
    }

    public async Task<ResultDto<IEnumerable<StaffShiftDto>>> GetAllAsync()
    {
        var items = await _staffShiftRepository.GetAllAsync();
        var list = items.Adapt<List<StaffShiftDto>>();
        await EnrichStaffAsync(list);
        return new ResultDto<IEnumerable<StaffShiftDto>> { IsSuccess = true, Message = "OK", Data = list };
    }

    public async Task<ResultDto<IEnumerable<StaffShiftDto>>> GetByStaffAsync(Guid staffId)
    {
        var items = await _staffShiftRepository.GetByStaffIdAsync(staffId);
        var list = items.Adapt<List<StaffShiftDto>>();
        await EnrichStaffAsync(list);
        return new ResultDto<IEnumerable<StaffShiftDto>> { IsSuccess = true, Message = "OK", Data = list };
    }

    public async Task<ResultDto<IEnumerable<StaffShiftDto>>> GetByDateRangeAsync(DateOnly start, DateOnly end)
    {
        var items = await _staffShiftRepository.GetByDateRangeAsync(start, end);
        var list = items.Adapt<List<StaffShiftDto>>();
        await EnrichStaffAsync(list);
        return new ResultDto<IEnumerable<StaffShiftDto>> { IsSuccess = true, Message = "OK", Data = list };
    }

    public async Task<ResultDto<IEnumerable<StaffShiftDto>>> GetByStaffAndDateRangeAsync(Guid staffId, DateOnly start, DateOnly end)
    {
        var items = await _staffShiftRepository.GetByStaffIdAndDateRangeAsync(staffId, start, end);
        var list = items.Adapt<List<StaffShiftDto>>();
        await EnrichStaffAsync(list);
        return new ResultDto<IEnumerable<StaffShiftDto>> { IsSuccess = true, Message = "OK", Data = list };
    }

    public async Task<ResultDto<StaffShiftDto>> CreateAsync(CreateStaffShiftDto dto)
    {
        if (dto.WorkDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
        {
            throw new ArgumentException("WorkDate cannot be in the past");
        }
        var existsSameDay = await _staffShiftRepository.ExistsForStaffOnDateAsync(dto.StaffId, dto.WorkDate);
        if (existsSameDay)
        {
            throw new InvalidOperationException("Staff already has a shift on this date");
        }
        var entity = new StaffShift
        {
            Id = Guid.NewGuid(),
            StaffId = dto.StaffId,
            ShiftId = dto.ShiftId,
            WordDate = dto.WorkDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        await _staffShiftRepository.AddAsync(entity);
        var ssdto = entity.Adapt<StaffShiftDto>();
        ssdto.Staff = await MapStaffAsync(entity.StaffId);
        return new ResultDto<StaffShiftDto> { IsSuccess = true, Message = "Created", Data = ssdto };
    }

    public async Task<ResultDto<bool>> UpdateAsync(Guid id, UpdateStaffShiftDto dto)
    {
        var entity = await _staffShiftRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return new ResultDto<bool> { IsSuccess = false, Message = "StaffShift not found" };
        }
        if (dto.WorkDate.HasValue && dto.WorkDate.Value < DateOnly.FromDateTime(DateTime.UtcNow.Date))
        {
            throw new ArgumentException("WorkDate cannot be in the past");
        }

        if (dto.StaffId.HasValue) entity.StaffId = dto.StaffId.Value;
        if (dto.ShiftId.HasValue) entity.ShiftId = dto.ShiftId.Value;
        if (dto.WorkDate.HasValue) entity.WordDate = dto.WorkDate.Value;
        if (dto.Status.HasValue) entity.Status = dto.Status.Value;
        entity.UpdatedAt = DateTime.UtcNow;
        await _staffShiftRepository.UpdateAsync(entity);
        return new ResultDto<bool> { IsSuccess = true, Message = "Updated", Data = true };
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
            PhoneNumber = string.Empty,
            Role = user.Data.Role
        };
    }

    private async Task EnrichStaffAsync(List<StaffShiftDto> items)
    {
        foreach (var dto in items)
        {
            dto.Staff = await MapStaffAsync(dto.StaffId);
        }
    }

    public async Task<ResultDto<bool>> DeleteAsync(Guid id)
    {
        if (!await _staffShiftRepository.ExistsAsync(id))
        {
            return new ResultDto<bool> { IsSuccess = false, Message = "StaffShift not found" };
        }
        await _staffShiftRepository.DeleteAsync(id);
        return new ResultDto<bool> { IsSuccess = true, Message = "Deleted", Data = true };
    }

    public async Task<ResultDto<IEnumerable<StaffDto>>> GetAvailableStaffAsync(DateOnly start, DateOnly end)
    {
        if (end < start)
        {
            return new ResultDto<IEnumerable<StaffDto>> { IsSuccess = false, Message = "End must be on or after start" };
        }

        var activeStaff = await _staffRepository.GetActiveAsync();
        var blockedStaffIds = await _leaveRepository.GetStaffIdsWithApprovedLeaveInRangeAsync(start, end);
        var available = activeStaff.Where(s => !blockedStaffIds.Contains(s.Id)).ToList();

        var result = new List<StaffDto>();
        foreach (var staff in available)
        {
            var user = await _authGrpcGateway.GetUserById(staff.UserId);
            if (user == null || !user.IsSuccess)
            {
                result.Add(new StaffDto
                {
                    StaffId = staff.Id,
                    UserId = staff.UserId,
                    FullName = "(Unknown User)",
                    Email = string.Empty,
                    PhoneNumber = string.Empty,
                    Role = -1
                });
                continue;
            }
            result.Add(new StaffDto
            {
                StaffId = staff.Id,
                UserId = staff.UserId,
                FullName = user.Data.FullName,
                Email = user.Data.Email,
                PhoneNumber = string.Empty,
                Role = user.Data.Role
            });
        }

        return new ResultDto<IEnumerable<StaffDto>>
        {
            IsSuccess = true,
            Message = "Available staff retrieved successfully",
            Data = result
        };
    }

    // Mapster handles mapping
}


