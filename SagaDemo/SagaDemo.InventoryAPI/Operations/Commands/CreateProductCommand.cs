namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class CreateProductCommand
    {
        public CreateProductCommand(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}