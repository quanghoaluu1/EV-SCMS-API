using Application.DTOs;
using Application.IService;
using Domain.Entities;
using Infrastructure.IRepository;

namespace Application;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;

        public StaffService(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public async Task<StaffDto?> GetStaffByIdAsync(Guid id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null) return null;

            return new StaffDto
            {
                StaffId = staff.Id,
                UserId = staff.UserId,
            };
        }

        public async Task<IEnumerable<StaffDto>> GetAllStaffAsync()
        {
            var staffs = await _staffRepository.GetAllAsync();
            return staffs.Select(staff => new StaffDto
            {
                StaffId = staff.Id,
                UserId = staff.UserId,
            });
        }

        public async Task<StaffDto> CreateStaffAsync(CreateStaffDto dto)
        {
            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                HireDate = dto.HireDate,
            };

            await _staffRepository.AddAsync(staff);

            return new StaffDto
            {
                StaffId = staff.Id,
                UserId = staff.UserId,
            };
        }

        public async Task<bool> UpdateStaffAsync(Guid id, UpdateStaffDto dto)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null) return false;


            await _staffRepository.UpdateAsync(staff);
            return true;
        }

        public async Task<bool> DeleteStaffAsync(Guid id)
        {
            if (!await _staffRepository.ExistsAsync(id)) return false;
            await _staffRepository.DeleteAsync(id);
            return true;
        }
    }
