namespace mark.davison.edinburgh.shared.models.Entities;

public class Board : EdinburghEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid ProjectId { get; set; }

    public virtual Project? Project { get; set; }
}
