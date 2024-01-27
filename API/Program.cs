using BL.Logic;
using BL.Logic.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<INewsService, NewsService>();
builder.Services.AddHostedService<TimedService>();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("corsPolicy",
        builder => builder.WithOrigins("http://localhost:4200") 
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});

var app = builder.Build();
app.UseRouting();
app.UseCors("corsPolicy");
// Add SignalR hub mapping
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NewsHub>("/newsHub");
    endpoints.MapControllers();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.Run();