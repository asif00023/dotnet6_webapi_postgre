using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using AutoMapper;
using matelso.dbmodels;
using matelso.viewmodels.ViewModel;
using matelso.repository.repository;
using matelso.app.core;
using System.Net;

namespace matelso.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactPersonsController : ControllerBase
    {
        private readonly MatelsoDataContext _context;
        private readonly IMapper _mapper;
        private readonly ContactRepository _repository;
        private readonly ILogger<ContactPersonsController> _logger;
        MatelsoResponse matelsoResponse;
        ResponseBody matelsoResponseBody;

        public ContactPersonsController(MatelsoDataContext context, IMapper mapper, ILogger<ContactPersonsController> logger)
        {
            _context = context;
            _mapper = mapper;            
            _logger = logger;
            _repository = new ContactRepository(_context, _mapper);
            matelsoResponse = new MatelsoResponse();
            matelsoResponseBody = new ResponseBody();
        }

        // GET: api/ContactPersons
        [HttpGet]
        public async Task<MatelsoResponse> GetContactPersons()
        {                                   
            var conatctPersons= await _repository.GetAllContactPersonsAsync();

            matelsoResponseBody.StatusCode = HttpStatusCode.OK;
            if (conatctPersons==null||conatctPersons.Value.Count()==0)
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

        // GET: api/ContactPersons/5
        [HttpGet("{id}")]
        public async Task<MatelsoResponse> GetContactPerson(int id)
        {
                       
            var contactPerson = await _repository.GetContactPersonById(id);
            
            matelsoResponseBody.StatusCode = HttpStatusCode.OK;
            if (contactPerson == null)
            {
                matelsoResponseBody.StatusMessage = "No Data Found";
                matelsoResponse.responseBody = matelsoResponseBody;
                return matelsoResponse;
            }
            matelsoResponseBody.objectVal = contactPerson.Value;
            matelsoResponse.responseBody = matelsoResponseBody;
            return matelsoResponse;
        }

        // PUT: api/ContactPersons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<MatelsoResponse> PutContactPerson(int id, ContactPersonViewModel contactPersonRm)
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
            if (_repository.DuplicateEmailAddressForUpdate(contactPersonRm.Email,id))
            {
                matelsoResponseBody.StatusCode = HttpStatusCode.Conflict;
                matelsoResponseBody.StatusMessage = "Email address must be unique";
                matelsoResponse.responseBody = matelsoResponseBody;
                return matelsoResponse;
            }
            ContactPersonViewModel contactPerson;
            (contactPerson,matelsoResponseBody.StatusCode,matelsoResponseBody.StatusMessage)= await _repository.UpdateContactPerson(contactPersonRm,id);
            
            matelsoResponseBody.objectVal = contactPerson;
            matelsoResponse.responseBody = matelsoResponseBody;
            return matelsoResponse;
        }

        // POST: api/ContactPersons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<MatelsoResponse> PostContactPerson(ContactPersonViewModel contactPersonRm)
        {
            if (!ModelState.IsValid)
            {

                //return BadRequest();
                matelsoResponseBody.StatusCode = HttpStatusCode.BadRequest;
                matelsoResponseBody.StatusMessage = "Validation error";
                matelsoResponse.responseBody = matelsoResponseBody;
                return matelsoResponse;
                
            }
            if (_repository.DuplicateEmailAddress(contactPersonRm.Email))
            {
                matelsoResponseBody.StatusCode = HttpStatusCode.Conflict;
                matelsoResponseBody.StatusMessage = "Email address must be unique";
                matelsoResponse.responseBody = matelsoResponseBody;
                return matelsoResponse;
            }
            ContactPersonViewModel contactPerson;
            
            (contactPerson,matelsoResponseBody.StatusCode,matelsoResponseBody.StatusMessage )= await _repository.SaveContactPerson(contactPersonRm);
            
            matelsoResponseBody.objectVal = contactPerson;
            matelsoResponse.responseBody = matelsoResponseBody;
            return matelsoResponse;

            //return CreatedAtAction("GetContactPerson", new { id = contactPerson.Id }, contactPerson);
        }

        // DELETE: api/ContactPersons/5
        [HttpDelete("{id}")]
        public async Task<MatelsoResponse> DeleteContactPerson(int id)
        {
            ////if (_context.ContactPersons == null)
            ////{
            ////    return NotFound();
            ////}
            
            (id, matelsoResponseBody.StatusCode, matelsoResponseBody.StatusMessage)= await _repository.DeleteContactPerson(id);

            matelsoResponseBody.objectVal = id;
            matelsoResponse.responseBody = matelsoResponseBody;
            return matelsoResponse;

            
        }
    }
}