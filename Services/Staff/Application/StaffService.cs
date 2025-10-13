using Application.DTOs;
using Application.IService;
using Domain.Entities;
using GrpcContract;
using Infrastructure.Interfaces;
using Infrastructure.IRepository;

namespace Application;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly IAuthGrpcGateway _authGrpcGateway;

        public StaffService(IStaffRepository staffRepository, IAuthGrpcGateway authGrpcGateway)
        {
            _staffRepository = staffRepository;
            _authGrpcGateway = authGrpcGateway;
        }

        public async Task<ResultDto<StaffDto>> GetStaffByIdAsync(Guid id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            
            if (staff == null)
            {
                return new ResultDto<StaffDto>
                {
                    IsSuccess = false,
                    Message = "Staff not found",
                    Data = null,
                };
            }

            var userReply = await _authGrpcGateway.GetUserById(staff.UserId);
            if (userReply == null || !userReply.Success)
            {
                return new ResultDto<StaffDto>()
                {
                    IsSuccess = false,
                    Message = "Failed to retrieve user information through gRPC",
                    Data = null,
                };
            }
            
            
            return new ResultDto<StaffDto>()
            {
                IsSuccess = true,
                Message = "Staff retrieved successfully",
                Data = new StaffDto
                {
                    StaffId = staff.Id,
                    UserId = staff.UserId,
                    FullName = userReply.FullName,
                    Email = userReply.Email,
                    PhoneNumber = string.Empty,
                    Role = userReply.Role
                }
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
