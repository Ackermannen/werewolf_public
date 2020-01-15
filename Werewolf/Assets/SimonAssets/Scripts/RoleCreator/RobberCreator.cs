using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobberCreator : AbstractRoleCreator
{
    public override AbstractRole CreateRole()
    {
        return new RobberRole();
    }
}
