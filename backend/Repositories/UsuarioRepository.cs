using Microsoft.EntityFrameworkCore;
using TiendaEcommerce.Api.Data;
using TiendaEcommerce.Api.Models;

namespace TiendaEcommerce.Api.Repositories
{
    /// <summary>
    /// Contrato de operaciones de acceso a datos para la entidad Usuario.
    /// </summary>
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> ObtenerTodosAsync();
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<bool> ExisteEmailAsync(string email, int? excluirId = null);
        Task<Usuario> CrearAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
        Task EliminarAsync(Usuario usuario);
    }

    /// <summary>
    /// Implementación del repositorio de usuarios usando Entity Framework Core.
    /// Todas las consultas son parametrizadas para prevenir inyección SQL.
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.IdUsuario)
                .ToListAsync();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task<bool> ExisteEmailAsync(string email, int? excluirId = null)
        {
            return await _context.Usuarios.AnyAsync(u =>
                u.Email.ToLower() == email.ToLower() &&
                (!excluirId.HasValue || u.IdUsuario != excluirId.Value));
        }

        public async Task<Usuario> CrearAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }
}