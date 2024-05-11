namespace mark.davison.edinburgh.shared.models.Entities;

public class Section : EdinburghEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid BoardId { get; set; }

    public virtual Board? Board { get; set; }
}
