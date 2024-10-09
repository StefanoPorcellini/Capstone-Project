namespace GestioneOrdini.Model.Order
{
    public class ItemWithFileDto
    {
        public int ItemId { get; set; } // ID dell'item esistente
        public IFormFile? File { get; set; } // Gestisce l'upload del file
        public int WorkTypeId { get; set; }
        public string WorkDescription { get; set; } // Aggiunta della proprietà
        public bool IsLaserCustom { get; set; } // Determina se è un lavoro custom

        public int? LaserStandardId { get; set; } // Nullable for custom laser
        public int? Quantity { get; set; } // Nullable for standard
        public bool IsPlotterCustom { get; set; } // Nuova proprietà per indicare se è un plotter custom

        public int? PlotterStandardId { get; set; } // Nullable for custom plotter
        public double? Base { get; set; } // Nullable for standard plotters
        public double? Height { get; set; } // Nullable for standard plotters
        public decimal? PricePerSquareMeter { get; set; } = 10;// Nullable for standard

    }
    public class OrderWithItemsDto
    {
        public Order Order { get; set; }
        public List<ItemWithFileDto> ItemsWithFiles { get; set; } // Lista di Item e relativi file
    }

}
