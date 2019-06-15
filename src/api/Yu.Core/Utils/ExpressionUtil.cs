using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Yu.Core.Utils
{
    public static class ExpressionUtil<T> where T : class
    {        
        // 生成ParameterExpression用来参与表达式树
        private static ParameterExpression t = Expression.Parameter(typeof(T));

        /// <summary>
        /// 根据参数获取表达式树列表
        /// </summary>
        /// <param name="tupleList">元组列表(entity属性,值,表达式操作符)</param>
        /// <returns></returns>
        public static List<Expression> GetExpressions(List<ValueTuple<string, object, ExpressionType>> tupleList)
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
        public static Expression CombinExpressions(List<Expression> expressions, ExpressionCombineType combineType)
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
        public static Expression<Func<T, bool>> GetLambda(Expression where)
        {
            return Expression.Lambda<Func<T, bool>>(where, t);
        }


    }

    /// <summary>
    /// 表达式操作类型
    /// </summary>
    public enum ExpressionType
    {
        Equal = 1, // 相等
        NotEqual = 2, // 不相等
        StringContain = 3, // 字符串包含
        ListContain = 4, // list包含

        // todo 添加更多
    }

    /// <summary>
    /// 表达式组合类型
    /// </summary>
    public enum ExpressionCombineType
    {
        And = 1, // 相等
        Or = 2, // 不相等

        // todo 添加更多
    }



}
