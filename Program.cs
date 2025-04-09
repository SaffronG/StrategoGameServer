using Microsoft.Extensions.Options;

string? MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => { options.AddPolicy(name: MyAllowSpecificOrigins, policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }); });

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();

app.Run();