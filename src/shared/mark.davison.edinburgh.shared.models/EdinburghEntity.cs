namespace mark.davison.edinburgh.shared.models;

public class EdinburghEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}
