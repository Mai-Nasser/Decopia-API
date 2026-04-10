using Microsoft.EntityFrameworkCore;
using Rules_Engine_API.Data;

using Rules_Engine_API.Repositories;
using Rules_Engine_API.Services;

var builder = WebApplication.CreateBuilder(args);

  

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


 builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });

 builder.Services.AddSingleton<IRulesEngine, RulesEngine>();

 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

 builder.Services.AddScoped<IEvaluationRepository, EvaluationRepository>();

 builder.Services.AddOpenApi();

var app = builder.Build();





 using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();  
}



 

app.UseHttpsRedirection();

app.UseRouting();

 app.UseCors("AllowVercel");
app.UseAuthorization();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();

app.Run();
