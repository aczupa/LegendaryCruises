using LegendaryCruises.Data;
using LegendaryCruises.Interfaces;
using LegendaryCruises.Models;
using Microsoft.EntityFrameworkCore;

namespace LegendaryCruises.Services
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _context;
        private readonly ICartService _cartService;
        private readonly IEmailService _emailService;
        private readonly IQRCodeService _qrService;

        public OrderService(
            DataContext context,
            ICartService cartService,
            IEmailService emailService,
            IQRCodeService qrService)
        {
            _context = context;
            _cartService = cartService;
            _emailService = emailService;
            _qrService = qrService;
        }

        public async Task<Order> CreateOrder(string userId)
        {
            var cart = await _cartService.GetCart(userId);

            if (cart == null || !cart.Items.Any())
                throw new Exception("Panier vide — impossible de créer une commande.");

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity),
                IsPaid = false
            };

            _context.Orders.Add(order);

            var cabinIds = cart.Items.Select(i => i.DateCabinId).Distinct().ToList();
            var cabins = await _context.DateCabins
                .Where(c => cabinIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id);

            foreach (var item in cart.Items)
            {
                if (!cabins.TryGetValue(item.DateCabinId, out var cabin))
                    throw new Exception("Cabine introuvable.");

                if (item.Quantity > cabin.Available)
                    throw new Exception("Le nombre de cabines disponibles est insuffisant.");

                cabin.Reserved += item.Quantity;

                var orderItem = new OrderItem
                {
                    Order = order,
                    CruiseId = item.CruiseId,
                    CruiseDateId = item.CruiseDateId,
                    DateCabinId = item.DateCabinId,
                    CabinType = item.CabinType,
                    Price = item.Price,
                    Quantity = item.Quantity
                };

                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            await _cartService.ClearCart(userId);

            return order;
        }

        public async Task MarkAsPaid(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Commande introuvable.");

            order.IsPaid = true;
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderById(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Items).ThenInclude(i => i.Cruise)
                .Include(o => o.Items).ThenInclude(i => i.CruiseDate)
                .Include(o => o.Items).ThenInclude(i => i.DateCabin)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task ProcessOrderAfterPayment(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Commande introuvable.");

            order.IsPaid = true;
            await _context.SaveChangesAsync();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == order.UserId);
            if (user == null)
                throw new Exception("Utilisateur introuvable.");

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == order.UserId);
            if (profile == null)
                throw new Exception("Profil utilisateur introuvable.");

            // GENERATE QR
            var (base64, bytes) = _qrService.GenerateQRCodeWithBytes($"ORDER-{order.Id}");

            var qrEntity = new QRCodeModel
            {
                UserProfileId = profile.Id,
                QRCodeBase64 = base64,
                Text = $"Billet de croisière #{order.Id}",
                UniqueCode = Guid.NewGuid().ToString(),
                QRCodeScanned = false,
                DateScan = null
            };

            _context.QRCodeModels.Add(qrEntity);
            await _context.SaveChangesAsync();

            // SEND EMAIL
            string html = $@"
<div style='font-family:Arial, sans-serif; color:#333; line-height:1.6;'>
    <h2 style='color:#003366;'>Confirmation de votre réservation</h2>

    <p>Bonjour,</p>

    <p>
        Nous avons le plaisir de vous confirmer que votre paiement a été validé avec succès.<br/>
        <strong>Numéro de commande : {orderId}</strong>
    </p>

    <h3 style='margin-top:25px; color:#003366;'>Détails de votre croisière</h3>
    <div style='border:1px solid #ddd; padding:15px; border-radius:8px; background:#fafafa;'>

        {string.Join("", order.Items.Select(item => $@"
            <div style='margin-bottom:20px;'>
                <p style='margin:0; font-size:16px;'>
                    <strong>Destination :</strong> {item.Cruise?.Title}
                </p>

                {(item.CruiseDate != null ? $@"
                    <p style='margin:0;'>
                        <strong>Dates :</strong> {item.CruiseDate.DepartureDate:dd MMM yyyy} → {item.CruiseDate.ReturnDate:dd MMM yyyy}
                    </p>
                " : "")}

                <p style='margin:0;'>
                    <strong>Cabine réservée :</strong> {item.CabinType}
                </p>

                <p style='margin:0;'>
                    <strong>Tarif :</strong> {item.Price} {(item.Cruise?.Currency ?? "€")} × {item.Quantity}
                </p>
            </div>
        "))}
    </div>

    <h3 style='margin-top:25px; color:#003366;'>Montant total payé</h3>
    <p style='font-size:18px; font-weight:bold;'>{order.TotalPrice} €</p>

    <p style='margin-top:30px;'>
        Toute l’équipe de <strong>Legendary Cruises</strong> vous remercie pour votre confiance.<br/>
        Nous avons hâte de vous accueillir à bord pour une expérience inoubliable.
    </p>

    <p style='margin-top:20px;'>
        <em>Ce message est une confirmation automatique, votre billet électronique et votre QR code d’embarquement.</em>
    </p>

    <hr style='margin-top:40px;' />

    <p style='font-size:12px; color:#777;'>
        Legendary Cruises — Service Client<br/>
        Email : legendary.cruises.booking@gmail.com<br/>
        Téléphone : +33 6 00 00 00 00
    </p>
</div>
";


            await _emailService.SendEmailWithAttachmentAsync(
                user.Email!,
                "Votre billet de croisière",
                html,
                bytes,
                $"billet-{order.Id}.png"
            );
        }
    }
}
