using Microsoft.AspNetCore.Mvc;

namespace SecilStoreCase.Ui.ViewComponents.LayoutViewComponents
{
    public class _AdminNavbarLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
