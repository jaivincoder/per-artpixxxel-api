

using api.artpixxel.data.Features.States;
using api.artpixxel.data.Models;
using LinqKit;
using System.Collections.Generic;
using System.Linq;

namespace api.artpixxel.repo.Utils.States
{
    public static class StateFilterUtils
    {
        public static bool EmptyFilter(StateFilterData filter)
        {
            return ((string.IsNullOrEmpty(filter.Search)) && ((filter.Filters.DeliveryFee == -1) && string.IsNullOrEmpty(filter.Filters.DeliveryFeeMatchMode))
              && (!filter.Filters.Countries.Any()));
        }

        public static ExpressionStarter<State> ApplyFilter(StateFilterData filter)
        {
            ExpressionStarter<State> predicate = PredicateBuilder.New<State>();



            if (filter.Filters.Countries.Any())
            {

                predicate = predicate.And(e => filter.Filters.Countries.Contains(e.Country.Name));
            }

            if (filter.Filters.DeliveryFee > -1)
            {
                if (filter.Filters.DeliveryFeeMatchMode == Operators.EQUALS)
                {
                    predicate = predicate.And(e => e.DeliveryFee == filter.Filters.DeliveryFee);
                }
                else if (filter.Filters.DeliveryFeeMatchMode == Operators.GREATER_THAN)
                {
                    predicate = predicate.And(e => e.DeliveryFee > filter.Filters.DeliveryFee);
                }
                else if (filter.Filters.DeliveryFeeMatchMode == Operators.GREATER_THAN_OR_EQUAL_TO)
                {
                    predicate = predicate.And(e => e.DeliveryFee >= filter.Filters.DeliveryFee);
                }
                else if (filter.Filters.DeliveryFeeMatchMode == Operators.LESS_THAN)
                {
                    predicate = predicate.And(e => e.DeliveryFee < filter.Filters.DeliveryFee);
                }
                else if (filter.Filters.DeliveryFeeMatchMode == Operators.LESS_THAN_OR_EQUALTO)
                {
                    predicate = predicate.And(e => e.DeliveryFee <= filter.Filters.DeliveryFee);
                }
                else if (filter.Filters.DeliveryFeeMatchMode == Operators.NOT_EQUALS)
                {
                    predicate = predicate.And(e => e.DeliveryFee != filter.Filters.DeliveryFee);
                }
            }


            if (!string.IsNullOrEmpty(filter.Search))
            {

                predicate = predicate.And(e =>
              
             (e.StateName.Contains(filter.Search)
             || (e.Country.Name.Contains(filter.Search))
          
             ));

            }

            return predicate;
        }



        public static List<StateModel> ApplySort(List<StateModel> query, StateFilterData filter)
        {
            if (!string.IsNullOrEmpty(filter.SortField))
            {
                if (filter.SortOrder == 1) // ascending
                {
                    if (filter.SortField == StateSortField.NAME)
                    {
                        query = query.OrderBy(e => e.Name).ToList();
                    }

                    else if (filter.SortField == StateSortField.DELIVERYFEE)
                    {
                        query = query.OrderBy(e => e.DeliveryFee).ToList();
                    }

                    else if (filter.SortField == StateSortField.COUNTRY)
                    {
                        query = query.OrderBy(e => e.Country.Name).ToList();
                    }

                    else if (filter.SortField == StateSortField.CITYCOUNT)
                    {
                        query = query.OrderBy(e => e.CityCount).ToList();
                    }
                }

                else
                {
                    if (filter.SortField == StateSortField.NAME)
                    {
                        query = query.OrderByDescending(e => e.Name).ToList();
                    }

                    else if (filter.SortField == StateSortField.DELIVERYFEE)
                    {
                        query = query.OrderByDescending(e => e.DeliveryFee).ToList();
                    }

                    else if (filter.SortField == StateSortField.COUNTRY)
                    {
                        query = query.OrderByDescending(e => e.Country.Name).ToList();
                    }

                    else if (filter.SortField == StateSortField.CITYCOUNT)
                    {
                        query = query.OrderByDescending(e => e.CityCount).ToList();
                    }
                }

            }


            return query;

        }

    }
}
