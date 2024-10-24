namespace Domain.Database;

public class Game : BaseEntity
{
    public DateTime CreatedAtDt { get; set; } = DateTime.Now;
    public DateTime UpdatedAtDt { get; set; } = DateTime.Now;

    public string State { get; set; } = default!;

    // null, if you did not do the join (.include in c#)
    public ICollection<Player>? Players { get; set; }
}