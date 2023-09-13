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
        Task<ActionResult<IEnumerable<ContactPersonReqestModel>>> GetAllContactPersonsAsync();
        Task<ActionResult<ContactPersonReqestModel>> GetContactPersonById(int id);

        Task<(ContactPersonReqestModel, HttpStatusCode, string)> UpdateContactPerson(ContactPersonReqestModel contactPersonRm, int id);
        Task<(ContactPersonReqestModel, HttpStatusCode, string)> SaveContactPerson(ContactPersonReqestModel contactPersonRm);
        Task<(int, HttpStatusCode, string)> DeleteContactPerson(int id);
        
    }
}
