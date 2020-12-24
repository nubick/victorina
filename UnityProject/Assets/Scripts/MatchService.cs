using Injection;

namespace Victorina
{
    public class MatchService
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
        public void Start()
        {
            MatchData.Phase = MatchPhase.Question;
            SendToPlayersService.Send(MatchData);

            TextQuestion textQuestion = new TextQuestion();
            textQuestion.Question = "Мэдж Уоттс считала, что брак ее сестры, Агаты Кристи, не будет удачным, вот и прислала невесте с десяток этих аксессуаров. Оказалась неправа.";
            textQuestion.Answer = "Носовые платки";
            
            SendToPlayersService.Send(textQuestion);
        }
    }
}