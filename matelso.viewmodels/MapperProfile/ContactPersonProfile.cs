

using AutoMapper;
using matelso.dbmodels;
using matelso.viewmodels.ViewModel;

namespace matelso.viewmodels.MapperProfile
{
    public class ContactPersonProfile:Profile
    {
        public ContactPersonProfile()
        {
            //CreateMap<ContactPersonViewModel, ContactPerson>().ReverseMap();
            CreateMap<ContactPersonViewModel, ContactPerson>().ReverseMap().ForMember(dest => dest.NotifyHasBirthdaySoon, opt => opt.MapFrom(src => String.Empty));
                        
        }
    }
}
