namespace Victorina.Commands
{
    public enum CommandType
    {
        RegisterPlayer,
        
        MasterMakePlayerAsCurrent,
        MasterUpdatePlayerName,
        MasterUpdatePlayerScore,
        
        //Round
        SelectRound,
        SelectRoundQuestion,
        StartRoundQuestion,
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
        RestartFinalRound,
        RemoveFinalRoundTheme,
        MakeFinalRoundBet,
        SendFinalRoundAnswer,
        ClearFinalRoundAnswer,
        AcceptFinalRoundAnswer,
        FinishFinalRound,

        //Question Show
        SendAnswerIntention,
        SelectPlayerForAnswer,
        SelectFastestPlayerForAnswer,
        RestartMedia,
        ShowAnswer,
        
        //Accepting Answer
        AcceptAnswerAsCorrect,
        AcceptAnswerAsWrong,
        CancelAcceptingAnswer,
        
        //DevTools
        SendPlayerLogs,
        SavePlayerLogs
    }
}