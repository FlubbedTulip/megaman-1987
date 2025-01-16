namespace Interfaces
{
    public interface IMovementState
    {
        void EnterState(IMovementContext movementContext);
        void ExitState(IMovementContext movementContext);
        void UpdateState(IMovementContext movementContext);
    }
}
