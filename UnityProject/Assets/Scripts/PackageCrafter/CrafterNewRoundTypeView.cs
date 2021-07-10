namespace Victorina
{
    public class CrafterNewRoundTypeView : ViewBase
    {
        public RoundType? SelectedRoundType { get; set; }

        protected override void OnShown()
        {
            SelectedRoundType = null;
        }

        public void OnSimpleRoundButtonClicked()
        {
            SelectedRoundType = RoundType.Simple;
            Hide();
        }

        public void OnFinalRoundButtonClicked()
        {
            SelectedRoundType = RoundType.Final;
            Hide();
        }
    }
}