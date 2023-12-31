﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using matelso.dbmodels;
using AutoMapper;
using matelso.app.core;
using matelso.repository.repository;
using System.Net;
using matelso.viewmodels.RequestModel;
using matelso.repository.interfaces;

namespace matelso.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        
        private readonly IMapper _mapper;
        private readonly IContactService _contactService;
        private readonly ILogger<ContactsController> _logger;
        MatelsoResponse matelsoResponse;
        ResponseBody matelsoResponseBody;

        
        public ContactsController(IMapper mapper, ILogger<ContactsController> logger, IContactService contactService)
        {
            _mapper = mapper;
            _logger = logger;
            _contactService = contactService;
            matelsoResponse = new MatelsoResponse();
            matelsoResponseBody = new ResponseBody();
        }

        // GET: api/Contacts
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<MatelsoResponse> GetContacts()
        {
            var conatctPersons = await _contactService.GetAllContactPersonsAsync();

            matelsoResponseBody.StatusCode = HttpStatusCode.OK;
            if (conatctPersons == null || conatctPersons.Value.Count() == 0)
            {
                matelsoResponseBody.StatusMessage = "No data found";
                matelsoResponse.responseBody = matelsoResponseBody;
                return matelsoResponse;
            }

            matelsoResponseBody.StatusMessage = "Object Retrive Successfully";
            matelsoResponseBody.objectVal = conatctPersons.Value;
            matelsoResponse.responseBody = matelsoResponseBody;
            return matelsoResponse;
        }

        
        // GET: api/Contacts/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<MatelsoResponse> GetContact(int id)
        {

            var contactPerson = await _contactService.GetContactPersonById(id);

            matelsoResponseBody.StatusCode = HttpStatusCode.OK;
            if (contactPerson == null)
            {
                matelsoResponseBody.StatusMessage = "No Data Found";
                matelsoResponse.responseBody = matelsoResponseBody;
                
            }
            else
            {
                matelsoResponseBody.StatusMessage = "Object Retrive Successfully";
                matelsoResponseBody.objectVal = contactPerson.Value;
                matelsoResponse.responseBody = matelsoResponseBody;
            }
            
            return matelsoResponse;
        }

        
        // PUT: api/Contacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<MatelsoResponse> PutContact(int id, ContactReqestModel contactPersonRm)
        {
            if (id != contactPersonRm.Id)
            {
                //return BadRequest();
                matelsoResponseBody.StatusCode = HttpStatusCode.BadRequest;
                matelsoResponseBody.StatusMessage = "Id not match";
                matelsoResponse.responseBody = matelsoResponseBody;
                return matelsoResponse;
            }
            if (!ModelState.IsValid)
            {

                //return BadRequest();
                matelsoResponseBody.StatusCode = HttpStatusCode.BadRequest;
                matelsoResponseBody.StatusMessage = "Validation error";
                matelsoResponse.responseBody = matelsoResponseBody;
                return matelsoResponse;

            }
            if (_contactService.DuplicateEmailAddressForUpdate(contactPersonRm.Email, id))
            {
                matelsoResponseBody.StatusCode = HttpStatusCode.Conflict;
                matelsoResponseBody.StatusMessage = "Email address must be unique";
                matelsoResponse.responseBody = matelsoResponseBody;
                return matelsoResponse;
            }
            ContactReqestModel contactPerson;
            (contactPerson, matelsoResponseBody.StatusCode, matelsoResponseBody.StatusMessage) = await _contactService.UpdateContactPerson(contactPersonRm, id);

            matelsoResponseBody.objectVal = contactPerson;
            matelsoResponse.responseBody = matelsoResponseBody;
            return matelsoResponse;
        }

        // POST: api/Contacts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<MatelsoResponse> PostContact(ContactReqestModel contactPersonRm)
        {
            if (!ModelState.IsValid)
            {

                //return BadRequest();
                matelsoResponseBody.StatusCode = HttpStatusCode.BadRequest;
                matelsoResponseBody.StatusMessage = "Validation error";
                matelsoResponse.responseBody = matelsoResponseBody;
                return matelsoResponse;

            }

            if (_contactService.DuplicateEmailAddress(contactPersonRm.Email))
            {
                matelsoResponseBody.StatusCode = HttpStatusCode.Conflict;
                matelsoResponseBody.StatusMessage = "Email address must be unique";
                matelsoResponse.responseBody = matelsoResponseBody;
                return matelsoResponse;
            }
            ContactReqestModel contactPerson;

            (contactPerson, matelsoResponseBody.StatusCode, matelsoResponseBody.StatusMessage) = await _contactService.SaveContactPerson(contactPersonRm);

            matelsoResponseBody.objectVal = contactPerson;
            matelsoResponse.responseBody = matelsoResponseBody;
            return matelsoResponse;

            //return CreatedAtAction("GetContactPerson", new { id = contactPerson.Id }, contactPerson);
        }

        // DELETE: api/ContactPersons/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<MatelsoResponse> DeleteContact(int id)
        {

            (id, matelsoResponseBody.StatusCode, matelsoResponseBody.StatusMessage) = await _contactService.DeleteContactPerson(id);

            matelsoResponseBody.objectVal = id;
            matelsoResponse.responseBody = matelsoResponseBody;
            return matelsoResponse;


        }

        
    }
}
