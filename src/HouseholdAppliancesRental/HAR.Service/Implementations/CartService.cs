using HAR.Common;
using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HAR.Service.Implementations
{
    public class CartService : ICartService
    {
        private readonly DataContext context;
        private readonly ICurrentUser currentUser;

        public CartService(DataContext context, ICurrentUser currentUser)
        {
            this.context = context;
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Finds current user's cart
        /// </summary>
        /// <returns>A cart including the bridge table with the products and their images</returns>
        public async Task<Cart?> FindByCurrentUserAsync()
        {
            return await this.context.Carts
                .Where(c => c.UserId == this.currentUser.UserId!)
                .Include(c => c.CartProducts)
                    .ThenInclude(cp => cp.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Updates current user's cart
        /// </summary>
        /// <returns>Custom response object</returns>
        public async Task<Response> Update()
        {
            try
            {
                Cart? cart = await this.FindByCurrentUserAsync();

                if (cart == null)
                {
                    return new Response()
                    {
                        IsSuccessful = false,
                        Message = "Количката не е намерена."
                    };
                }

                List<CartProduct> cartProducts = await this.context.CartProducts
                    .Where(cp => cp.Cart == cart)
                    .Include(cp => cp.Product)
                    .ToListAsync();

                cart.MonthPrice = 0;

                if (cartProducts != null && cartProducts.Count > 0)
                {
                    foreach (var cp in cartProducts)
                    {
                        cart.MonthPrice += cp.Product.Price * cp.ProductQuantity;
                    }
                }

                await this.context.SaveChangesAsync();

                return new Response
                {
                    IsSuccessful = true,
                    Message = "Количката е обновена успешно."
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccessful = false,
                    Message = "Количката не успя да се обнови.",
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Creates a cart for the coresponding user
        /// </summary>
        /// <param name="user">Object of a user</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> CreateCartAsync(User user)
        {
            try
            {
                Cart cart = new Cart()
                {
                    Id = Guid.Parse(user.Id),
                    MonthPrice = 0,
                    UserId = user.Id
                };

                this.context.Carts.Add(cart);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Количката бе успешно създадена."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Количката не успя да се създаде.",
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Adds a product to current user's cart or increments the quantity
        /// </summary>
        /// <param name="cart">Cart object</param>
        /// <param name="product">Product object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> AddProductAsync(Cart cart, Product product)
        {
            try
            {
                CartProduct? cartProduct = await this.context.CartProducts
                    .FirstOrDefaultAsync(cp => cp.Cart == cart && cp.Product == product);

                if (cartProduct != null)
                {
                    cartProduct.ProductQuantity += 1;

                    cart.MonthPrice += product.Price;

                    await this.context.SaveChangesAsync();

                    return new Response()
                    {
                        IsSuccessful = true,
                        Message = "Увелечена бройката на продукта в количката."
                    };
                }

                cartProduct = new CartProduct()
                {
                    Cart = cart,
                    Product = product,
                    ProductQuantity = 1,
                    RentMonths = 1
                };

                cart.MonthPrice += product.Price;

                this.context.CartProducts.Add(cartProduct);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Продуктът бе успешно добавен в количката."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Продуктът не успя да се добави в количката.",
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Reduces a product from current user's cart or removes it entirely
        /// </summary>
        /// <param name="cart">Cart object</param>
        /// <param name="product">Product object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> ReduceProductAsync(Cart cart, Product product)
        {
            try
            {
                CartProduct? cartProduct = await this.context.CartProducts
                    .FirstOrDefaultAsync(cp => cp.Cart == cart && cp.Product == product);

                if (cartProduct == null)
                {
                    return new Response()
                    {
                        IsSuccessful = false,
                        Message = "Продуктът не е в количката."
                    };
                }

                if (cartProduct.ProductQuantity <= 1)
                {
                    return await this.RemoveProductAsync(cart, product);
                }

                cartProduct.ProductQuantity -= 1;

                cart.MonthPrice -= product.Price;

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Продуктовата бройка бе успешно намалена."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Продуктовата бройка не успя да се намали.",
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Removes a product from current user's cart
        /// </summary>
        /// <param name="cart">Cart object</param>
        /// <param name="product">Product object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> RemoveProductAsync(Cart cart, Product product)
        {
            try
            {
                CartProduct? cartProduct = await this.context.CartProducts
                    .FirstOrDefaultAsync(cp => cp.Cart == cart && cp.Product == product);

                if (cartProduct == null)
                {
                    return new Response()
                    {
                        IsSuccessful = false,
                        Message = "Продуктът не е в количката."
                    };
                }

                cart.MonthPrice -= cartProduct.ProductQuantity * product.Price;

                this.context.CartProducts.Remove(cartProduct);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Продуктът бе успешно премахнат от количката."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Продуктът не успя да се премахне от количката.",
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Adds a month to the product from current user's cart
        /// </summary>
        /// <param name="cart">Cart object</param>
        /// <param name="product">Product object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> AddMonthAsync(Cart cart, Product product)
        {
            try
            {
                CartProduct? cartProduct = await this.context.CartProducts
                    .FirstOrDefaultAsync(cp => cp.Cart == cart && cp.Product == product);

                if (cartProduct == null)
                {
                    return new Response()
                    {
                        IsSuccessful = false,
                        Message = "Продуктът не е в количката."
                    };
                }

                cartProduct.RentMonths += 1;

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Месеците успошно увеличени за продукта."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Месеците не успяха да се увеличат за продукта.",
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Reduces a month from the product from current user's cart or removes it completely
        /// </summary>
        /// <param name="cart">Cart object</param>
        /// <param name="product">Product object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> ReduceMonthAsync(Cart cart, Product product)
        {
            try
            {
                CartProduct? cartProduct = await this.context.CartProducts
                    .FirstOrDefaultAsync(cp => cp.Cart == cart && cp.Product == product);

                if (cartProduct == null)
                {
                    return new Response()
                    {
                        IsSuccessful = false,
                        Message = "Продуктът не е в количката."
                    };
                }

                if (cartProduct.RentMonths <= 1)
                {
                    return await this.RemoveProductAsync(cart, product);
                }

                cartProduct.RentMonths -= 1;

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Месеците успошно намалени за продукта."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Месеците не успяха да се намалят за продукта.",
                    Details = ex.Message
                };
            }
        }
    }
}
