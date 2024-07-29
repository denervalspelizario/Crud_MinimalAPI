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



// método assincrono que retona lista de usuarios que será reutilizados em alguns endpoints
// e para não haver replicação de codigo decidi fazer 1 só separado
async Task<List<UsuarioModel>> GetUsuarios(AppDbContext context)
{
    return await context.Usuarios.ToListAsync();
}


// aqui ficará os endpoints de uma minimalapi

// Get que lista usuarios
app.MapGet("/Usuarios", async (AppDbContext context) =>
{
    // Retorna lista de usuários através do método GetUsuarios
    var usuarios = await GetUsuarios(context);
    return Results.Ok(usuarios);
});


// Get que buscar usuario por id
app.MapGet("/Usuario/{id}", async (AppDbContext context, int id) =>
{
    var usuario = await context.Usuarios.FindAsync(id);

    if (usuario == null)
    {
        return Results.NotFound("Usuário não localizado");
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

 
    // Retorna lista de usuários através do método GetUsuarios
    var usuarios = await GetUsuarios(context);
    return Results.Ok(usuarios);
});


// Put que edita os dados de um usuario
app.MapPut("/Usuario", async (AppDbContext context, UsuarioModel usuario) =>
{
    var usuarioEncontrado = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == usuario.Id);

    if(usuarioEncontrado == null)
    {
        return Results.NotFound("Usuario não encontrado");
    }

    usuarioEncontrado.Username = usuario.Username;
    usuarioEncontrado.Nome = usuario.Nome;
    usuarioEncontrado.Email = usuario.Email;

    // atualizando os dados do usuario
    context.Update(usuario);

    // salvando as alterações
    await context.SaveChangesAsync();

    // Retorna lista de usuários através do método GetUsuarios
    var usuarios = await GetUsuarios(context);
    return Results.Ok(usuarios);
});



// Delete que buscar usuario por id e deleta seu registro
app.MapDelete("/Usuario/{id}", async (AppDbContext context, int id) =>
{
    var usuario = await context.Usuarios.FindAsync(id);

    if (usuario == null)
    {
        return Results.NotFound("Usuário não localizado");
    }

    // excluindo usuario do banco
    context.Remove(usuario);

    // salvando as alterações
    await context.SaveChangesAsync();

    // Retorna usuario buscado pelo id
    return Results.NoContent();
});

app.Run();


