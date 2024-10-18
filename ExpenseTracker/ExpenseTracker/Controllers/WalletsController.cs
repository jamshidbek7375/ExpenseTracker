using ExpenseTracker.Application.Requests.Wallet;
using ExpenseTracker.Application.Stores.Interfaces;
using ExpenseTracker.Application.ViewModels.Wallet;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

public class WalletsController : Controller
{
    private readonly IWalletStore _store;

    public WalletsController(IWalletStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public IActionResult Index([FromQuery] GetWalletsRequest request)
    {
        var wallets = _store.GetAll(request);

        ViewBag.Search = request.Search;

        return View(wallets);
    }

    public IActionResult Details([FromRoute] WalletRequest request)
    {
        var wallet = _store.GetById(request);

        return View(wallet);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([FromForm] CreateWalletRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var createdWallet = _store.Create(request);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit([FromRoute] WalletRequest request)
    {
        var wallet = _store.GetById(request);

        return View(wallet);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit([FromForm] UpdateWalletRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        _store.Update(request);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete([FromRoute] WalletRequest request)
    {
        var wallet = _store.GetById(request);

        return View(wallet);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Delete")]
    public IActionResult DeleteConfirmed([FromForm] WalletRequest request)
    {
        _store.Delete(request);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Filters wallets
    /// </summary>
    /// <param name="search"></param>
    /// <returns>List of filtered wallets</returns>
    [Route("getWallets")]
    public ActionResult<WalletViewModel> GetWallets([FromQuery] GetWalletsRequest request)
    {
        var wallets = _store.GetAll(request);

        return Ok(wallets);
    }
}
