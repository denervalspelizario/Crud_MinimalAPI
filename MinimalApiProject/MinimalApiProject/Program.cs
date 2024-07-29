using Microsoft.EntityFrameworkCore;
using MinimalApiProject.Data;
using MinimalApiProject.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// adicionando o services do dbcontext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



// m�todo assincrono que retona lista de usuarios que ser� reutilizados em alguns endpoints
// e para n�o haver replica��o de codigo decidi fazer 1 s� separado
async Task<List<UsuarioModel>> GetUsuarios(AppDbContext context)
{
    return await context.Usuarios.ToListAsync();
}


// aqui ficar� os endpoints de uma minimalapi

// Get que lista usuarios
app.MapGet("/Usuarios", async (AppDbContext context) =>
{
    // Retorna lista de usu�rios atrav�s do m�todo GetUsuarios
    var usuarios = await GetUsuarios(context);
    return Results.Ok(usuarios);
});


// Get que buscar usuario por id
app.MapGet("/Usuario/{id}", async (AppDbContext context, int id) =>
{
    var usuario = await context.Usuarios.FindAsync(id);

    if (usuario == null)
    {
        return Results.NotFound("Usu�rio n�o localizado");
    }

    // Retorna usuario buscado pelo id
    return Results.Ok(usuario);
});


// Post que adiciona usuario
app.MapPost("/Usuario", async (AppDbContext context, UsuarioModel usuario) =>
{
    // adicionando usuario
    context.Add(usuario);

    //salvando dados no banco
    await context.SaveChangesAsync();   

 
    // Retorna lista de usu�rios atrav�s do m�todo GetUsuarios
    var usuarios = await GetUsuarios(context);
    return Results.Ok(usuarios);
});


// Put que edita os dados de um usuario
app.MapPut("/Usuario", async (AppDbContext context, UsuarioModel usuario) =>
{
    var usuarioEncontrado = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == usuario.Id);

    if(usuarioEncontrado == null)
    {
        return Results.NotFound("Usuario n�o encontrado");
    }

    usuarioEncontrado.Username = usuario.Username;
    usuarioEncontrado.Nome = usuario.Nome;
    usuarioEncontrado.Email = usuario.Email;

    // atualizando os dados do usuario
    context.Update(usuario);

    // salvando as altera��es
    await context.SaveChangesAsync();

    // Retorna lista de usu�rios atrav�s do m�todo GetUsuarios
    var usuarios = await GetUsuarios(context);
    return Results.Ok(usuarios);
});



// Delete que buscar usuario por id e deleta seu registro
app.MapDelete("/Usuario/{id}", async (AppDbContext context, int id) =>
{
    var usuario = await context.Usuarios.FindAsync(id);

    if (usuario == null)
    {
        return Results.NotFound("Usu�rio n�o localizado");
    }

    // excluindo usuario do banco
    context.Remove(usuario);

    // salvando as altera��es
    await context.SaveChangesAsync();

    // Retorna usuario buscado pelo id
    return Results.NoContent();
});

app.Run();


