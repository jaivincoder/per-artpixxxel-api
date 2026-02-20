using api.artpixxel.data.Models;
using api.artpixxel.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Seed
{

    public class Seed
    {
        public static async Task InitializeDataBase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ArtPixxelContext>();
                var rootFolder = serviceScope.ServiceProvider.GetService<IWebHostEnvironment>();
                context.Database.EnsureCreated();
                // context.Database.Migrate();

                //Look for any company
                if (context.Users.Any())
                {
                    return;
                }



                //Seed Flag List

                var FlagList = Seed.GetFlags().ToArray();
                await context.Flags.AddRangeAsync(FlagList);
                await context.SaveChangesAsync();

                //Seed Country List

                var CountryList = Seed.GetCountries(context).ToArray();
                await context.Countries.AddRangeAsync(CountryList);
                await context.SaveChangesAsync();


                //Seed states

                var states = Seed.GetStates(context).ToArray();
                await context.States.AddRangeAsync(states);
                await context.SaveChangesAsync();


                var cities = Seed.GetCities().ToArray();
                await context.Cities.AddRangeAsync(cities);
                await context.SaveChangesAsync();




            }
        }



        public static List<State> GetStates(ArtPixxelContext context)
        {
            List<State> States = new List<State>()
            {
                new State()
                {

                    StateName = "Alabama",
                    CountryId = context.Countries.First().Id,
                     DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                 new State()
                {

                    StateName = "Alaska",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 15m,
                    CreatedOn = DateTime.Now

                },
                  new State()
                {

                    StateName = "Arizona",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 20m,
                    CreatedOn = DateTime.Now

                },
                   new State()
                {

                    StateName = "Arkansas",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 5m,
                    CreatedOn = DateTime.Now

                },

                  new State()
                {

                    StateName = "California",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 5m,
                    CreatedOn = DateTime.Now

                },
                   new State()
                {

                    StateName = "Colorado",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 12m,
                    CreatedOn = DateTime.Now

                },
                    new State()
                {

                    StateName = "Connecticut",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 7m,
                    CreatedOn = DateTime.Now

                },

                     new State()
                {

                    StateName = "Delaware",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 0m,
                    CreatedOn = DateTime.Now

                },
                      new State()
                {

                    StateName = "Florida",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 5m,
                    CreatedOn = DateTime.Now

                },
                       new State()
                {

                    StateName = "Georgia",
                    DeliveryFee = 12m,
                    CountryId = context.Countries.First().Id,
                    CreatedOn = DateTime.Now

                },
                        new State()
                {

                    StateName = "Hawaii",
                    DeliveryFee = 15m,
                    CountryId = context.Countries.First().Id,
                    CreatedOn = DateTime.Now

                },
                         new State()
                {

                    StateName = "Idaho",
                    CountryId = context.Countries.First().Id,
                     DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },

                          new State()
                {

                    StateName = "Illinois",
                    DeliveryFee = 10m,
                    CountryId = context.Countries.First().Id,
                    CreatedOn = DateTime.Now

                },

                           new State()
                {

                    StateName = "Indiana",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                new State()
                {

                    StateName = "Iowa",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 30m,
                    CreatedOn = DateTime.Now

                },
                 new State()
                {

                    StateName = "Kansas",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 5m,
                    CreatedOn = DateTime.Now

                },
                  new State()
                {

                    StateName = "Kentucky",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 12m,
                    CreatedOn = DateTime.Now

                },
                   new State()
                {

                    StateName = "Louisiana",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                    new State()
                {

                    StateName = "Maine",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },

                new State()
                {

                    StateName = "Maryland",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                 new State()
                {

                    StateName = "Massachusetts",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 14m,
                    CreatedOn = DateTime.Now

                },
                  new State()
                {

                    StateName = "Michigan",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 11m,
                    CreatedOn = DateTime.Now

                },
                   new State()
                {

                    StateName = "Minnesota",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 8m,
                    CreatedOn = DateTime.Now

                },
                    new State()
                {

                    StateName = "Mississippi",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 9m,
                    CreatedOn = DateTime.Now

                },
                    new State()
                {

                    StateName = "Missouri",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 7m,
                    CreatedOn = DateTime.Now

                },
                     new State()
                {

                    StateName = "Montana",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                      new State()
                {

                    StateName = "Nebraska",
                    CountryId = context.Countries.First().Id,
                    CreatedOn = DateTime.Now

                },
                new State()
                {

                    StateName = "Nevada",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                 new State()
                {

                    StateName = "New Hampshire",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                  new State()
                {

                    StateName =  "New Jersey",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                   new State()
                {

                    StateName = "New Mexico",
                    CountryId = context.Countries.First().Id,
                    CreatedOn = DateTime.Now

                },
                    new State()
                {

                    StateName = "New York",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                     new State()
                {

                    StateName = "North Carolina",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                      new State()
                {

                    StateName = "North Dakota",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                 new State()
                {

                    StateName = "Ohio",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 12m,
                    CreatedOn = DateTime.Now

                },
                  new State()
                {

                    StateName = "Oklahoma",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                   new State()
                {

                    StateName = "Oregon",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },

                    new State()
                {

                    StateName = "Pennsylvania",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },

                     new State()
                {

                    StateName = "Rhode Island",
                    CountryId = context.Countries.First().Id,
                    CreatedOn = DateTime.Now

                },
                      new State()
                {

                    StateName = "South Carolina",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                       new State()
                {

                    StateName = "South Dakota",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                        new State()
                {

                    StateName = "Tennessee",
                    CountryId = context.Countries.First().Id,
                    CreatedOn = DateTime.Now

                },
                         new State()
                {

                    StateName = "Texas",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 8m,
                    CreatedOn = DateTime.Now

                },
                          new State()
                {

                    StateName = "Utah",
                    CountryId = context.Countries.First().Id,
                     DeliveryFee = 10m,
                    CreatedOn = DateTime.Now

                },
                           new State()
                {

                    StateName = "Vermont",
                    CountryId = context.Countries.First().Id,
                       DeliveryFee = 5m,
                    CreatedOn = DateTime.Now

                },
                            new State()
                {

                    StateName = "Virginia",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 12m,
                    CreatedOn = DateTime.Now

                },
                             new State()
                {

                    StateName = "Washington",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 5m,
                    CreatedOn = DateTime.Now

                },
                              new State()
                {

                    StateName =  "West Virginia",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 11m,
                    CreatedOn = DateTime.Now

                },
                               new State()
                {

                    StateName = "Wisconsin",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 12m,
                    CreatedOn = DateTime.Now

                },
                                new State()
                {

                    StateName = "Wyoming",
                    CountryId = context.Countries.First().Id,
                    DeliveryFee = 17m,
                    CreatedOn = DateTime.Now

                }
                               

            };
            return States;
        }




        public static List<City> GetCities()
        {
            return new List<City>
            {
                new City()
                {
                    CityName = "Montgomery",
                    DeliveryFee = 10m
                   
                },

                new City()
                {
                    CityName = "Juneau",
                     DeliveryFee = 5m

                },
                new City()
                {
                    CityName = "Phoenix",
                     DeliveryFee = 10m

                },
                new City()
                {
                    CityName = "Little Rock",
                     DeliveryFee = 10m

                },
                 new City()
                {
                    CityName = "Sacramento",
                     DeliveryFee = 6m

                },

                  new City()
                {
                    CityName = "Denver" ,
                     DeliveryFee = 12m

                },

                    new City()
                {
                    CityName = "Hartford",
                     DeliveryFee = 12m

                },

                new City()
                {
                    CityName = "Dover",
                     DeliveryFee = 12m

                },

                  new City()
                {
                    CityName = "Tallahassee",
                     DeliveryFee = 12m

                },
                    new City()
                {
                    CityName = "Atlanta",
                     DeliveryFee = 5m

                },

                    new City()
                {
                    CityName = "Honolulu",
                     DeliveryFee = 11m

                },
                    new City()
                {
                    CityName = "Boise",
                     DeliveryFee = 7m

                },
                    new City()
                {
                    CityName = "Springfield",
                     DeliveryFee = 7m

                },
                    new City()
                {
                    CityName = "Indianapolis",
                     DeliveryFee = 8m

                },
                    new City()
                {
                    CityName = "Des Moines",
                     DeliveryFee = 7m

                },
                    new City()
                {
                    CityName = "Topeka",
                     DeliveryFee = 10m

                },new City()
                {
                    CityName = "Frankfort",
                     DeliveryFee = 7m

                },
                    new City()
                {
                    CityName = "Baton Rouge",
                     DeliveryFee = 7m

                },
                    new City()
                {
                    CityName = "Augusta",
                     DeliveryFee = 7m

                },


                    new City()
                {
                    CityName = "Annapolis",
                     DeliveryFee = 7m

                },
                    new City()
                {
                    CityName = "Boston",
                     DeliveryFee = 12m

                },
                    new City()
                {
                    CityName = "Lansing",
                     DeliveryFee = 9m

                },
                    new City()
                {
                    CityName = "Saint Paul",
                     DeliveryFee = 8m

                },
                    new City()
                {
                    CityName = "Jackson",
                     DeliveryFee = 7m

                },
                    new City()
                {
                    CityName = "Jefferson City",
                     DeliveryFee = 7m

                },

                     new City()
                {
                    CityName = "Lincoln",
                     DeliveryFee = 7m

                },
                            new City()
                {
                    CityName =  "Carson City",
                     DeliveryFee = 7m

                },
                   new City()
                {
                    CityName = "Concord",
                     DeliveryFee = 7m

                },
                     new City()
                {
                    CityName = "Trenton",
                     DeliveryFee = 7m

                },
                       new City()
                {
                    CityName =  "Santa Fe",
                     DeliveryFee = 7m

                },
                         new City()
                {
                    CityName = "Albany",
                     DeliveryFee = 7m

                },
                           new City()
                {
                    CityName = "Raleigh",
                     DeliveryFee = 7m

                },
                             new City()
                {
                    CityName = "Bismarck",
                     DeliveryFee = 7m

                },
                               new City()
                {
                    CityName = "Columbus",
                     DeliveryFee = 7m

                },
                                 new City()
                {
                    CityName = "Oklahoma City",
                     DeliveryFee = 7m

                },
                                   new City()
                {
                    CityName = "Salem",
                     DeliveryFee = 7m

                },
                                     new City()
                {
                    CityName = "Columbia" ,
                     DeliveryFee = 7m

                },
                                       new City()
                {
                    CityName ="Pierre",
                     DeliveryFee = 7m

                },
                  new City()
                {
                    CityName = "Nashville",
                     DeliveryFee = 7m

                },

                    new City()
                {
                    CityName = "Austin",
                     DeliveryFee = 7m

                },
                      new City()
                {
                    CityName = "Salt Lake City",
                     DeliveryFee = 7m

                },
                  new City()
                {
                    CityName = "Montpelier",
                     DeliveryFee = 7m

                },
                  new City()
                {
                    CityName = "Richmond",
                     DeliveryFee = 7m

                },
                    new City()
                {
                    CityName = "Olympia",
                     DeliveryFee = 7m

                },
                      new City()
                {
                    CityName = "Charleston",
                     DeliveryFee = 7m

                },
                        new City()
                {
                    CityName = "Madison",
                     DeliveryFee = 7m

                },
                          new City()
                {
                    CityName ="Cheyenne",
                     DeliveryFee = 7m

                },
                            new City()
                {
                    CityName = "Wisconsin",
                     DeliveryFee = 7m

                },
                              new City()
                {
                    CityName = "Wyoming",
                     DeliveryFee = 7m,
                     

                }
                                
               


            };
        }

        public static List<Flag> GetFlags()
        {
            return new List<Flag>()
            {
                

           new Flag() {Name = "dz flag", CountryName = "Algeria"},
           new Flag() {Name = "ad flag", CountryName ="Andorra" },
           new Flag() {Name = "ae flag",CountryName = "U.A.E" },
           new Flag() {Name = "af flag",CountryName =  "Afghanistan"},
           new Flag() {Name = "ag flag",CountryName =  "Antigua"},
           new Flag() {Name = "ai flag",CountryName =  "Anguilla"},
           new Flag() {Name = "al flag",CountryName =  "Albania"},
           new Flag() {Name = "am flag",CountryName =  "Armenia"},
           new Flag() {Name = "an flag",CountryName =  "Netherlands Antilles"},
           new Flag() {Name = "ao flag",CountryName =  "Angola"},
           new Flag() {Name = "ar flag",CountryName =  "Argentina"},
           new Flag() {Name = "as flag",CountryName =  "American Samoa"},
           new Flag() {Name = "at flag",CountryName =  "Austria"},
           new Flag() {Name = "au flag",CountryName =  "Australia"},
           new Flag() {Name = "aw flag",CountryName =  "Aruba"},
           new Flag() {Name = "ax flag",CountryName =  "Aland Islands"},
           new Flag() {Name = "az flag",CountryName =  "Azerbaijan"},
           new Flag() {Name = "ba flag",CountryName =  "Bosnia"},
           new Flag() {Name = "bb flag",CountryName =  "Barbados"},
           new Flag() {Name = "bd flag",CountryName =  "Bangladesh"},
           new Flag() {Name = "be flag",CountryName =  "Belgium"},
           new Flag() {Name = "bf flag",CountryName =  "Burkina Faso"},
           new Flag() {Name = "bg flag",CountryName =  "Bulgaria"},
           new Flag() {Name = "bh flag",CountryName =  "Bahrain"},
           new Flag() {Name = "bi flag",CountryName =  "Burundi"},
           new Flag() {Name = "bj flag",CountryName =  "Benin"},
           new Flag() {Name = "bm flag",CountryName =  "Bermuda"},
           new Flag() {Name = "vg flag",CountryName = "British Virgin Islands"},
           new Flag() {Name = "bn flag",CountryName =  "Brunei"},
           new Flag() {Name = "bo flag",CountryName =  "Bolivia"},
           new Flag() {Name = "br flag",CountryName =  "Brazil"},
           new Flag() {Name = "bs flag",CountryName =  "Bahamas"},
           new Flag() {Name = "bt flag",CountryName =  "Bhutan"},
           new Flag() {Name = "bv flag",CountryName =  "Bouvet Island"},
           new Flag() {Name = "bw flag",CountryName =  "Botswana"},
           new Flag() {Name = "by flag",CountryName =  "Belarus"},
           new Flag() {Name = "bz flag",CountryName =  "Belize"},
           new Flag() {Name = "mm flag",CountryName =  "Burma"},
           new Flag() {Name = "tc flag",CountryName =  "Caicos Islands"},
           new Flag() {Name = "kh flag",CountryName =  "Cambodia"},
           new Flag() {Name = "cm flag",CountryName =  "Cameroon"},
           new Flag() {Name = "ca flag",CountryName =  "Canada"},
           new Flag() {Name = "ky flag",CountryName =  "Cayman Islands"},
           new Flag() {Name = "cd flag",CountryName =  "Congo"},
           new Flag() {Name = "cf flag",CountryName =  "Central African Republic"},
           new Flag() {Name = "km flag",CountryName =  "Comoros"},
           new Flag() {Name = "cg flag",CountryName =  "Congo Brazzaville"},
           new Flag() {Name = "ci flag",CountryName =  "Cote Divoire"},
           new Flag() {Name = "ck flag",CountryName =  "Cook Islands"},
           new Flag() {Name = "td flag",CountryName =  "Chad"},
           new Flag() {Name = "cl flag",CountryName =  "Chile"},
           new Flag() {Name = "cn flag",CountryName =  "China"},
           new Flag() {Name = "co flag",CountryName =  "Colombia"},
           new Flag() {Name = "cr flag",CountryName =  "Costa Rica"},
           new Flag() {Name = "cu flag",CountryName =  "Cuba"},
           new Flag() {Name = "cv flag",CountryName =  "Cape Verde"},
           new Flag() {Name = "cx flag",CountryName =  "Christmas Island"},
           new Flag() {Name = "hr flag",CountryName =  "Croatia"},
           new Flag() {Name = "cy flag",CountryName =  "Cyprus"},
           new Flag() {Name = "cz flag",CountryName =  "Czech Republic"},
           new Flag() {Name = "dj flag",CountryName =  "Djibouti"},
           new Flag() {Name = "dk flag",CountryName =  "Denmark"},
           new Flag() {Name = "dm flag",CountryName =  "Dominica"},
           new Flag() {Name = "do flag",CountryName =  "Dominican Republic"},
           new Flag() {Name = "ec flag",CountryName =  "Ecuador"},
           new Flag() {Name = "ee flag",CountryName =  "Estonia"},
           new Flag() {Name = "eg flag",CountryName =  "Egypt"},
           new Flag() {Name = "sv flag",CountryName =  "El Salvador"},
           new Flag() {Name = "gb eng flag",CountryName =  "England"},
           new Flag() {Name = "gq flag",CountryName =  "Equatorial Guinea"},
           new Flag() {Name = "er flag",CountryName =  "Eritrea"},
           new Flag() {Name = "et flag",CountryName =  "Ethiopia"},
           new Flag() {Name = "eu flag",CountryName =  "European Union"},
           new Flag() {Name = "fi flag",CountryName =  "Finland"},
           new Flag() {Name = "fj flag",CountryName =  "Fiji"},
           new Flag() {Name = "fk flag",CountryName =  "Falkland Islands"},
           new Flag() {Name = "fo flag",CountryName =  "Faroe Islands"},
           new Flag() {Name = "fr flag",CountryName =  "France"},
           new Flag() {Name = "gf flag",CountryName =  "French Guiana"},
           new Flag() {Name = "pf flag",CountryName =  "French Polynesia"},
           new Flag() {Name = "tf flag",CountryName =  "French Territories"},
           new Flag() {Name = "ga flag",CountryName =  "Gabon"},
           new Flag() {Name = "gb wls flag",CountryName =  "Wales"},
           new Flag() {Name = "gd flag",CountryName =  "Grenada"},
           new Flag() {Name = "ge flag",CountryName =  "Georgia"},
           new Flag() {Name = "gh flag",CountryName =  "Ghana"},
           new Flag() {Name = "gi flag",CountryName =  "Gibraltar"},
           new Flag() {Name = "gl flag",CountryName =  "Greenland"},
           new Flag() {Name = "gm flag",CountryName =  "Gambia"},
           new Flag() {Name = "de flag",CountryName =  "Germany"},
           new Flag() {Name = "gr flag",CountryName =  "Greece"},
           new Flag() {Name = "gn flag",CountryName =  "Guinea"},
           new Flag() {Name = "gp flag",CountryName =  "Guadeloupe"},
           new Flag() {Name = "gt flag",CountryName =  "Guatemala"},
           new Flag() {Name = "gu flag",CountryName =  "Guam"},
           new Flag() {Name = "gw flag",CountryName =  "Guinea-bissau"},
           new Flag() {Name = "gy flag",CountryName =  "Guyana"},
           new Flag() {Name = "ht flag",CountryName =  "Haiti"},
           new Flag() {Name = "hm flag",CountryName =  "Heard Island"},
           new Flag() {Name = "nl flag",CountryName =  "Holland"},
           new Flag() {Name = "hn flag",CountryName =  "Honduras"},
           new Flag() {Name = "hk flag",CountryName =  "Hong Kong"},
           new Flag() {Name = "hu flag",CountryName =  "Hungary"},
           new Flag() {Name = "id flag",CountryName =  "Indonesia"},
           new Flag() {Name = "ie flag",CountryName =  "Ireland"},
           new Flag() {Name = "il flag",CountryName =  "Israel"},
           new Flag() {Name = "in flag",CountryName =  "India"},
           new Flag() {Name = "io flag",CountryName =  "Indian Ocean Territory"},
           new Flag() {Name = "iq flag",CountryName =  "Iraq"},
           new Flag() {Name = "ir flag",CountryName =  "Iran"},
           new Flag() {Name = "is flag",CountryName =  "Iceland"},
           new Flag() {Name = "it flag",CountryName =  "Italy"},
           new Flag() {Name = "sj flag",CountryName =  "Jan Mayen"},
           new Flag() {Name = "jm flag",CountryName =  "Jamaica"},
           new Flag() {Name = "jo flag",CountryName =  "Jordan"},
           new Flag() {Name = "jp flag",CountryName =  "Japan"},
           new Flag() {Name = "ke flag",CountryName =  "Kenya"},
           new Flag() {Name = "kg flag",CountryName =  "Kyrgyzstan"},
           new Flag() {Name = "ki flag",CountryName =  "Kiribati"},
           new Flag() {Name = "kp flag",CountryName =  "North Korea"},
           new Flag() {Name = "kw flag",CountryName =  "Kuwait"},
           new Flag() {Name = "kz flag",CountryName =  "Kazakhstan"},
           new Flag() {Name = "la flag",CountryName =  "Laos"},
           new Flag() {Name = "lv flag",CountryName =  "Latvia"},
           new Flag() {Name = "lb flag",CountryName =  "Lebanon"},
           new Flag() {Name = "lr flag",CountryName =  "Liberia"},
           new Flag() {Name = "li flag",CountryName =  "Liechtenstein"},
           new Flag() {Name = "ls flag",CountryName =  "Lesotho"},
           new Flag() {Name = "lt flag",CountryName =  "Lithuania"},
           new Flag() {Name = "lu flag",CountryName =  "Luxembourg"},
           new Flag() {Name = "ly flag",CountryName =  "Libya"},
           new Flag() {Name = "mo flag",CountryName =  "Macau"},
           new Flag() {Name = "mk flag",CountryName =  "Macedonia"},
           new Flag() {Name = "mg flag",CountryName =  "Madagascar"},
           new Flag() {Name = "mh flag",CountryName =  "Marshall Islands"},
           new Flag() {Name = "mw flag",CountryName =  "Malawi"},
           new Flag() {Name = "my flag",CountryName =  "Malaysia"},
           new Flag() {Name = "mv flag",CountryName =  "Maldives"},
           new Flag() {Name = "ml flag",CountryName =  "Mali"},
           new Flag() {Name = "mt flag",CountryName =  "Malta"},
           new Flag() {Name = "mu flag",CountryName =  "Mauritius"},
           new Flag() {Name = "yt flag",CountryName =  "Mayotte"},
           new Flag() {Name = "mx flag",CountryName =  "Mexico"},
           new Flag() {Name = "mn flag",CountryName =  "Mongolia"},
           new Flag() {Name = "ma flag",CountryName =  "Morocco"},
           new Flag() {Name = "mc flag",CountryName =  "Monaco"},
           new Flag() {Name = "md flag",CountryName =  "Moldova"},
           new Flag() {Name = "me flag",CountryName =  "Montenegro"},
           new Flag() {Name = "mp flag",CountryName =  "Northern Mariana Islands"},
           new Flag() {Name = "mq flag",CountryName =  "Martinique"},
           new Flag() {Name = "mr flag",CountryName =  "Mauritania"},
           new Flag() {Name = "fm flag",CountryName =  "Micronesia"},
           new Flag() {Name = "ms flag",CountryName =  "Montserrat"},
           new Flag() {Name = "mz flag",CountryName =  "Mozambique"},
           new Flag() {Name = "na flag",CountryName =  "Namibia"},
           new Flag() {Name = "nc flag",CountryName =  "New Caledonia"},
           new Flag() {Name = "pg flag",CountryName =  "New Guinea"},
           new Flag() {Name = "nz flag",CountryName =  "New Zealand"},
           new Flag() {Name = "ne flag",CountryName =  "Niger"},
           new Flag() {Name = "ng flag",CountryName =  "Nigeria"},
           new Flag() {Name = "ni flag",CountryName =  "Nicaragua"},
           new Flag() {Name = "nf flag",CountryName =  "Norfolk Island"},
           new Flag() {Name = "no flag",CountryName =  "Norway"},
           new Flag() {Name = "np flag",CountryName =  "Nepal"},
           new Flag() {Name = "nr flag",CountryName =  "Nauru"},
           new Flag() {Name = "nu flag",CountryName =  "Niue"},
           new Flag() {Name = "om flag",CountryName =  "Oman"},
           new Flag() {Name = "pk flag",CountryName =  "Pakistan"},
           new Flag() {Name = "pw flag",CountryName =  "Palau"},
           new Flag() {Name = "ps flag",CountryName =  "Palestine"},
           new Flag() {Name = "pa flag",CountryName =  "Panama"},
           new Flag() {Name = "py flag",CountryName =  "Paraguay"},
           new Flag() {Name = "pe flag",CountryName =  "Peru"},
           new Flag() {Name = "ph flag",CountryName =  "Philippines"},
           new Flag() {Name = "pn flag",CountryName =  "Pitcairn Islands"},
           new Flag() {Name = "pl flag",CountryName =  "Poland"},
           new Flag() {Name = "pt flag",CountryName =  "Portugal"},
           new Flag() {Name = "pr flag",CountryName =  "Puerto Rico"},
           new Flag() {Name = "qa flag",CountryName =  "Qatar"},
           new Flag() {Name = "re flag",CountryName =  "Reunion"},
           new Flag() {Name = "ro flag",CountryName =  "Romania"},
           new Flag() {Name = "ru flag",CountryName =  "Russia"},
           new Flag() {Name = "rw flag",CountryName =  "Rwanda"},
           new Flag() {Name = "sh flag",CountryName =  "Saint Helena"},
           new Flag() {Name = "kn flag",CountryName =  "Saint Kitts And Nevis"},
           new Flag() {Name = "lc flag",CountryName =  "Saint Lucia"},
           new Flag() {Name = "ws flag",CountryName =  "Samoa"},
           new Flag() {Name = "gs flag",CountryName =  "Sandwich Islands"},
           new Flag() {Name = "sm flag",CountryName =  "San Marino"},
           new Flag() {Name = "st flag",CountryName =  "Sao Tome"},
           new Flag() {Name = "pm flag",CountryName =  "Saint Pierre"},
           new Flag() {Name = "vc flag",CountryName = "Saint Vincent"},
           new Flag() {Name = "sa flag",CountryName =  "Saudi Arabia"},
           new Flag() {Name = "gb sct flag",CountryName =  "Scotland"},
           new Flag() {Name = "cs flag",CountryName =  "Serbia"},
           new Flag() {Name = "rs flag",CountryName =  "Serbia"},
           new Flag() {Name = "sn flag",CountryName =  "Senegal"},
           new Flag() {Name = "sc flag",CountryName =  "Seychelles"},
           new Flag() {Name = "sl flag",CountryName =  "Sierra Leone"},
           new Flag() {Name = "sg flag",CountryName =  "Singapore"},
           new Flag() {Name = "sk flag",CountryName =  "Slovakia"},
           new Flag() {Name = "si flag",CountryName =  "Slovenia"},
           new Flag() {Name = "sb flag",CountryName =  "Solomon Islands"},
           new Flag() {Name = "so flag",CountryName =  "Somalia"},
           new Flag() {Name = "za flag",CountryName =  "South Africa"},
           new Flag() {Name = "kr flag",CountryName =  "South Korea"},
           new Flag() {Name = "es flag",CountryName =  "Spain"},
           new Flag() {Name = "lk flag",CountryName =  "Sri Lanka"},
           new Flag() {Name = "sd flag",CountryName =  "Sudan"},
           new Flag() {Name = "sr flag",CountryName =  "Suriname"},
           new Flag() {Name = "sz flag",CountryName =  "Swaziland"},
           new Flag() {Name = "se flag",CountryName =  "Sweden"},
           new Flag() {Name = "ch flag",CountryName =  "Switzerland"},
           new Flag() {Name = "sy flag",CountryName =  "Syria"},
           new Flag() {Name = "tw flag",CountryName =  "Taiwan"},
           new Flag() {Name = "tj flag",CountryName =  "Tajikistan"},
           new Flag() {Name = "tz flag",CountryName =  "Tanzania"},
           new Flag() {Name = "tl flag",CountryName =  "Timorleste"},
           new Flag() {Name = "th flag",CountryName =  "Thailand"},
           new Flag() {Name = "tg flag",CountryName =  "Togo"},
           new Flag() {Name = "tk flag",CountryName =  "Tokelau"},
           new Flag() {Name = "to flag",CountryName =  "Tonga"},
           new Flag() {Name = "tt flag",CountryName =  "Trinidad"},
           new Flag() {Name = "tm flag",CountryName =  "Turkmenistan"},
           new Flag() {Name = "tn flag",CountryName =  "Tunisia"},
           new Flag() {Name = "tr flag",CountryName =  "Turkey"},
           new Flag() {Name = "tv flag",CountryName =  "Tuvalu"},
           new Flag() {Name =  "ug flag",CountryName = "Uganda" },
           new Flag() {Name = "gb uk flag",CountryName = "UK" },
           new Flag() {Name =  "ua flag",CountryName = "Ukraine" },
           new Flag() {Name =  "us flag",CountryName = "United States" },
           new Flag() {Name =  "um flag",CountryName = "Us Minor Islands"},
           new Flag() {Name =  "vi flag",CountryName = "Us Virgin Islands"},
           new Flag() {Name =  "uy flag",CountryName = "Uruguay" },
           new Flag() {Name =  "uz flag",CountryName = "Uzbekistan"},
           new Flag() {Name =  "va flag",CountryName = "Vatican City"},
           new Flag() {Name =  "ve flag",CountryName = "Venezuela"},
           new Flag() {Name =  "vn flag",CountryName = "Vietnam"},
           new Flag() {Name =  "vu flag",CountryName = "Vanuatu"},
           new Flag() {Name = "gb wls flag",CountryName =  "Wales"},
           new Flag() {Name =  "wf flag",CountryName = "Wallis And Futuna"},
           new Flag() {Name = "eh flag",CountryName =  "Western Sahara"},
           new Flag() {Name = "ye flag",CountryName =  "Yemen"},
           new Flag() {Name = "zm flag",CountryName =  "Zambia"},
           new Flag() {Name = "zw flag",CountryName =  "Zimbabwe"}
            };

          
        }
        public static List<Country> GetCountries(ArtPixxelContext context)
        {
            List<Country> CountryLists = new List<Country>()
            {
                new Country()
                {

                   Name = "USA",
                   FlagId = context.Flags.First().Id,


                },



            };
            return CountryLists;
        }



    }
}