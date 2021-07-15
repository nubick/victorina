namespace Victorina
{
    public static class ServerEvents
    {
        public static ServerEvent FinalRoundStarted { get; } = new ServerEvent("FinalRoundStarted");
        public static ServerEvent<string> RoundQuestionSelected { get; } = new ServerEvent<string>("RoundQuestionSelected");
        public static ServerEvent<string> PlaySoundEffect { get; } = new ServerEvent<string>("PlaySoundEffect");

        public static ServerEvent RestartMedia { get; } = new ServerEvent("RestartMedia");
        public static ServerEvent PlayMedia { get; } = new ServerEvent("PlayMedia");
        public static ServerEvent PauseMedia { get; } = new ServerEvent("PauseMedia");
    }
}