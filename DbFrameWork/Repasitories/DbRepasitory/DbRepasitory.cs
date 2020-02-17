﻿using DbFramework.Attributes;
using DbFramework.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DbFramework.Repasitories.DbRepasitory
{
    public class DbRepasitory<TModel> : IRepasitory<TModel>
        where TModel : class, new()
    {
        private DbContext dbContext;
        private Mapper mapper;

        public DbRepasitory(string connectionString)
        {
            dbContext = new DbContext(connectionString);
            mapper = new Mapper();
        }

        public IEnumerable<TModel> ExecuteSelect(TModel model)
        {
            IEnumerable<IDataReader> reader =
                dbContext.ExecuteSelect(Query.SelectBuilder(mapper.GetTableName(model)));

            foreach (var data in reader)
            {
                yield return mapper.InitializeModel(model, data);
            }
        }

        public void ExecuteInsert(TModel model)
        {
            Dictionary<string, object> propertiesAndValues = mapper.GetPropertiesAndValue(model);
            dbContext.ExecuteInsert
                (Query.InsertBuilder(mapper.GetTableName(model), propertiesAndValues.Keys), propertiesAndValues);
        }

        public IEnumerable<TModel> ExecuteMultyInsert(List<TModel> t)
        {
            throw new NotImplementedException();
        }

        private class Mapper
        {
            public string GetTableName(object model)
            {
                Type type = model.GetType();
                TableNameAttribute attribute = type.GetCustomAttribute<TableNameAttribute>();
                return attribute == null ? type.Name : attribute.TableName;
            }

            public TModel InitializeModel(TModel model, IDataReader reader)
            {
                Type type = typeof(TModel);
                PropertyInfo[] propInfo = type.GetProperties();
                int i = -1;

                foreach (var prop in propInfo)
                {
                    i++;
                    if (!reader.IsDBNull(i))
                        prop.SetValue(model, reader.GetValue(i));
                    else
                        prop.SetValue(model, null);
                }

                return model;
            }

            public Dictionary<string, object> GetPropertiesAndValue(TModel model)
            {
                Dictionary<string, object> propertiesAndValues = new Dictionary<string, object>();

                Type type = typeof(TModel);
                PropertyInfo[] propInfo = type.GetProperties();

                int i = -1;

                foreach (var prop in propInfo)
                {
                    i++;
                    if (!Attribute.IsDefined(prop, typeof(IgnoreAttribute)) && prop.GetValue(model) != null)
                    {
                        propertiesAndValues.Add(prop.Name, prop.GetValue(model));
                    }
                }

                return propertiesAndValues;
            }

            //public static T ToModel<T>(this IDataReader dataReader) where T : class, new()
            //{
            //    Type type = typeof(T);

            //    var members = type
            //        .GetProperties()
            //        .Where(p => p.GetCustomAttribute<IgnoreAttribute>() == null)
            //        .ToList();

            //    var source = new T();

            //    for (int i = 0; i < members.Count; i++)
            //    {
            //        if (members[i].GetCustomAttribute<DateAttribute>() != null)
            //        {
            //            if (DateTime.TryParse(dataReader.GetValue(i).ToString(), out DateTime date))
            //            {
            //                members[i].SetValue(source, date);
            //            }

            //        }
            //        else
            //        {

            //            if (!dataReader.IsDBNull(i))
            //            {
            //                members[i].SetValue(source, dataReader.GetValue(i));
            //            }
            //            else
            //            {
            //                members[i].SetValue(source, null);
            //            }
            //        }

            //    }

            //    return source;
            //}
        }
    }
}