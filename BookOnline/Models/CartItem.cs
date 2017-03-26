using System;
using System.Collections.Generic;
using System.Web;

namespace BookOnline.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Stock { get; set; }

        private double price;
        public double Price
        {
            get { return this.price; }
            set { 
                this.price = value;
                this.subTotal = this.price * this.quantity;
            }
        }

        private int quantity;
        public int Quantity
        {
            get { return this.quantity; }
            set { 
                this.quantity = value;
                this.subTotal = this.price * this.quantity;
            }
        }

        private double subTotal;
        public double SubTotal
        {
            get { return this.subTotal; }            
        }

        public override bool Equals(object obj)
        {
            bool ans=false;
            if (obj is CartItem)
            { 
                CartItem ii=(CartItem)obj;
                if (ii.Id == this.Id)
                    ans = true;
            }
            return ans;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }


    }
}