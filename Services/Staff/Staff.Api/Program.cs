using Application;
using Application.IService;
using Infrastructure;
using Application.Mapping;
using Infrastructure.Interfaces;
using Infrastructure.IRepository;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Staff.Api.Converters;
using StaffGrpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    });
;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<AuthGrpc.Auth.AuthClient>(o =>
{
    o.Address = new Uri("https://localhost:7212");
});
builder.Services.AddScoped<IAuthGrpcGateway, AuthGrpcGateway>();
builder.Services.AddGrpc();
builder.Services.AddDbContext<StaffDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IStaffShiftRepository, StaffShiftRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IStaffShiftService, StaffShiftService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<StaffGrpcService>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Configure Mapster (register mappings)
MappingConfig.Register();

app.Run();