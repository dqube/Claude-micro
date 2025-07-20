namespace BuildingBlocks.Domain.Entities;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    string? DeletedBy { get; }
    
    void Delete(string? deletedBy = null);
    void Restore();
}