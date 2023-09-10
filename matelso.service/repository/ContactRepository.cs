using AutoMapper;
using matelso.dbmodels;
using matelso.repository.interfaces;
using matelso.viewmodels.ViewModel;
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
        public async Task<ActionResult<IEnumerable<ContactPersonViewModel>>> GetAllContactPersonsAsync()
        {
            
            var contactPersons = await _context.ContactPersons.ToListAsync();
            List<ContactPersonViewModel> contactPersonViewModels = new List<ContactPersonViewModel>();
            ContactPersonViewModel contactPersonViewModel;
            foreach (var contactPerson in contactPersons)
            {
                contactPersonViewModel = _mapper.Map<ContactPersonViewModel>(contactPerson);
                int dayLeftForBirthday = BirthdayRemainingDays((DateTime)contactPersonViewModel.Birthdate);
                if (dayLeftForBirthday <= 14)
                    contactPersonViewModel.NotifyHasBirthdaySoon = "Birthday will be in "+ dayLeftForBirthday + " days";
                contactPersonViewModels.Add(contactPersonViewModel);
            }
            return contactPersonViewModels;
        }

        public async Task<ActionResult<ContactPersonViewModel>> GetContactPersonById(int id)
        {
            var contactPerson = await _context.ContactPersons.FindAsync(id);
            if (contactPerson == null)
                return null;

            ContactPersonViewModel contactPersonVm = _mapper.Map<ContactPersonViewModel>(contactPerson);
            int dayLeftForBirthday = BirthdayRemainingDays((DateTime)contactPersonVm.Birthdate);
            if (dayLeftForBirthday <= 14)
                contactPersonVm.NotifyHasBirthdaySoon = "Birthday will be in " + dayLeftForBirthday + " days";
            

            return contactPersonVm;
        }

        public async Task<(ContactPersonViewModel, HttpStatusCode,string)> UpdateContactPerson(ContactPersonViewModel contactPersonRm,int id)
        {
            HttpStatusCode httpStatusCode;
            string message;

            ContactPerson contactPerson = _mapper.Map<ContactPerson>(contactPersonRm);
            ContactPerson contactPersonolder = await OldContactData(id);
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
            
            
            if (String.IsNullOrEmpty(contactPerson.Displayname))
            {
                contactPerson.Displayname = GenerateDisplayName(contactPerson.Salutation, contactPerson.Firstname, contactPerson.Lastname);
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

        private async Task<ContactPerson> OldContactData(int id)
        {
            return await _context.ContactPersons.FirstOrDefaultAsync(e => e.Id == id);
        }

        //private bool ContactPersonExists(int id)
        //{
        //    return (_context.ContactPersons?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
        
        public async Task<(ContactPersonViewModel, HttpStatusCode,string)> SaveContactPerson(ContactPersonViewModel contactPersonRm)
        {
            HttpStatusCode statusCode;
            
            ContactPerson contactPerson = _mapper.Map<ContactPerson>(contactPersonRm);
            if (String.IsNullOrEmpty(contactPerson.Displayname))
            {
                contactPerson.Displayname = GenerateDisplayName(contactPerson.Salutation,contactPerson.Firstname,contactPerson.Lastname);
            }
            
            contactPerson.CreationTimestamp = DateTime.Now;
            try
            {
                _context.ContactPersons.Add(contactPerson);
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
            contactPersonRm.Displayname=contactPerson.Displayname;

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
            var contactPerson = await _context.ContactPersons.FindAsync(id);
            if (contactPerson == null)
            {
                statusCode= HttpStatusCode.NotFound;
                msg = "Data not found";
                return (id,statusCode,msg);
            }
            try
            {
                _context.ContactPersons.Remove(contactPerson);
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
                count = _context.ContactPersons.Count(x => x.Email == email);
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
                count = _context.ContactPersons.Count(x => x.Email == email&&x.Id!=id);
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
