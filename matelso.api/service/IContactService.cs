﻿using matelso.dbmodels;
using matelso.viewmodels.RequestModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace matelso.repository.interfaces
{
    public interface IContactService
    {
        Task<ActionResult<IEnumerable<ContactReqestModel>>> GetAllContactPersonsAsync();
        Task<ActionResult<ContactReqestModel>> GetContactPersonById(int id);

        Task<(ContactReqestModel, HttpStatusCode, string)> UpdateContactPerson(ContactReqestModel contactPersonRm, int id);
        Task<(ContactReqestModel, HttpStatusCode, string)> SaveContactPerson(ContactReqestModel contactPersonRm);
        Task<(int, HttpStatusCode, string)> DeleteContactPerson(int id);
        bool DuplicateEmailAddress(string email);
        bool DuplicateEmailAddressForUpdate(string email, int id);


    }
}
