using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtProject2016.Models;
using WebMatrix.WebData;

namespace ArtProject2016.Functions
{
    public class SellControls
    {
        private ArtContext db = new ArtContext();

        public decimal GetPending()
        {
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? pending = db.OrderDetails.Where(cash => cash.ForSale.SellerId == WebSecurity.CurrentUserId && cash.Order.Paid
                && cash.OrderDetailStatus == "Shipment Processing" || cash.OrderDetailStatus == "Shipped" || cash.OrderDetailStatus == "Paid" || cash.OrderDetailStatus == "Delivered" && cash.Redeemed != true)
                                 .Sum(sub => (decimal?)sub.ForSale.Profit) ?? 0;


            //(from cartItems in storeDB.Carts
            //         where cartItems.CartId == ShoppingCartId
            //         select (int?)cartItems.Count *
            //         cartItems.Album.Price).Sum();


            return pending ?? decimal.Zero;
        }

        public decimal GetRedeemable()
        {
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? redeem = db.OrderDetails.Where(cash => cash.ForSale.SellerId == WebSecurity.CurrentUserId && cash.Order.Paid
                && cash.OrderDetailStatus == "Finished" && cash.Redeemed != true)
                                 .Sum(sub => (decimal?)sub.ForSale.Profit) ?? 0;


            //(from cartItems in storeDB.Carts
            //         where cartItems.CartId == ShoppingCartId
            //         select (int?)cartItems.Count *
            //         cartItems.Album.Price).Sum();


            return redeem ?? decimal.Zero;
        }
    }
}