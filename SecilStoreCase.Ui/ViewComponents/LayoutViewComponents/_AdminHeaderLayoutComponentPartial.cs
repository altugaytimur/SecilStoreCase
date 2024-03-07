using Microsoft.AspNetCore.Mvc;

namespace SecilStoreCase.Ui.ViewComponents.LayoutViewComponents
{
    public class _AdminHeaderLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
