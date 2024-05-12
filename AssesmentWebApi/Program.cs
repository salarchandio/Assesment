using AssesmentWebApi.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//DbContext pooling is particularly useful where there are frequent requests.
//Creating a new DbContext for each request is expensive.
builder.Services.AddDbContextPool<AssessmentDBContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("AssesmentCS"))
);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
