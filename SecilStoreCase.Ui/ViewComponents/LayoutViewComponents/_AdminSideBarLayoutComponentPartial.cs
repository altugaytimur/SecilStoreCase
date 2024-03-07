using Microsoft.AspNetCore.Mvc;

namespace SecilStoreCase.Ui.ViewComponents.LayoutViewComponents
{
    public class _AdminSidebarLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
