using API.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BibliotecaDbContext>();

var app = builder.Build();

app.MapPost("/api/livros", async ([FromServices] BibliotecaDbContext db, [FromBody] Livro livro) =>
{
    if(livro.Titulo is null || livro.Titulo.Length < 3) {
        return Results.BadRequest("Erro, Titulo tem menos de 3 Caracteres");
    }
    if(livro.Autor is null || livro.Autor.Length < 3) {
        return Results.BadRequest("Erro, Autor tem menos de 3 Caracteres");
    }

    db.Livros.Add(livro);
    await db.SaveChangesAsync();

    return Results.Created($"/api/livros/{livro.Id}", livro);
});

app.MapGet("/api/livros", async ([FromServices] BibliotecaDbContext db) => 
{
    return await db.Livros
    .Include( l=> l.Categoria)
    .ToListAsync();

});


app.Run();
