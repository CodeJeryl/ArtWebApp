using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;
using WebMatrix.WebData;

namespace ArtProject2016.Functions
{
    public class CartControls
    {

        private ArtContext db = new ArtContext();

        public void EmptyCart()
        {
            var cartItems = db.Carts.Where(
                cart => cart.UserAccountId == WebSecurity.CurrentUserId);

            foreach (var cartItem in cartItems)
            {
                db.Carts.Remove(cartItem);
            }
            // Save changes
            db.SaveChanges();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = db.Carts.Count(cartItem => cartItem.UserAccountId == WebSecurity.CurrentUserId);
                
                //from cartItems in db.Carts
                  //        where cartItems.CartId == ShoppingCartId
                    //      select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? total = db.Carts.Where(cartItems => cartItems.UserAccountId == WebSecurity.CurrentUserId)
                                 .Sum(sub => (decimal?) sub.ForSale.Price) ?? 0;
          
            
            //(from cartItems in storeDB.Carts
            //         where cartItems.CartId == ShoppingCartId
            //         select (int?)cartItems.Count *
            //         cartItems.Album.Price).Sum();

            
            return total ?? decimal.Zero;
        }
    }
}