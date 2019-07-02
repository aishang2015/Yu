using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Yu.Core.Expressions
{
    public class ExpressionGroup<T> where T : class
    {
        // 表达式组
        // item1 属性名，item2 属性值，item3表达式类型
        public List<ValueTuple<string, object, ExpressionType>> TupleList { get; set; }

        // 表达式组合类型
        public ExpressionCombineType ExpressionCombineType { get; set; }

        // 组合表达式组
        public List<ExpressionGroup<T>> ExpressionGroupsList { get; set; }

        public ExpressionGroup()
        {
            TupleList = new List<(string, object, ExpressionType)>();
            ExpressionCombineType = new ExpressionCombineType();
            ExpressionGroupsList = new List<ExpressionGroup<T>>();
        }

        public ExpressionGroup(
            List<(string, object, ExpressionType)> tupleList,
            ExpressionCombineType expressionCombineType,
            List<ExpressionGroup<T>> expressionGroupsList)
        {
            TupleList = tupleList;
            ExpressionCombineType = expressionCombineType;
            ExpressionGroupsList = expressionGroupsList;
        }

        // 获取表达式
        public Expression GetGroupExpression()
        {
            // 表达式操作工具
            var expressionUtil = new ExpressionUtil<T>();

            // 表达式组
            var eGroupExpressionList = new List<Expression>();

            // 如果表达式组不为空
            if (ExpressionGroupsList != null && ExpressionGroupsList.Count > 0)
            {
                // 递归生成过滤表达式并组合
                ExpressionGroupsList.ForEach(group =>
                {
                    eGroupExpressionList.Add(group.GetGroupExpression());
                });
            }

            var eplist = expressionUtil.GetExpressions(TupleList);
            eplist.AddRange(eGroupExpressionList);

            return expressionUtil.CombinExpressions(eplist, ExpressionCombineType);
        }

        // 取得lambda表达式
        public Expression<Func<T, bool>> GetLambda()
        {
            // 表达式操作工具
            var expressionUtil = new ExpressionUtil<T>();

            // 表达式组
            var eGroupExpressionList = new List<Expression>();

            // 如果表达式组不为空
            if (ExpressionGroupsList != null && ExpressionGroupsList.Count > 0)
            {
                // 递归生成过滤表达式并组合
                ExpressionGroupsList.ForEach(group =>
                {
                    eGroupExpressionList.Add(group.GetGroupExpression());
                });
            }

            var eplist = expressionUtil.GetExpressions(TupleList);
            eplist.AddRange(eGroupExpressionList);

            var expression = expressionUtil.CombinExpressions(eplist, ExpressionCombineType);
            return expressionUtil.GetLambda(expression);
        }

    }

    public class ExpressionGroup
    {
        // 表达式组
        // item1 属性名，item2 属性值，item3表达式类型
        public List<ValueTuple<string, object, ExpressionType>> TupleList { get; set; }

        // 表达式组合类型
        public ExpressionCombineType ExpressionCombineType { get; set; }

        // 组合表达式组
        public List<ExpressionGroup> ExpressionGroupsList { get; set; }

        // 实体类型
        private readonly Type _entityType;

        public ExpressionGroup(Type entityType)
        {
            TupleList = new List<(string, object, ExpressionType)>();
            ExpressionCombineType = new ExpressionCombineType();
            ExpressionGroupsList = new List<ExpressionGroup>();
            _entityType = entityType;
        }

        public ExpressionGroup(
            List<(string, object, ExpressionType)> tupleList,
            ExpressionCombineType expressionCombineType,
            List<ExpressionGroup> expressionGroupsList)
        {
            TupleList = tupleList;
            ExpressionCombineType = expressionCombineType;
            ExpressionGroupsList = expressionGroupsList;
        }

        // 获取表达式
        public Expression GetGroupExpression()
        {
            // 表达式操作工具
            var expressionUtil = new ExpressionUtil(_entityType);

            // 表达式组
            var eGroupExpressionList = new List<Expression>();

            // 如果表达式组不为空
            if (ExpressionGroupsList != null && ExpressionGroupsList.Count > 0)
            {
                // 递归生成过滤表达式并组合
                ExpressionGroupsList.ForEach(group =>
                {
                    eGroupExpressionList.Add(group.GetGroupExpression());
                });
            }

            var eplist = expressionUtil.GetExpressions(TupleList);
            eplist.AddRange(eGroupExpressionList);

            return expressionUtil.CombinExpressions(eplist, ExpressionCombineType);
        }

        // 取得lambda表达式
        public Expression GetLambda()
        {
            // 表达式操作工具
            var expressionUtil = new ExpressionUtil(_entityType);

            // 表达式组
            var eGroupExpressionList = new List<Expression>();

            // 如果表达式组不为空
            if (ExpressionGroupsList != null && ExpressionGroupsList.Count > 0)
            {
                // 递归生成过滤表达式并组合
                ExpressionGroupsList.ForEach(group =>
                {
                    eGroupExpressionList.Add(group.GetGroupExpression());
                });
            }

            var eplist = expressionUtil.GetExpressions(TupleList);
            eplist.AddRange(eGroupExpressionList);

            var expression = expressionUtil.CombinExpressions(eplist, ExpressionCombineType);
            return expressionUtil.GetLambda(expression);
        }

    }
}
