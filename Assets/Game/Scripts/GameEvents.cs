using System;

public static class GameEvents
{
    public static Action OnGameoverEvent;
    public static Action OnGameStartEvent;

    public static Action OnComboStartEvent;
    public static Action OnComboUpdateEvent;
    public static Action OnComboEndEvent;
    public static Action OnComboCancelledEvent;

    public static Action OnGameTimerStartEvent;
    public static Action OnGameTimerEndEvent;

    public static Action OnCountdownStartEvent;
    public static Action OnCountdownEndEvent;
    public static Action<OBJECT_TYPE> OnScoredEvent;

    public static Action GameResetEvent;
    public static Action SpawnObjectEvent;
}