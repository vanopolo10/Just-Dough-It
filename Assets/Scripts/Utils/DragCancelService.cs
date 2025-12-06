using System;

public static class DragCancelService
{
    public static event Action CancelRequested;

    public static void RequestCancel()
    {
        CancelRequested?.Invoke();
    }
}