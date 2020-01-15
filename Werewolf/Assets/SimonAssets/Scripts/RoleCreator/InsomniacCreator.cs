using Assets.SimonAssets.Scripts.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SimonAssets.Scripts.RoleCreator
{
    class InsomniacCreator : AbstractRoleCreator
    {
        public override AbstractRole CreateRole()
        {
            return new InsomniacRole();
        }
    }
}
