using api.artpixxel.repo.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Utils
{
    public static class GlobalUtils
    {
        public static SplittedName SplittName(List<string> Names)
        {
            if (Names.Any())
            {
                List<string> FirstNames = new();
                List<string> MiddleNames = new();
                List<string> LastNames = new();

                foreach (string employeeFullName in Names)
                {
                    string[] Splitted = employeeFullName.Split(" ");

                    if (!FirstNames.Contains(Splitted.ElementAt(0)))
                    {
                        FirstNames.Add(Splitted.ElementAt(0));
                    }



                    if (Splitted.Length == 3)
                    {
                        if (!MiddleNames.Contains(Splitted.ElementAt(1)))
                        {
                            MiddleNames.Add(Splitted.ElementAt(1));
                        }

                        if (!LastNames.Contains(Splitted.ElementAt(2)))
                        {
                            LastNames.Add(Splitted.ElementAt(2));
                        }

                    }

                    else
                    {
                        if (!LastNames.Contains(Splitted.ElementAt(1)))
                        {
                            LastNames.Add(Splitted.ElementAt(1));
                        }

                    }


                }


                return new SplittedName
                {
                    FirstNames = FirstNames,
                    LastNames = LastNames,
                    MiddleNames = MiddleNames
                };

            }


            return new SplittedName
            {
                FirstNames = new List<string>(),
                LastNames = new List<string>(),
                MiddleNames = new List<string>()
            };
        }
    }
}
