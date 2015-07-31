using System.ComponentModel;
using ByDesign.SOA.Common.Attributes;
using ByDesign.SOA.Common.IModels.[AREA];
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ByDesign.SOA.Common.Providers.Attributes;

namespace ByDesign.SOA.Common.Models.[AREA]
{
    /// <summary>
    ///  <see cref="I[MODEL]" /> Implementation Base Object
    /// </summary>
    [DataContract, System.Serializable]
    [Alias("[SQLNAME]")]
    [ByDesignTranslatable("SOA_MODEL_[MODEL_UPPER]", "[MODEL_DISPLAYNAME]")]
    [ByDesignHelpText("SOA_MODEL_HT_[MODEL_UPPER]", "[MODEL_HT]")]
    public class [MODEL] : #IF IsAuditable #THEN: Auditable,#ENDIF I[MODEL]
    {
        [MODEL_PROPERTIES]

        
    }
}
