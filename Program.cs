using Microsoft.EntityFrameworkCore;
using MagicOnion;
using MagicOnion.Server;

var builder = WebApplication.CreateBuilder(args);

// DIコンテナへの依存注入
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// MagicOnion
builder.Services.AddGrpc();
builder.Services.AddMagicOnion();

var app = builder.Build();

// Call MagicOnion
app.MapMagicOnionService();

// MapGroup
var todoItems = app.MapGroup("/todoitems");

# region Method Style
todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/complete", GetCompleteTodos);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPost("/", CreateTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);
# endregion

# region Lambda Style(old)
// todoItems.MapGet("/", 
//     async(TodoDb db) => await db.Todos.ToListAsync());

// todoItems.MapGet("/complete",
//     async(TodoDb db) => await db.Todos.Where(t => t.IsComplete).ToListAsync());

// todoItems.MapGet("/{id}",
//     async(int id, TodoDb db) 
//         => await db.Todos.FindAsync(id) is Todo todo 
//             ? Results.Ok(todo) 
//             : Results.NotFound());

// todoItems.MapPost("/", 
//     async(Todo todo, TodoDb db) => 
//     {
//         db.Todos.Add(todo);
//         await db.SaveChangesAsync();

//         return Results.Created($"/todoitems/{todo.Id}", todo);
//     });

// todoItems.MapPut("/{id}", async (int id, Todo inputTodo, TodoDb db) =>
// {
//     var todo = await db.Todos.FindAsync(id);

//     if (todo is null) return Results.NotFound();

//     todo.Name = inputTodo.Name;
//     todo.IsComplete = inputTodo.IsComplete;

//     await db.SaveChangesAsync();

//     return Results.NoContent();
// });

// todoItems.MapDelete("/{id}", async(int id, TodoDb db) =>
//     {
//         if(await db.Todos.FindAsync(id) is Todo todo)
//         {
//             db.Todos.Remove(todo);
//             await db.SaveChangesAsync();
//             return Results.Ok(todo);
//         }

//         return Results.NotFound();
//     });
# endregion

app.Run();


# region Methods
static async Task<IResult> GetAllTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Select(x => new TodoItemDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetCompleteTodos(TodoDb db) {
    return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoItemDTO(x)).ToListAsync());
}

static async Task<IResult> GetTodo(int id, TodoDb db)
{
    return await db.Todos.FindAsync(id)
        is Todo todo
            ? TypedResults.Ok(new TodoItemDTO(todo))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(TodoItemDTO todoItemDTO, TodoDb db)
{
    var todoItem = new Todo
    {
        IsComplete = todoItemDTO.IsComplete,
        Name = todoItemDTO.Name
    };

    db.Todos.Add(todoItem);
    await db.SaveChangesAsync();

    todoItemDTO = new TodoItemDTO(todoItem);

    return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
}

static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDb db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return TypedResults.NotFound();

    todo.Name = todoItemDTO.Name;
    todo.IsComplete = todoItemDTO.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDb db)
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.Ok(todo);
    }

    return TypedResults.NotFound();
}
# endregion

# region Methods(old)
// static async Task<IResult> GetAllTodos(TodoDb db)
// {
//     return TypedResults.Ok(await db.Todos.ToArrayAsync());
// }

// static async Task<IResult> GetCompleteTodos(TodoDb db)
// {
//     return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).ToListAsync());
// }

// static async Task<IResult> GetTodo(int id, TodoDb db)
// {
//     return await db.Todos.FindAsync(id)
//         is Todo todo
//             ? TypedResults.Ok(todo)
//             : TypedResults.NotFound();
// }

// static async Task<IResult> CreateTodo(Todo todo, TodoDb db)
// {
//     db.Todos.Add(todo);
//     await db.SaveChangesAsync();

//     return TypedResults.Created($"/todoitems/{todo.Id}", todo);
// }

// static async Task<IResult> UpdateTodo(int id, Todo inputTodo, TodoDb db)
// {
//     var todo = await db.Todos.FindAsync(id);

//     if (todo is null) return TypedResults.NotFound();

//     todo.Name = inputTodo.Name;
//     todo.IsComplete = inputTodo.IsComplete;

//     await db.SaveChangesAsync();

//     return TypedResults.NoContent();
// }

// static async Task<IResult> DeleteTodo(int id, TodoDb db)
// {
//     if (await db.Todos.FindAsync(id) is Todo todo)
//     {
//         db.Todos.Remove(todo);
//         await db.SaveChangesAsync();
//         return TypedResults.Ok(todo);
//     }

//     return TypedResults.NotFound();
// }
# endregion