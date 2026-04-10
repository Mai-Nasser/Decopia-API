using Microsoft.EntityFrameworkCore;
using Rules_Engine_API.Data;

using Rules_Engine_API.Repositories;
using Rules_Engine_API.Services;

var builder = WebApplication.CreateBuilder(args);

// إضافة CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercel", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5174",
            "https://localhost:5174", // 👈 أضيفي دي
            "https://decopia.vercel.app"
        )

         .AllowAnyHeader()
         .AllowAnyMethod();

    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ─── Controllers ────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ─── Rules Engine ───────────────────────────────────────────────
builder.Services.AddSingleton<IRulesEngine, RulesEngine>();

// ─── Database (SQL Server) ───────────────────────────────────────
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection")
//        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
//    )
//);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

// ─── Evaluation Repository ──────────────────────────────────────
builder.Services.AddScoped<IEvaluationRepository, EvaluationRepository>();

// ─── OpenAPI ────────────────────────────────────────────────────
builder.Services.AddOpenApi();

var app = builder.Build();





// ─── Auto-apply DB migrations on startup ────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // use db.Database.Migrate() if you add migrations
}



// ─── Pipeline ───────────────────────────────────────────────────
//if (app.Environment.IsDevelopment())
//    app.MapOpenApi();

app.UseHttpsRedirection();

app.UseRouting();

// تفعيل CORS قبل الـ endpoints
app.UseCors("AllowVercel");
app.UseAuthorization();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();

app.Run();