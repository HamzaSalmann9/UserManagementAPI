using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using UserManagementAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register in-memory user list
builder.Services.AddSingleton<List<User>>();

var app = builder.Build();

// --- MIDDLEWARE ORDER ---

// 1. Error handling middleware FIRST
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. Authentication middleware NEXT
app.UseMiddleware<AuthMiddleware>();

// 3. Logging middleware LAST
app.UseMiddleware<LoggingMiddleware>();

// ----------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/users", (List<User> users) => Results.Ok(users));
app.MapGet("/users/{id}", (int id, List<User> users) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/users", (User newUser, List<User> users) =>
{
    newUser.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
    users.Add(newUser);
    return Results.Created($"/users/{newUser.Id}", newUser);
});

app.MapPut("/users/{id}", (int id, User updatedUser, List<User> users) =>
{
    var existing = users.FirstOrDefault(u => u.Id == id);
    if (existing is null) return Results.NotFound();

    existing.Name = updatedUser.Name;
    existing.Email = updatedUser.Email;
    existing.Password = updatedUser.Password;
    existing.IsActive = updatedUser.IsActive;

    return Results.Ok(existing);
});

app.MapDelete("/users/{id}", (int id, List<User> users) =>
{
    var existing = users.FirstOrDefault(u => u.Id == id);
    if (existing is null) return Results.NotFound();

    users.Remove(existing);
    return Results.NoContent();
});

app.Run();
