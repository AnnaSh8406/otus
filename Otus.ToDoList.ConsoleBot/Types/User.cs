 

namespace Otus.ToDoList.ConsoleBot.Types;
/// <summary>
/// Сущность - пользователь, общающийся с ботом
/// </summary>
public class User
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public long Id { get; init; }
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string? Username { get; init; }
}
