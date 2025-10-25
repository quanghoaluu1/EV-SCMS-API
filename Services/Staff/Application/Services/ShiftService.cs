using Application.DTOs;
using Application.IService;
using Domain.Entities;
using Infrastructure.Interfaces;
using Mapster;

namespace Application;

public class ShiftService : IShiftService
{
    private readonly IShiftRepository _shiftRepository;

    public ShiftService(IShiftRepository shiftRepository)
    {
        _shiftRepository = shiftRepository;
    }

    public async Task<ShiftDto?> GetByIdAsync(Guid id)
    {
        var shift = await _shiftRepository.GetByIdAsync(id);
        if (shift == null) return null;
        return shift.Adapt<ShiftDto>();
    }

    public async Task<IEnumerable<ShiftDto>> GetAllAsync()
    {
        var items = await _shiftRepository.GetAllAsync();
        return items.Adapt<IEnumerable<ShiftDto>>();
    }

    public async Task<ShiftDto> CreateAsync(CreateShiftDto dto)
    {
        if (dto.EndTime <= dto.StartTime)
        {
            throw new ArgumentException("EndTime must be after StartTime");
        }
        var shift = new Shift
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
        };
        await _shiftRepository.AddAsync(shift);
        return shift.Adapt<ShiftDto>();
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateShiftDto dto)
    {
        var shift = await _shiftRepository.GetByIdAsync(id);
        if (shift == null) return false;
        if (dto.EndTime <= dto.StartTime)
        {
            throw new ArgumentException("EndTime must be after StartTime");
        }
        shift.Name = dto.Name;
        shift.StartTime = dto.StartTime;
        shift.EndTime = dto.EndTime;
        await _shiftRepository.UpdateAsync(shift);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _shiftRepository.ExistsAsync(id)) return false;
        await _shiftRepository.DeleteAsync(id);
        return true;
    }
}


