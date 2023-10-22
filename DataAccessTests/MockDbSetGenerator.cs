using DataAccess;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessTests;

public static class MockGenerator
{
    public static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
    {        
        var queryable = sourceList.AsQueryable();
        var dbSet = new Mock<DbSet<T>>();
        dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
        dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));
        dbSet.Setup(d => d.Remove(It.IsAny<T>())).Callback<T>((s) => sourceList.Remove(s));        
        return dbSet.Object;
    }

    //This is the version I've came up.
    public static Mock<T> GenerateDbSetMock<T, D>(List<D> Source) where T : class where D : class
    {
        var mock = new Mock<T>();
        var data = Source.AsQueryable();

        mock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(data.Provider);
        mock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(data.Expression);
        mock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(data.ElementType);
        mock.As<IQueryable<D>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());        
        return mock;
    }
}
