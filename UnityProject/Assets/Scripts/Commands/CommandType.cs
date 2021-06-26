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
        PassAuction,
        MakeBetAuction,
        MakeAllInAuction,
        MakeBetForPlayerAuction,
        FinishAuction,
        
        //Final Round
        RemoveFinalRoundTheme,
        MakeFinalRoundBet,
        SendFinalRoundAnswer,
        ClearFinalRoundAnswer,

        //Question Show
        SendAnswerIntention,
        SelectPlayerForAnswer,
        SelectFastestPlayerForAnswer,
        ShowAnswer,
        
        //Accepting Answer
        AcceptAnswerAsCorrect,
        AcceptAnswerAsWrong,
        CancelAcceptingAnswer,
        
        //Effects
        PlaySoundEffect,
        
        //DevTools
        SendPlayerLogs,
        SavePlayerLogs
    }
}