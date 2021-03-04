using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PlacetoPay.Integrations.Library.CSharp.Contracts;
using PlacetoPay.Integrations.Library.CSharp.Entities;
using PlacetoPay.Integrations.Library.CSharp.Message;
using Tienda.Models;
using Tienda.Servicios.Interfaces;
using P2P = PlacetoPay.Integrations.Library.CSharp.PlacetoPay;

namespace Tienda.Controllers
{
    public class OrdersController : Controller
    {
        #region Atributos
        private readonly IOrdersService _ordersService;
        private readonly IPaymentsService _paymentsService;
        private const string urlLocalhostDetails = "https://localhost:44336/Orders/Details/";
        private const string ipAddress = "181.78.12.121";
        private const string formatDate = "yyyy-MM-ddTHH:mm:sszzz";
        private const string currency = "COP"; 

        #endregion

        #region Constructor

        public OrdersController(IOrdersService ordersService, IPaymentsService paymentService)
        {
            this._ordersService = ordersService;
            this._paymentsService = paymentService;
        }

        #endregion

        #region Métodos

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View("Index", await _ordersService.GetOrders());
        }

        //// GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id, string message)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await this._ordersService.GetOrderDetails(id);

            ViewBag.PagosRechazados = response.IsRejected;
            ViewData["Success"] = response.Messagge;

            return View("Details", response);
        }

        //// GET: Orders/Payment/5
        public async Task<IActionResult> Payment(int? idOrden, string urlPago)
        {
            if (idOrden == null)
            {
                return NotFound();
            }

            var order = await _ordersService.GetOrder(idOrden.Value);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.UrlPago = urlPago == null ? "" : urlPago;

            return View("Payment", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(int idOrden)
        {
            var order = await _ordersService.GetOrder(idOrden);

            if (order == null)
            {
                return NotFound();
            }

            Gateway gateway =  _ordersService.GetGateway();

            Person person = new Person(
                                        document: order.CustomerDocument,
                                        documentType: "CC",
                                        name: order.CustomerName,
                                        surname: order.CustomerName.Replace(" ", ""),
                                        email: order.CustomerEmail,
                                        mobile: order.CustomerMobile
                                        );

            Amount amount = new Amount(Convert.ToDouble(order.ValorOrder), currency);
            PlacetoPay.Integrations.Library.CSharp.Entities.Payment payment = new PlacetoPay.Integrations.Library.CSharp.Entities.Payment($"TEST_{DateTime.Now:yyyyMMdd_hhmmss}_{order.Id}", $"Pago básico de prueba orden {order.Id} ", amount, false, person);
            RedirectRequest request = new RedirectRequest(payment,
                                                          urlLocalhostDetails + order.Id.ToString(),
                                                          ipAddress,
                                                          "PlacetoPay Sandbox",
                                                          (order.CreatedAt.AddMinutes(60)).ToString(formatDate),
                                                           person,
                                                           person);

            RedirectResponse response = gateway.Request(request);

            if (response.IsSuccessful())
            {
                Models.Payment pago = new Models.Payment()
                {
                    OrderId = order.Id,
                    Fecha = Convert.ToDateTime(response.Status.Date),
                    RequestId = Convert.ToInt32(response.RequestId),
                    UrlPago = response.ProcessUrl,
                    Status = response.Status.status,
                    Reason = response.Status.Reason,
                    Message = response.Status.Message
                };

                if (await _paymentsService.CreatePayment(pago) == 0)
                {
                    // NotFound Response Status 404.
                    return NotFound();
                }


                return RedirectToAction("Payment", "Orders", new { idOrden = order.Id, urlPago = response.ProcessUrl });
            }
            else
            {
                return RedirectToAction("Details", "Orders", new { id = order.Id, message = response.Status.Message });
            }
        }

        // GET: Orders/Create
        public IActionResult Create(string codigo, string descripcion, string imagen, string valor)
        {
            ViewBag.CodigoProducto = codigo;
            ViewBag.NombreProducto = descripcion;
            ViewBag.ImagenProducto = imagen;
            ViewBag.ValorProducto = valor;

            return View("Create");
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, string codigoProd, string descripcionProd, string valorProd)
        {
            if (ModelState.IsValid)
            {
                order.CreatedAt = DateTime.Now;
                order.UpdatedAt = DateTime.MinValue;
                order.ValorOrder = Convert.ToDouble(valorProd);
                order.Status = "CREATED";

                if (await _ordersService.CreateOrder(order) == 0)
                {
                    
                    return NotFound();
                }


                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = order.Id,
                    CodigoProducto = codigoProd,
                    NombreProducto = descripcionProd,
                    Cantidad = 1,
                    Valor = Convert.ToDouble(valorProd),
                    Total = Convert.ToDouble(valorProd)
                };

                if (await _ordersService.CreateOrderDetails(orderDetail) == 0)
                {
                   
                    return NotFound();
                }

                return RedirectToAction("Details", "Orders", new { id = order.Id });
            }
            return View("Create", order);
        }
        #endregion

    }
}
