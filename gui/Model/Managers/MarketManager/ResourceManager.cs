using PlayerInput.Model.Utils;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace PlayerInput.Model.Managers.MarketManager
{
    public class ResourceManager
    {
        // 1. Fields
        private List<LinkedList<Resource>> _empty = 
            ListUtils.EnumToList<ResourceType, LinkedList<Resource>>(_ => []);

        private List<LinkedList<Resource>> _filled =
            ListUtils.EnumToList<ResourceType, LinkedList<Resource>>(_ => []);

        // 2. Public Methods
        public void Create(int price, ResourceType type)
        {
            // Build resource
            var id = _empty[(int)type].Count;
            // Add it to empty
            _empty[(int)type].AddLast(new Resource(type, price, id));
        }

        public void Clear()
        {
            // Clear all fields
            _empty = ListUtils.EnumToList<ResourceType, LinkedList<Resource>>(_ => []);
            _filled = ListUtils.EnumToList<ResourceType, LinkedList<Resource>>(_ => []);
        }

        public ResourceType GetMostLimitedResourceType()
        {
            ResourceType resource = _empty
                .Select((list, index) => new { ResourceType = (ResourceType)index, list.Count })
                .OrderByDescending(r => r.Count)
                .First().ResourceType;

            Log.Information($"Most limited resource: {resource}");

            return resource;
        }

        public IReadOnlyDictionary<int, PriceTier> GetPriceTiers()
        {
            Dictionary<int, PriceTier> result = [];

            // Iterate over _empty
            foreach (var ResourceTypeList in _empty)
            {
                // Iterate over resources and group them by price
                foreach (var resource in ResourceTypeList)
                {
                    result.TryAdd(resource.Price, new PriceTier(resource.Price));
                    result[resource.Price].Add(resource);
                }
            }

            // Return the resulting dictionary as an IReadOnlyDictionary
            return result;
        }

        public void HandleResourceClick(Resource resource)
        {
            if (resource.Filled)
                ProcessFilled(resource);
            else
                ProcessEmpty(resource);
        }


        public int MoveToEmpty(ResourceType resourceType)
        {
            var current = _filled[(int)resourceType].First;
            if (current == null)
                return 0;

            var price = current.Value.Price;

            current.Value.Filled = false;
            _filled[(int)resourceType].Remove(current);
            _empty[(int)resourceType].AddLast(current.Value);


            return price;
        }

        public int MoveToFilled(ResourceType resourceType)
        {
            var current = _empty[(int)resourceType].Last;
            if( current == null)
                return 0;

            var price = current.Value.Price;

            current.Value.Filled = true;
            _empty[(int)resourceType].Remove(current); // Safely remove current
            _filled[(int)resourceType].AddFirst(current.Value);


            return price;
        }

        public bool HasStock(ResourceType resourceType)
        {
            return _filled[(int)resourceType].Count > 0;
        }

        // 3. Private Methods
        private void ProcessEmpty(Resource resource)
        {
            var found = _empty[(int)resource.Type].Find(resource);
            if (found == null)
                return;

            var current = _empty[(int)resource.Type].Last;

            while (current != null)
            {
                current.Value.Filled = true; // Change CurrentState
                _filled[(int)resource.Type].AddFirst(current.Value); // Add to filled
                var prev = current.Previous; // Save the reference to the previous node
                _empty[(int)resource.Type].Remove(current); // Safely remove current
                if (current == found) // Stop when we reach 'found'
                    break;
                current = prev; // Move to the previous node
            }
        }

        private void ProcessFilled(Resource resource)
        {
            var found = _filled[(int)resource.Type].Find(resource);
            if (found == null)
                return;

            var current = _filled[(int)resource.Type].First;
            while (current != null)
            {
                current.Value.Filled = false; // Change CurrentState
                _empty[(int)resource.Type].AddLast(current.Value); // Add to filled
                var next = current.Next; // Save the reference to next 
                _filled[(int)resource.Type].Remove(current); // Safely remove current
                if (current == found)
                    break;
                current = next; // Move to next Node   
            }
        }
    }
}
