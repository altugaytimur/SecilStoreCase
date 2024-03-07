using Microsoft.AspNetCore.Mvc;

namespace SecilStoreCase.Ui.ViewComponents.LayoutViewComponents
{
    public class _AdminFooterLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
