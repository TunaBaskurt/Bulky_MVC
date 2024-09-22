using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
   public  class ShoppingCartVM//Sepetim kısmı db den eklenen ürünler çeşitleri ve sayıları alınıyor
    {
        public IEnumerable<ShoppingCart>ShoppingCartList { get; set; }
        public OrderHeader OrderHeader { get; set; }
      
    }
}
