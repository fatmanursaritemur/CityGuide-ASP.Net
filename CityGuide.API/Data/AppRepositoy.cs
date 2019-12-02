using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityGuide.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityGuide.API.Data
{
    public class AppRepositoy : IAppRepository
    {
        private DataContext context;

        public AppRepositoy(DataContext context)
        {
            this.context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            context.Remove(entity);
        }

        public List<City> GetCities()
        {
            var cities = context.Cities.Include(c => c.Photos).ToList();
            return cities;
        }

        public City GetCityById(int cityId)
        {
            var city = context.Cities.Include(c => c.Photos).FirstOrDefault(c => c.Id == cityId);
            return city;
        }

        public Photo GetPhoto(int id)
        {
            var photo = context.Photos.FirstOrDefault(p => p.Id == id);
            return photo;
        }

        public List<Photo> GetPhotosByCity(int id)
        {
            var photos = context.Photos.Where(p => p.CityId == id).ToList();
            return photos;
        }

        public bool SaveAll()
        {
            return context.SaveChanges() > 0;
        }
    }
}
