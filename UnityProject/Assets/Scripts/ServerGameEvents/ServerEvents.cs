namespace Victorina
{
    public static class ServerEvents
    {
        public static ServerEvent FinalRoundStarted { get; } = new ServerEvent("FinalRoundStarted");
        public static ServerEvent<string> RoundQuestionSelected { get; } = new ServerEvent<string>("RoundQuestionSelected");
    }
}