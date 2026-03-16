public interface ITutorialGate
{
    void Enter();
    event System.Action Completed;

    UnityEngine.GameObject IconObject { get; }
}