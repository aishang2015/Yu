using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yu.Core.Utils;

namespace Yu.Core.Expressions
{
    public class ExpressionUtil<T> where T : class
    {
        // 生成ParameterExpression用来参与表达式树
        private ParameterExpression _parameterExpression = ParameterExpressionUtil.GetParameterExpression(typeof(T));

        /// <summary>
        /// 根据参数获取表达式树列表
        /// </summary>
        /// <param name="tupleList">元组列表(entity属性,值,表达式操作符)</param>
        /// <returns></returns>
        public List<Expression> GetExpressions(List<ValueTuple<string, object, ExpressionType>> tupleList)
        {
            // 表达式集合
            var expressionList = new List<Expression>();

            // 循环元组
            foreach (var tuple in tupleList)
            {
                // 值为空时不生成表达式
                if (tuple.Item2 != null && !string.IsNullOrEmpty(tuple.Item2.ToString()))
                {
                    // 左侧表达式
                    var left = Expression.Property(_parameterExpression, tuple.Item1);

                    // 右侧表达式
                    var right = Expression.Constant(tuple.Item2);

                    switch (tuple.Item3)
                    {
                        // 相等
                        case ExpressionType.Equal:
                            var equalExpression = Expression.Equal(left, right);
                            expressionList.Add(equalExpression);
                            break;

                        // 不等
                        case ExpressionType.NotEqual:
                            var notEqualExpression = Expression.NotEqual(left, right);
                            expressionList.Add(notEqualExpression);
                            break;

                        // 包含(string的bool Contains(String value)方法)
                        case ExpressionType.StringContain:
                            var containExpression = Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), right);
                            expressionList.Add(containExpression);
                            break;

                        // 不包含(string的bool Contains(String value)方法)
                        case ExpressionType.StringNotContain:
                            var notContainExpression = Expression.Not(Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), right));
                            expressionList.Add(notContainExpression);
                            break;

                        // 包含(List<T>的bool Contains(T t)方法)
                        case ExpressionType.ListContain:
                            var propertyType = typeof(T).GetProperty(tuple.Item1).PropertyType;
                            var genericListType = typeof(List<>).MakeGenericType(propertyType);
                            var listContainExpression = Expression.Call(right, genericListType.GetMethod("Contains", new Type[] { propertyType }), left);
                            expressionList.Add(listContainExpression);
                            break;

