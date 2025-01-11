namespace Interfaces
{
    public interface IMovementContext
    {
        float GravityScale { get; set; }
        
        bool IsNearLadder { get; set; }

    }
}
