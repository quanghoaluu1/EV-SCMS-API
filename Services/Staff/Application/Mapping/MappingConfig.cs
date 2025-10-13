using Application.DTOs;
using Domain.Entities;
using Mapster;

namespace Application.Mapping;

public static class MappingConfig
{
    public static void Register()
    {
        TypeAdapterConfig<Shift, ShiftDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.StartTime, src => src.StartTime)
            .Map(dest => dest.EndTime, src => src.EndTime);

        TypeAdapterConfig<StaffShift, StaffShiftDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.StaffId, src => src.StaffId)
            .Map(dest => dest.ShiftId, src => src.ShiftId)
            .Map(dest => dest.WorkDate, src => src.WordDate)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
            .Map(dest => dest.Shift, src => src.Shift == null ? null : src.Shift.Adapt<ShiftDto>());

        TypeAdapterConfig<LeaveRequest, LeaveRequestDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.StaffId, src => src.StaffId)
            .Map(dest => dest.StartDate, src => src.StartDate)
            .Map(dest => dest.EndDate, src => src.EndDate)
            .Map(dest => dest.Reason, src => src.Reason)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.ApprovedBy, src => src.ApprovedBy)
            .Map(dest => dest.ApprovedAt, src => src.ApprovedAt)
            .Map(dest => dest.ApprovalNotes, src => src.ApprovalNotes)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);
    }
}


