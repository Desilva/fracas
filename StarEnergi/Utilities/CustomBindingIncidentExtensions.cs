using StarEnergi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Infrastructure;

namespace StarEnergi.Utilities
{
    public static class CustomBindingIncidentExtensions
    {
        public static IQueryable<IREntity> ApplyFiltering(this IQueryable<IREntity> data, IList<IFilterDescriptor>
            filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                foreach (Object fd in filterDescriptors)
                {
                    Debug.WriteLine("c = " + fd.GetType().ToString());
                    if (fd is FilterDescriptor)
                    {
                        
                        FilterDescriptor f = fd as FilterDescriptor;

                        data = ExpressionBuilders(data,f);
                    }
                    else if (fd is CompositeFilterDescriptor)
                    {
                        CompositeFilterDescriptor f = fd as CompositeFilterDescriptor;
                        foreach (FilterDescriptor ff in f.FilterDescriptors)
                        {
                            data = ExpressionBuilders(data, ff);
                        }
                    }
                    
                }
                //data = data.Where(ExpressionBuilder.Expression<IREntity>(filterDescriptors));
            }
            return data;
        }

        private static IQueryable<IREntity> ExpressionBuilders(this IQueryable<IREntity> data, FilterDescriptor f)
        {
            string value;
            switch (f.Member)
            {
                case "rca_number":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.rca_number.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.rca_number.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.rca_number.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.rca_number != value);
                            break;
                        default:
                            data = data.Where(p => p.rca_number == value);
                            break;
                    }
                    break;
                case "reference_number":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.reference_number.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.reference_number.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.reference_number.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.reference_number != value);
                            break;
                        default:
                            data = data.Where(p => p.reference_number == value);
                            break;
                    }
                    break;
                case "title":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.title.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.title.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.title.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.title != value);
                            break;
                        default:
                            data = data.Where(p => p.title == value);
                            break;
                    }
                    break;
                case "date_incident":
                    DateTime val = (f.Value as DateTime?).Value;
                    switch (f.Operator)
                    {
                        case FilterOperator.IsGreaterThan:
                            data = data.Where(p => p.date_incident != null ? p.date_incident.Value.CompareTo(val) > 0 : false);
                            break;
                        case FilterOperator.IsGreaterThanOrEqualTo:
                            data = data.Where(p => p.date_incident != null ? p.date_incident.Value.CompareTo(val) >= 0 : false);
                            break;
                        case FilterOperator.IsLessThan:
                            data = data.Where(p => p.date_incident != null ? p.date_incident.Value.CompareTo(val) < 0 : false);
                            break;
                        case FilterOperator.IsLessThanOrEqualTo:
                            data = data.Where(p => p.date_incident != null ? p.date_incident.Value.CompareTo(val) <= 0 : false);
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.date_incident != null ? p.date_incident.Value.CompareTo(val) != 0 : true);
                            break;
                        default:
                            data = data.Where(p => p.date_incident != null ? p.date_incident.Value.CompareTo(val) == 0 : false);
                            break;
                    }
                    break;
                case "inves":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.inves.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.inves.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.inves.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.inves != value);
                            break;
                        default:
                            data = data.Where(p => p.inves == value);
                            break;
                    }
                    break;
                case "facility":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.facility.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.facility.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.facility.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.facility != value);
                            break;
                        default:
                            data = data.Where(p => p.facility == value);
                            break;
                    }
                    break;
                case "incident_location":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.incident_location.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.incident_location.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.incident_location.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.incident_location != value);
                            break;
                        default:
                            data = data.Where(p => p.incident_location == value);
                            break;
                    }
                    break;
                case "type_report":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.type_report.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.type_report.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.type_report.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.type_report != value);
                            break;
                        default:
                            data = data.Where(p => p.type_report == value);
                            break;
                    }
                    break;
                case "incident_type":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.incident_type.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.incident_type.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.incident_type.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.incident_type != value);
                            break;
                        default:
                            data = data.Where(p => p.incident_type == value);
                            break;
                    }
                    break;
                case "actual_loss":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.actual_loss.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.actual_loss.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.actual_loss.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.actual_loss != value);
                            break;
                        default:
                            data = data.Where(p => p.actual_loss == value);
                            break;
                    }
                    break;
                case "potential_loss":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.potential_loss.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.potential_loss.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.potential_loss.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.potential_loss != value);
                            break;
                        default:
                            data = data.Where(p => p.potential_loss == value);
                            break;
                    }
                    break;
                case "probability_str":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.probability_str.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.probability_str.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.probability_str.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.probability_str != value);
                            break;
                        default:
                            data = data.Where(p => p.probability_str == value);
                            break;
                    }
                    break;
                case "prepared_by":
                    value = f.Value.ToString();
                    switch (f.Operator)
                    {
                        case FilterOperator.Contains:
                            data = data.Where(p => p.prepared_by.Contains(value));
                            break;
                        case FilterOperator.StartsWith:
                            data = data.Where(p => p.prepared_by.StartsWith(value));
                            break;
                        case FilterOperator.EndsWith:
                            data = data.Where(p => p.prepared_by.EndsWith(value));
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.prepared_by != value);
                            break;
                        default:
                            data = data.Where(p => p.prepared_by == value);
                            break;
                    }
                    break;
                case "prepare_date":
                    DateTime val2 = (f.Value as DateTime?).Value;
                    switch (f.Operator)
                    {
                        case FilterOperator.IsGreaterThan:
                            data = data.Where(p => p.prepare_date != null ? p.prepare_date.Value.CompareTo(val2) > 0 : false);
                            break;
                        case FilterOperator.IsGreaterThanOrEqualTo:
                            data = data.Where(p => p.prepare_date != null ? p.prepare_date.Value.CompareTo(val2) >= 0 : false);
                            break;
                        case FilterOperator.IsLessThan:
                            data = data.Where(p => p.prepare_date != null ? p.prepare_date.Value.CompareTo(val2) < 0 : false);
                            break;
                        case FilterOperator.IsLessThanOrEqualTo:
                            data = data.Where(p => p.prepare_date != null ? p.prepare_date.Value.CompareTo(val2) <= 0 : false);
                            break;
                        case FilterOperator.IsNotEqualTo:
                            data = data.Where(p => p.prepare_date != null ? p.prepare_date.Value.CompareTo(val2) != 0 : true);
                            break;
                        default:
                            data = data.Where(p => p.prepare_date != null ? p.prepare_date.Value.CompareTo(val2) == 0 : false);
                            break;
                    }
                    break;
            }

            return data;
        }

        public static IEnumerable ApplyGrouping(this IQueryable<IREntity> data, IList<GroupDescriptor> groupDescriptors)
        {
            Func<IEnumerable<IREntity>, IEnumerable<AggregateFunctionsGroup>> selector = null;
            foreach (var group in groupDescriptors.Reverse())
            {
                if (selector == null)
                {
                    if (group.Member == "reference_number")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.reference_number);
                    }
                    else if (group.Member == "title")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.title);
                    }
                    else if (group.Member == "date_incident")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.date_incident);
                    }
                    else if (group.Member == "inves")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.investigation);
                    }
                    else if (group.Member == "facility")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.facility);
                    }
                    else if (group.Member == "incident_location")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.incident_location);
                    }
                    else if (group.Member == "type_report")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.type_of_report);
                    }
                    else if (group.Member == "incident_type")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.incident_type);
                    }
                    else if (group.Member == "actual_loss")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.actual_loss_severity);
                    }
                    else if (group.Member == "potential_loss")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.potential_loss_severity);
                    }
                    else if (group.Member == "probability_str")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.probability);
                    }
                    else if (group.Member == "prepared_by")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.prepared_by);
                    }
                    else if (group.Member == "prepare_date")
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.prepare_date);
                    }
                }
                else
                {
                    if (group.Member == "reference_number")
                    {
                        selector = BuildGroup(o => o.reference_number, selector);
                    }
                    else if (group.Member == "title")
                    {
                        selector = BuildGroup(o => o.title, selector);
                    }
                    else if (group.Member == "date_incident")
                    {
                        selector = BuildGroup(o => o.date_incident, selector);
                    }
                    else if (group.Member == "inves")
                    {
                        selector = BuildGroup(o => o.investigation, selector);
                    }
                    else if (group.Member == "facility")
                    {
                        selector = BuildGroup(o => o.facility, selector);
                    }
                    else if (group.Member == "incident_location")
                    {
                        selector = BuildGroup(o => o.incident_location, selector);
                    }
                    else if (group.Member == "type_report")
                    {
                        selector = BuildGroup(o => o.type_of_report, selector);
                    }
                    else if (group.Member == "incident_type")
                    {
                        selector = BuildGroup(o => o.incident_type, selector);
                    }
                    else if (group.Member == "actual_loss")
                    {
                        selector = BuildGroup(o => o.actual_loss_severity, selector);
                    }
                    else if (group.Member == "potential_loss")
                    {
                        selector = BuildGroup(o => o.potential_loss_severity, selector);
                    }
                    else if (group.Member == "probability_str")
                    {
                        selector = BuildGroup(o => o.probability, selector);
                    }
                    else if (group.Member == "prepared_by")
                    {
                        selector = BuildGroup(o => o.prepared_by, selector);
                    }
                    else if (group.Member == "prepare_date")
                    {
                        selector = BuildGroup(o => o.prepare_date, selector);
                    }
                }
            }
            return selector.Invoke(data).ToList();
        }
        private static Func<IEnumerable<IREntity>, IEnumerable<AggregateFunctionsGroup>> BuildGroup<T>(Func<IREntity, T>
            groupSelector, Func<IEnumerable<IREntity>, IEnumerable<AggregateFunctionsGroup>> selectorBuilder)
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
        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<IREntity> group,
            Func<IREntity, T> groupSelector)
        {
            return group.GroupBy(groupSelector)
                    .Select(i => new AggregateFunctionsGroup
                    {
                        Key = i.Key,
                        Items = i.ToList()
                    });
        }
        public static IQueryable<IREntity> ApplyPaging(this IQueryable<IREntity> data, int currentPage, int pageSize)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                data = data.Skip((currentPage - 1) * pageSize);
            }
            data = data.Take(pageSize);
            return data;
        }
        public static IQueryable<IREntity> ApplySorting(this IQueryable<IREntity> data,
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
                data = AddSortExpression(data, ListSortDirection.Descending, "reference_number");
            }
            return data;
        }
        private static IQueryable<IREntity> AddSortExpression(IQueryable<IREntity> data, ListSortDirection sortDirection,
            string memberName)
        {
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (memberName)
                {
                    case "reference_number":
                        data = data.OrderBy(order => order.reference_number);
                        break;
                    case "title":
                        data = data.OrderBy(order => order.title);
                        break;
                    case "date_incident":
                        data = data.OrderBy(order => order.date_incident);
                        break;
                    case "inves":
                        data = data.OrderBy(order => order.investigation);
                        break;
                    case "facility":
                        data = data.OrderBy(order => order.facility);
                        break;
                    case "incident_location":
                        data = data.OrderBy(order => order.incident_location);
                        break;
                    case "type_report":
                        data = data.OrderBy(order => order.type_of_report);
                        break;
                    case "incident_type":
                        data = data.OrderBy(order => order.incident_type);
                        break;
                    case "actual_loss":
                        data = data.OrderBy(order => order.actual_loss_severity);
                        break;
                    case "potential_loss":
                        data = data.OrderBy(order => order.potential_loss_severity);
                        break;
                    case "probability_str":
                        data = data.OrderBy(order => order.probability);
                        break;
                    case "prepared_by":
                        data = data.OrderBy(order => order.prepared_by);
                        break;
                    case "prepare_date":
                        data = data.OrderBy(order => order.prepare_date);
                        break;
                    case "rca_number":
                        data = data.OrderBy(order => order.rca_number);
                        break;
                }
            }
            else
            {
                switch (memberName)
                {
                    case "reference_number":
                        data = data.OrderByDescending(order => order.reference_number);
                        break;
                    case "title":
                        data = data.OrderByDescending(order => order.title);
                        break;
                    case "date_incident":
                        data = data.OrderByDescending(order => order.date_incident);
                        break;
                    case "inves":
                        data = data.OrderByDescending(order => order.investigation);
                        break;
                    case "facility":
                        data = data.OrderByDescending(order => order.facility);
                        break;
                    case "incident_location":
                        data = data.OrderByDescending(order => order.incident_location);
                        break;
                    case "type_report":
                        data = data.OrderByDescending(order => order.type_of_report);
                        break;
                    case "incident_type":
                        data = data.OrderByDescending(order => order.incident_type);
                        break;
                    case "actual_loss":
                        data = data.OrderByDescending(order => order.actual_loss_severity);
                        break;
                    case "potential_loss":
                        data = data.OrderByDescending(order => order.potential_loss_severity);
                        break;
                    case "probability_str":
                        data = data.OrderByDescending(order => order.probability);
                        break;
                    case "prepared_by":
                        data = data.OrderByDescending(order => order.prepared_by);
                        break;
                    case "prepare_date":
                        data = data.OrderByDescending(order => order.prepare_date);
                        break;
                    case "rca_number":
                        data = data.OrderByDescending(order => order.rca_number);
                        break;
                }
            }
            return data;
        }
    }
}