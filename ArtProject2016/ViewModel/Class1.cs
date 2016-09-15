using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtProject2016.ViewModel
{
    public class Class1
    {
        public int MyProperty { get; set; }
        public static int i = 5;
        public static string sd = "sd";

        
        private static void privatestaticvoidTO()
        {
         
        }

        private static void staticvoidTO()
        {

        }

        public static void publicStaticVoidTo()
        {
            privatestaticvoidTO(); // You can use the static method inside
        }

        public void VoidLngTO()
        {
          
            staticvoidTO();
        }
    }

    static class CompanyInfo
    {
        public static string GetCompanyName() { return "CompanyName"; }
        public static string GetCompanyAddress() { return "CompanyAddress"; }
        //...
    }
}