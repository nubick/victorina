namespace Victorina
{
    public class CatInBagInfo
    {
        public string Theme { get; set; }
        public int Price { get; set; }
        public bool CanGiveYourself { get; set; }

        public CatInBagInfo(string theme, int price, bool canGiveYourself)
        {
            Theme = theme;
            Price = price;
            CanGiveYourself = canGiveYourself;
        }
        
        public override string ToString()
        {
            return $"[CatInBag|{Theme}|{Price}|{CanGiveYourself}]";
        }
    }
}