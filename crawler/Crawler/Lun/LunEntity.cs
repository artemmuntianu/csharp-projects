namespace Crawler.Lun
{
    class LunEntity
    {
        public string LivingComplexName { get; set; }
        public double? AmountOfSqM { get; set; }
        public double? Price
        {
            get
            {
                return AmountOfSqM.GetValueOrDefault(0) * PriceForSqM.GetValueOrDefault(0);
            }
        }
        public double? PriceForSqM { get; set; }
        public int? NumberOfRooms { get; set; }
        public string Queue { get; set; }
        public string Url { get; set; }
    }
}
