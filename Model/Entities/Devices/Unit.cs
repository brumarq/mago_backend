namespace Model.Entities.Devices
{
    public class Unit : BaseEntity
    {
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        public Quantity? Quantity { get; set; }
        public double Factor { get; set; }
        public double Offset { get; set; }
    }
}
