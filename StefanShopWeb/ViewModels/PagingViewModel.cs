using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace StefanShopWeb.ViewModels
{
    public class PagingViewModel
    {
        public IQueryable<dynamic> SetPaging(int page, int pageSize, IQueryable<dynamic> customersQuery)
        {
            Page = page;
            PageSize = pageSize;
            var pageCount = (double)customersQuery.Count() / PageSize;
            MaxPages = (int)Math.Ceiling(pageCount);
            CurrentPage = Page;

            customersQuery = customersQuery.Skip((Page - 1) * PageSize).Take(PageSize);
            return customersQuery;
        }

        public IEnumerable<string> GetPages
        {
            get
            {
                int delta = 2;
                int left = CurrentPage - delta;
                int right = CurrentPage + delta + 1;

                var range = new List<string>();
                for (int i = 1; i <= MaxPages; i++)
                    if (i == 1 || i == MaxPages || (i >= left && i < right))
                        range.Add(i.ToString());

                var rangeIncludingDots = new List<string>();
                int l = 0;
                foreach (var i in range.Select(r => Convert.ToInt32(r)))
                {
                    if (l > 0)
                    {
                        if (i - l == 2)
                            rangeIncludingDots.Add((l + 1).ToString());
                        else if (i - l != 1)
                            rangeIncludingDots.Add("...");
                    }

                    rangeIncludingDots.Add(i.ToString());
                    l = i;
                }

                return rangeIncludingDots;
            }
        }

        public int PageSize { get; set; } = 4;
        public int Page { get; set; } = 1;
        public IEnumerable<SelectListItem> PageSizeOptions
        {
            get
            {
                return new[]
                {
                    new SelectListItem("Visa 4", "4"),
                    new SelectListItem("Visa 10", "10"),
                    new SelectListItem("Visa 15", "15")
                };
            }
        }   
        public int CurrentPage { get; set; }
        public int MaxPages { get; set; }   
    }
}