var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
             builder =>
             {
                 builder.WithOrigins("http://localhost:5173") // Altere para a URL do seu frontend
                        .AllowAnyMethod()
                        .AllowAnyHeader();
             });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Permitir servir arquivos estáticos
app.UseCors("AllowSpecificOrigin"); // Adicione esta linha

app.UseAuthorization();

app.MapControllers();

app.Run();
