

using api.artpixxel.data.Features.Carousels;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Models;
using LinqKit;
using System.Collections.Generic;
using System.Linq;

namespace api.artpixxel.repo.Utils.Carousels
{
    public class CarouselFilterUtils
    {
        public static bool EmptyFilter(Filter filter)
        {
            return string.IsNullOrEmpty(filter.Search);
        }

        public static ExpressionStarter<Carousel> ApplyFilter(Filter filter)
        {
            ExpressionStarter<Carousel> predicate = PredicateBuilder.New<Carousel>();

            predicate = predicate.And(e => (e.Heading.Contains(filter.Search)));

            return predicate;
        }



        public static List<CarouselModel> ApplySort(List<CarouselModel> query, Filter filter)
        {
            if (!string.IsNullOrEmpty(filter.SortField))
            {
                if (filter.SortOrder == 1) // ascending
                {
                    query = query.OrderBy(e => e.LinkLabel).ToList();
                }

                else
                {
                    query = query.OrderByDescending(e => e.LinkLabel).ToList();
                }
            }
           

            return query;
        }
    }
}
