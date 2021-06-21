namespace Victorina.Commands
{
    public enum CommandType
    {
        SelectRound,
        SelectRoundQuestion,
        StopRoundBlinking,
        FinishQuestion,

        //Cat In Bag
        GiveCatInBag,
        FinishCatInBag,
        
        //No Risk
        FinishNoRisk,
        
        //Auction
        FinishAuction,
        
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