

using api.artpixxel.data.Features.AddressBooks;
using api.artpixxel.data.Services;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using api.artpixxel.data.Features.Common;
using Microsoft.Data.SqlClient;
using api.artpixxel.data.Models;
using Microsoft.AspNetCore.Identity;

namespace api.artpixxel.repo.Features.AddressBooks
{
    public class AddressBookService : IAddressBookService
    {

        private readonly ArtPixxelContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;

        public AddressBookService(ArtPixxelContext context, ICurrentUserService currentUserService, UserManager<User> userManager)
        {
            _context = context;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }
        public async Task<AddressBookCRUDResponse> Create(AddressBookRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                Customer customer = await _context.Customers
                    .Where(e => e.UserId == _currentUserService.GetUserId())
                    .Include(e => e.User)
                    .FirstOrDefaultAsync();

                if (customer != null)
                {

                    if (string.IsNullOrEmpty(request.Id))
                    {
                        if (!await _context.AddressBooks.AnyAsync(e => e.CustomerId == customer.Id))
                        {
                            if (request.IsDefault == false)
                            {
                                @request.IsDefault = true;
                            }

                        }

                        else
                        {
                            if (@request.IsDefault == true)
                            {
                                AddressBook defaultAddressBook = await _context.AddressBooks.Where(e => e.CustomerId == customer.Id && e.IsDefault == true).FirstOrDefaultAsync();
                                if (defaultAddressBook != null)
                                {
                                    defaultAddressBook.IsDefault = false;
                                    _context.AddressBooks.Update(defaultAddressBook);
                                    await _context.SaveChangesAsync();

                                }
                            }
                        }


                        AddressBook addressBook = new()
                        {
                            AdditionalInformation = @request.AdditionalInformation,
                            Address = @request.Address,
                            //CityId = _context.Cities.Find(@request.CityId).Id,
                            StateId = _context.States.Find(request.StateId).Id,
                            CountryId = _context.Countries.Find(request.CountryId).Id,
                            CityName = request.CityName,
                            CustomerId = _context.Customers.Find(customer.Id).Id,
                            IsDefault = @request.IsDefault,
                            Name = @request.Name

                        };

                        //  customer.CityId = _context.Cities.Find(@request.CityId).Id;
                        customer.CityName = request.CityName;
                        customer.User.HomeAddress = request.Address;

                        _context.Customers.Update(customer);
                        await _context.SaveChangesAsync();

                        _context.AddressBooks.Add(addressBook);
                        int saveResult = await _context.SaveChangesAsync();

                        if (saveResult > 0)
                        {
                            return new AddressBookCRUDResponse
                            {
                                AddressBooks = await AddressBooks(customer),
                                Response = new BaseResponse
                                {
                                    Message = "Address book created.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }
                    }


                    else
                    {
                        AddressBook addressBook = await _context.AddressBooks.FindAsync(@request.Id);
                        if (addressBook != null)
                        {
                            if (@request.IsDefault)
                            {
                                List<AddressBook> addressBooks = await _context.AddressBooks.Where(c => c.CustomerId == customer.Id && (c.Id != @request.Id)).ToListAsync();
                                if (addressBooks.Any())
                                {
                                    foreach (AddressBook _addressBook in addressBooks)
                                    {
                                        _addressBook.IsDefault = false;
                                    }

                                    //customer.CityId = _context.Cities.Find(@request.CityId).Id;
                                    customer.CityName = request.CityName;
                                    customer.User.HomeAddress = request.Address;

                                    _context.Customers.Update(customer);
                                    await _context.SaveChangesAsync();


                                    _context.AddressBooks.UpdateRange(addressBooks);
                                    await _context.SaveChangesAsync();
                                }
                            }

                            else
                            {
                                AddressBook address = await _context.AddressBooks.Where(c => c.CustomerId == customer.Id && (c.Id != @request.Id)).FirstOrDefaultAsync();
                                if (address != null)
                                {
                                    address.IsDefault = true;

                                    //customer.CityId = _context.Cities.Find(address.CityId).Id;
                                    customer.CityName = address.CityName;
                                    customer.User.HomeAddress = address.Address;

                                    _context.Customers.Update(customer);
                                    await _context.SaveChangesAsync();

                                    _context.Update(address);
                                    await _context.SaveChangesAsync();
                                }


                                else
                                {
                                    @request.IsDefault = true;

                                    //customer.CityId = _context.Cities.Find(@request.CityId).Id;
                                    customer.CityName = request.CityName;
                                    customer.User.HomeAddress = request.Address;

                                    _context.Customers.Update(customer);
                                    await _context.SaveChangesAsync();
                                }
                            }

                            addressBook.AdditionalInformation = @request.AdditionalInformation;
                            addressBook.Address = @request.Address;
                            //addressBook.CityId = _context.Cities.Find(@request.CityId).Id;
                            addressBook.CityName = request.CityName;
                            if (addressBook.StateId != request.StateId)
                            {
                                addressBook.StateId = _context.States.Find(request.StateId).Id;
                            }

                            if (addressBook.CountryId != request.CountryId)
                            {
                                addressBook.CountryId = _context.Countries.Find(request.CountryId).Id;
                            }

                            addressBook.IsDefault = @request.IsDefault;
                            addressBook.Name = @request.Name;


                            _context.AddressBooks.Update(addressBook);
                            int updateResult = await _context.SaveChangesAsync();
                            if (updateResult > 0)
                            {
                                return new AddressBookCRUDResponse
                                {
                                    AddressBooks = await AddressBooks(customer),
                                    Response = new BaseResponse
                                    {
                                        Message = "Address book updated.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };
                            }
                        }

                        return new AddressBookCRUDResponse
                        {
                            AddressBooks = await AddressBooks(customer),
                            Response = new BaseResponse
                            {
                                Message = "Reference to addressbook couldn't be resolved. This address book may have been removed.",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Null AddressBook Reference"
                            }
                        };
                    }

                }


                return new AddressBookCRUDResponse
                {
                    AddressBooks = new List<CustomerAddressBook>(),
                    Response = new BaseResponse
                    {
                        Message = "Account couldn't be verified",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Authentication Failure"
                    }
                };


            }
            catch (Exception ex)
            {

                return new AddressBookCRUDResponse
                {
                    AddressBooks = new List<CustomerAddressBook>(),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source

                    }
                };
            }
        }

        public async Task<AddressBookCRUDResponse> Delete(AddressBookDelete request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if (await _context.Customers.AnyAsync(e => e.Id == @request.CustomerId))
                {
                    if (await _context.AddressBooks.AnyAsync(e => e.Id == @request.Id))
                    {
                        AddressBook addressBook = await _context.AddressBooks.FindAsync(@request.Id);
                        if (addressBook != null)
                        {
                            _context.AddressBooks.Remove(addressBook);
                            await _context.SaveChangesAsync();

                            var rt = new AddressBookCRUDResponse
                            {
                                AddressBooks = await AddressBooks(request.CustomerId),
                                Response = new BaseResponse
                                {
                                    Message = "Address book removal succeded.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }

                            };

                            return rt;

                        }

                    }


                    return new AddressBookCRUDResponse
                    {
                        AddressBooks = await AddressBooks(request.CustomerId),
                        Response = new BaseResponse
                        {
                            Message = "Address book not found. This address book may have been deleted.",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Null AddressBook Reference"
                        }

                    };
                }


                return new AddressBookCRUDResponse
                {
                    AddressBooks = new List<CustomerAddressBook>(),
                    Response = new BaseResponse
                    {
                        Message = "Account couldn't be verified",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Authentication Failure"
                    }

                };
            }
            catch (Exception)
            {

                throw;
            }
        }


        private async Task<List<CustomerAddressBook>> AddressBooks(string customerId)
        {
            try
            {
                return await _context.AddressBooks.Where(e => e.CustomerId == customerId)
                    .Include(c => c.City)
                     .Include(st => st.State)
                     .Include(s => s.Country).ThenInclude(f => f.Flag)
                    .Select(a => new CustomerAddressBook
                    {
                        AdditionalInformation = a.AdditionalInformation,
                        Address = a.Address,
                        City = string.IsNullOrEmpty(a.CityId) ? new BaseOption { Id = Guid.NewGuid().ToString(), Name = a.CityName } : new BaseOption { Id = a.City.Id, Name = a.City.CityName },
                        State = string.IsNullOrEmpty(a.StateId) ? new BaseOption { Id = string.Empty, Name = string.Empty } : new BaseOption { Id = a.State.Id, Name = a.State.StateName },
                        Country = string.IsNullOrEmpty(a.CountryId) ? new CountryOption { Id = string.Empty, Name = string.Empty, Flag = string.Empty } : new CountryOption { Id = a.Country.Id, Name = a.Country.Name, Flag = a.Country.Flag.Name },
                        CustomerId = a.CustomerId,
                        Id = a.Id,
                        IsDefault = a.IsDefault,
                        Name = a.Name

                    }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<List<CustomerAddressBook>> AddressBooks(Customer customer)
        {
            try
            {
                return await _context.AddressBooks.Where(e => e.CustomerId == customer.Id)
                    .Include(c => c.City)
                    .Include(st => st.State)
                    .Include(s => s.Country).ThenInclude(f => f.Flag)
                    .Select(a => new CustomerAddressBook
                    {
                        AdditionalInformation = a.AdditionalInformation,
                        Address = a.Address,
                        //City = new BaseOption { Id = a.City.Id, Name = a.City.CityName },
                        City = string.IsNullOrEmpty(a.CityId) ? new BaseOption { Id = Guid.NewGuid().ToString(), Name = a.CityName } : new BaseOption { Id = a.City.Id, Name = a.City.CityName },
                        State = string.IsNullOrEmpty(a.StateId) ? new BaseOption { Id = string.Empty, Name = string.Empty } : new BaseOption { Id = a.State.Id, Name = a.State.StateName },
                        Country = string.IsNullOrEmpty(a.CountryId) ? new CountryOption { Id = string.Empty, Name = string.Empty, Flag = string.Empty } : new CountryOption { Id = a.Country.Id, Name = a.Country.Name, Flag = a.Country.Flag.Name },
                        CustomerId = a.CustomerId,
                        Id = a.Id,
                        IsDefault = a.IsDefault,
                        Name = a.Name

                    }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
