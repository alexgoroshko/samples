using System;

namespace MsGraph.GraphModels.Attributes
{

    /// <summary>
    /// Causes (user's) property to be excluded from $select query
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SkipGraphPropertyAttribute :Attribute
    {

    }

}
