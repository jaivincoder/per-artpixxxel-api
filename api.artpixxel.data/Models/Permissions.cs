using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.data.Models
{
   public static class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
          {
            $"Permissions.{module}.Create",
            $"Permissions.{module}.View",
            $"Permissions.{module}.Edit",
            $"Permissions.{module}.Delete",
             $"Permissions.{module}.Cancel",
            $"Permissions.{module}.Export",
          };
        }

          public static class CustomerType
          {
            public const string Create = "Permissions.CustomerType.Create";
            public const string View = "Permissions.CustomerType.View";
            public const string Edit = "Permissions.CustomerType.Edit";
            public const string Delete = "Permissions.CustomerType.Delete";
            public const string Export = "Permissions.CustomerType.Export";
          }

        public static class HomeSlider
        {
            public const string Create = "Permissions.HomeSlider.Create";
            public const string View = "Permissions.HomeSlider.View";
            public const string Edit = "Permissions.HomeSlider.Edit";
            public const string Delete = "Permissions.HomeSlider.Delete";
            public const string Export = "Permissions.HomeSlider.Export";
        }

        public static class MixMatch
        {
            public const string Create = "Permissions.MixMatch.Create";
            public const string View = "Permissions.MixMatch.View";
            public const string Edit = "Permissions.MixMatch.Edit";
            public const string Delete = "Permissions.MixMatch.Delete";
            public const string Export = "Permissions.MixMatch.Export";
        }

        public static class MixMatchCategory
        {
            public const string Create = "Permissions.MixMatchCategory.Create";
            public const string View = "Permissions.MixMatchCategory.View";
            public const string Edit = "Permissions.MixMatchCategory.Edit";
            public const string Delete = "Permissions.MixMatchCategory.Delete";
            public const string Export = "Permissions.MixMatchCategory.Export";
        }

        public static class Order
        {
            public const string View = "Permissions.Order.View";
            public const string Cancel = "Permissions.Order.Cancel";
            public const string Export = "Permissions.Order.Export";
        }

        public static class OrderStatus
        {
            public const string Create = "Permissions.OrderStatus.Create";
            public const string View = "Permissions.OrderStatus.View";
            public const string Edit = "Permissions.OrderStatus.Edit";
            public const string Delete = "Permissions.OrderStatus.Delete";
            public const string Export = "Permissions.OrderStatus.Export";
        }

        public static class Permission
        {
          
            public const string View = "Permissions.Permission.View";
            public const string Edit = "Permissions.Permission.Edit";
            public const string Export = "Permissions.Permission.Export";
        }

        public static class State
        {
            public const string Create = "Permissions.State.Create";
            public const string View = "Permissions.State.View";
            public const string Edit = "Permissions.State.Edit";
            public const string Delete = "Permissions.State.Delete";
            public const string Export = "Permissions.State.Export";
        }


        public static class Country
        {
            public const string Create = "Permissions.Country.Create";
            public const string View = "Permissions.Country.View";
            public const string Edit = "Permissions.Country.Edit";
            public const string Delete = "Permissions.Country.Delete";
            public const string Export = "Permissions.Country.Export";
        }

       
        public static class UserRole
        {
            public const string Create = "Permissions.UserRole.Create";
            public const string View = "Permissions.UserRole.View";
            public const string Edit = "Permissions.UserRole.Edit";
            public const string Delete = "Permissions.UserRole.Delete";
            public const string Export = "Permissions.UserRole.Export";
        }

        public static class Employee
        {
            public const string Create = "Permissions.Employee.Create";
            public const string View = "Permissions.Employee.View";
            public const string Edit = "Permissions.Employee.Edit";
            public const string Delete = "Permissions.Employee.Delete";
            public const string Export = "Permissions.Employee.Export";
        }

        public static class WallArt
        {
            public const string Create = "Permissions.WallArt.Create";
            public const string View = "Permissions.WallArt.View";
            public const string Edit = "Permissions.WallArt.Edit";
            public const string Delete = "Permissions.WallArt.Delete";
            public const string Export = "Permissions.WallArt.Export";
        }

        public static class WallArtCategory
        {
            public const string Create = "Permissions.WallArtCategory.Create";
            public const string View = "Permissions.WallArtCategory.View";
            public const string Edit = "Permissions.WallArtCategory.Edit";
            public const string Delete = "Permissions.WallArtCategory.Delete";
            public const string Export = "Permissions.WallArtCategory.Export";
        }

        public static class WallArtSize
        {
            public const string Create = "Permissions.WallArtSize.Create";
            public const string View = "Permissions.WallArtSize.View";
            public const string Edit = "Permissions.WallArtSize.Edit";
            public const string Delete = "Permissions.WallArtSize.Delete";
            public const string Export = "Permissions.WallArtSize.Export";
        }

        public static class Frame
        {
            public const string Create = "Permissions.Frame.Create";
            public const string View = "Permissions.Frame.View";
            public const string Edit = "Permissions.Frame.Edit";
            public const string Delete = "Permissions.Frame.Delete";
            public const string Cancel = "Permissions.Frame.Cancel";
            public const string Export = "Permissions.Frame.Export";
        }
    }

}
