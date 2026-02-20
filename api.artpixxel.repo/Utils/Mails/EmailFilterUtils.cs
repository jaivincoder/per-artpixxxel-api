
using api.artpixxel.data.Features.Emails;
using api.artpixxel.data.Models;
using api.artpixxel.repo.Extensions;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace api.artpixxel.repo.Utils.Mails
{
    public static class EmailFilterUtils
    {
        public static bool EmptyFilter(EmailListFilterData filter)
        {
            return ((string.IsNullOrEmpty(filter.Search)) && (string.IsNullOrEmpty(filter.Filters.SignupDate)));
        }


        public static ExpressionStarter<EmailList> ApplyFilter(EmailListFilterData filter)
        {
            try
            {
                ExpressionStarter<EmailList> predicate = PredicateBuilder.New<EmailList>();

                if (!string.IsNullOrEmpty(filter.Filters.SignupDate))
                {
                    DateTime date = DateTime.ParseExact(filter.Filters.SignupDate, DefaultDateFormat.ddMMyyyy, CultureInfo.InvariantCulture);
                    if (filter.Filters.SignupDateMatchMode == MatchMode.DateIS)
                    {
                        predicate = predicate.And(e => e.CreatedOn.Date == date);
                    }
                    else if (filter.Filters.SignupDateMatchMode == MatchMode.DateIsNT)
                    {
                        predicate = predicate.And(e => e.CreatedOn.Date != date);
                    }

                    else if (filter.Filters.SignupDateMatchMode == MatchMode.DateAfter)
                    {
                        predicate = predicate.And(e => e.CreatedOn.Date > date);
                    }
                    else if (filter.Filters.SignupDateMatchMode == MatchMode.DateBefore)
                    {
                        predicate = predicate.And(e => e.CreatedOn.Date < date);
                    }

                }


                if (!string.IsNullOrEmpty(filter.Search))
                {
                    predicate = predicate.And(e => e.EmailAddress.Contains(filter.Search) || ((e.FirstName != null) && (e.FirstName.Contains(filter.Search)))
                    || ((e.LastName != null) && (e.LastName.Contains(filter.Search))));
                }


                return predicate;
            }
            
            catch(Exception)
            {
                throw;
            }


        }




        public static List<EmailListResponse> ApplySort(List<EmailListResponse> query, EmailListFilterData filter)
        {
            if (!string.IsNullOrEmpty(filter.SortField))
            {
                if (filter.SortOrder == 1)
                {
                    if (filter.SortField == EmailSortField.FIRSTNAME)
                    {
                        query = query.OrderBy(e => e.FirstName).ToList();
                    }

                    else if (filter.SortField == EmailSortField.LASTNAME)
                    {
                        query = query.OrderBy(e => e.LastName).ToList();
                    }

                    else if (filter.SortField == EmailSortField.EMAILADDRESS)
                    {
                        query = query.OrderBy(e => e.EmailAddress).ToList();
                    }

                    else if (filter.SortField == EmailSortField.SIGNUPDATEFILTERED)
                    {
                        query = query.OrderBy(e => e.SignupDate.DDMMYYYY()).ToList();
                    }

                }

                else
                {
                    if (filter.SortField == EmailSortField.FIRSTNAME)
                    {
                        query = query.OrderByDescending(e => e.FirstName).ToList();
                    }

                    else if (filter.SortField == EmailSortField.LASTNAME)
                    {
                        query = query.OrderByDescending(e => e.LastName).ToList();
                    }

                    else if (filter.SortField == EmailSortField.EMAILADDRESS)
                    {
                        query = query.OrderByDescending(e => e.EmailAddress).ToList();
                    }

                    else if (filter.SortField == EmailSortField.SIGNUPDATEFILTERED)
                    {
                        query = query.OrderByDescending(e => e.SignupDate.DDMMYYYY()).ToList();
                    }
                }
            }


            return query;
        }


    }
}
