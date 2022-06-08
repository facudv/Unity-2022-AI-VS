public enum NotifyActionObserver{StartGame,EndGame}

public interface IObserver 
{
    void OnNotify(NotifyActionObserver action);
}
