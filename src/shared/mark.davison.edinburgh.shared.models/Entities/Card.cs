namespace mark.davison.edinburgh.shared.models.Entities;

public class Card : EdinburghEntity
{
    public string Title { get; set; } = string.Empty;

    public Guid SectionId { get; set; }

    public virtual Section? Section { get; set; }
}
