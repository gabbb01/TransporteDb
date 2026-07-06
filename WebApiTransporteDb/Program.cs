using WebApiTransporteDb.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar los controladores
builder.Services.AddControllers();

// 2. Inyectar el TransporteService como SINGLETON (Crítico para mantener los TDA en memoria)
builder.Services.AddSingleton<TransporteService>();

// 3. Configurar Swagger para probar la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Configurar CORS para permitir peticiones desde Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PermitirAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();