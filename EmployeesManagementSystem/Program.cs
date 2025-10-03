using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.Repositories;
using EmployeesManagementSystem.Services;
var builder = WebApplication.CreateBuilder(args); 
builder.Services.AddControllers(); 
builder.Services.AddScoped<AdminService>(); 
builder.Services.AddScoped<UserService>(); 
builder.Services.AddScoped<AdminRepository>();
builder.Services.AddScoped<UserRepository>(); 
builder.Services.AddScoped<AppDbContext>(); 
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 
builder.Services.AddSwaggerGen(); 
var app = builder.Build(); 
if (app.Environment.IsDevelopment()) 
{ 
    app.UseSwagger(); app.UseSwaggerUI();
}
app.UseHttpsRedirection(); 
app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization(); 
app.MapControllers(); 
app.Run(); 
