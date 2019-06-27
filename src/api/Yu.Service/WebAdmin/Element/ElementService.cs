﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Model.WebAdmin.Element.InputModels;
using Yu.Model.WebAdmin.Element.OutputModels;
using Ele = Yu.Data.Entities.Front.Element;
using EleTree = Yu.Data.Entities.Front.ElementTree;

namespace Yu.Service.WebAdmin.Element
{
    public class ElementService : IElementService
    {
        // 元素的仓储
        private readonly IRepository<Ele, Guid> _elementRepository;

        // 元素树的仓储
        private readonly IRepository<EleTree, Guid> _elementTreeRepository;

        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        public ElementService(IRepository<Ele, Guid> elementRepository,
            IRepository<EleTree, Guid> elementTreeRepository, IUnitOfWork<BaseIdentityDbContext> unitOfWork)
        {
            _elementRepository = elementRepository;
            _elementTreeRepository = elementTreeRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 检查元素唯一识别（创建元素）
        /// </summary>
        public List<string> HaveSameIdentification(string identification)
        {
            var eles = _elementRepository.GetByWhereNoTracking(e => e.Identification == identification);
            return eles.Select(ele => ele.Name).ToList();
        }

        /// <summary>
        /// 检查元素唯一识别（更新元素）
        /// </summary>
        public List<string> HaveSameIdentification(Guid elementId, string identification)
        {
            var eles = _elementRepository.GetByWhereNoTracking(e => e.Identification == identification && e.Id != elementId);
            return eles.Select(ele => ele.Name).ToList();
        }

        /// <summary>
        /// 创建新元素
        /// </summary>
        /// <param name="elementDetail">元素内容</param>
        public async Task CreateElement(ElementDetail elementDetail)
        {
            var ele = await _elementRepository.InsertAsync(Mapper.Map<Ele>(elementDetail));

            // 如果没有上级id则为根节点
            if (!string.IsNullOrEmpty(elementDetail.UpId))
            {
                var upid = Guid.Parse(elementDetail.UpId);
                var eletree = _elementTreeRepository.GetByWhere(et => et.Descendant == upid);
                foreach (var tree in eletree)
                {
                    await _elementTreeRepository.InsertAsync(new EleTree()
                    {
                        Ancestor = tree.Ancestor,
                        Descendant = ele.Id,
                        Length = tree.Length + 1
                    });
                }
            }

            // 插入自身
            await _elementTreeRepository.InsertAsync(new EleTree()
            {
                Ancestor = ele.Id,
                Descendant = ele.Id,
                Length = 0
            });

            // 工作单元提交
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 删除元素
        /// </summary>
        /// <param name="elementId">元素ID</param>
        public async Task DeleteElement(Guid elementId)
        {
            var eleIds = _elementTreeRepository.GetByWhere(et => et.Ancestor == elementId).Select(et => et.Descendant);
            _elementRepository.DeleteRange(e => eleIds.Contains(e.Id));
            _elementTreeRepository.DeleteRange(et => eleIds.Contains(et.Ancestor) || eleIds.Contains(et.Descendant));

            // 工作单元提交
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 取得所有元素
        /// </summary>
        public IEnumerable<ElementResult> GetAllElement()
        {
            var result = new List<ElementResult>();
            var elements = _elementRepository.GetAll();
            foreach (var element in elements)
            {
                var elementTree = _elementTreeRepository.GetByWhere(et => et.Descendant == element.Id && et.Length == 1)
                    .FirstOrDefault();
                result.Add(new ElementResult
                {
                    Id = element.Id.ToString(),
                    UpId = elementTree == null ? string.Empty : elementTree.Ancestor.ToString(),
                    ElementType = (int)element.ElementType,
                    Identification = element.Identification,
                    Name = element.Name,
                    Route = element.Route,
                });
            }
            return result;
        }

        /// <summary>
        /// 更新元素
        /// </summary>
        /// <param name="elementDetail">元素内容</param>
        public async Task UpdateElement(ElementDetail elementDetail)
        {
            _elementRepository.Update(Mapper.Map<Ele>(elementDetail));
            await _unitOfWork.CommitAsync();
        }
    }
}