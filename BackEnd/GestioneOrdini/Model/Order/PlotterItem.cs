using GestioneOrdini.Model.Order;

public class PlotterItem : Item
{
    public bool IsCustom { get; set; } // Nuova proprietà per indicare se è un plotter custom

    public int? PlotterStandardId { get; set; } // Nullable for custom plotter
    public double? Base { get; set; } // Nullable for standard plotters
    public double? Height { get; set; } // Nullable for standard plotters
    public double? PricePerSquareMeter { get; set; } // Nullable for standard
    public int? Quantity { get; set; } // Nullable for standard
    public virtual PlotterStandard PlotterStandard { get; set; }
}
