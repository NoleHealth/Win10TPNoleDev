using System.Web.Mvc;

namespace BackOffice2.Areas.[REG]
{
    public class [REG]AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get{ return "[REG]"; }
        }
        
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("[REG]_default",
                "[REG]/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}
