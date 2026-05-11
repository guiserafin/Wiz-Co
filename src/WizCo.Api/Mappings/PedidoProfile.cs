namespace WizCo.Api.Mappings;

using AutoMapper;
using WizCo.Api.DTOs.Responses;
using WizCo.Api.Entities;

public sealed class PedidoProfile : Profile
{
    public PedidoProfile()
    {
        CreateMap<ItemPedido, ItemPedidoResponse>();

        CreateMap<Pedido, PedidoResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Pedido, PedidoDetalhadoResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens));
    }
}
