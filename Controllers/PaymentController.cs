using LibrayBackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace LibrayBackEnd.Controllers
{
    public class PaymentController : ControllerBase
    {
        private readonly IOptions<StripeOptions> _options;
        private readonly IStripeClient _client;


        public PaymentController(IOptions<StripeOptions> options)
        {
            _options = options;
            _client = new StripeClient(options.Value.SecretKey);
        }

        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent(int amount)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "gbp",
                PaymentMethod = "pm_card_visa"
            };
            var service = new PaymentIntentService(_client);
            try
            {
                var paymentIntent = await service.CreateAsync(options);
                return Ok(new CreatePaymentIntentResponse
                {
                    ClientSecret = paymentIntent.ClientSecret
                });
            } catch (Stripe.StripeException ex)
            {
                return BadRequest(new
                {
                    Error = new
                    {
                        Message = ex.Message
                    }

                });

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }

}

    