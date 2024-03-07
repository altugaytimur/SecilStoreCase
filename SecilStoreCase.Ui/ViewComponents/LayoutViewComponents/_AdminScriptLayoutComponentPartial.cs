using Microsoft.AspNetCore.Mvc;

namespace SecilStoreCase.Ui.ViewComponents.LayoutViewComponents
{
    public class _AdminScriptLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