                        // 不包含(List<T>的bool Contains(T t)方法)
                        case ExpressionType.ListNotContain:
                            propertyType = typeof(T).GetProperty(tuple.Item1).PropertyType;
                            genericListType = typeof(List<>).MakeGenericType(propertyType);
                            var listNotContainExpression = Expression.Not(Expression.Call(right, genericListType.GetMethod("Contains", new Type[] { propertyType }), left));
                            expressionList.Add(listNotContainExpression);
                            break;


                    }
                }
            }
            return expressionList;

        }

        /// <summary>
        /// 用指定的操作连接表达式
        /// </summary>
        /// <param name="expressions">表达式集合</param>
        /// <param name="combineType">连接方式</param>
        /// <returns>表达式</returns>
        public Expression CombinExpressions(List<Expression> expressions, ExpressionCombineType combineType)
        {
            Expression where = Expression.Constant(true);
            if (expressions.Count() > 0)
            {
                switch (combineType)
                {
                    case ExpressionCombineType.And:
                        where = expressions[0];
                        for (int i = 1; i < expressions.Count(); i++)
                        {
                            where = Expression.And(where, expressions[i]);
                        }
                        break;
                    case ExpressionCombineType.Or:
                        where = expressions[0];
                        for (int i = 1; i < expressions.Count(); i++)
                        {
                            where = Expression.Or(where, expressions[i]);
                        }
                        break;
                }
            }
            return where;
        }

        /// <summary>
        /// 根据表达式类
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Expression<Func<T, bool>> GetLambda(Expression where)
        {
            return Expression.Lambda<Func<T, bool>>(where, _parameterExpression);
        }

        /// <summary>
        /// 组合lambda表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public LambdaExpression JoinLambdaExpression(List<LambdaExpression> expressions, ExpressionCombineType combineType)
        {
            if (expressions.Count() == 0)
                return null;
            LambdaExpression lambda = expressions[0];
            if (expressions.Count() > 0)
            {
                foreach (var ex in expressions)
                {
                    var invokedExpr = Expression.Invoke(lambda, ex.Parameters.Cast<Expression>());
                    switch (combineType)
                    {
                        case ExpressionCombineType.And:
                            lambda = Expression.Lambda(Expression.And(ex.Body, invokedExpr), lambda.Parameters);
                            break;
                        case ExpressionCombineType.Or:
                            lambda = Expression.Lambda(Expression.Or(ex.Body, invokedExpr), lambda.Parameters);
                            break;
                    }
                }

            }
            return lambda;
        }

    }


    public class ExpressionUtil
    {
        // 生成ParameterExpression用来参与表达式树
        private readonly ParameterExpression _parameterExpression;

        // 实体类型
        private readonly Type _entityType;

        public ExpressionUtil(Type type)
        {
            _entityType = type;
            _parameterExpression = ParameterExpressionUtil.GetParameterExpression(type);
        }

        /// <summary>
        /// 根据参数获取表达式树列表
        /// </summary>
        /// <param name="tupleList">元组列表(entity属性,值,表达式操作符)</param>
        /// <returns></returns>
        public List<Expression> GetExpressions(List<ValueTuple<string, object, ExpressionType>> tupleList)
        {
            // 表达式集合
            var expressionList = new List<Expression>();

            // 循环元组
            foreach (var tuple in tupleList)
            {
                // 值为空时不生成表达式
                if (tuple.Item2 != null && !string.IsNullOrEmpty(tuple.Item2.ToString()))
                {
                    // 获取属性的类型
                    var propertyType = _entityType.GetProperty(tuple.Item1).PropertyType;

                    // 左侧表达式
                    var left = Expression.Property(_parameterExpression, tuple.Item1);

                    // 右侧表达式
                    var value = ReflectionUtil.ConvertToType(tuple.Item2, tuple.Item3, propertyType);
                    var right = Expression.Constant(value);

                    switch (tuple.Item3)
                    {
                        // 相等
                        case ExpressionType.Equal:
                            var equalExpression = Expression.Equal(left, right);
                            expressionList.Add(equalExpression);
                            break;

                        // 不等
                        case ExpressionType.NotEqual:
                            var notEqualExpression = Expression.NotEqual(left, right);
                            expressionList.Add(notEqualExpression);
                            break;

                        // 包含(string的bool Contains(String value)方法)
                        case ExpressionType.StringContain:
                            var containExpression = Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), right);
                            expressionList.Add(containExpression);
                            break;

                        // 不包含(string的bool Contains(String value)方法)
                        case ExpressionType.StringNotContain:
                            var notContainExpression = Expression.Not(Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), right));
                            expressionList.Add(notContainExpression);
                            break;

                        // 包含(List<T>的bool Contains(T t)方法)
                        case ExpressionType.ListContain:

                            var genericListType = typeof(List<>).MakeGenericType(propertyType);
                            var listContainExpression = Expression.Call(right, genericListType.GetMethod("Contains", new Type[] { propertyType }), left);
                            expressionList.Add(listContainExpression);
                            break;

                        // 不包含(List<T>的bool Contains(T t)方法)
                        case ExpressionType.ListNotContain:
                            genericListType = typeof(List<>).MakeGenericType(propertyType);
                            var listNotContainExpression = Expression.Not(Expression.Call(right, genericListType.GetMethod("Contains", new Type[] { propertyType }), left));
                            expressionList.Add(listNotContainExpression);
                            break;
                    }
                }
            }
            return expressionList;

        }

        /// <summary>
        /// 用指定的操作连接表达式
        /// </summary>
        /// <param name="expressions">表达式集合</param>
        /// <param name="combineType">连接方式</param>
        /// <returns>表达式</returns>
        public Expression CombinExpressions(List<Expression> expressions, ExpressionCombineType combineType)
        {
            Expression where = Expression.Constant(true);
            if (expressions.Count() > 0)
            {
                switch (combineType)
                {
                    case ExpressionCombineType.And:
                        where = expressions[0];
                        for (int i = 1; i < expressions.Count(); i++)
                        {
                            where = Expression.And(where, expressions[i]);
                        }
                        break;
                    case ExpressionCombineType.Or:
                        where = expressions[0];
                        for (int i = 1; i < expressions.Count(); i++)
                        {
                            where = Expression.Or(where, expressions[i]);
                        }
                        break;
                }
            }
            return where;
        }

        /// <summary>
        /// 根据表达式类
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public LambdaExpression GetLambda(Expression where)
        {
            var lambdaType = (typeof(Func<,>).MakeGenericType(_entityType, typeof(bool)));
            return Expression.Lambda(lambdaType, where, _parameterExpression);
        }

        /// <summary>
        /// 组合lambda表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public LambdaExpression JoinLambdaExpression(List<LambdaExpression> expressions, ExpressionCombineType combineType)
        {
            var lambdaType = (typeof(Func<,>).MakeGenericType(_entityType, typeof(bool)));

            if (expressions.Count() == 0)
                return null;
            LambdaExpression lambda = expressions[0];

            if (expressions.Count() > 0)
            {
                foreach (var ex in expressions)
                {
                    var invokedExpr = Expression.Invoke(lambda, ex.Parameters.Cast<Expression>());
                    switch (combineType)
                    {
                        case ExpressionCombineType.And:
                            lambda = Expression.Lambda(lambdaType, Expression.And(ex.Body, invokedExpr), lambda.Parameters);
                            break;
                        case ExpressionCombineType.Or:
                            lambda = Expression.Lambda(lambdaType, Expression.Or(ex.Body, invokedExpr), lambda.Parameters);
                            break;
                    }
                }

            }
            return lambda;
        }
    }

    // 储存parameterExpression
    public static class ParameterExpressionUtil
    {
        private static Dictionary<Type, ParameterExpression> _parameterDic = new Dictionary<Type, ParameterExpression>();

        public static ParameterExpression GetParameterExpression(Type type)
        {
            if (!_parameterDic.ContainsKey(type))
            {
                _parameterDic.Add(type, Expression.Parameter(type));
            }
            return _parameterDic[type];
        }
    }

}