namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class CreateProductCommand
    {
        public CreateProductCommand(string name, int pointsCost)
        {
            Name = name;
            PointsCost = pointsCost;
        }

        public string Name { get; }

        public int PointsCost { get; }
    }
}