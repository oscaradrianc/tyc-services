using System.Collections.Generic;
using System.Linq;
using Tyc.Interface.Repositories;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Textos.Repositories;

public class TextoRepository : ITextoRepository
{
    public Texto GetById(TycBaseContext context, int id)
    {
        return context.GetTable<Texto>()
            .FirstOrDefault(x => x.TextText == id);
    }

    public List<Texto> GetAll(TycBaseContext context)
    {
        return context.GetTable<Texto>()
            .Where(x => x.TextEstado == EstadoTexto.Activo)
            .OrderByDescending(x => x.TextFechaCreacion)
            .ToList();
    }

    public List<Texto> GetByEmpresa(TycBaseContext context, int EmpresaId, bool soloActivos = true)
    {
        var query = context.GetTable<Texto>()
            .Where(x => x.EmpresaId == EmpresaId);

        if (soloActivos)
            query = query.Where(x => x.TextEstado == EstadoTexto.Activo);

        return query.OrderBy(x => x.TextTipoTexto).ToList();
    }

    public Texto GetByEmpresaYTipo(TycBaseContext context, int EmpresaId, string tipoTexto)
    {
        return context.GetTable<Texto>()
            .FirstOrDefault(x => x.EmpresaId == EmpresaId
                && x.TextTipoTexto == tipoTexto
                && x.TextEstado == EstadoTexto.Activo);
    }

    public Texto Create(TycBaseContext context, Texto entity)
    {
        context.GetTable<Texto>().InsertOnSubmit(entity);
        context.SubmitChanges();
        return entity;
    }

    public Texto Update(TycBaseContext context, Texto entity)
    {
        var existing = context.GetTable<Texto>()
            .FirstOrDefault(x => x.TextText == entity.TextText);

        if (existing == null)
            return null;

        existing.TextTipoTexto = entity.TextTipoTexto;
        existing.TextTextoDelosTerminos = entity.TextTextoDelosTerminos;
        existing.TextEstado = entity.TextEstado;
        existing.UsuaUsuario = entity.UsuaUsuario;

        context.SubmitChanges();
        return existing;
    }

    public bool Delete(TycBaseContext context, int id)
    {
        var entity = context.GetTable<Texto>()
            .FirstOrDefault(x => x.TextText == id);

        if (entity == null)
            return false;

        // Soft delete
        entity.TextEstado = EstadoTexto.Inactivo;
        context.SubmitChanges();

        return true;
    }

    public bool CambiarEstado(TycBaseContext context, int id, string estado)
    {
        var entity = context.GetTable<Texto>()
            .FirstOrDefault(x => x.TextText == id);

        if (entity == null)
            return false;

        entity.TextEstado = estado;
        context.SubmitChanges();

        return true;
    }

    public bool Exists(TycBaseContext context, int id)
    {
        return context.GetTable<Texto>().Any(x => x.TextText == id);
    }

    public bool ExisteTextoParaEmpresaYTipo(TycBaseContext context, int EmpresaId, string tipoTexto, int? excludeId = null)
    {
        var query = context.GetTable<Texto>()
            .Where(x => x.EmpresaId == EmpresaId && x.TextTipoTexto == tipoTexto);

        if (excludeId.HasValue)
            query = query.Where(x => x.TextText != excludeId.Value);

        return query.Any();
    }
}