using AutoMapper;
using matelso.dbmodels;
using matelso.repository.interfaces;
using matelso.viewmodels.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace matelso.repository.repository
{
    public class ContactRepository:IContactRepository
    {
        private readonly MatelsoDataContext _context;
        private readonly IMapper _mapper;
        
        private bool disposed = false;
        
        public ContactRepository()
        {
            _context = new MatelsoDataContext();
        }
        
        public ContactRepository(MatelsoDataContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            
        }
        
        public int BirthdayRemainingDays(DateTime birthday)
        {
            DateTime today = DateTime.Today;
            DateTime next = birthday.AddYears(today.Year - birthday.Year);

            if (next < today)
            {
                if (!DateTime.IsLeapYear(next.Year + 1))
                    next = next.AddYears(1);
                else
                    next = new DateTime(next.Year + 1, birthday.Month, birthday.Day);
            }

            int numDays = (next - today).Days;
            return numDays;
        }
        public async Task<ActionResult<IEnumerable<ContactReqestModel>>> GetAllContactPersonsAsync()
        {
            
            var contactPersons = await _context.Contacts.ToListAsync();
            List<ContactReqestModel> contactPersonViewModels = new List<ContactReqestModel>();
            ContactReqestModel contactPersonViewModel;
            foreach (var contactPerson in contactPersons)
            {
                contactPersonViewModel = _mapper.Map<ContactReqestModel>(contactPerson);
                int dayLeftForBirthday = BirthdayRemainingDays((DateTime)contactPersonViewModel.Birthdate);
                if (dayLeftForBirthday <= 14)
                    contactPersonViewModel.NotifyHasBirthdaySoon = "Birthday will be in "+ dayLeftForBirthday + " days";
                contactPersonViewModels.Add(contactPersonViewModel);
            }
            return contactPersonViewModels;
        }

        public async Task<ActionResult<ContactReqestModel>> GetContactPersonById(int id)
        {
            var contactPerson = await _context.Contacts.FindAsync(id);
            if (contactPerson == null)
                return null;

            ContactReqestModel contactPersonVm = _mapper.Map<ContactReqestModel>(contactPerson);
            int dayLeftForBirthday = BirthdayRemainingDays((DateTime)contactPersonVm.Birthdate);
            if (dayLeftForBirthday <= 14)
                contactPersonVm.NotifyHasBirthdaySoon = "Birthday will be in " + dayLeftForBirthday + " days";
            

            return contactPersonVm;
        }

        public async Task<(ContactReqestModel, HttpStatusCode,string)> UpdateContactPerson(ContactReqestModel contactPersonRm,int id)
        {
            HttpStatusCode httpStatusCode;
            string message;

            Contact contactPerson = _mapper.Map<Contact>(contactPersonRm);
            Contact contactPersonolder = await OldContactData(id);
            if (contactPersonolder == null)
            {
                Log.Error("No Data found with this id");
                httpStatusCode = HttpStatusCode.Conflict;
                message = "No Data found with this id";
                return (contactPersonRm, httpStatusCode, message);
            }
            else
            {
                contactPerson.CreationTimestamp = contactPersonolder.CreationTimestamp;
                contactPerson.LastChangeTimestamp = DateTime.Now;
                _context.Entry(contactPersonolder).State = EntityState.Detached;                
            }
            
            
            if (String.IsNullOrEmpty(contactPerson.DisplayName))
            {
                contactPerson.DisplayName = GenerateDisplayName(contactPerson.Salutation, contactPerson.FirstName, contactPerson.LastName);
            }
            _context.Entry(contactPerson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                httpStatusCode = HttpStatusCode.OK;
            }
            catch (DbUpdateConcurrencyException r)
            {
                Log.Error(r.Message);
                httpStatusCode = HttpStatusCode.InternalServerError;
                message = r.Message;
                return (contactPersonRm, httpStatusCode, message);
            }
            int dayLeftForBirthday = BirthdayRemainingDays((DateTime)contactPersonRm.Birthdate);
            if (dayLeftForBirthday <= 14)
                contactPersonRm.NotifyHasBirthdaySoon = "Birthday will be in " + dayLeftForBirthday + " days";
            return (contactPersonRm, httpStatusCode,"Data Updated");
            //return true;
        }

        private async Task<Contact> OldContactData(int id)
        {
            return await _context.Contacts.FirstOrDefaultAsync(e => e.Id == id);
        }

        //private bool ContactPersonExists(int id)
        //{
        //    return (_context.ContactPersons?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
        
        public async Task<(ContactReqestModel, HttpStatusCode,string)> SaveContactPerson(ContactReqestModel contactPersonRm)
        {
            HttpStatusCode statusCode;
            
            Contact contactPerson = _mapper.Map<Contact>(contactPersonRm);
            if (String.IsNullOrEmpty(contactPerson.DisplayName))
            {
                contactPerson.DisplayName = GenerateDisplayName(contactPerson.Salutation,contactPerson.FirstName,contactPerson.LastName);
            }
            
            contactPerson.CreationTimestamp = DateTime.Now;
            try
            {
                _context.Contacts.Add(contactPerson);
                await _context.SaveChangesAsync();
                statusCode = HttpStatusCode.Created;
            }
            catch (DbException r)
            {
                statusCode = HttpStatusCode.InternalServerError;
                Log.Error(r.Message);
                //throw r;
                return (contactPersonRm, statusCode,r.Message);
            }
            contactPersonRm.Id = contactPerson.Id;
            contactPersonRm.Displayname=contactPerson.DisplayName;

            int dayLeftForBirthday = BirthdayRemainingDays((DateTime)contactPersonRm.Birthdate);
            if (dayLeftForBirthday <= 14)
                contactPersonRm.NotifyHasBirthdaySoon = "Birthday will be in " + dayLeftForBirthday + " days";
            else
                contactPersonRm.NotifyHasBirthdaySoon = "";
            return (contactPersonRm, statusCode,"Data Saved Success");
        }

        public async Task<(int,HttpStatusCode,string)> DeleteContactPerson(int id)
        {
            HttpStatusCode statusCode;
            string msg;
            var contactPerson = await _context.Contacts.FindAsync(id);
            if (contactPerson == null)
            {
                statusCode= HttpStatusCode.NotFound;
                msg = "Data not found";
                return (id,statusCode,msg);
            }
            try
            {
                _context.Contacts.Remove(contactPerson);
                await _context.SaveChangesAsync();
                statusCode = HttpStatusCode.Accepted;
                msg = "Data has been deleted";
            }
            catch (DbException r)
            {
                Log.Error(r.Message);
                statusCode = HttpStatusCode.InternalServerError;
                msg = r.Message;
                return (id,statusCode,msg);
            }
            return (id, statusCode, msg);

        }
        public string GenerateDisplayName(string salutation, string firstName, string lastName)
        {
            return salutation + " " + firstName + " " + lastName;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool DuplicateEmailAddress(string email)
        {
            int count;
            try
            {
                count = _context.Contacts.Count(x => x.Email == email);
            }
            catch (Exception r)
            {
                Log.Error(r.Message);
                count = 1;
            }
            if (count > 0)
                return true;
            else
                return false;
            
        }

        public bool DuplicateEmailAddressForUpdate(string email, int id)
        {
            int count;
            try
            {
                count = _context.Contacts.Count(x => x.Email == email&&x.Id!=id);
            }
            catch (Exception r)
            {
                Log.Error(r.Message);
                count = 1;
            }
            if (count > 0)
                return true;
            else
                return false;
        }
    }
}
