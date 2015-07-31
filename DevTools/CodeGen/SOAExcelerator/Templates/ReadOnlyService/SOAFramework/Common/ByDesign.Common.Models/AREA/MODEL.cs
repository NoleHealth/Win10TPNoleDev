using System.ComponentModel;
using ByDesign.SOA.Common.Attributes;
using ByDesign.SOA.Common.IModels.[AREA];
using System;
using System.Runtime.Serialization;
using ByDesign.SOA.Common.Providers.Attributes;

namespace ByDesign.SOA.Common.Models.[AREA]
{
    /// <summary>
    ///  <see cref="I[MODEL]" /> Implementation Base Object
    /// </summary>
    [DataContract, Serializable]
    //TODO PW#[TICKET] remove Alias if Model and Table name match
    [Alias("[SQLNAME]")]
    [ByDesignTranslatable("SOA_MODEL_[MODEL_UPPER]", "[MODEL_DISPLAYNAME]")]
    [ByDesignHelpText("SOA_MODEL_HT_[MODEL_UPPER]", "[MODEL_HT]")]
    public class [MODEL] : #IF IsAuditable #THEN: Auditable,#ENDIF I[MODEL]
    {
        [MODEL_PROPERTIES]
    }
}
