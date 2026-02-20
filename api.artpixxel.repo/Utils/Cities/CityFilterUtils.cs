using api.artpixxel.data.Features.Cities;
using api.artpixxel.data.Models;
using LinqKit;
using System.Collections.Generic;
using System.Linq;

namespace api.artpixxel.repo.Utils.Cities
{
   public class CityFilterUtils
    {
        public static bool EmptyFilter(CityFilterData filter)
        {
            return ((string.IsNullOrEmpty(filter.Search)) && ((filter.Filters.DeliveryFee == -1) && string.IsNullOrEmpty(filter.Filters.DeliveryFeeMatchMode))
              && (!filter.Filters.Countries.Any()) && (!filter.Filters.States.Any()));
        }

        public static ExpressionStarter<City> ApplyFilter(CityFilterData filter)
        {
            ExpressionStarter<City> predicate = PredicateBuilder.New<City>();



            if (filter.Filters.Countries.Any())
            {

                predicate = predicate.And(e => filter.Filters.Countries.Contains(e.State.Country.Name));
            }

            if (filter.Filters.States.Any())
            {

                predicate = predicate.And(e => filter.Filters.States.Contains(e.State.StateName));
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

             (e.CityName.Contains(filter.Search)
             || (e.State.Country.Name.Contains(filter.Search))
             || (e.State.StateName.Contains(filter.Search))

             ));

            }

            return predicate;
        }



        public static List<CityModel> ApplySort(List<CityModel> query, CityFilterData filter)
        {
            if (!string.IsNullOrEmpty(filter.SortField))
            {
                if (filter.SortOrder == 1) // ascending
                {
                    if (filter.SortField == CitySortField.NAME)
                    {
                        query = query.OrderBy(e => e.Name).ToList();
                    }

                    else if (filter.SortField == CitySortField.DELIVERYFEE)
                    {
                        query = query.OrderBy(e => e?.DeliveryFee).ToList();
                    }

                    else if (filter.SortField == CitySortField.COUNTRY)
                    {
                        query = query.OrderBy(e => e?.Country?.Name).ToList();
                    }

                    else if (filter.SortField == CitySortField.STATE)
                    {
                        query = query.OrderBy(e => e?.State?.Name).ToList();
                    }
                }

                else
                {
                    if (filter.SortField == CitySortField.NAME)
                    {
                        query = query.OrderByDescending(e => e.Name).ToList();
                    }

                    else if (filter.SortField == CitySortField.DELIVERYFEE)
                    {
                        query = query.OrderByDescending(e => e?.DeliveryFee).ToList();
                    }

                    else if (filter.SortField == CitySortField.COUNTRY)
                    {
                        query = query.OrderByDescending(e => e?.Country?.Name).ToList();
                    }

                    else if (filter.SortField == CitySortField.STATE)
                    {
                        query = query.OrderByDescending(e => e?.State.Name).ToList();
                    }
                }

            }


            return query;

        }
    }
}
