using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StarEnergi.Models;
using Telerik.Web.Mvc.Infrastructure;
using Telerik.Web.Mvc;

namespace StarEnergi.Utilities
{
    
    public static class CustomBindingExtensions
    {
        public static IQueryable<she_observation> ApplyFiltering(this IQueryable<she_observation> data, IList<IFilterDescriptor>
            filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                data = data.Where(ExpressionBuilder.Expression<she_observation>(filterDescriptors));
            }
            return data;
        }

        public static IQueryable<she_observation> ApplyUserFiltering(this IQueryable<she_observation> data, int id, string name)
        {
            string ids = "#" + id.ToString();
            data = from t in data where t.observer.EndsWith(ids) select t;
            return data;
        }

        public static IEnumerable ApplyGrouping(this IQueryable<she_observation> data, IList<GroupDescriptor> groupDescriptors)
        {
            Func<IEnumerable<she_observation>, IEnumerable<AggregateFunctionsGroup>> selector = null;
            foreach (var group in groupDescriptors.Reverse())
            {
                if (selector == null)
                {
                    if (group.Member == "date_time")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.date_time);
                    }
                    else if (group.Member == "observer")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.observer);
                    }
                    else if (group.Member == "department")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.department);
                    }
                    else if (group.Member == "location")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.location);
                    }
                    else if (group.Member == "quality")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.quality);
                    }
                }
                else
                {
                    if (group.Member == "date_time")
                    {
                        selector = BuildGroup(o => o.date_time, selector);
                    }
                    else if (group.Member == "observer")
                    {
                        selector = BuildGroup(o => o.observer, selector);
                    }
                    else if (group.Member == "department")
                    {
                        selector = BuildGroup(o => o.department, selector);
                    }
                    else if (group.Member == "location")
                    {
                        selector = BuildGroup(o => o.location, selector);
                    }
                    else if (group.Member == "quality")
                    {
                        selector = BuildGroup(o => o.quality, selector);
                    }
                }
            }
            return selector.Invoke(data).ToList();
        }
        private static Func<IEnumerable<she_observation>, IEnumerable<AggregateFunctionsGroup>> BuildGroup<T>(Func<she_observation, T>
            groupSelector, Func<IEnumerable<she_observation>, IEnumerable<AggregateFunctionsGroup>> selectorBuilder)
        {
            var tempSelector = selectorBuilder;
            return g => g.GroupBy(groupSelector)
                         .Select(c => new AggregateFunctionsGroup
                         {
                             Key = c.Key,
                             HasSubgroups = true,
                             Items = tempSelector.Invoke(c).ToList()
                         });
        }
        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<she_observation> group,
            Func<she_observation, T> groupSelector)
        {
            return group.GroupBy(groupSelector)
                    .Select(i => new AggregateFunctionsGroup
                    {
                        Key = i.Key,
                        Items = i.ToList()
                    });
        }
        public static IQueryable<she_observation> ApplyPaging(this IQueryable<she_observation> data, int currentPage, int pageSize)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                data = data.Skip((currentPage - 1) * pageSize);
            }
            data = data.Take(pageSize);
            return data;
        }
        public static IQueryable<she_observation> ApplySorting(this IQueryable<she_observation> data,
            IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
        {
            if (groupDescriptors.Any())
            {
                foreach (var groupDescriptor in groupDescriptors.Reverse())
                {
                    data = AddSortExpression(data, groupDescriptor.SortDirection, groupDescriptor.Member);
                }
            }
            if (sortDescriptors.Any())
            {
                foreach (SortDescriptor sortDescriptor in sortDescriptors)
                {
                    data = AddSortExpression(data, sortDescriptor.SortDirection, sortDescriptor.Member);
                }
            }
            else
            {
                data = AddSortExpression(data, ListSortDirection.Descending, "date_time");
            }
            return data;
        }
        private static IQueryable<she_observation> AddSortExpression(IQueryable<she_observation> data, ListSortDirection sortDirection,
            string memberName)
        {
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (memberName)
                {
                    case "date_time":
                        data = data.OrderBy(order => order.date_time);
                        break;
                    case "observer":
                        data = data.OrderBy(order => order.observer);
                        break;
                    case "department":
                        data = data.OrderBy(order => order.department);
                        break;
                    case "location":
                        data = data.OrderBy(order => order.location);
                        break;
                    case "quality":
                        data = data.OrderBy(order => order.quality);
                        break;
                }
            }
            else
            {
                switch (memberName)
                {
                    case "date_time":
                        data = data.OrderByDescending(order => order.date_time);
                        break;
                    case "observer":
                        data = data.OrderByDescending(order => order.observer);
                        break;
                    case "department":
                        data = data.OrderByDescending(order => order.department);
                        break;
                    case "location":
                        data = data.OrderByDescending(order => order.location);
                        break;
                }
            }
            return data;
        }
    }
}