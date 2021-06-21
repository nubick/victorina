namespace Victorina.Commands
{
    public enum CommandType
    {
        SelectRound,
        SelectRoundQuestion,
        StopRoundBlinking,
        FinishQuestionCommand,

        //Cat In Bag
        GiveCatInBagCommand,
        FinishCatInBagCommand,
        
        FinishAuctionCommand,
        
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