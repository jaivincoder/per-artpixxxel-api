

using System.Collections.Generic;

namespace api.artpixxel.data.Features.Permission
{
    public class RoleClaimsModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Action { get; set; }
        public bool Selected { get; set; }


    }


    public class RolePermissions
    {
        public string Name { get; set; }
        public bool AllAllowed { get; set; }
        public List<RoleClaimsModel> RoleClaims { get; set; }
    }



    public class UserPermissionResponseModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        
        public List<RolePermissions> Permissions { get; set; }
    }



    public class PermissionResponseModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        
        public List<RolePermissions> Permissions { get; set; }
    }
}
