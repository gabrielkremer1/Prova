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

app.MapGet("/api/livros/{id}", async([FromServices] BibliotecaDbContext db, int id) =>
{
    var livro = await db.Livros.Include( l => l.Categoria).FirstOrDefaultAsync(l => l.Id == id);

    return livro is not null ? Results.Ok(livro):
    Results.NotFound("Livro nao encontrado");
});

app.MapPut("/api/livros/{id}", async ([FromServices] BibliotecaDbContext db, int id,[FromBody] Livro livroAtualizado) =>
{
    var livro = await db.Livros.FindAsync(id);

    if (livro is null) return Results.NotFound("Livro nao encontrado");
    if (livroAtualizado.Id !=0 && id != livroAtualizado.Id) { 
        return Results.BadRequest("O id da URL nao correponde ao Id do Livro no corpo.");
    }
    if (livroAtualizado.Titulo is null || livroAtualizado.Titulo.Length < 3){ 
        return Results.BadRequest("Erro para criar livro");
    }

    livro.Titulo = livroAtualizado.Titulo;
    livro.Autor = livroAtualizado.Autor;
    livro.CategoriaId = livroAtualizado.CategoriaId;

    await db.SaveChangesAsync();
    
    return Results.Ok(livro);

});

app.MapDelete("/api/livros/{id}", async ([FromServices] BibliotecaDbContext db, int id) =>
{
    var livro
}
);

app.Run();
