

using api.artpixxel.data.Features.Orders;
using api.artpixxel.data.Models;
using api.artpixxel.repo.Extensions;
using api.artpixxel.repo.Utils.Models;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace api.artpixxel.repo.Utils.Orders
{
   public static class OrderFilterUtils
    {
        public static bool EmptyFilter(OrderFilterData filter)
        {
            return ((string.IsNullOrEmpty(filter.Search)) && ((filter.Filters.SubTotal == -1) && string.IsNullOrEmpty(filter.Filters.SubTotalMatchMode))
               && (string.IsNullOrEmpty(filter.Filters.OrderDate))
               && (!filter.Filters.Cities.Any()) 
               && (!filter.Filters.Countries.Any()) 
               && (!filter.Filters.FullNames.Any())
               && (!filter.Filters.OrderStates.Any())
               && (!filter.Filters.OrderStatuses.Any())
               && (!filter.Filters.States.Any()));
        }

        public static ExpressionStarter<Order> ApplyFilter(OrderFilterData filter)
        {
            try
            {
                ExpressionStarter<Order> predicate = PredicateBuilder.New<Order>();

                if (filter.Filters.Cities.Any())
                {


                    predicate = predicate.And(e => filter.Filters.Cities.Contains(e.City.CityName));
                }


                if (filter.Filters.Countries.Any())
                {


                    predicate = predicate.And(e => filter.Filters.Countries.Contains(e.Country.Name));
                }

                if (filter.Filters.FullNames.Any())
                {

                    SplittedName splitName = GlobalUtils.SplittName(filter.Filters.FullNames);

                    predicate = predicate.And(e => splitName.FirstNames.Contains(e.Customer.User.FirstName) && splitName.LastNames.Contains(e.Customer.User.LastName));
                }

                if (filter.Filters.OrderStates.Any())
                {
                   
                    foreach (string orderState in filter.Filters.OrderStates)
                    {
                        OrderState _orderState;
                        
                        if (Enum.TryParse(orderState, out _orderState))
                        {
                            OrderState state = (OrderState)Enum.Parse(typeof(OrderState), orderState);
                           
                                predicate = predicate.Or(e => e.OrderState == state);
                           

                        }
                    }
                  

                       
                }


                if (filter.Filters.OrderStatuses.Any())
                {


                    predicate = predicate.And(e => filter.Filters.OrderStatuses.Contains(e.Status.Label));
                }


                if (filter.Filters.States.Any())
                {


                    predicate = predicate.And(e => filter.Filters.States.Contains(e.State.StateName));
                }

                if (!string.IsNullOrEmpty(filter.Filters.OrderDate))
                {
                    DateTime date = DateTime.ParseExact(filter.Filters.OrderDate, DefaultDateFormat.ddMMyyyy, CultureInfo.InvariantCulture);
                    if (filter.Filters.OrderDateMatchMode == MatchMode.DateIS)
                    {
                        predicate = predicate.And(e => e.CreatedOn.Date == date);
                    }
                    else if (filter.Filters.OrderDateMatchMode == MatchMode.DateIsNT)
                    {
                        predicate = predicate.And(e => e.CreatedOn.Date != date);
                    }

                    else if (filter.Filters.OrderDateMatchMode == MatchMode.DateAfter)
                    {
                        predicate = predicate.And(e => e.CreatedOn.Date > date);
                    }
                    else if (filter.Filters.OrderDateMatchMode == MatchMode.DateBefore)
                    {
                        predicate = predicate.And(e => e.CreatedOn.Date < date);
                    }

                }


                if (filter.Filters.SubTotal > -1)
                {
                    if (filter.Filters.SubTotalMatchMode == Operators.EQUALS)
                    {
                        predicate = predicate.And(e => e.SubTotal == filter.Filters.SubTotal);
                    }
                    else if (filter.Filters.SubTotalMatchMode == Operators.GREATER_THAN)
                    {
                        predicate = predicate.And(e => e.SubTotal > filter.Filters.SubTotal);
                    }
                    else if (filter.Filters.SubTotalMatchMode == Operators.GREATER_THAN_OR_EQUAL_TO)
                    {
                        predicate = predicate.And(e => e.SubTotal >= filter.Filters.SubTotal);
                    }
                    else if (filter.Filters.SubTotalMatchMode == Operators.LESS_THAN)
                    {
                        predicate = predicate.And(e => e.SubTotal < filter.Filters.SubTotal);
                    }
                    else if (filter.Filters.SubTotalMatchMode == Operators.LESS_THAN_OR_EQUALTO)
                    {
                        predicate = predicate.And(e => e.SubTotal <= filter.Filters.SubTotal);
                    }
                    else if (filter.Filters.SubTotalMatchMode == Operators.NOT_EQUALS)
                    {
                        predicate = predicate.And(e => e.SubTotal != filter.Filters.SubTotal);
                    }
                }


                if (!string.IsNullOrEmpty(filter.Search))
                {


                    OrderState orderState;
                    if (Enum.TryParse(filter.Search, out orderState))
                    {
                        OrderState state = (OrderState)Enum.Parse(typeof(OrderState), filter.Search);
                        predicate = predicate.And(e =>
                   ((e.Customer.User.FirstName.Contains(filter.Search) || e.Customer.User.LastName.Contains(filter.Search)) && (e.Customer.User.MiddleName == null || e.Customer.User.MiddleName == string.Empty))
                   || ((e.Customer.User.FirstName.Contains(filter.Search) || e.Customer.User.LastName.Contains(filter.Search)) || (e.Customer.User.MiddleName != null && e.Customer.User.MiddleName.Contains(filter.Search)))
                || (e.OrderState == state)
                || (e.Status.Label.Contains(filter.Search))

                || (string.IsNullOrEmpty(e.StateId) ? e.StateId != null : e.State.StateName.Contains(filter.Search))
                || (string.IsNullOrEmpty(e.CityId) ? e.CityId != null : e.City.CityName.Contains(filter.Search))
                 || (string.IsNullOrEmpty(e.CountryId) ? e.CountryId != null : e.Country.Name.Contains(filter.Search))

                || (e.InvoiceNumber.Contains(filter.Search))
                );
                    }

                    else
                    {
                        predicate = predicate.And(e =>
                   ((e.Customer.User.FirstName.Contains(filter.Search) || e.Customer.User.LastName.Contains(filter.Search)) && (e.Customer.User.MiddleName == null || e.Customer.User.MiddleName == string.Empty))
                   || ((e.Customer.User.FirstName.Contains(filter.Search) || e.Customer.User.LastName.Contains(filter.Search)) || (e.Customer.User.MiddleName != null && e.Customer.User.MiddleName.Contains(filter.Search)))
               
                || (e.Status.Label.Contains(filter.Search))

                || (string.IsNullOrEmpty(e.StateId) ? e.StateId != null : e.State.StateName.Contains(filter.Search))
                || (string.IsNullOrEmpty(e.CityId) ? e.CityId != null : e.City.CityName.Contains(filter.Search))
                 || (string.IsNullOrEmpty(e.CountryId) ? e.CountryId != null : e.Country.Name.Contains(filter.Search))

                || (e.InvoiceNumber.Contains(filter.Search))
                );

                    }


                       




                }


                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
           
        }


        public static List<OrderModel> ApplySort(List<OrderModel> query, OrderFilterData filter)
        {
            if (!string.IsNullOrEmpty(filter.SortField))
            {
                if (filter.SortOrder == 1) // ascending
                {
                    if (filter.SortField == OrderSortField.INVOICENUMBER)
                    {
                        query = query.OrderBy(e => e.InvoiceNumber).ToList();
                    }

                    else if (filter.SortField == OrderSortField.CITY)
                    {

                        query = query.OrderBy(e => e?.City?.Name).ToList();
                    }
                    else if (filter.SortField == OrderSortField.COUNTRY)
                    {

                        query = query.OrderBy(e => e?.Country?.Name).ToList();
                    }
                    else if (filter.SortField == OrderSortField.DATEFILTERED)
                    {

                        query = query.OrderBy(e => e.Date.DDMMYYYY()).ToList();
                    }

                    else if (filter.SortField == OrderSortField.FULLNAME)
                    {

                        query = query.OrderBy(e => e.FullName).ToList();
                    }
                   
                 
                    if (filter.SortField == OrderSortField.STATE)
                    {

                        query = query.OrderBy(e => e?.State?.Name).ToList();
                    }
                    if (filter.SortField == OrderSortField.SUBTOTAL)
                    {

                        query = query.OrderBy(e => e.SubTotal).ToList();
                    }
                    if (filter.SortField == OrderSortField.ORDERSTATE)
                    {

                        query = query.OrderBy(e => e.OrderState.Name).ToList();
                    }

                    if (filter.SortField == OrderSortField.STATUS)
                    {

                        query = query.OrderBy(e => e.Status.Label).ToList();
                    }


                }
                else
                {
                    if (filter.SortField == OrderSortField.INVOICENUMBER)
                    {
                        query = query.OrderByDescending(e => e.InvoiceNumber).ToList();
                    }

                    else if (filter.SortField == OrderSortField.CITY)
                    {

                        query = query.OrderByDescending(e => e?.City?.Name).ToList();
                    }
                    else if (filter.SortField == OrderSortField.COUNTRY)
                    {

                        query = query.OrderByDescending(e => e?.Country?.Name).ToList();
                    }
                    else if (filter.SortField == OrderSortField.DATEFILTERED)
                    {

                        query = query.OrderByDescending(e => e.Date.DDMMYYYY()).ToList();
                    }

                    else if (filter.SortField == OrderSortField.FULLNAME)
                    {

                        query = query.OrderByDescending(e => e.FullName).ToList();
                    }


                    if (filter.SortField == OrderSortField.STATE)
                    {

                        query = query.OrderByDescending(e => e?.State?.Name).ToList();
                    }
                    if (filter.SortField == OrderSortField.SUBTOTAL)
                    {

                        query = query.OrderByDescending(e => e.SubTotal).ToList();
                    }
                    if (filter.SortField == OrderSortField.ORDERSTATE)
                    {

                        query = query.OrderByDescending(e => e.OrderState.Name).ToList();
                    }

                    if (filter.SortField == OrderSortField.STATUS)
                    {

                        query = query.OrderByDescending(e => e.Status.Label).ToList();
                    }
                }

              
            }

            return query;

        }

       

    }
}
