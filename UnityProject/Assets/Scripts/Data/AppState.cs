namespace Victorina
{
    public class AppState
    {
        public string LastJoinPlayerName { get; set; }
        public string LastJoinGameCode { get; set; }
        public ReactiveProperty<float> Volume { get; } = new ReactiveProperty<float>();
        public string PlayerGuid { get; set; }
    }
}