using Application.DTOs;
using Application.IService;
using Domain.Entities;
using GrpcContract;
using Infrastructure.Interfaces;
using Infrastructure.IRepository;
using Mapster;

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
            if (userReply == null || !userReply.IsSuccess)
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
                    FullName = userReply.Data.FullName,
                    Email = userReply.Data.Email,
                    PhoneNumber = string.Empty,
                    Role = userReply.Data.Role
                }
            };
        }
        
        public async Task<ResultDto<StaffDto>> GetStaffByUserIdAsync(Guid userId)
        {
            var staff = await _staffRepository.GetByUserIdAsync(userId);
            
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
            if (userReply == null || !userReply.IsSuccess)
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
                    FullName = userReply.Data.FullName,
                    Email = userReply.Data.Email,
                    PhoneNumber = userReply.Data.PhoneNumber,
                    Role = userReply.Data.Role
                }
            };
        }

        public async Task<ResultDto<IEnumerable<StaffDto>>> GetAllStaffAsync()
        {
            var staffs = await _staffRepository.GetAllAsync();
            var staffDtos = staffs.Adapt<List<StaffDto>>();

            var tasks = staffDtos.Select(async staffDto =>
            {
                var userReply = await _authGrpcGateway.GetUserById(staffDto.UserId);
                if (userReply?.IsSuccess == true)
                {
                    staffDto.FullName = userReply.Data.FullName;
                    staffDto.Email = userReply.Data.Email;
                    staffDto.Role = userReply.Data.Role;
                }
            });

            await Task.WhenAll(tasks);

            return new ResultDto<IEnumerable<StaffDto>>()
            {
                IsSuccess = true,
                Message = "Staffs retrieved successfully",
                Data = staffDtos,
            };
        }

        public async Task<ResultDto<List<StaffDto>>> GetAllStaffByRoleAsync(int role)
        {
            var userReply = await _authGrpcGateway.GetUserByRole(role);
            if (userReply is not { IsSuccess: true })
            {
                return new ResultDto<List<StaffDto>>()
                {
                    IsSuccess = false,
                    Message = "Failed to retrieve users by role through gRPC",
                    Data = null,
                };
            }
            var staffDtos = new List<StaffDto>();
            foreach (var user in userReply.Data)
            {
                var staff = await _staffRepository.GetByUserIdAsync(Guid.Parse(user.Id));
                if (staff != null)
                {
                    var staffDto = new StaffDto
                    {
                        StaffId = staff.Id,
                        UserId = staff.UserId,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role
                    };
                    staffDtos.Add(staffDto);
                }
            }
            return new ResultDto<List<StaffDto>>()
            {
                IsSuccess = true,
                Message = "Staffs retrieved successfully",
                Data = staffDtos,
            };
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
