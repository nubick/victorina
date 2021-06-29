namespace Victorina
{
    public static class ServerEvents
    {
        public static ServerGameEvent FinalRoundStarted { get; } = new ServerGameEvent("FinalRoundStarted");
    }
}