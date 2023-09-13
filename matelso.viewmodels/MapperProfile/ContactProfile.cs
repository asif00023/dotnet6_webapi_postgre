

using AutoMapper;
using matelso.dbmodels;
using matelso.viewmodels.RequestModel;

namespace matelso.viewmodels.MapperProfile
{
    public class ContactProfile:Profile
    {
        public ContactProfile()
        {
            //CreateMap<ContactPersonViewModel, ContactPerson>().ReverseMap();
            CreateMap<ContactReqestModel, Contact>().ReverseMap().ForMember(dest => dest.NotifyHasBirthdaySoon, opt => opt.MapFrom(src => String.Empty));
                        
        }
    }
}
