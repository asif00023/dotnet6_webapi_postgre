using matelso.dbmodels;
using matelso.viewmodels.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace matelso.repository.interfaces
{
    public interface IContactRepository
    {
        Task<ActionResult<IEnumerable<ContactPersonViewModel>>> GetAllContactPersonsAsync();
        Task<ActionResult<ContactPersonViewModel>> GetContactPersonById(int id);

        Task<(ContactPersonViewModel, HttpStatusCode, string)> UpdateContactPerson(ContactPersonViewModel contactPersonRm, int id);
        Task<(ContactPersonViewModel, HttpStatusCode, string)> SaveContactPerson(ContactPersonViewModel contactPersonRm);
        Task<(int, HttpStatusCode, string)> DeleteContactPerson(int id);
        
    }
}
