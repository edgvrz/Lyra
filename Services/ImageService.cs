using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Lyra.Services
{
    public class ImagenService
    {
        private readonly IWebHostEnvironment _env;

        private static readonly string[] _tiposPermitidos =
            { "image/jpeg", "image/png", "image/webp", "image/jpg" };

        private const long _maxBytes = 5 * 1024 * 1024; // 5 MB

        public ImagenService(IWebHostEnvironment env) => _env = env;

        /// <summary>
        /// Guarda la imagen en wwwroot/uploads/prendas/ y devuelve la ruta relativa.
        /// Retorna null si el archivo no es válido.
        /// </summary>
        public async Task<string?> GuardarPrendaAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            if (!_tiposPermitidos.Contains(file.ContentType.ToLower()))
                return null;

            if (file.Length > _maxBytes)
                return null;

            var carpeta = Path.Combine(_env.WebRootPath, "uploads", "prendas");
            Directory.CreateDirectory(carpeta); // crea si no existe

            var extension = Path.GetExtension(file.FileName).ToLower();
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta  = Path.Combine(carpeta, nombreArchivo);

            using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/prendas/{nombreArchivo}";
        }

        /// <summary>
        /// Elimina una imagen anterior si existe.
        /// </summary>
        public void EliminarImagen(string? rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa)) return;
            var rutaFisica = Path.Combine(_env.WebRootPath,
                rutaRelativa.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(rutaFisica))
                File.Delete(rutaFisica);
        }
    }
}