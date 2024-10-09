using AutoMapper;
using GestioneOrdini.Model.Clients;
using GestioneOrdini.Model.Customer;
using GestioneOrdini.Model.Order;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapping OrderCreationDto to Order
        CreateMap<OrderCreationDto, Order>()
            .ForMember(dest => dest.Items, opt => opt.Ignore()) // Gestiamo separatamente gli Items
            .ForMember(dest => dest.Status, opt => opt.Ignore()) // Gestito separatamente
            .ForMember(dest => dest.Customer, opt => opt.Ignore()) // Evita mappature cicliche o non necessarie

            // Mapping delle proprietà di base
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.MaxDeliveryDate, opt => opt.MapFrom(src => src.MaxDeliveryDate))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId));

        // Mapping da Order a OrderDto
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.StatusName))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items)); //Assumo che ItemWithFileDto sia già mappato
                                                                                  //Aggiungi altri mapping necessari per le proprietà di OrderDto

        // Mapping da Customer a CustomerDto
        CreateMap<Customer, CustomerDto>();

        //Mapping da Item a ItemWithFileDto (se necessario)
        CreateMap<Item, ItemWithFileDto>();  // Aggiungi questo se non hai già un mapping

        // Mapping da ItemWithFileDto a Item (LaserItem o PlotterItem)
        CreateMap<ItemWithFileDto, LaserItem>()
            .ForMember(dest => dest.FileName, opt => opt.Ignore())
            .ForMember(dest => dest.FilePath, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.LaserStandardId, opt => opt.MapFrom(src => src.ItemId)) // Mappa l'ID standard
            .ForMember(dest => dest.WorkTypeId, opt => opt.MapFrom(src => src.WorkTypeId));

        CreateMap<ItemWithFileDto, PlotterItem>()
            .ForMember(dest => dest.FileName, opt => opt.Ignore())
            .ForMember(dest => dest.FilePath, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.PlotterStandardId, opt => opt.MapFrom(src => src.ItemId)) // Mappa l'ID standard
            .ForMember(dest => dest.WorkTypeId, opt => opt.MapFrom(src => src.WorkTypeId));

        // In caso di necessità, aggiungi eventuali configurazioni custom
    }
}
