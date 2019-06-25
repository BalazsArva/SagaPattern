using System.Collections.Generic;

namespace SagaDemo.InventoryAPI.DataAccess.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int PointsCost { get; set; }

        public byte[] RowVersion { get; set; }

        public virtual ICollection<ProductReservation> Reservations { get; set; } = new List<ProductReservation>();

        public virtual ICollection<ProductStockAddedEvent> ProductStockAddedEvents { get; set; } = new List<ProductStockAddedEvent>();

        public virtual ICollection<ProductStockRemovedEvent> ProductStockRemovedEvents { get; set; } = new List<ProductStockRemovedEvent>();

        public virtual ICollection<ProductTakenOutEvent> ProductTakenOutEvents { get; set; } = new List<ProductTakenOutEvent>();

        public virtual ICollection<ProductBroughtBackEvent> ProductBroughtBackEvents { get; set; } = new List<ProductBroughtBackEvent>();
    }
}