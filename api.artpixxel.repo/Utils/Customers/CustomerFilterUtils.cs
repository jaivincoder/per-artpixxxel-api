

using api.artpixxel.data.Features.Customers;
using api.artpixxel.data.Models;
using api.artpixxel.repo.Extensions;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace api.artpixxel.repo.Utils.Customers
{
    public static class CustomerFilterUtils
    {


        public static bool EmptyFilter(CustomerFilterData filter)
        {
            return ((string.IsNullOrEmpty(filter.Search)) && ((filter.Filters.TotalOrder == -1) && string.IsNullOrEmpty(filter.Filters.TotalOrderMatchMode))
               && (string.IsNullOrEmpty(filter.Filters.DateRegistered))
               && (!filter.Filters.Categories.Any()) && (!filter.Filters.Cities.Any()) && (!filter.Filters.Countries.Any()) && (!filter.Filters.States.Any()));
        }
        public static ExpressionStarter<Customer> ApplyFilter(CustomerFilterData filter)
        {
            ExpressionStarter<Customer> predicate = PredicateBuilder.New<Customer>();

          

            if (filter.Filters.Categories.Any())
            {

            
                predicate = predicate.And(e => filter.Filters.Categories.Contains(e.Category.Name));
            }
            if (filter.Filters.Cities.Any())
            {
                predicate = predicate.And(e => (e.CityId != null) && (filter.Filters.Cities.Contains(e.City.CityName)));
            }

            if (filter.Filters.Countries.Any())
            {
                predicate = predicate.And(e => (e.CityId != null) && (filter.Filters.Countries.Contains(e.City.State.Country.Name)));
            }

            if (filter.Filters.States.Any())
            {
                predicate = predicate.And(e => (e.CityId != null) && (filter.Filters.States.Contains(e.City.State.StateName)));
            }

            if (!string.IsNullOrEmpty(filter.Filters.DateRegistered))
            {
                DateTime date = DateTime.ParseExact(filter.Filters.DateRegistered, DefaultDateFormat.ddMMyyyy, CultureInfo.InvariantCulture);
                if (filter.Filters.DateRegisteredMatchMode == MatchMode.DateIS)
                {
                    predicate = predicate.And(e => e.CreatedOn.Date == date);
                }
                else if (filter.Filters.DateRegisteredMatchMode == MatchMode.DateIsNT)
                {
                    predicate = predicate.And(e => e.CreatedOn != date);
                }

                else if (filter.Filters.DateRegisteredMatchMode == MatchMode.DateAfter)
                {
                    predicate = predicate.And(e => e.CreatedOn > date);
                }
                else if (filter.Filters.DateRegisteredMatchMode == MatchMode.DateBefore)
                {
                    predicate = predicate.And(e => e.CreatedOn < date);
                }

            }


            if (filter.Filters.TotalOrder > -1)
            {
                if (filter.Filters.TotalOrderMatchMode == Operators.EQUALS)
                {
                    predicate = predicate.And(e => e.TotalOrder == filter.Filters.TotalOrder);
                }
                else if (filter.Filters.TotalOrderMatchMode == Operators.GREATER_THAN)
                {
                   predicate = predicate.And(e => e.TotalOrder > filter.Filters.TotalOrder);
                }
                else if (filter.Filters.TotalOrderMatchMode == Operators.GREATER_THAN_OR_EQUAL_TO)
                {
                    predicate = predicate.And(e => e.TotalOrder >= filter.Filters.TotalOrder);
                }
                else if (filter.Filters.TotalOrderMatchMode == Operators.LESS_THAN)
                {
                    predicate = predicate.And(e => e.TotalOrder < filter.Filters.TotalOrder);
                }
                else if (filter.Filters.TotalOrderMatchMode == Operators.LESS_THAN_OR_EQUALTO)
                {
                    predicate = predicate.And(e => e.TotalOrder <= filter.Filters.TotalOrder);
                }
                else if (filter.Filters.TotalOrderMatchMode == Operators.NOT_EQUALS)
                {
                    predicate = predicate.And(e => e.TotalOrder != filter.Filters.TotalOrder);
                }
            }


            if (!string.IsNullOrEmpty(filter.Search))
            {





                predicate = predicate.And(e =>
                ((e.User.FirstName.Contains(filter.Search) ||
                  e.User.LastName.Contains(filter.Search)) && (e.User.MiddleName == null || e.User.MiddleName == string.Empty))
                  || ((e.User.FirstName.Contains(filter.Search) || e.User.LastName.Contains(filter.Search)) || (e.User.MiddleName != null && e.User.MiddleName.Contains(filter.Search))) 
             || (e.User.PhoneNumber.Contains(filter.Search)
             || (e.User.Email.Contains(filter.Search))
             || (e.User.UserName.Contains(filter.Search))
             || (string.IsNullOrEmpty(e.User.StateId) ? e.User.StateId != null : e.User.State.StateName.Contains(filter.Search))
             || (string.IsNullOrEmpty(e.User.StateId) ? e.User.StateId != null : e.User.State.Country.Name.Contains(filter.Search))
             || (string.IsNullOrEmpty(e.CityId) ? e.CityId != null : e.City.CityName.Contains(filter.Search))
             || (e.Category.Name.Contains(filter.Search))
             ));




            }



            return predicate;

        }





        public static List<CustomerModel> ApplySort(List<CustomerModel> query, CustomerFilterData filter)
        {
            if (!string.IsNullOrEmpty(filter.SortField))
            {
                if (filter.SortOrder == 1) // ascending
                {
                    if (filter.SortField == CustomerSortField.CATEGORY)
                    {
                        query = query.OrderBy(e => e.Category.Name).ToList();
                    }

                    else if (filter.SortField == CustomerSortField.CITY)
                    {

                        query = query.OrderBy(e => e?.City?.Name).ToList();
                    }
                    else if (filter.SortField == CustomerSortField.COUNTRY)
                    {

                        query = query.OrderBy(e => e?.Country?.Name).ToList();
                    }
                    else if (filter.SortField == CustomerSortField.DATEREGISTERED)
                    {
                      
                        query = query.OrderBy(e => e.DateRegistered.DDMMYYYY()).ToList();
                    }

                    else if (filter.SortField == CustomerSortField.EMAILADDRESS)
                    {

                        query = query.OrderBy(e => e.EmailAddress).ToList();
                    }
                    else if (filter.SortField == CustomerSortField.FULLNAME)
                    {

                        query = query.OrderBy(e => e.FirstName).ToList();
                    }
                    if (filter.SortField == CustomerSortField.LASTLOGIN)
                    {

                        query = query.OrderBy(e => e?.LastLoginDate?.DDMMYYYYHHMMSStt()).ToList();
                    }
                    if (filter.SortField == CustomerSortField.STATE)
                    {

                        query = query.OrderBy(e => e?.State?.Name).ToList();
                    }
                    if (filter.SortField == CustomerSortField.TOTALORDER)
                    {

                        query = query.OrderBy(e => e.TotalOrder).ToList();
                    }
                    if (filter.SortField == CustomerSortField.USERNAME)
                    {

                        query = query.OrderBy(e => e.Username).ToList();
                    }


                }

                else
                {
                    if (filter.SortField == CustomerSortField.CATEGORY)
                    {
                        query = query.OrderByDescending(e => e.Category.Name).ToList();
                    }

                    else if (filter.SortField == CustomerSortField.CITY)
                    {

                        query = query.OrderByDescending(e => e?.City?.Name).ToList();
                    }
                    else if (filter.SortField == CustomerSortField.COUNTRY)
                    {

                        query = query.OrderByDescending(e => e?.State?.Name).ToList();
                    }
                    else if (filter.SortField == CustomerSortField.DATEREGISTERED)
                    {

                        query = query.OrderByDescending(e => e.DateRegistered.DDMMYYYY()).ToList();
                    }

                    else if (filter.SortField == CustomerSortField.EMAILADDRESS)
                    {

                        query = query.OrderByDescending(e => e.EmailAddress).ToList();
                    }
                    else if (filter.SortField == CustomerSortField.FULLNAME)
                    {

                        query = query.OrderByDescending(e => e.FirstName).ToList();
                    }
                    if (filter.SortField == CustomerSortField.LASTLOGIN)
                    {

                        query = query.OrderByDescending(e => e?.LastLoginDate?.DDMMYYYYHHMMSStt()).ToList();
                    }
                    if (filter.SortField == CustomerSortField.STATE)
                    {

                        query = query.OrderByDescending(e => e?.State?.Name).ToList();
                    }
                    if (filter.SortField == CustomerSortField.TOTALORDER)
                    {

                        query = query.OrderByDescending(e => e.TotalOrder).ToList();
                    }
                    if (filter.SortField == CustomerSortField.USERNAME)
                    {

                        query = query.OrderByDescending(e => e.Username).ToList();
                    }

                }
            }

            return query;

        }


            
        
    }
}
