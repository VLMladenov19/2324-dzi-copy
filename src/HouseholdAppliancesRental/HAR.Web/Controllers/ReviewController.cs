using HAR.Common;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Web.Models.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HAR.Web.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IReviewService reviewService;
        private readonly ICurrentUser currentUser;

        public ReviewController(IReviewService reviewService, ICurrentUser currentUser)
        {
            this.reviewService = reviewService;
            this.currentUser = currentUser;
        }

        [HttpGet]
        public IActionResult Create(Guid productId)
        {
            CreateReviewViewModel model = new CreateReviewViewModel()
            {
                ProductId = productId
            };
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReviewViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            Review review = new Review()
            {
                Rating = model.Rating,
                Comment = model.Comment,
                UserId = this.currentUser.UserId!,
                ProductId = model.ProductId
            };

            Response response = await this.reviewService.CreateReviewAsync(review);

            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("CreateReviewErrors", response.Message!);
                return this.View(model);
            }

            return this.RedirectToAction("Details", "Product", new { id = model.ProductId });
        }
    }
}
