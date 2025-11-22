using DepartmentManager.Server.Models;
using DepartmentManager.Server.Reposistory;
using DepartmentManager.Server.Reposistory.Implementation;
using DepartmentManager.Server.Reposistory.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection")));

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRepository<Member>, Repository<Member>>();
builder.Services.AddScoped<IRepository<City>, Repository<City>>();
builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
builder.Services.AddScoped<IRepository<Affiliation>, Repository<Affiliation>>();
builder.Services.AddScoped<IRepository<MemberAffiliation>, Repository<MemberAffiliation>>();
builder.Services.AddScoped<IRepository<ApprovalRequest>, Repository<ApprovalRequest>>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMemberAffiliationRepository, MemberAffiliationRepository>();
builder.Services.AddScoped<IAffiliationRepository, AffiliationRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IApprovalRequestRepository, ApprovalRequestRepository>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
