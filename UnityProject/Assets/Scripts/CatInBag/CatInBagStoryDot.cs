namespace Victorina
{
    public class CatInBagStoryDot : StoryDot
    {
        public string Theme { get; set; }
        public int Price { get; set; }
        public bool CanGiveYourself { get; set; }

        public CatInBagStoryDot() { }
        
        public CatInBagStoryDot(string theme, int price, bool canGiveYourself = false)
        {
            Theme = theme;
            Price = price;
            CanGiveYourself = canGiveYourself;
        }

        public override string ToString()
        {
            return $"[cibsd|{Theme}|{Price}|{CanGiveYourself}]";
        }
    }
}