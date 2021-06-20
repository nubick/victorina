namespace Victorina.Commands
{
    public enum CommandType
    {
        SelectRound,
        SelectRoundQuestion,
        StopRoundBlinking,
        GiveCatInBagCommand,
        FinishQuestionCommand,

        //Final Round
        RemoveFinalRoundTheme,
        MakeFinalRoundBet,
        SendFinalRoundAnswer,
        ClearFinalRoundAnswer,

        //DevTools
        SendPlayerLogs,
        SavePlayerLogs
    }
}