using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Yu.Core.Expressions
{
    public class ExpressionUtil<T> where T : class
    {
        // 生成ParameterExpression用来参与表达式树
        private ParameterExpression t = ParameterExpressionUtil.GetParameterExpression(typeof(T));

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
                    var left = Expression.Property(t, tuple.Item1);

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

                        // 包含(List<T>的bool Contains(T t)方法)
                        case ExpressionType.ListContain:

                            var propertyType = typeof(T).GetProperty(tuple.Item1).PropertyType;
                            var genericListType = typeof(List<>).MakeGenericType(propertyType);
                            var listContainExpression = Expression.Call(right, genericListType.GetMethod("Contains", new Type[] { propertyType }), left);
                            expressionList.Add(listContainExpression);
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
            switch (combineType)
            {
                case ExpressionCombineType.And:
                    where = Expression.Constant(true);
                    foreach (var expression in expressions)
                    {
                        where = Expression.And(where, expression);
                    }
                    break;
                case ExpressionCombineType.Or:
                    where = expressions.Count > 0 ? Expression.Constant(false) : Expression.Constant(true);
                    foreach (var expression in expressions)
                    {
                        where = Expression.Or(where, expression);
                    }
                    break;
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
            return Expression.Lambda<Func<T, bool>>(where, t);
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

                        // 包含(List<T>的bool Contains(T t)方法)
                        case ExpressionType.ListContain:

                            var propertyType = _entityType.GetProperty(tuple.Item1).PropertyType;
                            var genericListType = typeof(List<>).MakeGenericType(propertyType);
                            var listContainExpression = Expression.Call(right, genericListType.GetMethod("Contains", new Type[] { propertyType }), left);
                            expressionList.Add(listContainExpression);
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
            switch (combineType)
            {
                case ExpressionCombineType.And:
                    where = Expression.Constant(true);
                    foreach (var expression in expressions)
                    {
                        where = Expression.And(where, expression);
                    }
                    break;
                case ExpressionCombineType.Or:
                    where = expressions.Count > 0 ? Expression.Constant(false) : Expression.Constant(true);
                    foreach (var expression in expressions)
                    {
                        where = Expression.Or(where, expression);
                    }
                    break;
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
