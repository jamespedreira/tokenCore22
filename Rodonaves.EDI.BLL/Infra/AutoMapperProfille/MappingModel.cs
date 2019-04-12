using AutoMapper;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Model;

namespace Rodonaves.EDI.BLL.Infra.AutoMapperProfille
{
    public class MappingModel : Profile
    {
        public MappingModel ()
        {
            CreateMap<ProviderInfo, ProviderDTO> ();
            CreateMap<ProviderDTO, ProviderInfo> ();

            CreateMap<TaskInfo, TaskDTO> ();
            CreateMap<TaskDTO, TaskInfo> ();

            CreateMap<TriggerInfo, TriggerDTO> ();
            CreateMap<TriggerDTO, TriggerInfo> ();

            CreateMap<DetailedDayInfo, DetailedDayDTO> ();
            CreateMap<DetailedDayDTO, DetailedDayInfo> ();

            CreateMap<DetailedMonthInfo, DetailedMonthDTO> ();
            CreateMap<DetailedMonthDTO, DetailedMonthInfo> ();

            CreateMap<ScriptDTO, ScriptInfo> ();
            CreateMap<ScriptInfo, ScriptDTO> ();

            CreateMap<LayoutGroupDTO, LayoutGroupInfo> ();
            CreateMap<LayoutGroupInfo, LayoutGroupDTO> ();

            CreateMap<LayoutBandDTO, LayoutBandInfo>();
            CreateMap<LayoutBandInfo, LayoutBandDTO>();

            CreateMap<LayoutDictionaryDTO, LayoutDictionaryInfo>();
            CreateMap<LayoutDictionaryInfo, LayoutDictionaryDTO>();

            CreateMap<LayoutColumnDTO, LayoutColumnInfo>().ForMember(dest => dest.LayoutDictionaries, opt => opt.MapFrom(src => src.LayoutDictionaries));
            CreateMap<LayoutColumnInfo, LayoutColumnDTO>().ForMember(dest => dest.LayoutDictionaries, opt => opt.MapFrom(src => src.LayoutDictionaries));

            CreateMap<LayoutHeaderDTO, LayoutHeaderInfo>().ForMember(dest => dest.LayoutBands, opt => opt.MapFrom(src => src.LayoutBands));
            CreateMap<LayoutHeaderInfo, LayoutHeaderDTO>().ForMember(dest => dest.LayoutBands, opt => opt.MapFrom(src => src.LayoutBands));

            CreateMap<ActionDTO, ActionInfo> ();
            CreateMap<ActionInfo, ActionDTO> ();

            CreateMap<ActionObjectDTO, ActionObjectInfo> ();
            CreateMap<ActionObjectInfo, ActionObjectDTO> ();

            CreateMap<PersonDTO, PersonInfo> ();
            CreateMap<PersonInfo, PersonDTO> ();

            CreateMap<CustomerDTO, CustomerInfo>();
            CreateMap<CustomerInfo, CustomerDTO>();

            CreateMap<AgreementDTO, AgreementInfo>();
            CreateMap<AgreementInfo, AgreementDTO>();

            CreateMap<AgreementProcessDTO, AgreementProcessInfo>();
            CreateMap<AgreementProcessInfo, AgreementProcessDTO>();

            CreateMap<AgreementOccurrenceDTO, AgreementOccurrenceInfo>();
            CreateMap<AgreementOccurrenceInfo, AgreementOccurrenceDTO>();

            CreateMap<LayoutFileNameInfo, LayoutFileNameDTO>().ForMember(dest => dest.LayoutColumns, opt => opt.MapFrom(src => src.LayoutColumns));
            CreateMap<LayoutFileNameDTO, LayoutFileNameInfo>().ForMember(dest => dest.LayoutColumns, opt => opt.MapFrom(src => src.LayoutColumns)); 

            CreateMap<EmailInfo, EmailDTO>();
            CreateMap<EmailDTO, EmailInfo>();

            CreateMap<FtpInfo, FtpDTO>();
            CreateMap<FtpDTO, FtpInfo>();

            CreateMap<CommunicationChannelInfo, CommunicationChannelDTO>();
            CreateMap<CommunicationChannelDTO, CommunicationChannelInfo>();

            CreateMap<AgreementCommunicationChannelInfo, AgreementCommunicationChannelDTO>();
            CreateMap<AgreementCommunicationChannelDTO, AgreementCommunicationChannelInfo>();

            CreateMap<AgreementCommunicationChannelInfo, CommunicationChannelInfo>();
            CreateMap<CommunicationChannelInfo, AgreementCommunicationChannelInfo>();

            CreateMap<GenerateReturnInfo, GenerateReturnDTO>().ForMember(dest => dest.CommunicationChannels, opt => opt.MapFrom(src => src.CommunicationChannels));
            CreateMap<GenerateReturnDTO, GenerateReturnInfo>().ForMember(dest => dest.CommunicationChannels, opt => opt.MapFrom(src => src.CommunicationChannels));

            CreateMap<LogInfo, LogDTO>();
            CreateMap<LogDTO, LogInfo>();

            CreateMap<GenerateReturnDetailedStatusInfo, GenerateReturnDetailedStatusDTO>();
            CreateMap<GenerateReturnDetailedStatusDTO, GenerateReturnDetailedStatusInfo>();

            CreateMap<OperationDetailedInfo, OperationDetailedDTO>();
            CreateMap<OperationDetailedDTO, OperationDetailedInfo>();
        }
    }
}