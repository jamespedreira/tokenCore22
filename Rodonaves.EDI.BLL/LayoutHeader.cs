using AutoMapper;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Model;
using RTEFramework.BLL.Infra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class LayoutHeader : BaseCrudBll<ILayoutHeaderDal, LayoutHeaderDTO, LayoutHeaderInfo>, ILayoutHeader
    {
        #region Ctor

        private readonly ILayoutHeaderDal _layoutHeaderDal;
        private readonly ILayoutBandDal _layoutBandDal;
        private readonly ILayoutColumnDal _layoutColumnDal;
        private readonly ILayoutDictionaryDal _layoutDictionaryDal;
        private readonly ILayoutFileNameDal _layoutFileNameDal;

        public LayoutHeader(ILayoutHeaderDal dal,
            ILayoutBandDal layoutBandDal,
            ILayoutColumnDal layoutColumnDal,
            IMapper mapper,
            ILayoutDictionaryDal layoutDictionaryDal,
            ILayoutFileNameDal layoutFileNameDal) : base(dal, mapper)
        {
            _layoutHeaderDal = dal;
            _layoutBandDal = layoutBandDal;
            _layoutColumnDal = layoutColumnDal;
            _layoutDictionaryDal = layoutDictionaryDal;
            _layoutFileNameDal = layoutFileNameDal;
        }

        #endregion

        #region Methods

        public async Task<List<LayoutHeaderDTO>> GetAllAsync(long companyId)
        {
            var list = await _dal.GetAllAsync(companyId);

            return _mapper.Map<List<LayoutHeaderInfo>, List<LayoutHeaderDTO>>(list);
        }

        public new async Task<int> InsertAsync(LayoutHeaderDTO dto)
        {
            using (var transaction = _dal.BeginTransaction())
            {
                var success = await base.InsertAsync(dto);

                if (!success)
                    throw new BusinessException("Não foi possível inserir o layout");

                success = await InsertBandsAsync(dto, transaction);
                if (!success)
                    throw new BusinessException("Não foi possível inserir o layout");

                success = await InsertFilesAsync(dto, transaction);
                if (!success)
                    throw new BusinessException("Não foi possível inserir o layout");

                _dal.FinallyTransaction(success, transaction);

                return dto.Id;
            }

        }
        private async Task<bool> InsertBandsAsync(LayoutHeaderDTO dto, IDbTransaction transaction)
        {
            var success = true;
            foreach (var item in dto.LayoutBands)
            {
                item.Id = _layoutBandDal.GetNextId(transaction);
                item.LayoutHeaderId = dto.Id;
                success = await _layoutBandDal.InsertAsync(_mapper.Map<LayoutBandDTO, LayoutBandInfo>(item));
                if (!success)
                    return false;

                foreach (var column in item.LayoutColumns)
                {
                    column.Register = item.KeyBand;
                    column.Id = _layoutColumnDal.GetNextId(transaction);
                    column.LayoutBandId = item.Id;
                    success = await _layoutColumnDal.InsertAsync(_mapper.Map<LayoutColumnDTO, LayoutColumnInfo>(column));

                    if (!success)
                        return false;

                    foreach (var dic in column.LayoutDictionaries)
                    {
                        dic.Id = _layoutDictionaryDal.GetNextId();
                        dic.LayoutColumnId = column.Id;
                        success = await _layoutDictionaryDal.InsertAsync(_mapper.Map<LayoutDictionaryDTO, LayoutDictionaryInfo>(dic));
                    }

                }
            }

            return success;
        }
        private async Task<bool> InsertFilesAsync(LayoutHeaderDTO dto, IDbTransaction transaction)
        {
            var success = true;
            foreach (var item in dto.LayoutFileNames)
            {
                item.Id = _layoutFileNameDal.GetNextId(transaction);
                item.LayoutHeaderId = dto.Id;
                success = await _layoutFileNameDal.InsertAsync(_mapper.Map<LayoutFileNameDTO, LayoutFileNameInfo>(item));
                if (!success)
                    return false;
                success = await InsertColumnsAsync(item.LayoutColumns, item.Id, transaction);
            }

            return success;
        }
        private async Task<bool> InsertColumnsAsync(List<LayoutColumnDTO> LayoutColumns, int parentId, IDbTransaction transaction)
        {
            var success = false;
            foreach (var column in LayoutColumns)
            {
                column.Id = _layoutColumnDal.GetNextId(transaction);
                column.LayoutFileNameId = parentId;

                success = await _layoutColumnDal.InsertAsync(_mapper.Map<LayoutColumnDTO, LayoutColumnInfo>(column));

                if (!success)
                    return false;

                foreach (var dic in column.LayoutDictionaries)
                {
                    dic.Id = _layoutDictionaryDal.GetNextId();
                    dic.LayoutColumnId = column.Id;
                    success = await _layoutDictionaryDal.InsertAsync(_mapper.Map<LayoutDictionaryDTO, LayoutDictionaryInfo>(dic));
                }

            }

            return success;
        }

        public override async Task<bool> UpdateAsync(LayoutHeaderDTO dto)
        {
            var success = false;
            using (var transaction = _dal.BeginTransaction())
            {
                try
                {
                    success = await base.UpdateAsync(dto);
                    if (!success)
                        throw new BusinessException("Não foi possível editar o layout");

                    success = await UpdateBandAsync(dto, transaction);
                    if (!success)
                        throw new BusinessException("Não foi possível editar o layout");

                    success = await UpdateFilesAsync(dto, transaction);
                    if (!success)
                        throw new BusinessException("Não foi possível editar o layout");
                }
                finally
                {
                    _dal.FinallyTransaction(success, transaction);
                }

                return success;
            }
        }
        private async Task<bool> UpdateBandAsync(LayoutHeaderDTO dto, IDbTransaction transaction)
        {
            var success = false;
            try
            {
                var deleted = await DeleteBandsAsync(success, _mapper.Map<LayoutHeaderDTO, LayoutHeaderInfo>(dto));
                if (!deleted)
                    return false;

                success = await InsertBandsAsync(dto, transaction);
            }
            catch
            {
                success = false;
            }
            return success;
        }
        private async Task<bool> UpdateFilesAsync(LayoutHeaderDTO dto, IDbTransaction transaction)
        {
            try
            {
                var filesOrignial = await _layoutFileNameDal.GetByLayoutHeaderIdAsync(dto.Id);

                var count = dto.LayoutFileNames.Count;
                var success = true;

                foreach (var item in filesOrignial.Where(x => !dto.LayoutFileNames.Select(y => y.Id).Contains(x.Id)))
                {
                    success =  await DeleteFileNamesColumnsAsync(success, item);

                    if (success)
                        success = await _layoutFileNameDal.DeleteAsync(item);
                }

                if(success)
                    foreach (var file in dto.LayoutFileNames)
                    {
                        if (file.Id > 0)
                        {
                            var deletedColumnsSuccess = await DeleteFileNamesColumnsAsync(success, _mapper.Map<LayoutFileNameDTO, LayoutFileNameInfo>(file));
                            if (deletedColumnsSuccess == false)
                                return false;

                            success = await InsertColumnsAsync(file.LayoutColumns, file.Id, transaction);
                            success = await _layoutFileNameDal.UpdateAsync(_mapper.Map<LayoutFileNameDTO, LayoutFileNameInfo>(file));
                        }
                        else
                        {
                            success = await InsertFilesAsync(dto, transaction);

                        }
                    }

                return success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            bool success = false;

            using (var transaction = _dal.BeginTransaction())
            {
                try
                {
                    var layoutHeader = _mapper.Map<LayoutHeaderDTO, LayoutHeaderInfo>(await GetByIdAsync(id));

                    success = await DeleteBandsAsync(success, layoutHeader);
                    if (!success)
                        throw new BusinessException("Não foi possível deletar o layout");

                    foreach(var file in layoutHeader.LayoutFileNames)
                    {
                        success = await DeleteFileNamesColumnsAsync(success, file);
                        if (!success)
                            throw new BusinessException($"Não foi possível deletar as colunas do arquivo {file.Description}");

                        if(!await _layoutFileNameDal.DeleteAsync(file))
                            throw new BusinessException($"Não foi possível deletar o arquivo {file.Description}");
                    }

                    success = await base.DeleteAsync(id);
                    if (!success)
                        throw new BusinessException("Não foi possível deletar o layout");
                }
                catch (BusinessException ex)
                {
                    success = false;
                    throw new BusinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    success = false;
                }
                finally
                {
                    _dal.FinallyTransaction(success, transaction);
                }

                return success;
            }
        }
        private async Task<bool> DeleteFileNamesColumnsAsync(bool success, LayoutFileNameInfo layoutFileNameHeader)
        {

            layoutFileNameHeader.LayoutColumns = await _layoutColumnDal.GetByLayoutFileNameAsync(layoutFileNameHeader.Id);
            if (layoutFileNameHeader.LayoutColumns.Count == 0)
                return true;

            foreach (var col in layoutFileNameHeader.LayoutColumns)
            {
                col.LayoutDictionaries = await _layoutDictionaryDal.GetByLayoutColumnIdAsync(col.Id);
                foreach (var dic in col.LayoutDictionaries)
                {
                    success = await _layoutDictionaryDal.DeleteAsync(dic);
                    if (!success) break;
                }
                success = await _layoutColumnDal.DeleteAsync(col);
                if (!success) break;
            }


            return success;
        }
        private async Task<bool> DeleteBandsAsync(bool success, LayoutHeaderInfo layoutHeader)
        {
            layoutHeader.LayoutBands = await _layoutBandDal.GetByLayoutHeaderAsync(layoutHeader.Id);
            if (layoutHeader.LayoutBands.Count == 0)
                return true;

            foreach (var band in layoutHeader.LayoutBands)
            {
                band.LayoutColumns = await _layoutColumnDal.GetByLayoutBandAsync(band.Id);
                foreach (var col in band.LayoutColumns)
                {
                    col.LayoutDictionaries = await _layoutDictionaryDal.GetByLayoutColumnIdAsync(col.Id);
                    foreach (var dic in col.LayoutDictionaries)
                    {
                        success = await _layoutDictionaryDal.DeleteAsync(dic);
                        if (!success) break;
                    }
                    success = await _layoutColumnDal.DeleteAsync(col);
                    if (!success) break;
                }

                success = await _layoutBandDal.DeleteAsync(band);
                if (!success) break;

            }

            return success;
        }

        public async Task<List<LayoutHeaderDTO>> GetByParamsAsync(int id, long? layoutGroupId, string description, bool? active, long? scriptId, long companyId, string processType, int page, int amountByPage)
        {
            var list = await _dal.GetByParamsAsync(id, layoutGroupId, description, active, scriptId, companyId, processType, page, amountByPage);

            for (int i = 0; i < list.Count; i++)
            {
                list[i]= await GetLayoutHeaderDependecies(list[i]);
            }

            return _mapper.Map<List<LayoutHeaderInfo>, List<LayoutHeaderDTO>>(list);
        }

        public async Task<List<LayoutHeaderDTO>> GetByProcessType(long companyId, int processType, int customerId, int page, int amountByPage)
        {
            var list = await _dal.GetByProcessType(companyId, processType, customerId, page, amountByPage);

            return _mapper.Map<List<LayoutHeaderInfo>, List<LayoutHeaderDTO>>(list);
        }

        public async Task<LayoutHeaderDTO> GetByIdAsync(int id)
        {
            var info = await _dal.GetByIdAsync(id);

            info = await GetLayoutHeaderDependecies(info);

            return _mapper.Map<LayoutHeaderInfo, LayoutHeaderDTO>(info);
        }

        private async Task<LayoutHeaderInfo> GetLayoutHeaderDependecies(LayoutHeaderInfo item)
        {
            item.LayoutBands = await _layoutBandDal.GetByLayoutHeaderAsync(item.Id);
            foreach (var band in item.LayoutBands)
            {
                band.LayoutColumns = await _layoutColumnDal.GetByLayoutBandAsync(band.Id);
                foreach (var col in band.LayoutColumns)
                {
                    col.LayoutDictionaries = await _layoutDictionaryDal.GetByLayoutColumnIdAsync(col.Id);
                }
            }

            item.LayoutFileNames = await _layoutFileNameDal.GetByLayoutHeaderIdAsync(item.Id);
            foreach (var fileName in item.LayoutFileNames)
            {
                fileName.LayoutColumns = await _layoutColumnDal.GetByLayoutFileNameAsync(fileName.Id);
                foreach (var col in fileName.LayoutColumns)
                {
                    col.LayoutDictionaries = await _layoutDictionaryDal.GetByLayoutColumnIdAsync(col.Id);
                }
            }

            return item;
        }

        #endregion
    }
}